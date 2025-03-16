using ServiceStack.DataAnnotations;
using System;

namespace APIServer.Models
{
    public class Message
    {
        [PrimaryKey]
        [AutoIncrement]
        public int MessageID { get; set; }

        [References(typeof(Conversation))]
        public int ConversationID { get; set; }

        [References(typeof(User))]
        public int SenderID { get; set; }

        public string Content { get; set; }

        [References(typeof(MessageType))]
        public byte MessageType { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
