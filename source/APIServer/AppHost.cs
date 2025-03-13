using ServiceStack;

namespace APIServer
{
    public class AppHost : AppSelfHostBase
    {
        private readonly string ConnectString = "";
        public AppHost() : base("GeckoMessengerAPI", typeof(AuthService).Assembly, typeof(MessageService).Assembly) { }

        public override void Configure(Funq.Container container)
        {
        }
    }
}
