using APIServer.Models;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace APIServer.Services
{
    public class AuthService : Service
    {
        public IDbConnectionFactory DB { get; set; }

        //Đăng nhập
        public object Post(DTOs.Login request)
        {
            if ((request.Username.IsNullOrEmpty() && request.Email.IsNullOrEmpty()) ||
                request.Password.IsNullOrEmpty())
            {
                return new HttpResult(new DTOs.LoginResponse
                {
                    Error = "RequiredField",
                    Message = "This field cannot be empty."
                }, HttpStatusCode.BadRequest);
            }

            using (var db = DB.Open())
            {
                var user = db.Single<Users>(u =>
                    u.Username == request.Username || (!request.Email.IsNullOrEmpty() && u.Email == request.Email));

                if (user == null || !verifyPassword(request.Password, user.HashPassword))
                {
                    return new HttpResult(new DTOs.LoginResponse
                    {
                        Error = "Incorrect",
                        Message = "The username or password is incorrect."
                    }, HttpStatusCode.Unauthorized);
                }

                var authFeature = HostContext.GetPlugin<AuthFeature>();
                var jwtProvider = authFeature.AuthProviders.OfType<JwtAuthProvider>().FirstOrDefault();

                if (jwtProvider == null)
                {
                    return new HttpResult(new DTOs.LoginResponse
                    {
                        Error = "ServerError",
                        Message = "JWT Authentication is not configured."
                    }, HttpStatusCode.InternalServerError);
                }

                var token = jwtProvider.CreateJwtBearerToken(new AuthUserSession
                {
                    UserAuthId = user.UserID.ToString(),
                });

                return new HttpResult(new DTOs.LoginResponse
                {
                    Token = token,
                    UserID = user.UserID,
                    Message = "Login successful"
                }, HttpStatusCode.OK);
            }
        }

        //Đăng ký
        public object Post(DTOs.Register request)
        {
            if (request.Username.IsNullOrEmpty() || request.Email.IsNullOrEmpty() ||
                request.Password.IsNullOrEmpty() || request.PhoneNumber.IsNullOrEmpty())
            {
                return new HttpResult(new DTOs.RegisterResponse
                {
                    Error = "RequiredField",
                    Message = "All fields are required."
                }, HttpStatusCode.BadRequest);
            }

            using (var db = DB.Open())
            {
                if (db.Exists<Users>(u => u.Username == request.Username || u.Email == request.Email))
                {
                    return new HttpResult(new DTOs.RegisterResponse
                    {
                        Error = "AlreadyExists",
                        Message = "Username or Email already exists."
                    }, HttpStatusCode.Conflict);
                }

                var user = new Users
                {
                    Username = request.Username,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    Birthday = request.Birthday.Date,
                    HashPassword = hashPassword(request.Password),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Bio = "",
                    Avatar = "storages/DefaultAvatar.png",
                    LastLogin = DateTime.UtcNow
                };
                Debug.WriteLine(request.Birthday.ToString("yyyy-MM-dd"));
                db.Save(user);

                var contactPrivacy = db.Single<Privacy>(p => p.PrivacyName == "CONTACT");
                var publicPrivacy = db.Single<Privacy>(p => p.PrivacyName == "PUBLIC");

                var userSetting = new UserSettings
                {
                    UserID = user.UserID,
                    StatusPrivacy = contactPrivacy.PrivacyID,
                    BioPrivacy = publicPrivacy.PrivacyID,
                    PhoneNumberPrivacy = contactPrivacy.PrivacyID,
                    EmailPrivacy = contactPrivacy.PrivacyID,
                    BirthdayPrivacy = publicPrivacy.PrivacyID,
                    CallPrivacy = contactPrivacy.PrivacyID,
                    InviteGroupPrivacy = contactPrivacy.PrivacyID,
                    MessagePrivacy = publicPrivacy.PrivacyID,
                };
                db.Save(userSetting);

                var authFeature = HostContext.GetPlugin<AuthFeature>();
                var jwtProvider = authFeature.AuthProviders.OfType<JwtAuthProvider>().FirstOrDefault();

                if (jwtProvider == null)
                {
                    return new HttpResult(new DTOs.RegisterResponse
                    {
                        Error = "ServerError",
                        Message = "JWT Authentication is not configured."
                    }, HttpStatusCode.InternalServerError);
                }

                var token = jwtProvider.CreateJwtBearerToken(new AuthUserSession
                {
                    UserAuthId = user.UserID.ToString(),
                });

                return new HttpResult(new DTOs.RegisterResponse
                {
                    Token = token,
                    UserID = user.UserID,
                    Message = "Registration successful"
                }, HttpStatusCode.Created);
            }
        }

        //Đổi mật khẩu
        public object Put(DTOs.ChangePassword request)
        {
            if (request.OldPassword.IsNullOrEmpty() || request.NewPassword.IsNullOrEmpty() || request.ConfirmPassword.IsNullOrEmpty())
            {
                return new HttpResult(new DTOs.ChangePasswordResponse
                {
                    Error = "RequiredField",
                    Message = "Old password and new password are required."
                }, HttpStatusCode.BadRequest);
            }

            using (var db = DB.Open())
            {
                var userId = int.Parse(GetSession().UserAuthId);
                var user = db.SingleById<Users>(userId);

                if (user == null)
                {
                    return new HttpResult(new DTOs.ChangePasswordResponse
                    {
                        Error = "NotFound",
                        Message = "User not found."
                    }, HttpStatusCode.NotFound);
                }

                // Xác thực mật khẩu cũ
                if (!verifyPassword(request.OldPassword, user.HashPassword))
                {
                    return new HttpResult(new DTOs.ChangePasswordResponse
                    {
                        Error = "Incorrect",
                        Message = "Current password is incorrect."
                    }, HttpStatusCode.Unauthorized);
                }

                // Hash và cập nhật mật khẩu mới
                user.HashPassword = hashPassword(request.NewPassword);
                db.Update<Users>(new { HashPassword = user.HashPassword }, u => u.UserID == userId);

                return new HttpResult(new DTOs.ChangePasswordResponse
                {
                    Message = "Password changed successfully"
                }, HttpStatusCode.OK);
            }
        }

        private static string hashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        private static bool verifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
