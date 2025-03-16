using ServiceStack.DataAnnotations;

namespace APIServer.Models
{
    public class Privacy
    {
        [PrimaryKey]
        [AutoIncrement]
        public byte PrivacyID { get; set; }

        [StringLength(16)]
        public string PrivacyName { get; set; }
    }
}
