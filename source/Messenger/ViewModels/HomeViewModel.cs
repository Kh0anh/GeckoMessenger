using Messenger.Services;
using Messenger.Views;
using Messenger.Views.Contact;
using Messenger.Views.Inbox;
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
        public ICommand CreateGroupCommand { get; }
        public ICommand LogoutCommand { get; }

        private InboxUserControl InboxUserControl = new InboxUserControl(new InboxViewModel());
        private MainContactUserControl MainContactUserControl = new Views.Contact.MainContactUserControl(new MainContactViewModel());

        public HomeViewModel(MainViewModel mainViewModel)
        {
            _MainViewModel = mainViewModel;

            SwitchToInboxCommand = new RelayCommand(_ => SwitchToInbox());
            SwitchToContactCommand = new RelayCommand(_ => SwitchToContact());
            OpenSettingsCommand = new RelayCommand(_ => OpenSettings());
            OpenProfileViewCommand = new RelayCommand(_ => OpenProfileView());
            CreateGroupCommand = new RelayCommand(_ => CreateGroup());
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
        private void CreateGroup()
        {
            CreateGroup settingsView = new CreateGroup(new CreateGroupViewModel());
            settingsView.ShowDialog();
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
            NavigationTo(MainContactUserControl);
        }

        private void SwitchToInbox()
        {
            NavigationTo(InboxUserControl);
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
