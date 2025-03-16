using ServiceStack.DataAnnotations;
using System;

namespace APIServer.Models
{
    public class BannedAccount
    {
        [PrimaryKey]
        [AutoIncrement]
        public int BanID { get; set; }

        [References(typeof(Manager))]
        public int CreatorID { get; set; }

        [References(typeof(User))]
        public int BannedID { get; set; }

        public string Reason { get; set; }

        public DateTime Expired { get; set; }

        [Default("GETDATE()")]
        public DateTime CreatedAt { get; set; }
    }
}
