using ServiceStack.DataAnnotations;
using System;

namespace APIServer.Models
{
    public class Conversation
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ConversationID { get; set; }

        [StringLength(32)]
        public string ConversationName { get; set; }

        [StringLength(32)]
        public string ConversationTitle { get; set; }

        [References(typeof(User))]
        public int CreatorID { get; set; }

        [References(typeof(ConversationType))]
        public byte ConversationTypeID { get; set; }

        [References(typeof(GroupType))]
        public byte? GroupTypeID { get; set; }

        public DateTime CreatedAt { get; set; }

        [Reference]
        public User Creator { get; set; }

        [Reference]
        public ConversationType ConversationType { get; set; }

        [Reference]
        public GroupType GroupType { get; set; }
    }
}
