using ServiceStack.DataAnnotations;

namespace APIServer.Models
{
    public class ConversationType
    {
        [PrimaryKey]
        [AutoIncrement]
        public byte ConversationTypeID { get; set; }

        [StringLength(16)]
        public string ConversationTypeName { get; set; }
    }
}
