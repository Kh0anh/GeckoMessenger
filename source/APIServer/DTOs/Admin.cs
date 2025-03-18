using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIServer.DTOs
{
    public class Admin
    {
        [Route("/admin/ban")]
        public class Ban : IReturn<BanResponse>
        {
            public int BanUserID { get; set; }
            public int MessageID { get; set; }
            public string Content { get; set; }
            public string Reason { get; set; }
            public DateTime? BanExpirationDate { get; set; }
        }

        public class BanResponse
        {
            public string Error { get; set; }
            public string Message { get; set; }
            public BanStatus Status { get; set; }
        }


        [Route("/admin/unban")]
        public class UnBan : IReturn<UnBanResponse>
        {
            public int UnbanUserID { get; set; }
        }

        public class UnBanResponse
        {
            public string Error { get; set; }
            public string Message { get; set; }
            public bool Status { get; set; }
        }

        public enum BanStatus
        { 
            Temporary,
            Permanent
        }
    }
}
