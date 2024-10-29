using SPA_JWT_Sample.Models.Services.Responses;
using SPA_JWT_Sample.Services.Interfaces;

namespace SPA_JWT_Sample.Services
{
    public class UserService : IUserService
    {
        public UserDTO GetUserById(string id)
        {
            //show me sample
            return new UserDTO
            {
                Id = id,
                Username = "test",
                Email = "",
                Role = "Admin",
            };
        }
    }
}
