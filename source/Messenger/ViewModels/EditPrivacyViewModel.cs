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
        public ObservableCollection<Privacy> PrivacyOptions { get; set; }
        private Privacy _selectedActiveStatus;
        private bool _isSettingSelectedActiveStatus; // Flag to prevent infinite loop

        public Privacy SelectedActiveStatus
        {
            get => _selectedActiveStatus;
            set
            {
                if (_isSettingSelectedActiveStatus) return; // Prevent loop
                _selectedActiveStatus = value;
                OnPropertyChanged(nameof(SelectedActiveStatus));
                SavePrivacy?.Execute(_selectedActiveStatus);
            }
        }

        public byte ActiveStatus { get; set; }
        public ICommand SavePrivacy { get; set; }
        public EditPrivacyViewModel(int? userID = null)
        {
            SavePrivacy = new RelayCommand(_ => EditPrivacy());
            PrivacyOptions = new ObservableCollection<Privacy>
                {
                    new Privacy { PrivacyID = 1, PrivacyName = "Ẩn" },
                    new Privacy { PrivacyID = 2, PrivacyName = "Liên hệ" },
                    new Privacy { PrivacyID = 3, PrivacyName = "Công khai" }
                };
            var userService = ServiceLocator.GetService<IUserService>();
            if (userID != null)
            {
                LoadUserPrivacy(userID.Value);
            }
        }

        public void EditPrivacy()
        {
            var userService = ServiceLocator.GetService<IUserService>();
            var client = new JsonServiceClient(ConfigurationManager.AppSettings["APIUrl"]);
            client.BearerToken = userService.User.AuthToken;

            var updatePrivacy = new DTOs.UpdatePrivacy
            {
                ActiveStatus = SelectedActiveStatus?.PrivacyID ?? default(byte),
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
                LoadUserPrivacy(userService.User.UserID);
            }
        }

        public void LoadUserPrivacy(int userID)
        {
            var userService = ServiceLocator.GetService<IUserService>();
            var client = new JsonServiceClient(ConfigurationManager.AppSettings["APIUrl"]);
            client.BearerToken = userService.User.AuthToken;

            var getPrivacy = new DTOs.GetPrivacy { UserID = userID };
            var response = client.Get(getPrivacy);

            if (response == null)
            {
                Debug.WriteLine("Error: response is null.");
                return;
            }
            else
            {
                Debug.WriteLine($"response is not null = {(response.Data.ActiveStatus).GetType()}");
            }

            if (response.Error != null)
            {
                Debug.WriteLine($"Error loading user privacy: {response.Message}");
                return;
            }

            // Log giá trị ActiveStatus từ response
            Debug.WriteLine($"ActiveStatus từ response: {response.Data.ActiveStatus}");

            // Log các phần tử trong PrivacyOptions
            foreach (var privacy in PrivacyOptions)
            {
                Debug.WriteLine($"PrivacyID: {privacy.PrivacyID}, PrivacyName: {privacy.PrivacyName}");
            }

            // Set the flag to prevent triggering SavePrivacy
            _isSettingSelectedActiveStatus = true;

            SelectedActiveStatus = PrivacyOptions.FirstOrDefault(p => p.PrivacyID == response.Data.ActiveStatus);

            _isSettingSelectedActiveStatus = false; // Reset the flag

            // Kiểm tra kết quả
            if (SelectedActiveStatus == null)
            {
                Debug.WriteLine("SelectedActiveStatus là null. Không tìm thấy PrivacyID khớp.");
            }
            else
            {
                Debug.WriteLine($"SelectedActiveStatus được gán thành: {SelectedActiveStatus.PrivacyName}");
            }

            // Kiểm tra xem giá trị ActiveStatus có khớp với bất kỳ PrivacyID nào trong PrivacyOptions hay không
            foreach (var privacy in PrivacyOptions)
            {
                if (privacy.PrivacyID == response.Data.ActiveStatus)
                {
                    Debug.WriteLine($"Khớp với PrivacyID: {privacy.PrivacyID}");
                }
            }
        }
    }
}
