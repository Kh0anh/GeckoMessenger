using Messenger.Services;
using ServiceStack;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Messenger.ViewModels
{
    public class ChangePasswordViewModel : BaseViewModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string _error;
        public string Error { get { return _error; } set { _error = value; OnPropertyChanged(nameof(Error)); } }
        public ICommand ChangePasswordCommand { get; }
        public ChangePasswordViewModel()
        {
            ChangePasswordCommand = new RelayCommand(async _ => await ChangePassword());
        }

        private async Task ChangePassword()
        {
            string oldPassword = OldPassword;
            string newPassword = NewPassword;
            string confirmPassword = ConfirmPassword;

            var userService = ServiceLocator.GetService<IUserService>();
            var client = new JsonServiceClient(ConfigurationManager.AppSettings["APIUrl"]);
            client.BearerToken = userService.User.AuthToken;

            var changePassword = new DTOs.ChangePassword
            {
                OldPassword = oldPassword,
                NewPassword = newPassword,
                ConfirmPassword = confirmPassword
            };

            try
            {
                var response = await client.PutAsync(changePassword);
                if (string.IsNullOrEmpty(response.Error))
                {
                    Error = response.Message;
                    return;
                }
            }
            catch (WebServiceException ex)
            {
                if (ex.ResponseDto is DTOs.ChangePasswordResponse errorResponse)
                {
                    Error = errorResponse.Message;
                    OnPropertyChanged(nameof(Error));
                }
                Debug.WriteLine(ex.Message);
                return;
            }
        }
    }
}
