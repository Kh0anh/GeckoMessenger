using ServiceStack.DataAnnotations;
using System;

namespace APIServer.Models
{
    public class Participants
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ParticipantID { get; set; }

        [References(typeof(Conversations))]
        public int ConversationID { get; set; }

        [References(typeof(Users))]
        public int UserID { get; set; }

        [StringLength(32)]
        public string NickName { get; set; }

        [References(typeof(ConversationRole))]
        public byte ConversationRoleID { get; set; }

        [Default("GETDATE()")]
        public DateTime CreatedAt { get; set; }

        public DateTime? DeleteDate { get; set; }

        [Reference]
        public Conversations Conversation { get; set; }

        [Reference]
        public Users User { get; set; }

        [Reference]
        public ConversationRole ConversationRole { get; set; }
    }
}
