using System.Security.Claims;

namespace SPA_JWT_Sample.Services.Interfaces
{
    public interface IAzureExtraIdService
    {
        /// <summary>
        /// 驗證 Azure AD Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<ClaimsPrincipal> ValidateAzureAdToken(string token);
    }
}
