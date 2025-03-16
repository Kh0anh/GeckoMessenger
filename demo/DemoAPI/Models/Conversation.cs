using ServiceStack.DataAnnotations;

namespace DemoAPI.Models
{
    [Alias("Conversations")]
    public class Conversation
    {
        [AutoIncrement]
        public int ConversationID { get; set; }

        [References(typeof(User))]
        public int UserAID { get; set; }

        [References(typeof(User))]
        public int UserBID { get; set; }
    }
}
