using ServiceStack.DataAnnotations;

namespace APIServer.Models
{
    public class ReportStatus
    {
        [PrimaryKey]
        [AutoIncrement]
        public byte ReportStatusID { get; set; }

        [StringLength(16)]
        public string ReportStatusName { get; set; }
    }
}
