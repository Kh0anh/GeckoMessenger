using ServiceStack.DataAnnotations;
using System;

namespace APIServer.Models
{
    public class MessageDelete
    {
        [PrimaryKey]
        [AutoIncrement]
        public int MessageDeleteID { get; set; }

        [References(typeof(Message))]
        public int MessageID { get; set; }

        [References(typeof(User))]
        public int DeleteByUserID { get; set; }

        [References(typeof(DeleteType))]
        public byte DeleteTypeID { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
