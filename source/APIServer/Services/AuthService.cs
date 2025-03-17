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
    public class AuthService : Service
    {
        public IDbConnectionFactory DB { get; set; }

        public object Post(DTOs.Login request)
        {
            if ((request.Username.IsNullOrEmpty() && request.Email.IsNullOrEmpty()) ||
                request.Password.IsNullOrEmpty())
            {
                return new HttpResult(new { Error = "RequiredField", Message = "This field cannot be empty." })
                {
                    StatusCode = HttpStatusCode.OK
                };
            }

            using (var db = DB.Open())
            {
                var user = db.Single<User>(u =>
                    u.Username == request.Username || (!request.Email.IsNullOrEmpty() && u.Email == request.Email));

                if (user == null || !verifyPassword(request.Password, user.HashPassword))
                {
                    return new HttpResult(new { Error = "Incorrect", Message = "The username or password is incorrect." })
                    {
                        StatusCode = HttpStatusCode.OK
                    };
                }

                var authFeature = HostContext.GetPlugin<AuthFeature>();
                var jwtProvider = authFeature.AuthProviders.OfType<JwtAuthProvider>().FirstOrDefault();

                if (jwtProvider == null)
                {
                    return new HttpResult(new { Error = "ServerError", Message = "JWT Authentication is not configured." })
                    {
                        StatusCode = HttpStatusCode.InternalServerError
                    };
                }

                var token = jwtProvider.CreateJwtBearerToken(new AuthUserSession
                {
                    UserAuthId = user.UserID.ToString(),
                });
                return new HttpResult(new DTOs.LoginResponse
                {
                    UserID = user.UserID,
                    Username = user.Username,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Birthday = user.Birthday,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Bio = user.Bio,
                    Avatar = user.Avatar,
                    Token = token,
                }, HttpStatusCode.OK);
            }
        }

        public object Post(DTOs.Register request)
        {
            if (request.Username.IsNullOrEmpty() || request.Email.IsNullOrEmpty() ||
                request.Password.IsNullOrEmpty() || request.PhoneNumber.IsNullOrEmpty())
            {
                return new HttpResult(new { Error = "RequiredField", Message = "All fields are required." })
                {
                    StatusCode = HttpStatusCode.OK
                };
            }

            using (var db = DB.Open())
            {
                if (db.Exists<User>(u => u.Username == request.Username || u.Email == request.Email))
                {
                    return new HttpResult(new { Error = "AlreadyExists", Message = "Username or Email already exists." })
                    {
                        StatusCode = HttpStatusCode.OK
                    };
                }

                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    Birthday = request.Birthday,
                    HashPassword = hashPassword(request.Password),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Avatar = "storages/DefaultAvatar.png",
                    CreatedAt = DateTime.UtcNow
                };
                db.Save(user);

                var contactPrivacy = db.Single<Privacy>(p => p.PrivacyName == "CONTACT");
                var publicPrivacy = db.Single<Privacy>(p => p.PrivacyName == "PUBLIC");

                var userSetting = new UserSetting
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

                return new HttpResult(new DTOs.RegisterResponse { }, HttpStatusCode.OK);
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
