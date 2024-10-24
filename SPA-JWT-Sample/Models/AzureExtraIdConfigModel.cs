namespace SPA_JWT_Sample.Models
{
    public class AzureExtraIdConfigModel
    {
        /// <summary>
        /// Azure AD 應用程式的 ClientId
        /// </summary>
        public required string ApplicationId { get; set; }

     
        /// <summary>
        /// Azure AD 的 TenantId
        /// </summary>
        public required string TenantId { get; set; }
    }
}
