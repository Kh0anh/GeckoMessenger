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
    public class MainViewModel : BaseViewModel
    {

        private object _currentView;

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        public ICommand SwitchToRegisterCommand { get; }
        public ICommand SwitchToLoginCommand { get; }

        public MainViewModel()
        {
            SwitchToRegisterCommand = new RelayCommand(_ => SwitchToRegister());
            SwitchToLoginCommand = new RelayCommand(_ => SwitchToLogin());

            Task.Run(() =>
            {
                var userService = ServiceLocator.GetService<IUserService>();

                if (userService.User == null)
                {
                    Application.Current.Dispatcher.Invoke(SwitchToLogin);
                    return;
                }

                var client = new JsonServiceClient(ConfigurationManager.AppSettings["APIUrl"]);
                client.BearerToken = userService.User.AuthToken;

                var getInfo = new DTOs.GetInfo
                {
                    UserID = userService.User.UserID,
                };

                try
                {
                    var response = client.Get(getInfo);

                    if (!string.IsNullOrEmpty(response.Error))
                    {
                        throw new Exception(response.Message);
                    }

                    userService.User = new Services.UserInfo
                    {
                        UserID = response.Data.UserID,
                        Username = response.Data.Username,
                        FullName = response.Data.FirstName + " " + response.Data.LastName,
                        AvatarBase64 = LoadImage.LoadImageFromUrlAsBase64(ConfigurationManager.AppSettings["APIUrl"] + response.Data.Avatar),
                        AuthToken = userService.User.AuthToken,
                    };

                    Debug.WriteLine($"UserID={userService.User.UserID}\tUsername={userService.User.Username}\tFullName={userService.User.FullName}\tAvatar={userService.User.Avatar}\tAuthToken={userService.User.AuthToken}");

                    userService.SaveUser();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        NavigationTo(new HomeUserControl(new HomeViewModel(this)));
                    });
                }
                catch (Exception err)
                {
                    Debug.WriteLine(err);
                    Application.Current.Dispatcher.Invoke(SwitchToLogin);
                }
            });
        }

        public void NavigationTo(object view)
        {
            if (view != null)
            {
                CurrentView = view;
            }
        }

        private void SwitchToRegister()
        {
            NavigationTo(new RegisterUserControl(new RegisterViewModel(this)));
        }

        private void SwitchToLogin()
        {
            NavigationTo(new LoginUserControl(new LoginViewModel(this)));
        }
    }
}
