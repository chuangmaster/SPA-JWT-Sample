using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SPA_JWT_Sample.Models;
using SPA_JWT_Sample.Services;
using System.Text;

namespace SPA_JWT_Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddSingleton<SPA_JWT_Sample.Services.Interfaces.IAuthorizationService, AuthorizationService>();
            builder.Services.AddSingleton<SPA_JWT_Sample.Services.Interfaces.IAzureExtraIdService, AzureExtraIdService>();
            builder.Services.AddSingleton<AzureExtraIdConfigModel>(new AzureExtraIdConfigModel
            {
                ApplicationId = builder.Configuration["AzureAd:ApplicationId"] ?? "ApplicationId",
                TenantId = builder.Configuration["AzureAd:TenantId"] ?? "TenantId"
            });
            builder.Services.AddSingleton<AuthorizationConfigModel>(new AuthorizationConfigModel
            {
                Issuer = builder.Configuration["JWT:Issuer"] ?? "Issuer",
                Secret = builder.Configuration["JWT:Secret"] ?? "Secret",
                Audience = builder.Configuration["JWT:Audience"] ?? "Audience"
            });

            // Add CORS service
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:8080")
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials();
                    });
            });

            // Add JWT service
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = true,
                        ValidAudiences = new[] { "A" },
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"] ?? "default secret"))
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Cookies["access_token"];
                            context.Token = accessToken;
                            return Task.CompletedTask;
                        }
                    };
                });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Ensure CORS is applied early in the pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseCors("AllowSpecificOrigin");
            }
            else
            {
                app.UseCors(builder =>
                {
                    builder.WithOrigins("http://localhost:8080")
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                });
            }
            //app.UseCors("AllowSpecificOrigin");

            app.UseHttpsRedirection();

            // Use Authentication and Authorization after CORS
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
