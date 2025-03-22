using ServiceStack.DataAnnotations;
using System;

namespace APIServer.Models
{
    public class MessageDelete
    {
        [PrimaryKey]
        [AutoIncrement]
        public int MessageDeleteID { get; set; }

        [References(typeof(Messages))]
        public int MessageID { get; set; }

        [References(typeof(Users))]
        public int DeleteByUserID { get; set; }

        [References(typeof(DeleteType))]
        public byte DeleteTypeID { get; set; }

        [Default("GETDATE()")]
        public DateTime CreatedAt { get; set; }
    }
}
