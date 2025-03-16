using System;
using System.Text;
using APIServer.Plugins;
using APIServer.Services;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace APIServer
{
    public class AppHost : AppSelfHostBase
    {
        public AppHost() : base("GeckoMessengerAPI", typeof(AuthService).Assembly) { }

        public override void Configure(Funq.Container container)
        {
            container.AddSingleton<IDbConnectionFactory>(new OrmLiteConnectionFactory(
                Program.ConnectString,
                SqlServerDialect.Provider));

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
