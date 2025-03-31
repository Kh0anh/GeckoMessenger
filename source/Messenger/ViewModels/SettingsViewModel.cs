using Messenger.Services;
using System.Drawing;
using System.Windows.Media;

namespace Messenger.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public ImageSource Avatar { get; set; }
        public string FullName {  get; set; }
        public string Username { get; set; }
        public SettingsViewModel()
        {
            var userService = ServiceLocator.GetService<IUserService>();
            if (userService != null)
            {
                Avatar =  userService.User.Avatar;
                FullName = userService.User.FullName;
                Username = userService.User.Username;
            }
        }
    }
}
