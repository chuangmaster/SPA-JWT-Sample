namespace SPA_JWT_Sample.Models.Services.Responses
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
