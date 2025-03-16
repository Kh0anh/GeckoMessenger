using ServiceStack.DataAnnotations;

namespace APIServer.Models
{
    public class ConversationRole
    {
        [PrimaryKey]
        [AutoIncrement]
        public byte ConversationRoleID { get; set; }

        [StringLength(16)]
        public string ConversationRoleName { get; set; }
    }
}
