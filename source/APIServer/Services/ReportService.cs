using APIServer.Models;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace APIServer.Services
{
    public class ReportService : Service
    {
        public IDbConnectionFactory DB { get; set; }

        // Tạo báo cáo mới
        public object Post(DTOs.Report request)
        {
            if ((request.ReportedMessageID == null && request.ReportedUserID == null) || 
                (request.ReportedMessageID != null && request.ReportedUserID != null))
            {
                return new HttpResult(new DTOs.ReportResponse
                {
                    Error = "InvalidReport",
                    Message = "Must report either a message or a user, not both or neither"
                }, HttpStatusCode.BadRequest);
            }

            if (string.IsNullOrEmpty(request.ReportReason))
            {
                return new HttpResult(new DTOs.ReportResponse
                {
                    Error = "RequiredField",
                    Message = "Report reason is required"
                }, HttpStatusCode.BadRequest);
            }

            using (var db = DB.Open())
            {
                var newReport = new Models.Report
                {
                    ReporterID = int.Parse(GetSession().UserAuthId),
                    ReportedID = request.ReportedUserID ?? 0,
                    MessageID = request.ReportedMessageID,
                    ReportReason = request.ReportReason,
                    ReportStatusID = 1, // Pending status
                    CreatedAt = DateTime.UtcNow
                };

                // Lưu báo cáo vào database
                db.Insert(newReport);

                return new HttpResult(new DTOs.ReportResponse
                {
                    Message = "Report submitted successfully",
                    Status = DTOs.ReportStatus.Pending
                }, HttpStatusCode.Created);
            }
        }
    }
}
