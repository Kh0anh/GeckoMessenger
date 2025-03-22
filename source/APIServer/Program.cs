using ServiceStack;
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
            Licensing.RegisterLicense("\r\nTRIAL30WEB-e3JlZjpUUklBTDMwV0VCLG5hbWU6My8yMC8yMDI1IGNjNDliNTZiZTBlMDQ1YzU5MThmOGQxMGQ4MGYyMTNlLHR5cGU6VHJpYWwsbWV0YTowLGhhc2g6U0xVNUlLWFkvMWFVQ01BSzNBT1ZEUnlkZmF5UW1pSmhPM2t6TDFnWWNlWm1JZEE0eXUyb1Y2WitDTUxsQ2p0RExoSlRCNHBwUE9iVkFsYVVwWUpuL2N1Z2pzdGRlMGlRMUNxN3NxbjlubnV6bXcvQ1RGaG9SQkZzOFhQWUMvaEh5em5tSWVielk3SVNiRnBlREJUNWYxTUI1bU1sUmdBL1NIcWFNZFdNVXY4PSxleHBpcnk6MjAyNS0wNC0xOX0=\r\n");
            appHost.Init();
            appHost.Start(APIUrl);

            Console.WriteLine($"ServiceStack API running at {APIUrl}");
            Console.ReadLine();
        }
    }
}
