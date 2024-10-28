namespace SPA_JWT_Sample.Services.Interfaces
{
    public interface IAuthorizationService
    {
        string GenerateJwtToken();


        string GenerateRelyToken();
    }
}
