using System.Collections.Generic;

namespace Messenger.ViewModels
{
    public class BlockListViewModel : BaseViewModel
    {
        private List<Contact> _BlockContacts;
        public List<Contact> BlockContacts
        {
            get => _BlockContacts;
            set
            {
                _BlockContacts = value;
                OnPropertyChanged();
            }
        }
        public BlockListViewModel()
        {
            //BlockContacts = new List<Contact>()
            //{
            //    new Contact
            //    {
            //        Avatar = "User0",
            //        FullName = "Jerry Vo",
            //    },
            //    new Contact
            //    {
            //        Avatar = "User1",
            //        FullName = "Jerry Nguyen",
            //    },
            //    new Contact
            //    {
            //        Avatar = "DownMan",
            //        FullName = "Down Main Anime",
            //    }
            //};
        }
    }
}
