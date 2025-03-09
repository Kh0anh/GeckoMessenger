using System.Collections.Generic;

namespace Messenger.ViewModels
{
    public class ContactViewModel : BaseViewModel
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
        public ContactViewModel()
        {
            Contacts = new List<Contact>()
            {
                new Contact
                {
                    Avatar = "User0",
                    FullName = "Jerry Vo",
                    Username = "tomandbaby",
                    Email = "jerry@hotmail.com",
                    Bio = "Tìm FWB"
                },
                new Contact
                {
                    Avatar = "User1",
                    FullName = "Jerry Nguyen",
                    Username = "User001235421",
                    Bio = "Tìm gia huy"
                },
                 new Contact
                {
                    Avatar = "DownMan",
                    FullName = "Down Main Anime",
                    Username = "Down2004",
                    Email = "downtown@hotmail.com",
                    PhoneNumber = "0927283913",
                    Bio = "Alo"
                }
            };
        }
    }
    public class Contact
    {
        public string Avatar { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Bio { get; set; }

    }
}