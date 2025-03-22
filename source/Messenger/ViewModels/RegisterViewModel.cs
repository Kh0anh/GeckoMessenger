using Messenger.Services;
using Messenger.Utils;
using Messenger.Views;
using ServiceStack;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Messenger.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public DateTime Birthday { get; set; } = new DateTime(2004, 10, 11);

        public string _Error;
        public string Error { get { return _Error; } set { _Error = value; OnPropertyChanged(nameof(Error)); } }

        public ICommand RegisterCommand { get; set; }
        public MainViewModel _MainViewModel { get; set; }
        public RegisterViewModel(MainViewModel mainViewModel)
        {
            _MainViewModel = mainViewModel;

            RegisterCommand = new RelayCommand(_ => Register());
        }

        public void Register()
        {
            var username = Username;
            var firstName = FirstName;
            var lastName = LastName;
            var email = Email;
            var phoneNumber = PhoneNumber;
            var password = Password;
            var birthday = Birthday;
            Reset();

            Task.Run(() =>
            {
                var client = new JsonServiceClient(ConfigurationManager.AppSettings["APIUrl"]);

                var register = new DTOs.Register
                {
                    Username = username,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    Birthday = birthday,
                    Password = password,
                    FirstName = firstName,
                    LastName = lastName,
                };

                string jwtToken;
                int userID;

                try
                {
                    var response = client.Post(register);

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

        public void Reset()
        {
            Username = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            PhoneNumber = string.Empty;
            Password = string.Empty;
            Birthday = new DateTime(2004, 10, 11);
            Error = string.Empty;
        }
    }
}
