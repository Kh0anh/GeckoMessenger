using Messenger.Services;
using Messenger.Views;
using System.Windows.Input;

namespace Messenger.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IUserService _userService;

        private object _currentView;

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public ICommand SwitchToRegisterCommand { get; }
        public ICommand SwitchToLoginCommand { get; }

        public MainViewModel()
        {
            _userService = ServiceLocator.GetService<IUserService>();
            SwitchToRegisterCommand = new RelayCommand(_ => SwitchToRegister());
            SwitchToLoginCommand = new RelayCommand(_ => SwitchToLogin());

            if (_userService.User == null)
            {
                SwitchToLogin();
            }
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
            NavigationTo(new RegisterUserControl(new RegisterViewModel()));
        }

        private void SwitchToLogin()
        {
            NavigationTo(new LoginUserControl(new LoginViewModel(_userService, this)));
        }
    }
}
