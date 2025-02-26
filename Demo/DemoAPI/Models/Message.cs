using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoAPI.Models
{
    public class Message
    {
        [Required]
        [AutoIncrement]
        public int MessageID { get; set; }

        [Required]
        [References(typeof(User))]
        public int SenderID { get; set; }

        [Required]
        [References(typeof(User))]
        public int ReceiverID { get; set; }

        [Required]
        public string Content { get; set; }

        [StringLength(8)]
        [Required]
        public string MessageType { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
