using ServiceStack.DataAnnotations;
using System;

namespace APIServer.Models
{
    public class DeleteConversations
    {
        [PrimaryKey]
        [AutoIncrement]
        public int DeletedConversationID { get; set; }

        [References(typeof(Conversations))]
        public int ConversationID { get; set; }

        [References(typeof(Users))]
        public int UserID { get; set; }

        [Default("GETDATE()")]
        public DateTime CreatedAt { get; set; }
    }
}
