using Microsoft.IdentityModel.Tokens;

namespace SPA_JWT_Sample.Services.Interfaces
{
    public interface IAuthorizationService
    {
        string GenerateJwtToken();

        string GenerateRefreshToken();

        /// <summary>
        /// 取得 RSA 公鑰（僅含公鑰參數），供 JWT 驗證用
        /// </summary>
        RsaSecurityKey GetPublicKey();

        /// <summary>
        /// 取得 JWK 格式的公鑰 JSON 字串
        /// </summary>
        string GetPublicKeyJwkJson();
    }
}
