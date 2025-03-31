<<<<<<< HEAD
﻿using Messenger.Services;
=======
﻿using APIServer.Models;
using Messenger.Services;
>>>>>>> 4ffd7326e3587c336809acb8219698eda1a94f89
using Messenger.Utils;
using ServiceStack;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
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
        private Services.UserInfo _UserInfo;
        public ContactListViewModel()
        {
            Contacts = new ObservableCollection<Contact>();

            var userService = ServiceLocator.GetService<IUserService>();
            if (userService.User != null)
            {
                _UserInfo = userService.User;
                LoadContact();
            }
        }

        public void LoadContact()
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
                            UserAvatar = LoadImage.LoadImageFromUrl(contact.Avatar),
                            UsertFullName = contact.LastName + " " + contact.FirstName,
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
    }
    public class Contact
    {
        public int UserID { get; set; }
        public ImageSource UserAvatar { get; set; }
        public string UsertFullName { get; set; }
    }
}