using ServiceStack.DataAnnotations;
using System;

namespace APIServer.Models
{
    public class Contacts
    {
        [PrimaryKey]
        [References(typeof(Users))]
        public int ContactID { get; set; }

        [PrimaryKey]
        [References(typeof(Users))]
        public int UserID { get; set; }

        [Default("GETDATE()")]
        public DateTime AddedAt { get; set; }

        public DateTime? BlockAt { get; set; }
    }
}
