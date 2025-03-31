using HandyControl.Tools.Command;
using Messenger.Services;
using Messenger.Utils;
using ServiceStack;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Messenger.ViewModels
{
    public class CreateGroupViewModel : BaseViewModel
    {
        public int SelectedCount
        {
            get
            {
                return GroupContacts.Count(c => c.IsSelected);
            }
        }
        private ObservableCollection<GroupContact> _GroupContacts;
        public ObservableCollection<GroupContact> GroupContacts
        {
            get => _GroupContacts;
            set
            {
                _GroupContacts = value;
                OnPropertyChanged(nameof(GroupContacts));
            }
        }
        public ICommand ChangeSelectCommand { get; }
        private Services.UserInfo _UserInfo;
        public CreateGroupViewModel()
        {
            GroupContacts = new ObservableCollection<GroupContact>();
            ChangeSelectCommand = new RelayCommand<Object>(c => ChangeSelect(c));

            var userService = ServiceLocator.GetService<IUserService>();
            if (userService.User != null)
            {
                _UserInfo = userService.User;
                LoadContacts();
            }
        }
        private void ChangeSelect(object groupContactObj)
        {
            if (groupContactObj is GroupContact groupContact)
            {
                groupContact.IsSelected = !groupContact.IsSelected;
                OnPropertyChanged(nameof(SelectedCount));
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
                        var newGroupContact = new GroupContact
                        {
                            UserID = contact.UserID,
                            UserAvatar = LoadImage.LoadImageFromUrl(ConfigurationManager.AppSettings["APIUrl"] + contact.Avatar),
                            UserFullName = contact.LastName + " " + contact.FirstName,
                        };

                        App.Current.Dispatcher.Invoke(() =>
                        {
                            GroupContacts.Add(newGroupContact);
                        });
                    });
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err);
            }
        }
        public class GroupContact : BaseViewModel
        {
            public int UserID { get; set; }
            public ImageSource UserAvatar { get; set; }
            public string UserFullName { get; set; }
            private bool _isSelected = false;
            public bool IsSelected
            {
                get => _isSelected;
                set
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }
    }
}
