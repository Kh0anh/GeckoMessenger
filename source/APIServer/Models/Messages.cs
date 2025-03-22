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

        [CustomField("VARCHAR(MAX) COLLATE Latin1_General_100_CI_AS_SC_UTF8")]
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