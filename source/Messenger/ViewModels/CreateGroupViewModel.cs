using HandyControl.Tools.Command;
using Messenger.Services;
using Messenger.Utils;
using Microsoft.Win32;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
        public string GroupName { get; set; }

        private ImageSource _Avatar;
        public ImageSource Avatar
        {
            get => _Avatar; set
            {
                _Avatar = value;
                OnPropertyChanged(nameof(Avatar));
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
        public ICommand SelectAvatarCommand { get; }
        public ICommand RemoveAvatarCommand { get; }
        public ICommand NewGroupCommand { get; }
        public event Action Close;
        private Services.UserInfo _UserInfo;
        public CreateGroupViewModel()
        {
            GroupContacts = new ObservableCollection<GroupContact>();
            ChangeSelectCommand = new RelayCommand<Object>(c => ChangeSelect(c));
            SelectAvatarCommand = new RelayCommand(_ => SelectAvatar());
            RemoveAvatarCommand = new RelayCommand(_ => RemoveAvatar());
            NewGroupCommand = new RelayCommand(_ => NewGroup());

            var userService = ServiceLocator.GetService<IUserService>();
            if (userService.User != null)
            {
                _UserInfo = userService.User;
                LoadContacts();
            }
        }

        private void NewGroup()
        {
            var userService = ServiceLocator.GetService<IUserService>();

            var client = new JsonServiceClient(ConfigurationManager.AppSettings["APIUrl"]);
            client.BearerToken = userService.User.AuthToken;

            List<int> participants = new List<int> { };
            foreach (var gc in GroupContacts)
            {
                if (gc.IsSelected)
                    participants.Add(gc.UserID);
            }

            var newGroup = new DTOs.NewGroup
            {
                GroupTitle = GroupName,
                GroupAvatar = ImageSourceToPngByteArray(Avatar),
                Participants = participants.ToArray()
            };

            try
            {
                var newGroupReponse = client.Post(newGroup);
            }
            catch (Exception err)
            {
                Debug.WriteLine(err);
            }

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Close?.Invoke();
            });
        }

        public byte[] ImageSourceToPngByteArray(ImageSource imageSource)
        {
            if (imageSource == null)
                return new byte[] { 0 };

            // Ép kiểu ImageSource thành BitmapSource
            BitmapSource bitmapSource = imageSource as BitmapSource;
            if (bitmapSource == null)
                throw new ArgumentException("ImageSource phải là BitmapSource.");

            // Sử dụng PngBitmapEncoder để mã hóa thành PNG
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

            using (MemoryStream stream = new MemoryStream())
            {
                encoder.Save(stream);
                return stream.ToArray(); // Trả về mảng byte định dạng PNG
            }
        }

        private void SelectAvatar()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Chọn Ảnh",
                Filter = "Hình ảnh (*.jpg;*.png;*.jpeg)|*.jpg;*.png;*.jpeg"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                Avatar = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }
        private void RemoveAvatar()
        {
            Avatar = null;
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
