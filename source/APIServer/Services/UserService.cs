using APIServer.Models;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;

namespace APIServer.Services
{
    public class UserService : Service
    {
        public IDbConnectionFactory DB { get; set; }

        // Kiểm tra thông tin tồn tại
        public object Post(DTOs.ExistedInfo request)
        {
            using (var db = DB.Open())
            {
                var exists = db.Exists<User>(u => 
                    (!request.Username.IsNullOrEmpty() && u.Username == request.Username) ||
                    (!request.Email.IsNullOrEmpty() && u.Email == request.Email) ||
                    (!request.PhoneNumber.IsNullOrEmpty() && u.PhoneNumber == request.PhoneNumber)
                );

                return new HttpResult(new DTOs.ExistedInfoResponse
                {
                    IsExisted = exists,
                    Message = exists ? "Information already exists" : "Information available"
                });
            }
        }

        // Lấy thông tin user
        public object Get(DTOs.GetInfo request)
        {
            using (var db = DB.Open())
            {
                var user = db.SingleById<User>(request.UserID);
                if (user == null)
                {
                    return new HttpResult(new DTOs.GetInfoResponse
                    {
                        Error = "NotFound",
                        Message = "User not found"
                    }, HttpStatusCode.NotFound);
                }

                return new HttpResult(new DTOs.GetInfoResponse
                {
                    Data = new DTOs.UserInfo
                    {
                        UserID = user.UserID,
                        Username = user.Username,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Birthday = user.Birthday,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Avatar = user.Avatar
                    }
                });
            }
        }

        // Cập nhật trạng thái hoạt động
        public object Post(DTOs.ActiveStatus request)
        {
            using (var db = DB.Open())
            {
                var user = db.SingleById<User>(request.UserID);
                if (user == null)
                {
                    return new HttpResult(new DTOs.ActiveStatusResponse
                    {
                        Error = "NotFound",
                        Message = "User not found"
                    }, HttpStatusCode.NotFound);
                }

                // Cập nhật LastLogin
                user.LastLogin = DateTime.UtcNow;
                db.Update(user);

                return new HttpResult(new DTOs.ActiveStatusResponse
                {
                    Message = "Last login updated successfully",
                    IsActive = true
                });
            }
        }

        // Lấy trạng thái hoạt động
        public object GetActiveStatus(int userId)
        {
            using (var db = DB.Open())
            {
                var user = db.SingleById<User>(userId);
                if (user == null)
                {
                    return new HttpResult(new DTOs.ActiveStatusResponse
                    {
                        Error = "NotFound",
                        Message = "User not found"
                    }, HttpStatusCode.NotFound);
                }

                // Kiểm tra trạng thái hoạt động
                bool isActive = (DateTime.UtcNow - user.LastLogin).TotalMinutes <= 5;

                return new HttpResult(new DTOs.ActiveStatusResponse
                {
                    Message = "Status retrieved successfully",
                    IsActive = isActive
                });
            }
        }

        // Cập nhật thông tin
        public object Put(DTOs.UpdateInfo request)
        {
            var userId = int.Parse(GetSession().UserAuthId);
            using (var db = DB.Open())
            {
                var user = db.SingleById<User>(userId);
                if (user == null)
                {
                    return new HttpResult(new DTOs.UpdateInfoResponse
                    {
                        Error = "NotFound",
                        Message = "User not found"
                    }, HttpStatusCode.NotFound);
                }

                // Kiểm tra email/phone đã tồn tại chưa
                if (!request.Email.IsNullOrEmpty() && request.Email != user.Email &&
                    db.Exists<User>(u => u.Email == request.Email))
                {
                    return new HttpResult(new DTOs.UpdateInfoResponse
                    {
                        Error = "Duplicated",
                        Message = "Email already exists"
                    }, HttpStatusCode.Conflict);
                }

                if (!request.PhoneNumber.IsNullOrEmpty() && request.PhoneNumber != user.PhoneNumber &&
                    db.Exists<User>(u => u.PhoneNumber == request.PhoneNumber))
                {
                    return new HttpResult(new DTOs.UpdateInfoResponse
                    {
                        Error = "Duplicated",
                        Message = "Phone number already exists"
                    }, HttpStatusCode.Conflict);
                }

                // Cập nhật thông tin
                db.Update<User>(new
                {
                    Email = request.Email ?? user.Email,
                    PhoneNumber = request.PhoneNumber ?? user.PhoneNumber,
                    Birthday = request.Birthday,
                    FirstName = request.FirstName ?? user.FirstName,
                    LastName = request.LastName ?? user.LastName
                }, u => u.UserID == userId);

                return new HttpResult(new DTOs.UpdateInfoResponse
                {
                    Message = "Information updated successfully"
                });
            }
        }

        // Cập nhật avatar
        public object Put(DTOs.UpdateAvatar request)
        {
            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrEmpty(request.Avatar))
            {
                return new HttpResult(new DTOs.UpdateAvatarResponse
                {
                    Error = "RequiredField",
                    Message = "Avatar data is required"
                }, HttpStatusCode.BadRequest);
            }

