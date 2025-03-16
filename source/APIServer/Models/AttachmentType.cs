using ServiceStack.DataAnnotations;

namespace APIServer.Models
{
    public class AttachmentType
    {
        [PrimaryKey]
        [AutoIncrement]
        public byte AttachmentTypeID { get; set; }

        [StringLength(16)]
        public string AttachmentTypeName { get; set; }
    }
}
