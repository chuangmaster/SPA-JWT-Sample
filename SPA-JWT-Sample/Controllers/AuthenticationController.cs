using Microsoft.AspNetCore.Mvc;

namespace SPA_JWT_Sample.Controllers
{
    public class AuthenticationController : Controller
    {
        private IConfiguration Configuration;
        public AuthenticationController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        /// <summary>
        /// use login model to validate and generate jwt token into cookie
        /// </summary>
        /// <returns></returns>
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

        private string GenerateJwtToken( IConfiguration config)
        {
            // Generate JWT token logic here
            
            return "generated_token";
        }
    }


    public class LoginModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
