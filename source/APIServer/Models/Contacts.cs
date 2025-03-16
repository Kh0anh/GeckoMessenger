using ServiceStack.DataAnnotations;
using System;

namespace APIServer.Models
{
    public class Contact
    {
        [PrimaryKey]
        [References(typeof(User))]
        public int ContactID { get; set; }

        [PrimaryKey]
        [References(typeof(User))]
        public int UserID { get; set; }

        public DateTime AddedAt { get; set; }

        public DateTime? BlockAt { get; set; }
    }
}
