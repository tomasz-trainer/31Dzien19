using P06Shop.Shared.Auth;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http.Json;
using System.Text;

namespace P06Shop.Shared.Services.AuthService
{
    public class AuthApiClientService : IAuthApiClientService
    {
        private readonly HttpClient _httpClient;

        public AuthApiClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ServiceResponse<bool>> ChangePassword(string newPassword)
        {
            var result = await _httpClient.PostAsJsonAsync("api/auth/change-password", newPassword);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<string>> Login(UserLoginDto userLoginDto)
        {
            var result = await _httpClient.PostAsJsonAsync("api/auth/login", userLoginDto);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<string>>();
        }

        public async Task<ServiceResponse<int>> Register(UserRegisterDto userRegisterDto)
        {
            var result = await _httpClient.PostAsJsonAsync("api/auth/register", userRegisterDto);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<int>>();
        }
    }
}
