using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using SPA_JWT_Sample.Models.Services.Request;
using SPA_JWT_Sample.Services.Interfaces;

namespace SPA_JWT_Sample.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly AuthorizationConfigDTO _authorizationConfigModel;

        // 保存完整金鑰對（含私鑰），用於簽章
        private readonly RsaSecurityKey _signingKey;

        // 僅含公鑰參數，用於暴露給外部驗證
        private readonly RsaSecurityKey _verificationKey;

        public AuthorizationService(AuthorizationConfigDTO authorizationConfigModel, RSA rsa)
        {
            _authorizationConfigModel = authorizationConfigModel;

            // 完整金鑰對 → 用於 JWT 簽章
            _signingKey = new RsaSecurityKey(rsa);

            // ExportParameters(false) → includePrivateParameters = false，僅匯出公鑰（n、e），不含私鑰
            var publicParams = rsa.ExportParameters(includePrivateParameters: false);
            var publicRsa = RSA.Create();
            publicRsa.ImportParameters(publicParams);
            _verificationKey = new RsaSecurityKey(publicRsa);
        }

        public string GenerateJwtToken()
        {
            var credentials = new SigningCredentials(_signingKey, SecurityAlgorithms.RsaSha256);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, "demo-user"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "admin"),
            };

            var token = new JwtSecurityToken(
                issuer: _authorizationConfigModel.Issuer,
                audience: _authorizationConfigModel.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            RandomNumberGenerator.Fill(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public RsaSecurityKey GetPublicKey() => _verificationKey;

        public string GetPublicKeyJwkJson()
        {
            var jwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(_verificationKey);
            // 只回傳公鑰欄位（n、e），不含私鑰
            var publicJwk = new
            {
                kty = jwk.Kty,
                use = "sig",
                alg = "RS256",
                n = jwk.N,
                e = jwk.E,
            };
            return JsonSerializer.Serialize(publicJwk);
        }
    }
}
