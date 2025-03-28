using Messenger.Views.Settings;
using System.Windows.Input;

namespace Messenger.ViewModels
{
    public class MainSettingsViewModel : BaseViewModel
    {
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
        public ICommand SwitchToMainCommand { get; }
        public ICommand SwitchToEditProfileCommand { get; }
        public ICommand SwitchToEditPrivacyCommand { get; }
        public ICommand SwitchToChangePasswordCommand { get; }
        public MainSettingsViewModel()
        {
            SwitchToMainCommand = new RelayCommand(_ => SwitchToMain());
            SwitchToEditProfileCommand = new RelayCommand(_ => SwitchToEditProfile());
            SwitchToEditPrivacyCommand = new RelayCommand(_ => SwitchToEditPrivacy());
            SwitchToChangePasswordCommand = new RelayCommand(_ => SwitchToChangePassword());
            SwitchToMain();
        }
        public void NavigationTo(object view)
        {
            if (view != null)
            {
                CurrentView = view;
            }
        }
        private void SwitchToMain()
        {
            NavigationTo(new SettingsUserControl(new SettingsViewModel()));
        }
        private void SwitchToEditProfile()
        {
            NavigationTo(new EditProfileUserControl(new EditProfileViewModel()));
        }
        private void SwitchToEditPrivacy()
        {
            NavigationTo(new EditPrivacyUserControl(new EditPrivacyViewModel()));
        }
        private void SwitchToChangePassword()
        {
            NavigationTo(new ChangePasswordUserControl(new ChangePasswordViewModel()));
        }
    }
}
