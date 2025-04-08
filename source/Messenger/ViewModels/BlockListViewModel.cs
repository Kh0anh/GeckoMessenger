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
    public class BlockListViewModel : BaseViewModel
    {
        private ObservableCollection<Contact> _BlockContacts;
        public ObservableCollection<Contact> BlockContacts
        {
            get => _BlockContacts;
            set
            {
                _BlockContacts = value;
                OnPropertyChanged();
            }
        }
        public ICommand AddBackContactCommand { get; set; }
        public ICommand UnblockContactCommand { get; set; }

        private Services.UserInfo _UserInfo;
        public BlockListViewModel()
        {
            BlockContacts = new ObservableCollection<Contact>();

            AddBackContactCommand = new RelayCommand<Object>(c => AddBack(c));
            UnblockContactCommand = new RelayCommand<Object>(c => UnblockBack(c));

            var userService = ServiceLocator.GetService<IUserService>();
            if (userService.User != null)
            {
                _UserInfo = userService.User;
                LoadBlockContacts();
            }
        }

        private void AddBack(Object contactObj)
        {
            if (contactObj is Contact contact)
            {
                var client = new JsonServiceClient(ConfigurationManager.AppSettings["APIUrl"]);
                client.BearerToken = _UserInfo.AuthToken;

                var unblockContact = new DTOs.UnblockContact
                {
                    UserID = contact.UserID,
                };

                try
                {
                    client.Post(unblockContact);
                    BlockContacts.Remove(contact);
                }
                catch (Exception err)
                {
                    Debug.WriteLine($"{err}");
                }
            }
        }

        private void UnblockBack(Object contactObj)
        {
            if (contactObj is Contact contact)
            {
                var client = new JsonServiceClient(ConfigurationManager.AppSettings["APIUrl"]);
                client.BearerToken = _UserInfo.AuthToken;

                var unblockContact = new DTOs.DeleteContact
                {
                    UserID = contact.UserID,
                };

                try
                {
                    client.Post(unblockContact);
                    BlockContacts.Remove(contact);
                }
                catch (Exception err)
                {
                    Debug.WriteLine($"{err}");
                }
            }
        }

        public void LoadBlockContacts()
        {
            var client = new JsonServiceClient(ConfigurationManager.AppSettings["APIUrl"]);
            client.BearerToken = _UserInfo.AuthToken;

            var getContacts = new DTOs.GetBlockContacts();

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
                            BlockContacts.Add(newContact);
                        });
                    });
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err);
            }
        }
    }
}
