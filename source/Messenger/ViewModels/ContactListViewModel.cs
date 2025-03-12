using System.Collections.Generic;

namespace Messenger.ViewModels
{
    public class ContactListViewModel : BaseViewModel
    {
        private List<Contact> _Contacts;
        public List<Contact> Contacts
        {
            get => _Contacts;
            set
            {
                _Contacts = value;
                OnPropertyChanged();
            }
        }
        public ContactListViewModel()
        {
            Contacts = new List<Contact>()
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
    public class Contact
    {
        public string Avatar { get; set; }
        public string FullName { get; set; }
    }
}