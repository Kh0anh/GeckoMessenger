using ServiceStack.DataAnnotations;
using System;

namespace APIServer.Models
{
    public class Report
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ReportID { get; set; }

        [References(typeof(User))]
        public int ReporterID { get; set; }

        [References(typeof(User))]
        public int ReportedID { get; set; }

        [References(typeof(Message))]
        public int? MessageID { get; set; }

        public string ReportReason { get; set; }

        [References(typeof(ReportStatus))]
        public byte ReportStatusID { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
