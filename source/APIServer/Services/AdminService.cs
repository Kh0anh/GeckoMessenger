using APIServer.Models;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System;
using System.Linq;
using System.Net;

namespace APIServer.Services
{
    public class AdminService : Service
    {
        public IDbConnectionFactory DB { get; set; }

        // Ban user
        public object Post(DTOs.Admin.Ban request)
        {
            // Kiểm tra quyền quản trị
            if (!IsAdmin())
            {
                return new HttpResult(new DTOs.Admin.BanResponse
                {
                    Error = "Forbidden",
                    Message = "You don't have permission to perform this action"
                }, HttpStatusCode.Forbidden);
            }

            // Kiểm tra dữ liệu đầu vào
            if (request.BanUserID <= 0)
            {
                return new HttpResult(new DTOs.Admin.BanResponse
                {
                    Error = "RequiredField",
                    Message = "User ID is required"
                }, HttpStatusCode.BadRequest);
            }

            if (string.IsNullOrEmpty(request.Reason))
            {
                return new HttpResult(new DTOs.Admin.BanResponse
                {
                    Error = "RequiredField",
                    Message = "Ban reason is required"
                }, HttpStatusCode.BadRequest);
            }

            using (var db = DB.Open())
            {
                // Kiểm tra người dùng bị ban tồn tại
                var user = db.SingleById<Users>(request.BanUserID);
                if (user == null)
                {
                    return new HttpResult(new DTOs.Admin.BanResponse
                    {
                        Error = "NotFound",
                        Message = "User not found"
                    }, HttpStatusCode.NotFound);
                }

                // Kiểm tra tin nhắn tồn tại (nếu có)
                if (request.MessageID > 0)
                {
                    var message = db.SingleById<Models.Messages>(request.MessageID);
                    if (message == null)
                    {
                        return new HttpResult(new DTOs.Admin.BanResponse
                        {
                            Error = "NotFound",
                            Message = "Message not found"
                        }, HttpStatusCode.NotFound);
                    }
                }

                // Kiểm tra ban hiện tại
                var existingBan = db.Single<BannedAccounts>(b => b.BannedID == request.BanUserID && b.Expired > DateTime.UtcNow);
                if (existingBan != null)
                {
                    return new HttpResult(new DTOs.Admin.BanResponse
                    {
                        Error = "AlreadyBanned",
                        Message = "User is already banned",
                        Status = DTOs.Admin.BanStatus.Temporary
                    }, HttpStatusCode.Conflict);
                }

                var ban = new BannedAccounts
                {
                    CreatorID = int.Parse(GetSession().UserAuthId),
                    BannedID = request.BanUserID,
                    Reason = request.Reason,
                    Expired = request.BanExpirationDate ?? DateTime.MaxValue
                };

                db.Insert(ban);

                var status = request.BanExpirationDate.HasValue ?
                    DTOs.Admin.BanStatus.Temporary :
                    DTOs.Admin.BanStatus.Permanent;

                return new HttpResult(new DTOs.Admin.BanResponse
                {
                    Message = "User has been banned",
                    Status = DTOs.Admin.BanStatus.Temporary
                }, HttpStatusCode.OK);
            }
        }

        // Unban user
        public object Post(DTOs.Admin.UnBan request)
        {
            // Kiểm tra quyền quản trị
            if (!IsAdmin())
            {
                return new HttpResult(new DTOs.Admin.UnBanResponse
                {
                    Error = "Forbidden",
                    Message = "You don't have permission to perform this action"
                }, HttpStatusCode.Forbidden);
            }

            // Kiểm tra dữ liệu đầu vào
            if (request.UnbanUserID <= 0)
            {
                return new HttpResult(new DTOs.Admin.UnBanResponse
                {
                    Error = "RequiredField",
                    Message = "User ID is required"
                }, HttpStatusCode.BadRequest);
            }

            using (var db = DB.Open())
            {
                // Kiểm tra người dùng tồn tại
                var user = db.SingleById<Users>(request.UnbanUserID);
                if (user == null)
                {
                    return new HttpResult(new DTOs.Admin.UnBanResponse
                    {
                        Error = "NotFound",
                        Message = "User not found"
                    }, HttpStatusCode.NotFound);
                }

                // Kiểm tra xem người dùng có bị ban hay không
                var activeBan = db.Single<BannedAccounts>(b => b.BannedID == request.UnbanUserID && b.Expired > DateTime.UtcNow);
                if (activeBan == null)
                {
                    return new HttpResult(new DTOs.Admin.UnBanResponse
                    {
                        Error = "NotBanned",
                        Message = "User is not banned",
                        Status = false
                    }, HttpStatusCode.BadRequest);
                }

                // Cập nhật thông tin ban
                activeBan.Expired = DateTime.UtcNow;
                db.Update(activeBan);

                // Cập nhật trạng thái người dùng
                db.Update<Users>(new { IsBanned = false }, u => u.UserID == request.UnbanUserID);

                return new HttpResult(new DTOs.Admin.UnBanResponse
                {
                    Message = "User has been unbanned successfully",
                    Status = true
                }, HttpStatusCode.OK);
            }
        }

        // Kiểm tra quyền admin
        private bool IsAdmin()
        {
            if (!IsAuthenticated)
                return false;

            var userId = int.Parse(GetSession().UserAuthId);
            using (var db = DB.Open())
            {
                return db.Exists<Managers>(m => m.ManagerID == userId);
            }
        }
    }
}
