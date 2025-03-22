using Messenger.Services;
using Messenger.Utils;
using Messenger.Views;
using ServiceStack;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Messenger.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly MainViewModel _MainViewModel;

        public string Username { get; set; }
        public string Password { get; set; }
        public string Error { get; set; }

        public ICommand LoginCommand { get; }
        public LoginViewModel(MainViewModel mainViewModel)
        {

            _MainViewModel = mainViewModel;

            LoginCommand = new RelayCommand(_ => Login());
        }

        private void Login()
        {
            var username = Username;
            var password = Password;
            Task.Run(() =>
            {
                var client = new JsonServiceClient(ConfigurationManager.AppSettings["APIUrl"]);

                var login = new DTOs.Login();
                if (Regex.IsMatch(username, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                {
                    login.Email = username;
                    login.Password = password;
                }
                else
                {
                    login.Username = username;
                    login.Password = password;
                }

                string jwtToken;
                int userID;

                try
                {
                    var response = client.Post(login);

                    if (!string.IsNullOrEmpty(response.Error))
                    {
                        Error = response.Message;
                        return;
                    }

                    jwtToken = response.Token;
                    userID = response.UserID;
                }
                catch (WebServiceException err)
                {
                    if (err.ResponseDto is DTOs.RegisterResponse errorResponse)
                    {
                        Error = errorResponse.Message;
                    }
                    Debug.WriteLine(err);
                    return;
                }

                client.BearerToken = jwtToken;

                var getInfo = new DTOs.GetInfo
                {
                    UserID = userID
                };

                try
                {
                    var response = client.Get(getInfo);

                    if (!string.IsNullOrEmpty(response.Error))
                    {
                        Error = response.Message;
                        return;
                    }

                    var userService = ServiceLocator.GetService<IUserService>();
                    userService.User = new Services.UserInfo
                    {
                        UserID = response.Data.UserID,
                        Username = response.Data.Username,
                        FullName = response.Data.FirstName + " " + response.Data.LastName,
                        AvatarBase64 = LoadImage.LoadImageFromUrlAsBase64(ConfigurationManager.AppSettings["APIUrl"] + response.Data.Avatar),
                        AuthToken = jwtToken,
                    };

                    Debug.WriteLine($"UserID={userService.User.UserID}\tUsername={userService.User.Username}\tFullName={userService.User.FullName}\tAvatar={userService.User.Avatar}\tAuthToken={userService.User.AuthToken}");

                    userService.SaveUser();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _MainViewModel.NavigationTo(new HomeUserControl(new HomeViewModel(_MainViewModel)));
                    });
                }
                catch (Exception err)
                {
                    Debug.WriteLine(err);
                    return;
                }
            });
        }
    }
}
