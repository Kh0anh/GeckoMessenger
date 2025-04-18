﻿using APIServer.Models;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;

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
                var exists = db.Exists<Users>(u =>
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
                var user = db.SingleById<Users>(request.UserID);
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
                        Bio = user.Bio,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Birthday = user.Birthday,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Avatar = user.Avatar,
                        CreatedAt = user.CreatedAt,
                    }
                }, HttpStatusCode.OK);
            }
        }

        // Cập nhật trạng thái hoạt động
        public object Post(DTOs.ActiveStatus request)
        {
            using (var db = DB.Open())
            {
                var user = db.SingleById<Users>(request.UserID);
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
                }, HttpStatusCode.OK);
            }
        }

        // Lấy trạng thái hoạt động
        public object GetActiveStatus(int userId)
        {
            using (var db = DB.Open())
            {
                var user = db.SingleById<Users>(userId);
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
                }, HttpStatusCode.OK);
            }
        }

        // Cập nhật thông tin
        public object Put(DTOs.UpdateInfo request)
        {
            var userId = int.Parse(GetSession().UserAuthId);
            using (var db = DB.Open())
            {
                var user = db.SingleById<Users>(userId);
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
                    db.Exists<Users>(u => u.Email == request.Email))
                {
                    return new HttpResult(new DTOs.UpdateInfoResponse
                    {
                        Error = "Duplicated",
                        Message = "Email already exists"
                    }, HttpStatusCode.Conflict);
                }

                if (!request.PhoneNumber.IsNullOrEmpty() && request.PhoneNumber != user.PhoneNumber &&
                    db.Exists<Users>(u => u.PhoneNumber == request.PhoneNumber))
                {
                    return new HttpResult(new DTOs.UpdateInfoResponse
                    {
                        Error = "Duplicated",
                        Message = "Phone number already exists"
                    }, HttpStatusCode.Conflict);
                }

                // Cập nhật thông tin
                db.Update<Users>(new
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
                }, HttpStatusCode.OK);
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
                var user = db.SingleById<Users>(userId);
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
                    db.Update<Users>(new { Avatar = avatarUrl }, u => u.UserID == userId);

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
            var session = this.GetSession();
            if (!session.IsAuthenticated)
            {
                return new HttpResult(new { Error = "TokenUnauthorized", Message = "User is not authenticated." })
                {
                    StatusCode = HttpStatusCode.Unauthorized
                };
            }

            int userID = int.Parse(session.UserAuthId);

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
                var query = db.From<Users>();

                // Tìm kiếm theo tên (FirstName hoặc LastName) hoặc email
                query = query.Where(u =>
                    u.UserID != userID &&
                    (u.FirstName.ToLower().Contains(request.Query.ToLower()) ||
                    u.LastName.ToLower().Contains(request.Query.ToLower()) ||
                    u.Username.ToLower().Contains(request.Query.ToLower()) ||
                    u.Email.ToLower().Contains(request.Query.ToLower()) ||
                    u.PhoneNumber.ToLower().Contains(request.Query.ToLower()))
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
                }, HttpStatusCode.OK);
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

        //Cập nhật quyền riêng tư
        public object Put(DTOs.UpdatePrivacy request)
        {
            // Lấy uid và kiểm tra xem có tồn tại hay không?
            if (!int.TryParse(GetSession().UserAuthId, out int userId))
            {
                return new HttpResult(new DTOs.UpdatePrivacyResponse
                {
                    Error = "InvalidUserID",
                    Message = "User ID is invalid"
                }, HttpStatusCode.BadRequest);
            }

            using (var db = DB.Open())
            {
                var userSettings = db.SingleById<UserSettings>(userId);
                if (userSettings == null)
                {
                    return new HttpResult(new DTOs.UpdatePrivacyResponse
                    {
                        Error = "InvalidPrivacyID",
                        Message = "Invalid Privacy ID"
                    }, HttpStatusCode.BadRequest);
                }

                // Cập nhật thông tin quyền riêng tư
                userSettings.StatusPrivacy = db.Single<Privacy>(u => u.PrivacyName == request.ActiveStatus).PrivacyID;
                db.Update(userSettings);
                Debug.WriteLine(request.ActiveStatus);

                return new HttpResult(new DTOs.UpdatePrivacyResponse
                {
                    Message = "Privacy updated successfully"
                }, HttpStatusCode.OK);
            }
        }

        // Lấy quyền riêng tư
        public object Get(DTOs.GetPrivacy request)
        {
            using (var db = DB.Open())
            {
                var user = db.SingleById<UserSettings>(request.UserID);
                if (user == null)
                {
                    return new HttpResult(new DTOs.GetPrivacyResponse
                    {
                        Error = "NotFound",
                        Message = "User settings not found"
                    }, HttpStatusCode.NotFound);
                }

                return new HttpResult(new DTOs.GetPrivacyResponse
                {
                    Data = new DTOs.PrivacyInfo
                    {
                        ActiveStatus = db.Single<Privacy>(u => u.PrivacyID == user.StatusPrivacy).PrivacyName,
                        BioPrivacy = db.Single<Privacy>(u => u.PrivacyID == user.BioPrivacy).PrivacyName,
                        PhoneNumberPrivacy = db.Single<Privacy>(u => u.PrivacyID == user.PhoneNumberPrivacy).PrivacyName,
                        EmailPrivacy = db.Single<Privacy>(u => u.PrivacyID == user.EmailPrivacy).PrivacyName,
                        BirthdayPrivacy = db.Single<Privacy>(u => u.PrivacyID == user.BirthdayPrivacy).PrivacyName,
                        CallPrivacy = db.Single<Privacy>(u => u.PrivacyID == user.CallPrivacy).PrivacyName,
                        InviteGroupPrivacy = db.Single<Privacy>(u => u.PrivacyID == user.InviteGroupPrivacy).PrivacyName,
                        MessagePrivacy = db.Single<Privacy>(u => u.PrivacyID == user.MessagePrivacy).PrivacyName,
                    }
                }, HttpStatusCode.OK);
            }
        }

        public object Get(DTOs.GetAES request)
        {
            var userID = int.Parse(GetSession().UserAuthId);
            using (var db = DB.Open())
            {
                var user = db.Single<AesKeys>(k => k.ConversationID == request.ConversationID && k.UserID == userID);
                if (user == null)
                {
                    return new HttpResult(new DTOs.UpdateInfoResponse
                    {
                        Error = "NotFound",
                        Message = "User not found"
                    }, HttpStatusCode.NotFound);
                }

                return new HttpResult(new DTOs.GetAESResponse
                {
                    EncryptedAesKey = db.Single<AesKeys>(ea => ea.EncryptedAesKey == user.EncryptedAesKey).EncryptedAesKey,
                    IV = db.Single<AesKeys>(iv => iv.IV == user.IV).IV
                }, HttpStatusCode.OK);
            }
        }
    }
}