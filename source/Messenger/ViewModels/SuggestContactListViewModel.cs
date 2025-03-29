using HandyControl.Tools.Command;
using Messenger.Services;
using Messenger.Utils;
using ServiceStack;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Messenger.ViewModels
{
    public class SuggestContactListViewModel : BaseViewModel
    {
        private ObservableCollection<Contact> _SuggestContacts;
        public ObservableCollection<Contact> SuggestContacts
        {
            get => _SuggestContacts;
            set
            {
                _SuggestContacts = value;
                OnPropertyChanged();
            }
        }
        private Services.UserInfo _UserInfo;
        public ICommand AddContactSuggestCommand { get; set; }
        public ICommand RemoveContactSuggestCommand { get; set; }

        public SuggestContactListViewModel()
        {
            SuggestContacts = new ObservableCollection<Contact>();

            AddContactSuggestCommand = new RelayCommand<Object>(c => AddContactSuggest(c));
            RemoveContactSuggestCommand = new RelayCommand<Object>(c => RemoveContactSuggest(c));

            var userService = ServiceLocator.GetService<IUserService>();
            if (userService.User != null)
            {
                _UserInfo = userService.User;
                LoadSuggestContacts();
            }
        }

        public void LoadSuggestContacts()
        {
            var client = new JsonServiceClient(ConfigurationManager.AppSettings["APIUrl"]);
            client.BearerToken = _UserInfo.AuthToken;

            var getContacts = new DTOs.GetSuggestContact();

            try
            {
                var response = client.Get(getContacts);

                foreach (var contact in response.Contacts)
                {
                    Task.Run(() =>
                    {
                        var newContact = new Contact
                        {
                            UserID = contact.UserID,
                            UserAvatar = LoadImage.LoadImageFromUrl(ConfigurationManager.AppSettings["APIUrl"] + contact.Avatar),
                            UserFullName = contact.LastName + " " + contact.FirstName,
                        };

                        App.Current.Dispatcher.Invoke(() =>
                        {
                            SuggestContacts.Add(newContact);
                        });
                    });
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err);
            }
        }

        private void AddContactSuggest(Object contactObj)
        {
            if (contactObj is Contact contact)
            {
                var client = new JsonServiceClient(ConfigurationManager.AppSettings["APIUrl"]);
                client.BearerToken = _UserInfo.AuthToken;

                var addContact = new DTOs.AddContact
                {
                    UserID = contact.UserID,
                };

                try
                {
                    client.Post(addContact);
                    SuggestContacts.Remove(contact);
                }
                catch (Exception err)
                {
                    Debug.WriteLine($"{err}");
                }
            }
        }

        private void RemoveContactSuggest(Object contactObj)
        {
            if (contactObj is Contact contact)
            {
                SuggestContacts.Remove(contact);
            }
        }
    }
}