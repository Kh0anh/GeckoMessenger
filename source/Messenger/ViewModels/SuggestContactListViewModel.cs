using System.Collections.Generic;

namespace Messenger.ViewModels
{
    public class SuggestContactListViewModel : BaseViewModel
    {
        private List<Contact> _SuggestContacts;
        public List<Contact> SuggestContacts
        {
            get => _SuggestContacts;
            set
            {
                _SuggestContacts = value;
                OnPropertyChanged();
            }
        }
        public SuggestContactListViewModel()
        {
            SuggestContacts = new List<Contact>()
            {
                new Contact
                {
                    Avatar = "User0",
                    FullName = "Jerry Vo",
                },
                new Contact
                {
                    Avatar = "User1",
                    FullName = "Jerry Nguyen",
                },
                new Contact
                {
                    Avatar = "DownMan",
                    FullName = "Down Main Anime",
                }
            };
        }
    }
}
