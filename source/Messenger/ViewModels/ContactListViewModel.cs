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
using System.Windows.Media;

namespace Messenger.ViewModels
{
    public class ContactListViewModel : BaseViewModel
    {
        private ObservableCollection<Contact> _Contacts;
        public ObservableCollection<Contact> Contacts
        {
            get => _Contacts;
            set
            {
                _Contacts = value;
                OnPropertyChanged(nameof(Contacts));
            }
        }

        public ICommand DeleteContactCommand { get; set; }
        public ICommand BlockContactCommand { get; set; }

        private Services.UserInfo _UserInfo;
        public ContactListViewModel()
        {
            Contacts = new ObservableCollection<Contact>();

            DeleteContactCommand = new RelayCommand<Object>(c => DeleteContact(c));
            BlockContactCommand = new RelayCommand<Object>(c => BlockContact(c));

            var userService = ServiceLocator.GetService<IUserService>();
            if (userService.User != null)
            {
                _UserInfo = userService.User;
                LoadContacts();
            }
        }

        public void LoadContacts()
        {
            var client = new JsonServiceClient(ConfigurationManager.AppSettings["APIUrl"]);
            client.BearerToken = _UserInfo.AuthToken;

            var getContacts = new DTOs.GetContacts();

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
                            Contacts.Add(newContact);
                        });
                    });
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err);
            }
        }
        private void DeleteContact(Object contactObj)
        {

            if (contactObj is Contact contact)
            {
                var client = new JsonServiceClient(ConfigurationManager.AppSettings["APIUrl"]);
                client.BearerToken = _UserInfo.AuthToken;

                var deleteContact = new DTOs.DeleteContact
                {
                    UserID = contact.UserID,
                };

                try
                {
                    client.Post(deleteContact);
                    Contacts.Remove(contact);
                }
                catch (Exception err)
                {
                    Debug.WriteLine($"{err}");
                }
            }
        }

        private void BlockContact(Object contactObj)
        {

            if (contactObj is Contact contact)
            {
                var client = new JsonServiceClient(ConfigurationManager.AppSettings["APIUrl"]);
                client.BearerToken = _UserInfo.AuthToken;

                var blockContact = new DTOs.BlockContact
                {
                    UserID = contact.UserID,
                };

                try
                {
                    client.Post(blockContact);
                    Contacts.Remove(contact);
                }
                catch (Exception err)
                {
                    Debug.WriteLine($"{err}");
                }
            }
        }
    }
    public class Contact
    {
        public int UserID { get; set; }
        public ImageSource UserAvatar { get; set; }
        public string UserFullName { get; set; }
    }
}