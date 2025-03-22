using ServiceStack.DataAnnotations;
using System;

namespace APIServer.Models
{
    public class Messages
    {
        [PrimaryKey]
        [AutoIncrement]
        public int MessageID { get; set; }

        [References(typeof(Conversations))]
        public int ConversationID { get; set; }

        [References(typeof(Users))]
        public int SenderID { get; set; }

        public string Content { get; set; }

        [References(typeof(MessageType))]
        public byte MessageType { get; set; }

        [Default("GETDATE()")]
        public DateTime CreatedAt { get; set; }

        [Reference]
        public Conversations Conversation { get; set; }

        [Reference]
        public Users Sender { get; set; }

        [Reference]
        public MessageType MessageTypeRef { get; set; }
    }
}