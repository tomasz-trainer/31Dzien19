using P06Shop.Shared;
using P06Shop.Shared.Auth;

namespace P05Shop.API.Services
{
    public interface IAuthService
    {
        Task<ServiceResponse<string>> Login(string email, string password);

        Task<ServiceResponse<int>> Register(User user, string newPassword);

        Task<ServiceResponse<bool>> ChangePassword(int userId, string newPassword);
    }
}