            var userId = int.Parse(GetSession().UserAuthId);
            using (var db = DB.Open())
            {
                var user = db.SingleById<User>(userId);
                if (user == null)
                {
                    return new HttpResult(new DTOs.UpdateAvatarResponse
                    {
                        Error = "NotFound",
                        Message = "User not found"
                    }, HttpStatusCode.NotFound);
                }

                try
                {
                    // Lưu avatar và cập nhật đường dẫn
                    var avatarUrl = UpdateAvatar(request.Avatar, userId);
                    db.Update<User>(new { Avatar = avatarUrl }, u => u.UserID == userId);

                    return new HttpResult(new DTOs.UpdateAvatarResponse
                    {
                        Message = "Avatar updated successfully",
                        AvatarUrl = avatarUrl
                    }, HttpStatusCode.OK);
                }
                catch (HttpError httpError)
                {
                    // Xử lý HttpError từ phương thức UpdateAvatar
                    return new HttpResult(new DTOs.UpdateAvatarResponse
                    {
                        Error = httpError.ErrorCode,
                        Message = httpError.Message
                    }, httpError.StatusCode);
                }
                catch (Exception)
                {
                    // Xử lý các lỗi khác
                    return new HttpResult(new DTOs.UpdateAvatarResponse
                    {
                        Error = "ServerError",
                        Message = "Failed to update avatar"
                    }, HttpStatusCode.InternalServerError);
                }
            }
        }

        // Tìm kiếm user
        public object Get(DTOs.SearchUser request)
        {
            if (request.Query.IsNullOrEmpty())
            {
                return new HttpResult(new DTOs.SearchUserResponse
                {
                    Error = "RequiredField",
                    Message = "Search query is required"
                }, HttpStatusCode.BadRequest);
            }

            if (request.MaxResult <= 0)
            {
                request.MaxResult = 20; // Mặc định 20 kết quả
            }

            using (var db = DB.Open())
            {
                var query = db.From<User>();
                
                // Tìm kiếm theo tên (FirstName hoặc LastName) hoặc email
                var searchTerm = $"%{request.Query}%";
                query = query.Where(u => 
                    u.FirstName.Contains(searchTerm) ||
                    u.LastName.Contains(searchTerm) ||
                    u.Email.Contains(searchTerm) ||
                    u.PhoneNumber.Contains(searchTerm)
                );

                var users = db.Select(query.Limit(request.MaxResult))
                    .Select(u => new DTOs.UserInfo
                    {
                        UserID = u.UserID,
                        Username = u.Username,
                        Email = u.Email,
                        PhoneNumber = u.PhoneNumber,
                        Birthday = u.Birthday,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Avatar = u.Avatar
                    }).ToList();

                return new HttpResult(new DTOs.SearchUserResponse
                {
                    Message = users.Count > 0 ? $"Found {users.Count} users" : "No users found",
                    Users = users
                });
            }
        }

        //Cập nhật avatar
        private string UpdateAvatar(string avatarBase64, int userId)
        {
            if (string.IsNullOrEmpty(avatarBase64))
            {
                throw new HttpError(HttpStatusCode.BadRequest, "RequiredField", "Avatar data cannot be empty");
            }

            // Tạo thư mục lưu trữ nếu chưa tồn tại
            string storageDir = Path.Combine("Resources", "avatars");
            if (!Directory.Exists(storageDir))
            {
                Directory.CreateDirectory(storageDir);
            }

            // Xử lý chuỗi base64
            string base64Data = avatarBase64;
            if (avatarBase64.Contains(","))
            {
                base64Data = avatarBase64.Split(',')[1];
            }

            // Giải mã base64 thành binary
            byte[] imageBytes;
            try
            {
                imageBytes = Convert.FromBase64String(base64Data);
            }
            catch (FormatException)
            {
                throw new HttpError(HttpStatusCode.BadRequest, "InvalidFormat", "Invalid base64 format");
            }

            // Kiểm tra kích thước ảnh (giới hạn ở 2MB)
            if (imageBytes.Length > 2 * 1024 * 1024)
            {
                throw new HttpError(HttpStatusCode.BadRequest, "FileTooLarge", "Avatar is too large. Maximum size is 2MB");
            }

            try
            {
                // Lưu file với tên đặc biệt để tránh ghi đè
                string fileName = $"{userId}_{DateTime.UtcNow.Ticks}.jpg";
                string filePath = Path.Combine(storageDir, fileName);
                
                // Ghi file
                File.WriteAllBytes(filePath, imageBytes);
                
                // Trả về đường dẫn tương đối để hiển thị trong response
                return $"/Resources/avatars/{fileName}";
            }
            catch (Exception)
            {
                throw new HttpError(HttpStatusCode.InternalServerError, "ServerError", "Failed to save avatar");
            }
        }
    }
}
