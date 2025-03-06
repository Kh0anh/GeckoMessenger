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

        public HomeViewModel()
        {
            SwitchToInboxCommand = new RelayCommand(_ => SwitchToInbox());
            SwitchToContactCommand = new RelayCommand(_ => SwitchToContact());

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
            //NavigationTo(new Views.Inbox.ChatUserControl());
        }

        public void SwitchToInbox()
        {
            NavigationTo(new Views.Inbox.InboxUserControl(new InboxViewModel()));
        }
    }
}
