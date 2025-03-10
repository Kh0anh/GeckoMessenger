using System;

namespace Messenger.ViewModels
{
    public class EditProfileViewModel : BaseViewModel
    {
        public string EditUsername { get; set; }
        public string EditEmail { get; set; }
        public string EditPhoneNumber { get; set; }
        public DateTime EditBirthday { get; set; }

        public EditProfileViewModel()
        {
        }
    }
}