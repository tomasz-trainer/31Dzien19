using P06Shop.Shared.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace P06Shop.Shared.Services.AuthService
{
    public interface IAuthService
    {
        Task<ServiceResponse<string>> Login(string email, string password);

        Task<ServiceResponse<int>> Register(User user, string password);

        Task<ServiceResponse<bool>> ChangePassword(int userId, string newPassword);
    }
}
