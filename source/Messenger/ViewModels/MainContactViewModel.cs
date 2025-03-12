using System.Windows.Input;

namespace Messenger.ViewModels
{
    public class MainContactViewModel : BaseViewModel
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
        public ICommand SwitchToContactListCommand { get; }
        public ICommand SwitchToBlockListCommand { get; }
        public ICommand SwitchToSuggestContactCommand { get; }

        public MainContactViewModel()
        {
            SwitchToContactListCommand = new RelayCommand(_ => SwitchToContactList());
            SwitchToBlockListCommand = new RelayCommand(_ => SwitchToBlockList());
            SwitchToSuggestContactCommand = new RelayCommand(_ => SwitchToSuggestList());

            SwitchToContactList();
        }
        public void NavigationTo(object view)
        {
            if (view != null)
            {
                CurrentView = view;
            }
        }

        private void SwitchToContactList()
        {
            NavigationTo(new Views.Contact.ContactListUserControl(new ContactListViewModel()));
        }
        private void SwitchToBlockList()
        {
            NavigationTo(new Views.Contact.BlockListUserControl(new BlockListViewModel()));
        }
        private void SwitchToSuggestList()
        {
            NavigationTo(new Views.Contact.SuggestContactListUserControl(new SuggestContactListViewModel()));
        }
    }
}
