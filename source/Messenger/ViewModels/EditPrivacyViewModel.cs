using APIServer.Models;
using Messenger.Services;
using ServiceStack;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;


namespace Messenger.ViewModels
{
    public class EditPrivacyViewModel : BaseViewModel
    {
        public ObservableCollection<PrivacyConfig> PrivacyOptions { get; set; }

        private PrivacyConfig _SelectedActiveStatus;
        public PrivacyConfig SelectedActiveStatus
        {
            get => _SelectedActiveStatus;
            set
            {
                _SelectedActiveStatus = value;
                SavePrivacy?.Execute(_SelectedActiveStatus);
            }
        }

        public byte ActiveStatus { get; set; }
        public ICommand SavePrivacy { get; set; }

        private UserInfo _UserInfo { get; set; }
        public EditPrivacyViewModel()
        {
            SavePrivacy = new RelayCommand(_ => EditPrivacy());
            PrivacyOptions = new ObservableCollection<PrivacyConfig>
                {
                    new PrivacyConfig { PrivacyTitle = "Ẩn", PrivacyName=  "NOBODY"},
                    new PrivacyConfig { PrivacyTitle = "Liên hệ", PrivacyName=  "CONTACT"},
                    new PrivacyConfig { PrivacyTitle = "Công khai", PrivacyName=  "PUBLIC" }
                };

            var userService = ServiceLocator.GetService<IUserService>();
            if (userService.User == null)
            {
                return;
            }

            _UserInfo = userService.User;
            LoadUserPrivacy();
        }

        public void EditPrivacy()
        {
            var client = new JsonServiceClient(ConfigurationManager.AppSettings["APIUrl"]);
            client.BearerToken = _UserInfo.AuthToken;

            var updatePrivacy = new DTOs.UpdatePrivacy
            {
                ActiveStatus = SelectedActiveStatus.PrivacyName,
            };

            var response = client.Put(updatePrivacy);
            if (response.Error != null)
            {
                Debug.WriteLine($"Error updating user privacy: {response.Message}");
                return;
            }
            else
            {
                Debug.WriteLine("Privacy updated successfully!");
            }
        }

        public void LoadUserPrivacy()
        {
            var client = new JsonServiceClient(ConfigurationManager.AppSettings["APIUrl"]);
            client.BearerToken = _UserInfo.AuthToken;

            var getPrivacy = new DTOs.GetPrivacy { UserID = _UserInfo.UserID };
            var response = client.Get(getPrivacy);

            if (response.Error != null)
            {
                Debug.WriteLine($"{response.Error}");
                return;
            }

            _SelectedActiveStatus = PrivacyOptions.SingleOrDefault<PrivacyConfig>(p => p.PrivacyName == response.Data.ActiveStatus);
            Debug.WriteLine(_SelectedActiveStatus.PrivacyName);
            OnPropertyChanged(nameof(SelectedActiveStatus));
        }
    }
    public class PrivacyConfig
    {
        public string PrivacyTitle { get; set; }
        public string PrivacyName { get; set; }
    }
}
