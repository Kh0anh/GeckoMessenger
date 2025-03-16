using ServiceStack.DataAnnotations;

namespace APIServer.Models
{
    public class MessageType
    {
        [PrimaryKey]
        [AutoIncrement]
        public byte MessageTypeID { get; set; }

        [StringLength(16)]
        public string MessageTypeName { get; set; }
    }
}
