using Messenger.Services;
using Messenger.Views;
using Messenger.Views.Settings;
using System.Windows.Input;


namespace Messenger.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly MainViewModel _MainViewModel;

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

        public Services.UserInfo UserInfo { get; set; }

        public ICommand SwitchToInboxCommand { get; }
        public ICommand SwitchToContactCommand { get; }
        public ICommand OpenSettingsCommand { get; }
        public ICommand OpenProfileViewCommand { get; }
        public ICommand LogoutCommand { get; }

        public HomeViewModel(MainViewModel mainViewModel)
        {
            _MainViewModel = mainViewModel;

            SwitchToInboxCommand = new RelayCommand(_ => SwitchToInbox());
            SwitchToContactCommand = new RelayCommand(_ => SwitchToContact());
            OpenSettingsCommand = new RelayCommand(_ => OpenSettings());
            OpenProfileViewCommand = new RelayCommand(_ => OpenProfileView());
            LogoutCommand = new RelayCommand(_ => Logout());

            var userService = ServiceLocator.GetService<IUserService>();
            UserInfo = userService.User;

            SwitchToInbox();
        }

        private void Logout()
        {
            var userService = ServiceLocator.GetService<IUserService>();
            userService.ClearUser();

            _MainViewModel.SwitchToLoginCommand.Execute(null);
        }

        public void NavigationTo(object view)
        {
            if (view != null)
            {
                _currentView = view;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        private void SwitchToContact()
        {
            NavigationTo(new Views.Contact.MainContactUserControl(new MainContactViewModel()));
        }

        private void SwitchToInbox()
        {
            NavigationTo(new Views.Inbox.InboxUserControl(new InboxViewModel()));
        }

        private void OpenSettings()
        {
            MainSettingsView settingsView = new MainSettingsView(new MainSettingsViewModel());
            settingsView.ShowDialog();
        }

        private void OpenProfileView()
        {
            ProfileView settingsView = new ProfileView(new ProfileViewModel(UserInfo.UserID));
            settingsView.ShowDialog();
        }
    }
}
