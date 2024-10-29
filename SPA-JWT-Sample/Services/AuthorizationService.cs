using Microsoft.IdentityModel.Tokens;
using SPA_JWT_Sample.Models.Services.Request;
using SPA_JWT_Sample.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace SPA_JWT_Sample.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly AuthorizationConfigDTO _authorizationConfigModel;
        public AuthorizationService(AuthorizationConfigDTO authorizationConfigModel)
        {
            _authorizationConfigModel = authorizationConfigModel;
        }
        public string GenerateJwtToken(IConfiguration config, bool isAdmin, string platform)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authorizationConfigModel.Secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>()
            {
                //new Claim(ClaimTypes.Role, isAdmin? "admin": "normalUser") // 添加角色 Claim
                //new Claim("roles", JsonSerializer.Serialize(new
                //{
                //    A = isAdmin ? "admin" : "normalUser",
                //    B = "User"
                //}))
            };
            if (!string.IsNullOrEmpty(platform) && platform == "A")
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Aud, "A"));
                claims.Add(new Claim(ClaimTypes.Role, isAdmin ? "admin" : "normalUser"));
            }
            if (!string.IsNullOrEmpty(platform) && platform == "B")
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Aud, "B"));
                claims.Add(new Claim(ClaimTypes.Role, "User"));

            }

            var token = new JwtSecurityToken(
                issuer: _authorizationConfigModel.Issuer,
                //audience: config["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );
            var tokenHelper = new JwtSecurityTokenHandler();
            return tokenHelper.WriteToken(token);
        }

        public string GenerateJwtToken()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authorizationConfigModel.Secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            bool isAdmin = true;
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Aud, "A"),
                new Claim(ClaimTypes.Role, isAdmin ? "admin" : "normalUser")
            };

            var token = new JwtSecurityToken(
                issuer: _authorizationConfigModel.Issuer,
                //audience: config["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );
            var tokenHelper = new JwtSecurityTokenHandler();
            return tokenHelper.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            throw new NotImplementedException();
        }
    }
}
