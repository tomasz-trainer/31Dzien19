using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using P06Shop.Shared;
using P06Shop.Shared.Auth;
using P06Shop.Shared.Services.AuthService;
using System;
using System.Collections.Generic;
using System.Text;

namespace P03WeatherForecastWPF.Client.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {


        private readonly IAuthApiClientService _authApiClientService;
        private readonly IMeesageDialogService _meesageDialogService;
        public static string Token { get; set; }


        public LoginViewModel(IAuthApiClientService authApiClientService, IMeesageDialogService meesageDialogService)
        {
            _authApiClientService = authApiClientService;
            _meesageDialogService= meesageDialogService;
        }

        [ObservableProperty]
        private UserLoginDto userLoginDto = new UserLoginDto();


        [RelayCommand]
        public async Task Login()
        {
            try
            {
                var result = await _authApiClientService.Login(UserLoginDto);

                if (result.Success)
                {
                    Token = result.Data;
                }
                else
                {
                    _meesageDialogService.ShowMessage(result.Message);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            

        }

    }
}
