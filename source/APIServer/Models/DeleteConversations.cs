using ServiceStack.DataAnnotations;
using System;

namespace APIServer.Models
{
    public class DeleteConversation
    {
        [PrimaryKey]
        [AutoIncrement]
        public int DeletedConversationID { get; set; }

        [References(typeof(Conversation))]
        public int ConversationID { get; set; }

        [References(typeof(User))]
        public int UserID { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
