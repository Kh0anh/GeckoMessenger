using Messenger.Views.Settings;
using System.Windows.Input;

namespace Messenger.ViewModels
{
    public class HomeViewModel : BaseViewModel
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

        public ICommand SwitchToInboxCommand { get; }
        public ICommand SwitchToContactCommand { get; }
        public ICommand OpenSettingsCommand { get; }

        public HomeViewModel()
        {
            SwitchToInboxCommand = new RelayCommand(_ => SwitchToInbox());
            SwitchToContactCommand = new RelayCommand(_ => SwitchToContact());
            OpenSettingsCommand = new RelayCommand(_ => OpenSettings());

            SwitchToInbox();
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
            NavigationTo(new Views.Contact.ContactUserControl(new ContactViewModel()));
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
    }
}
