using DemoAPI.Models;
using ServiceStack;
using System;
using System.Collections.Generic;

namespace DemoAPI.DTOs
{
    [Route("/messages/send")]
    public class SendMessage : IReturn<SendMessageResponse>
    {
        public int ConversationID { get; set; }
        public string Content { get; set; }
        public string MessageType { get; set; }
    }

    [Route("/messages/newConversation")]
    public class NewConversation : IReturn<SendMessageResponse>
    {
        public int WithUserID { get; set; }
    }

    [Route("/messages/getByConversation")]
    public class GetByConversation : IReturn<List<Message>>
    {
        public int ConversationID { get; set; }
        public int? Limit { get; set; }
        public DateTime Beforce { get; set; }
        public DateTime After { get; set; }
    }

    [Route("/messages/getConversations")]
    public class GetConversations : IReturn<List<ConversationItem>> { }

    public class ConversationItem
    {
        public int ConversationID { get; set; }
        public int ReceiverID { get; set; }
    }

    public class SendMessageResponse
    {
        public bool Success { get; set; }
        public int MessageId { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class NewConversationResponse
    {
        public bool Success { get; set; }
        public int ConversationID { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }
}
