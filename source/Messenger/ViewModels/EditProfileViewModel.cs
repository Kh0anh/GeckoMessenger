using Messenger.Services;
using ServiceStack;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;

namespace Messenger.ViewModels
{
    public class EditProfileViewModel : BaseViewModel
    {
        public ImageSource Avatar { get; set; }
        private string _userName;
        private string _bio;
        private string _firstName;
        private string _lastName;
        private string _fullName;
        private string _email;
        private string _phoneNumber;
        private DateTime _birthday;
        public string Username { get => _userName; set => SetProperty(ref _userName, value); }
        public string Bio { get => _bio; set => SetProperty(ref _bio, value); }
        public string Firstname { get => _firstName; set => SetProperty(ref _firstName, value); }
        public string Lastname { get => _lastName; set => SetProperty(ref _lastName, value); }
        public string Fullname { get => _fullName; set => SetProperty(ref _fullName, value); }
        public string Email { get => _email; set => SetProperty(ref _email, value); }
        public string PhoneNumber { get => _phoneNumber; set => SetProperty(ref _phoneNumber, value); }
        public DateTime Birthday { get => _birthday; set => SetProperty(ref _birthday, value); }

        public string EditUsername { get; set; }
        public string EditBio { get; set; }
        public string EditFirstname { get; set; }
        public string EditLastname { get; set; }
        public string EditEmail { get; set; }
        public string EditPhoneNumber { get; set; }
        public DateTime EditBirthday { get; set; }

        public ICommand SaveProfile { get; set; }
        public EditProfileViewModel()
        {
            SaveProfile = new RelayCommand(_ => EditProfile(), _ => CanEditProfile());
            var userService = ServiceLocator.GetService<IUserService>();
            if (userService?.User != null)
            {
                Avatar = userService.User.Avatar;
                LoadUserProfile(userService.User.UserID);
            }
        }

        public async void EditProfile()
        {
            try
            {
                var userService = ServiceLocator.GetService<IUserService>();
                var client = new JsonServiceClient(ConfigurationManager.AppSettings["APIUrl"]);
                client.BearerToken = userService.User.AuthToken;

                var updateInfo = new DTOs.UpdateInfo
                {
                    Bio = EditBio,
                    Email = EditEmail,
                    PhoneNumber = EditPhoneNumber,
                    Birthday = EditBirthday,
                    FirstName = EditFirstname,
                    LastName = EditLastname
                };

                var response = await client.PutAsync(updateInfo);
                if (response.Error != null)
                {
                    Debug.WriteLine($"Error updating user profile: {response.Message}");
                    return;
                }
                Debug.WriteLine("Profile updated successfully!");
                LoadUserProfile(userService.User.UserID);
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public bool CanEditProfile()
        {
            return !string.IsNullOrEmpty(Username) ||
                   !string.IsNullOrEmpty(Email) ||
                   !string.IsNullOrEmpty(PhoneNumber) ||
                   Birthday != default;
        }

        public void LoadUserProfile(int userID)
        {
            var userService = ServiceLocator.GetService<IUserService>();
            var client = new JsonServiceClient(ConfigurationManager.AppSettings["APIUrl"]);
            client.BearerToken = userService.User.AuthToken;

            var getInfo = new DTOs.GetInfo { UserID = userID };
            var response = client.Get(getInfo);

            if (response.Error != null)
            {
                Debug.WriteLine($"Error loading user profile: {response.Message}");
                return;
            }

            Username = response.Data.Username;
            Bio = response.Data.Bio;
            Email = response.Data.Email;
            PhoneNumber = response.Data.PhoneNumber;
            Birthday = response.Data.Birthday;
            Fullname = response.Data.FirstName + " " + response.Data.LastName;

            EditUsername = response.Data.Username;
            EditBio = response.Data.Bio;
            EditEmail = response.Data.Email;
            EditPhoneNumber = response.Data.PhoneNumber;
            EditBirthday = response.Data.Birthday;
            EditFirstname = response.Data.FirstName;
            EditLastname = response.Data.LastName;
        }
    }
}