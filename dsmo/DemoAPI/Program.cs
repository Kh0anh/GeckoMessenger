using System;

namespace DemoAPI
{
    internal class Program
    {
        //Địa chỉ API
        public static readonly string APIUrl = "http://localhost:8080/";

        //Mật mã JWT
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
