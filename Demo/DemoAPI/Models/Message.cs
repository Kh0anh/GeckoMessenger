using ServiceStack.DataAnnotations;
using System;

namespace DemoAPI.Models
{
    [Alias("Messages")]
    public class Message
    {
        [AutoIncrement]
        public int MessageID { get; set; }

        [References(typeof(Conversation))]
        public int ConversationsID { get; set; }

        [References(typeof(User))]
        public int SenderID { get; set; }

        public string Content { get; set; }

        [Required]
        [StringLength(8)]
        public string MessageType { get; set; }

        [Default(typeof(System.DateTime), "CURRENT_TIMESTAMP")]
        public DateTime CreatedAt { get; set; }
    }
}
