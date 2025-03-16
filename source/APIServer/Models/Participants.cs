using ServiceStack.DataAnnotations;
using System;

namespace APIServer.Models
{
    public class Participant
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ParticipantID { get; set; }

        [References(typeof(Conversation))]
        public int ConversationID { get; set; }

        [References(typeof(User))]
        public int UserID { get; set; }

        [StringLength(32)]
        public string NickName { get; set; }

        [References(typeof(ConversationRole))]
        public byte ConversationRoleID { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? DeleteDate { get; set; }
    }
}
