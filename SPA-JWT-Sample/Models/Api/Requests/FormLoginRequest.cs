namespace SPA_JWT_Sample.Models.Api.Requests
{
    public class FormLoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
    }
}
