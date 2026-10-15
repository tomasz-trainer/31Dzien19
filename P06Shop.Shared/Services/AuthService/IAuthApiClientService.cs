using P06Shop.Shared.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace P06Shop.Shared.Services.AuthService
{
    public interface IAuthApiClientService
    {
        Task<ServiceResponse<string>> Login(UserLoginDto userLoginDto);
        
        Task<ServiceResponse<int>> Register(UserRegisterDto userRegisterDto);

        Task<ServiceResponse<bool>> ChangePassword(string newPassword);
    }
}
