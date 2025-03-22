using APIServer.Plugins;
using APIServer.Services;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System;
using System.Text;

namespace APIServer
{
    public class AppHost : AppSelfHostBase
    {
        public AppHost() : base("GeckoMessengerAPI", typeof(AdminService).Assembly, typeof(AuthService).Assembly, typeof(ChatService).Assembly, typeof(ContactService).Assembly, typeof(ReportService).Assembly, typeof(UserService).Assembly) { }

        public override void Configure(Funq.Container container)
        {
            container.AddSingleton<IDbConnectionFactory>(new OrmLiteConnectionFactory(
                Program.ConnectString,
                SqlServerDialect.Provider));
            OrmLiteConfig.DialectProvider.GetStringConverter().UseUnicode = true;

            Plugins.Add(new FilePlugin());

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
                        }
                    }
                }));
        }
    }
}
