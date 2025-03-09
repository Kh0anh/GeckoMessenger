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
        public MainSettingsViewModel()
        {
            SwitchToMainCommand = new RelayCommand(_ => SwitchToMain());
            SwitchToEditProfileCommand = new RelayCommand(_ => SwitchToEditProfile());
            SwitchToEditPrivacyCommand = new RelayCommand(_ => SwitchToEditPrivacy());

            SwitchToMain();
        }
        public void NavigationTo(object view)
        {
            if (view != null)
            {
                _currentView = view;
                OnPropertyChanged(nameof(CurrentView));
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
    }
}
