namespace SPA_JWT_Sample.Models.Services.Request
{
    public class AuthorizationConfigDTO
    {
        /// <summary>
        /// Secret
        /// </summary>
        public required string Secret { get; set; }

        /// <summary>
        /// Issuer
        /// </summary>
        public required string Issuer { get; set; }

        /// <summary>
        /// Audience
        /// </summary>
        public string? Audience { get; set; }
    }
}
