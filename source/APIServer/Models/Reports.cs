using ServiceStack.DataAnnotations;
using System;

namespace APIServer.Models
{
    public class Reports
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ReportID { get; set; }

        [References(typeof(Users))]
        public int ReporterID { get; set; }

        [References(typeof(Users))]
        public int ReportedID { get; set; }

        [References(typeof(Models.Messages))]
        public int? MessageID { get; set; }

        public string ReportReason { get; set; }

        [References(typeof(ReportStatus))]
        public byte ReportStatusID { get; set; }

        [Default("GETDATE()")]
        public DateTime CreatedAt { get; set; }
    }
}
