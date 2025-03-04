using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Services
{
    public static class ServiceLocator
    {
        private static ServiceProvider _provider;

        public static void ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IUserService, UserService>();
            _provider = services.BuildServiceProvider();
        }

        public static T GetService<T>() => _provider.GetService<T>();
    }
}
