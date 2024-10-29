using SPA_JWT_Sample.Models.Services.Responses;

namespace SPA_JWT_Sample.Services.Interfaces
{
    public interface IUserService
    {
        //Get User by id method
        UserDTO GetUserById(string id);

    }
}
