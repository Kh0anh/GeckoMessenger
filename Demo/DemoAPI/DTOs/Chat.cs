using DemoAPI.Models;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoAPI.DTOs
{
    [Route("/messages/send")]
    public class SendMessage : IReturn<SendMessageResponse>
    {
        public int ReceiverID { get; set; }
        public string Content { get; set; }
        public string MessageType { get; set; }
    }

    [Route("/messages")]
    public class GetMessages : IReturn<List<Message>>
    {
        public int? AfterId { get; set; }
    }

    public class SendMessageResponse
    {
        public int MessageId { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }
}
