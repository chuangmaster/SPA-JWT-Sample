using Microsoft.IdentityModel.Tokens;
using SPA_JWT_Sample.Models.Services;
using SPA_JWT_Sample.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SPA_JWT_Sample.Services
{
    public class AzureExtraIdService : IAzureExtraIdService
    {

        private readonly AzureExtraIdConfigModel _configModel;

        public AzureExtraIdService(AzureExtraIdConfigModel configModel)
        {
            _configModel = configModel;
        }
        public Task<ClaimsPrincipal> ValidateAzureAdToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // 1. 設定 TokenValidationParameters
            var validationParameters = new TokenValidationParameters
            {
                // 單租用戶
                ValidateIssuer = true,
                ValidIssuer = $"https://login.microsoftonline.com/{_configModel.TenantId}/v2.0", // 替換為您的 tenantId, 此為單一租用戶的時候的做法

                //多租用戶
                //ValidateIssuer = true,
                //IssuerValidator = (issuer, securityToken, validationParameters)=>
                //{
                //	if(issuer == "https://login.microsoftonline.com/9188040d-6c67-4c5b-b112-36a304b66dad/v2.0"){
                //		return issuer;
                //	}
                //	throw new SecurityTokenInvalidIssuerException("Invalid issuer");
                //},

                //ValidIssuer = "https://login.microsoftonline.com/common/v2.0", // 替換為您的 tenantId


                ValidateAudience = true,
                ValidAudience = _configModel.ApplicationId,  // 替換為您的 Azure AD 應用程式的 ClientId

                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,

                // 2. 動態抓取 Azure AD 的簽名密鑰
                IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
                {
                    // JWKS 端點
                    var client = new HttpClient();
                    var jwks = client.GetStringAsync("https://login.microsoftonline.com/common/discovery/v2.0/keys").Result;

                    // 解析 JWKS 回應，返回簽名密鑰
                    return new JsonWebKeySet(jwks).GetSigningKeys();
                }
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                return Task.FromResult(principal); // 返回已驗證的 ClaimsPrincipal
            }
            catch (SecurityTokenException ex)
            {
                Console.WriteLine($"Token validation failed: {ex.Message}");
                return Task.FromResult<ClaimsPrincipal>(null);
            }
        }
    }
}
