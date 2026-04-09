using Microsoft.AspNetCore.Mvc;
using SPA_JWT_Sample.Models.Api.Requests;
using SPA_JWT_Sample.Models.Api.Responses;
using SPA_JWT_Sample.Services.Interfaces;

namespace SPA_JWT_Sample.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAzureExtraIdService _azureExtraIdService;
        private readonly IUserService _userService;

        public AuthenticationController(
            IAuthorizationService authorizationService,
            IAzureExtraIdService azureExtraIdService,
            IUserService userService)
        {
            _authorizationService = authorizationService;
            _azureExtraIdService = azureExtraIdService;
            _userService = userService;
        }

        /// <summary>
        /// 使用 Azure AD token 驗證，並以 RS256 產生 JWT 回傳至 Response body
        /// 前端收到後，後續請求需放入 Authorization: Bearer {token} header
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginModel)
        {
            var response = new LoginResponse();
            var validResult = await _azureExtraIdService.ValidateAzureAdToken(loginModel.TokenId);
            if (validResult == null)
            {
                response.Message = "Invalid token";
                return BadRequest(response);
            }

            var userId = validResult.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";
            var user = _userService.GetUserById(userId);
            if (user == null)
            {
                response.Message = "User not found";
                return BadRequest(response);
            }

            response.AccessToken  = _authorizationService.GenerateJwtToken();
            response.RefreshToken = _authorizationService.GenerateRefreshToken();
            response.ExpiresIn    = 1800;
            return Ok(response);
        }

        /// <summary>
        /// 使用帳號密碼驗證（示範用），以 RS256 產生 JWT 回傳至 Response body
        /// </summary>
        [HttpPost("token")]
        public IActionResult Token([FromBody] FormLoginRequest loginModel)
        {
            if (loginModel.Username != "admin" || loginModel.Password != "password")
                return Unauthorized();

            var response = new LoginResponse
            {
                AccessToken  = _authorizationService.GenerateJwtToken(),
                RefreshToken = _authorizationService.GenerateRefreshToken(),
                ExpiresIn    = 1800
            };
            return Ok(response);
        }

        /// <summary>
        /// 回傳 RSA 公鑰（JWK 格式）
        /// 外部服務可從此端點取得公鑰，自行離線驗證 JWT 簽章
        /// </summary>
        [HttpGet("public-key")]
        public IActionResult PublicKey()
        {
            var jwk = _authorizationService.GetPublicKeyJwkJson();
            return Content(jwk, "application/json");
        }
    }
}

