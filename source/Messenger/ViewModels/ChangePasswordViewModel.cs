using Messenger.Services;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
            ChangePasswordCommand = new RelayCommand(_ => ChangePassword());
        }

        private void ChangePassword()
        {
            string oldPassword = OldPassword;
            string newPassword = NewPassword;
            string confirmPassword = ConfirmPassword;
            Task.Run(() =>
            {
                var userService = ServiceLocator.GetService<IUserService>();
                var client = new JsonServiceClient(ConfigurationManager.AppSettings["APIUrl"]);
                client.BearerToken = userService.User.AuthToken;

                if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
                {
                    Error = "All password fields must be filled.";
                }

                if (newPassword != confirmPassword)
                {
                    Error = "New password and confirm password do not match.";
                }

                var changePassword = new DTOs.ChangePassword
                {
                    OldPassword = oldPassword,
                    NewPassword = newPassword,
                    ConfirmPassword = confirmPassword
                };

                try
                {
                    var response = client.Put(changePassword);
                    if (!string.IsNullOrEmpty(response.Error))
                    {
                        Error = response.Message;
                    }
                    else
                    {
                        Error = "Password changed successfully";
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            });
        }
    }
}
