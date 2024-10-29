namespace SPA_JWT_Sample.Models.Api.Responses
{
    public class LoginResponse : BaseResponse
    {
        public string? RefreshToken { get; set; }
        
        public int ExpiresIn { get; set; }
        
        public string TokenType { get; set; } = "Bearer";
    }
}
