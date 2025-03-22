using Messenger.Utils;
using Newtonsoft.Json;
using System.Windows.Media;

namespace Messenger.Services
{

    public interface IUserService
    {
        UserInfo User { get; set; }
        void SaveUser();
        void LoadUser();
        void ClearUser();
    }
    public class UserInfo
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        [JsonIgnore]
        public ImageSource Avatar { get; set; }
        public string AuthToken { get; set; }
        public string AvatarBase64
        {
            get
            {
                if (Avatar == null) return null;
                return Converts.ConvertImageToBase64(Avatar);
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    Avatar = Converts.ConvertBase64ToImage(value);
                }
            }
        }
    }

    public class UserService : IUserService
    {
        private const string UserFilePath = "app.json";

        public UserInfo User { get; set; }

        public UserService()
        {
            LoadUser();
        }

        public void SaveUser()
        {
            if (User != null)
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(User);
                System.IO.File.WriteAllText(UserFilePath, json);
            }
        }

        public void LoadUser()
        {
            if (System.IO.File.Exists(UserFilePath))
            {
                string json = System.IO.File.ReadAllText(UserFilePath);
                User = Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfo>(json);
            }
        }

        public void ClearUser()
        {
            User = null;
            if (System.IO.File.Exists(UserFilePath))
            {
                System.IO.File.Delete(UserFilePath);
            }
        }
    }
}
