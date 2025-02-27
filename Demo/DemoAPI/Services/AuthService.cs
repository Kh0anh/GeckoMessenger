using DemoAPI.Models;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System.Linq;
using System.Net;

namespace DemoAPI.Services
{
    public class AuthService : Service
    {
        public IDbConnectionFactory DB { get; set; }
        public object Post(DTOs.Register request)
        {
            var passwordHash = HashPassword(request.Password);

            using (var db = DB.Open())
            {
                if (db.Exists<User>(u => u.Username == request.Username))
                    throw new HttpError(HttpStatusCode.Conflict, "UserAlreadyExists", "User already exists");

                var newUser = new User
                {
                    Username = request.Username,
                    PasswordHash = passwordHash
                };

                long userID = db.Insert(newUser, selectIdentity: true);

                return new HttpResult(new DTOs.RegisterResponse
                {
                    Success = true,
                    UserID = (int)userID
                }, HttpStatusCode.Created);
            }
        }
        public object Post(DTOs.Login request)
        {
            using (var db = DB.Open())
            {
                var user = db.Single<User>(u => u.Username == request.Username);

                if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
                    throw new HttpError(HttpStatusCode.Unauthorized, "Incorrect", "The username or password is incorrect");

                var authFeature = HostContext.GetPlugin<AuthFeature>();
                var jwtProvider = authFeature.AuthProviders.OfType<JwtAuthProvider>().FirstOrDefault();

                var token = jwtProvider.CreateJwtBearerToken(new AuthUserSession
                {
                    UserAuthId = user.UserID.ToString(),
                    UserAuthName = user.Username,
                    IsAuthenticated = true,
                });
                return new HttpResult(new DTOs.LoginResponse
                {
                    Success = true,
                    Token = token
                }, HttpStatusCode.OK);
            }
        }

        public object Post(DTOs.UserExist request)
        {
            var passwordHash = HashPassword(request.Username);

            using (var db = DB.Open())
            {
                if (db.Exists<User>(u => u.Username == request.Username))
                    return new HttpResult(new DTOs.UserExistResponse
                    {
                        Exist = true,
                    }, HttpStatusCode.OK);

                return new HttpResult(new DTOs.UserExistResponse
                {
                    Exist = false,
                }, HttpStatusCode.OK);
            }
        }

        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }
        public static bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
