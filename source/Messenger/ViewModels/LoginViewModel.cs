using Messenger.Services;
using Messenger.Views;
using System;
using System.Windows.Input;

namespace Messenger.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly MainViewModel _mainViewModel;

        public string username { get; set; }
        public string password { get; set; }

        public ICommand LoginCommand { get; }
        public LoginViewModel(IUserService userService, MainViewModel mainViewModel)
        {
            _userService = userService;
            _mainViewModel = mainViewModel;

            LoginCommand = new RelayCommand(_ => Login());
        }

        private void Login()
        {
            if (username == "test" && password == "test")
            {
                Console.WriteLine("login");
                _mainViewModel.NavigationTo(new HomeUserControl(new HomeViewModel()));
            }
        }
    }
}
