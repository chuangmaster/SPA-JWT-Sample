using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace SPA_JWT_Sample.Controllers
{
    public class AuthenticationController : ControllerBase
    {
        private IConfiguration Configuration;
        public AuthenticationController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 使用登入模型來驗證並生成 JWT token 到 cookie
        /// </summary>
        /// <returns></returns>
        [HttpPost("login")]
        public IActionResult Login(LoginModel loginModel)
        {
            if (loginModel.Username == "admin" && loginModel.Password == "password")
            {
                var token = GenerateJwtToken(Configuration);
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // 在開發環境中可以設置為 false，生產環境中應設置為 true (目前測試下來都使用true並使用https做傳遞成功率比較高)
                    SameSite = SameSiteMode.None, // 確保跨站點 cookie 可以被發送
                };
                Response.Cookies.Append("access_token", token, cookieOptions);
                return Ok("Login success");
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// 使用登入模型來驗證並生成 JWT token 到 cookie
        /// </summary>
        /// <returns></returns>
        [HttpPost("token")]
        public IActionResult Token([FromBody]LoginModel loginModel)
        {
            if (loginModel.Username == "admin" && loginModel.Password == "password")
            {
                var token = GenerateJwtToken(Configuration);
                return Ok(new { Token = token});
            }
            else
            {
                return Unauthorized();
            }
        }
        private string GenerateJwtToken(IConfiguration config)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Secret"] ?? "default secret"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: config["JWT:Issuer"],
                audience: config["JWT:Audience"],
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );
            var tokenHelper = new JwtSecurityTokenHandler();
            return tokenHelper.WriteToken(token);
        }
    }


    public class LoginModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
