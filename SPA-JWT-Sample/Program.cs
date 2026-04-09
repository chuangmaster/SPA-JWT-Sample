using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SPA_JWT_Sample.Models.Services.Request;
using SPA_JWT_Sample.Services;
using SPA_JWT_Sample.Services.Interfaces;
using IAuthorizationService = SPA_JWT_Sample.Services.Interfaces.IAuthorizationService;

namespace SPA_JWT_Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 於啟動時產生 RSA 2048 金鑰對，以 Singleton 共用（含私鑰，供簽章使用）
            var rsa = RSA.Create(2048);
            builder.Services.AddSingleton(rsa);

            // 僅含公鑰的 RSA 實例，專供 JWT 驗證（不含私鑰）
            var rsaPublicOnly = RSA.Create();
            rsaPublicOnly.ImportParameters(rsa.ExportParameters(false));

            // Add services to the container.
            builder.Services.AddSingleton<IAuthorizationService, AuthorizationService>();
            builder.Services.AddSingleton<IAzureExtraIdService, AzureExtraIdService>();
            builder.Services.AddSingleton<IUserService, UserService>();
            builder.Services.AddSingleton<AzureExtraIdConfigDTO>(
                new AzureExtraIdConfigDTO
                {
                    ApplicationId =
                        builder.Configuration["AzureAd:ApplicationId"] ?? "ApplicationId",
                    TenantId = builder.Configuration["AzureAd:TenantId"] ?? "TenantId",
                }
            );
            builder.Services.AddSingleton<AuthorizationConfigDTO>(
                new AuthorizationConfigDTO
                {
                    Issuer = builder.Configuration["JWT:Issuer"] ?? "Issuer",
                    Secret = builder.Configuration["JWT:Secret"] ?? "Secret",
                    Audience = builder.Configuration["JWT:Audience"] ?? "Audience",
                }
            );

            // Add CORS service
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowSpecificOrigin",
                    builder =>
                    {
                        builder
                            .WithOrigins("http://localhost:8080")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    }
                );
            });

            // Add JWT service（RS256 非對稱驗證，從 Authorization: Bearer header 讀取）
            builder
                .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration["JWT:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration["JWT:Audience"],
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        // 使用純公鑰的 RSA 實例驗證簽章（不持有私鑰）
                        IssuerSigningKey = new RsaSecurityKey(rsaPublicOnly),
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
                    builder
                        .WithOrigins("http://localhost:8080")
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
