using Messenger.Services;
using ServiceStack;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;


namespace Messenger.ViewModels
{
    public class EditPrivacyViewModel : BaseViewModel
    {
        public ObservableCollection<PrivacyConfig> PrivacyOptions { get; set; }

        private PrivacyConfig _SelectedActiveStatus;
        private PrivacyConfig _SelectedBioPrivacy;
        private PrivacyConfig _SelectedPhoneNumberPrivacy;
        private PrivacyConfig _SelectedEmailPrivacy;
        private PrivacyConfig _SelectedBirthdayPrivacy;
        private PrivacyConfig _SelectedCallPrivacy;
        private PrivacyConfig _SelectedInviteGroupPrivacy;
        private PrivacyConfig _SelectedMessagePrivacy;
        public PrivacyConfig SelectedActiveStatus
        {
            get => _SelectedActiveStatus;
            set
            {
                _SelectedActiveStatus = value;
                SavePrivacyCommand?.Execute(_SelectedActiveStatus);
            }
        }
        public PrivacyConfig SelectedBioPrivacy
        {
            get => _SelectedBioPrivacy;
            set
            {
                _SelectedBioPrivacy = value;
                SavePrivacyCommand?.Execute(_SelectedBioPrivacy);
            }
        }
        public PrivacyConfig SelectedPhoneNumberPrivacy
        {
            get => _SelectedPhoneNumberPrivacy;
            set
            {
                _SelectedPhoneNumberPrivacy = value;
                SavePrivacyCommand?.Execute(_SelectedPhoneNumberPrivacy);
            }
        }
        public PrivacyConfig SelectedEmailPrivacy
        {
            get => _SelectedEmailPrivacy;
            set
            {
                _SelectedEmailPrivacy = value;
                SavePrivacyCommand?.Execute(_SelectedEmailPrivacy);
            }
        }
        public PrivacyConfig SelectedBirthdayPrivacy
        {
            get => _SelectedBirthdayPrivacy;
            set
            {
                _SelectedBirthdayPrivacy = value;
                SavePrivacyCommand?.Execute(_SelectedBirthdayPrivacy);
            }
        }
        public PrivacyConfig SelectedCallPrivacy
        {
            get => _SelectedCallPrivacy;
            set
            {
                _SelectedCallPrivacy = value;
                SavePrivacyCommand?.Execute(_SelectedCallPrivacy);
            }
        }
        public PrivacyConfig SelectedInviteGroupPrivacy
        {
            get => _SelectedInviteGroupPrivacy;
            set
            {
                _SelectedInviteGroupPrivacy = value;
                SavePrivacyCommand?.Execute(_SelectedInviteGroupPrivacy);
            }
        }
        public PrivacyConfig SelectedMessagePrivacy
        {
            get => _SelectedMessagePrivacy;
            set
            {
                _SelectedMessagePrivacy = value;
                SavePrivacyCommand?.Execute(_SelectedMessagePrivacy);
            }
        }

        public ICommand SavePrivacyCommand { get; set; }

        private UserInfo UserInfo { get; set; }
        public EditPrivacyViewModel()
        {
            SavePrivacyCommand = new RelayCommand(async _ => await EditPrivacy());
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

            UserInfo = userService.User;
            _ = LoadUserPrivacy();
        }

        public async Task EditPrivacy()
        {
            var client = new JsonServiceClient(ConfigurationManager.AppSettings["APIUrl"]);
            client.BearerToken = UserInfo.AuthToken;

            var updatePrivacy = new DTOs.UpdatePrivacy
            {
                ActiveStatus = SelectedActiveStatus.PrivacyName,
                BioPrivacy = SelectedBioPrivacy.PrivacyName,
                PhoneNumberPrivacy = SelectedPhoneNumberPrivacy.PrivacyName,
                EmailPrivacy = SelectedEmailPrivacy.PrivacyName,
                BirthdayPrivacy = SelectedBirthdayPrivacy.PrivacyName,
                CallPrivacy = SelectedCallPrivacy.PrivacyName,
                InviteGroupPrivacy = SelectedInviteGroupPrivacy.PrivacyName,
                MessagePrivacy = SelectedMessagePrivacy.PrivacyName
            };

            var response = await client.PutAsync(updatePrivacy);
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

        public async Task LoadUserPrivacy()
        {
            var client = new JsonServiceClient(ConfigurationManager.AppSettings["APIUrl"]);
            client.BearerToken = UserInfo.AuthToken;

            var getPrivacy = new DTOs.GetPrivacy { UserID = UserInfo.UserID };
            var response = await client.GetAsync(getPrivacy);

            if (response.Error != null)
            {
                Debug.WriteLine($"{response.Error}");
                return;
            }

            _SelectedActiveStatus = PrivacyOptions.SingleOrDefault<PrivacyConfig>(p => p.PrivacyName == response.Data.ActiveStatus);
            _SelectedBioPrivacy = PrivacyOptions.SingleOrDefault<PrivacyConfig>(p => p.PrivacyName == response.Data.BioPrivacy);
            _SelectedPhoneNumberPrivacy = PrivacyOptions.SingleOrDefault<PrivacyConfig>(p => p.PrivacyName == response.Data.PhoneNumberPrivacy);
            _SelectedEmailPrivacy = PrivacyOptions.SingleOrDefault<PrivacyConfig>(p => p.PrivacyName == response.Data.EmailPrivacy);
            _SelectedBirthdayPrivacy = PrivacyOptions.SingleOrDefault<PrivacyConfig>(p => p.PrivacyName == response.Data.BirthdayPrivacy);
            _SelectedCallPrivacy = PrivacyOptions.SingleOrDefault<PrivacyConfig>(p => p.PrivacyName == response.Data.CallPrivacy);
            _SelectedInviteGroupPrivacy = PrivacyOptions.SingleOrDefault<PrivacyConfig>(p => p.PrivacyName == response.Data.InviteGroupPrivacy);
            _SelectedMessagePrivacy = PrivacyOptions.SingleOrDefault<PrivacyConfig>(p => p.PrivacyName == response.Data.MessagePrivacy);
            OnPropertyChanged(nameof(SelectedActiveStatus));
            OnPropertyChanged(nameof(SelectedBioPrivacy));
            OnPropertyChanged(nameof(SelectedPhoneNumberPrivacy));
            OnPropertyChanged(nameof(SelectedEmailPrivacy));
            OnPropertyChanged(nameof(SelectedBirthdayPrivacy));
            OnPropertyChanged(nameof(SelectedCallPrivacy));
            OnPropertyChanged(nameof(SelectedInviteGroupPrivacy));
            OnPropertyChanged(nameof(SelectedMessagePrivacy));
        }
    }
    public class PrivacyConfig
    {
        public string PrivacyTitle { get; set; }
        public string PrivacyName { get; set; }
    }
}
