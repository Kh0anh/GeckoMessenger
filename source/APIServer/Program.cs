using System;

namespace APIServer
{
    internal class Program
    {
        public static readonly string APIUrl = "http://localhost:8080/";
        public static readonly string ConnectString = "Server=kh0anh.hopto.org,1433;Database=GeckoMessenger;User Id=sa;Password=giahuyst;";
        public static readonly string JWTKey = "0000000000000000000000000000000";
        static void Main(string[] args)
        {
            var appHost = new AppHost();
            appHost.Init();
            appHost.Start(APIUrl);

            Console.WriteLine($"ServiceStack API running at {APIUrl}");
            Console.ReadLine();
        }
    }
}
