using Messenger.Services;
using Messenger.Utils;
using ServiceStack;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Messenger.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        public ImageSource Avatar { get; set; }
        public string Activicty { get; set; }
        public string FullName { get; set; }
        public string Bio { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string JoinDate { get; set; }

        public ICommand Edit { get; set; }
        public ProfileViewModel(int userID)
        {
            Task.Run(() =>
            {
                var userService = ServiceLocator.GetService<IUserService>();

                var client = new JsonServiceClient(ConfigurationManager.AppSettings["APIUrl"]);
                client.BearerToken = userService.User.AuthToken;

                var getInfo = new DTOs.GetInfo
                {
                    UserID = userID
                };

                try
                {
                    var response = client.Get(getInfo);

                    if (!string.IsNullOrEmpty(response.Error))
                    {
                        CloseWindow();
                        return;
                    }

                    if (!string.IsNullOrEmpty(response.Data.Avatar))
                    {
                        Avatar = LoadImage.LoadImageFromUrl(ConfigurationManager.AppSettings["APIUrl"] + response.Data.Avatar);
                    }

                    if (!string.IsNullOrEmpty(response.Data.FirstName))
                    {
                        FullName = string.Format("{0} {1}", response.Data.LastName, response.Data.FirstName);
                    }

                    if (!string.IsNullOrEmpty(response.Data.Bio))
                    {
                        Bio = response.Data.Bio;
                    }
                    else
                    {
                        Bio = "Không có tiểu sử";
                    }
                    if (!string.IsNullOrEmpty(response.Data.Username))
                    {
                        Username = response.Data.Username;
                    }

                    if (!string.IsNullOrEmpty(response.Data.Email))
                    {
                        Email = response.Data.Email;
                    }

                    if (!string.IsNullOrEmpty(response.Data.PhoneNumber))
                    {
                        PhoneNumber = response.Data.PhoneNumber;
                    }

                    if (response.Data.CreatedAt != null)
                    {
                        JoinDate = response.Data.CreatedAt.ToString("dd/MM/yyyy");
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        CallPropertyChangedAll();
                    });
                }
                catch (Exception err)
                {
                    Debug.WriteLine(err);
                    CloseWindow();
                }
            });
        }
        private void CallPropertyChangedAll()
        {
            OnPropertyChanged(nameof(Avatar));
            OnPropertyChanged(nameof(FullName));
            OnPropertyChanged(nameof(Bio));
            OnPropertyChanged(nameof(Username));
            OnPropertyChanged(nameof(Email));
            OnPropertyChanged(nameof(PhoneNumber));
            OnPropertyChanged(nameof(JoinDate));
        }
        private void CloseWindow()
        {
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            window?.Close();
        }
    }
}
