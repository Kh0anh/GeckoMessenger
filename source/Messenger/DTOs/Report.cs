using ServiceStack;
using ServiceStack.DataAnnotations;

namespace Messenger.DTOs
{
    [Route("/report")]
    public class Report : IReturn<ReportResponse>
    {
        [Required]
        public int ReporterID { get; set; }

        public int? ReportedMessageID { get; set; }
        public int? ReportedUserID { get; set; }

        [Required]
        [StringLength(500)]
        public string ReportReason { get; set; }
    }

    public class ReportResponse
    {
        public string Error { get; set; }
        public string Message { get; set; }
        public ReportStatus Status { get; set; }
    }

    public enum ReportStatus
    {
        Pending,
        Reviewed,
        Rejected
    }
}
