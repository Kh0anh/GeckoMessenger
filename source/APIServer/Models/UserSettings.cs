using ServiceStack.DataAnnotations;

namespace APIServer.Models
{
    public class UserSetting
    {
        [PrimaryKey]
        [References(typeof(User))]
        public int UserID { get; set; }

        [References(typeof(Privacy))]
        public byte StatusPrivacy { get; set; }

        [References(typeof(Privacy))]
        public byte BioPrivacy { get; set; }

        [References(typeof(Privacy))]
        public byte PhoneNumberPrivacy { get; set; }

        [References(typeof(Privacy))]
        public byte EmailPrivacy { get; set; }

        [References(typeof(Privacy))]
        public byte BirthdayPrivacy { get; set; }

        [References(typeof(Privacy))]
        public byte CallPrivacy { get; set; }

        [References(typeof(Privacy))]
        public byte InviteGroupPrivacy { get; set; }

        [References(typeof(Privacy))]
        public byte MessagePrivacy { get; set; }
    }
}
