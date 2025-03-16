using DemoAPI.Models;
using DemoAPI.Services;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Configuration;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System;
using System.Text;

namespace DemoAPI
{
    public class AppHost : AppSelfHostBase
    {
        public AppHost() : base("Demo API", typeof(AuthService).Assembly, typeof(MessageService).Assembly) { }

        public override void Configure(Funq.Container container)
        {
            //Lấy connect string trong App.config
            var connectionString = ConfigUtils.GetConnectionString("DemoDB");
            container.AddSingleton<IDbConnectionFactory>(new OrmLiteConnectionFactory(
                connectionString,
                SqlServerDialect.Provider));

            //Mở kết nối và kiểm tra bản có tồn tại chưa nếu chưa thì tạo
            var db = container.Resolve<IDbConnectionFactory>().Open();
            db.CreateTableIfNotExists<User>();
            db.CreateTableIfNotExists<Conversation>();
            db.CreateTableIfNotExists<Message>();

            //Thêm xác thực JWT
            Plugins.Add(new AuthFeature(() => new AuthUserSession(),
                new IAuthProvider[]
                {
                    new JwtAuthProvider(AppSettings)
                    {
                        AuthKey = Encoding.UTF8.GetBytes(Program.JWTKey),
                        RequireSecureConnection = false,
                        ExpireTokensIn = TimeSpan.FromDays(365),
                         PopulateSessionFilter = (session, jwtPayload, req) =>
                         {
                            session.UserAuthId = jwtPayload["sub"];
                            session.UserAuthName = jwtPayload["name"];
                         }
                    }
                }));

        }
    }
}
