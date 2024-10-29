using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SPA_JWT_Sample.Models.Api.Requests;
using SPA_JWT_Sample.Models.Api.Responses;
using SPA_JWT_Sample.Services;
using SPA_JWT_Sample.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace SPA_JWT_Sample.Controllers
{
    public class AuthenticationController : ControllerBase
    {
        private IConfiguration Configuration;
        private readonly IAuthorizationService _authorizationService;
        private readonly IAzureExtraIdService _azureExtraIdService;
        private readonly IUserService _userService;

        public AuthenticationController(
            IConfiguration configuration,
            IAuthorizationService authenticationService,
            IAzureExtraIdService azureExtraIdService,
            IUserService userService)
        {
            Configuration = configuration;
            _authorizationService = authenticationService;
            _azureExtraIdService = azureExtraIdService;
            _userService = userService;

        }

        /// <summary>
        /// 使用登入模型來驗證並生成 JWT token 到 cookie
        /// </summary>
        /// <returns></returns>
        [HttpPost("login")]
        public IActionResult Login(LoginRequest loginModel)
        {
            var response = new LoginResponse();
            var validReuslt = _azureExtraIdService.ValidateAzureAdToken(loginModel.TokenId);
            if (validReuslt != null)
            {
                var user = _userService.GetUserById("");
                if (user != null)
                {
                    var jwt = _authorizationService.GenerateJwtToken();
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true, // 在開發環境中可以設置為 false，生產環境中應設置為 true 
                        SameSite = SameSiteMode.None, // 確保跨站點 cookie 可以被發送
                    };
                    Response.Cookies.Append("access_token", jwt, cookieOptions);
                    var refreshToken = _authorizationService.GenerateRefreshToken();

                    response.RefreshToken = refreshToken;
                    response.ExpiresIn = 10800;
                    return Ok(response);
                }
                else
                {
                    response.Message = "User not found";
                    return BadRequest(response);
                }
            }
            else
            {
                response.Message = "Invalid token";
                return BadRequest(response);
            }
        }

        /// <summary>
        /// 使用登入模型來驗證並生成 JWT token 到 cookie
        /// </summary>
        /// <returns></returns>
        [HttpPost("token")]
        public IActionResult Token([FromBody] FormLoginRequest loginModel)
        {
            if (loginModel.Username == "admin" && loginModel.Password == "password")
            {
                var token = _authorizationService.GenerateJwtToken();
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // 在開發環境中可以設置為 false，生產環境中應設置為 true 
                    SameSite = SameSiteMode.None, // 確保跨站點 cookie 可以被發送
                };
                Response.Cookies.Append("access_token", token, cookieOptions);

                return Ok(new { Token = token });
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
