using DemoAPI.Models;
using Serilog;
using ServiceStack;
using ServiceStack.Configuration;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System;
using System.ComponentModel;
using System.Configuration;

namespace DemoAPI
{
    public class AppHost : AppHostBase
    {
        public AppHost() : base("Demo API", typeof(AppHost).Assembly) { }

        public override void Configure(Funq.Container container)
        {
            var connectionString = ConfigUtils.GetConnectionString("DefaultConnection");
            container.AddSingleton<IDbConnectionFactory>(new OrmLiteConnectionFactory(
                connectionString,
                SqlServerDialect.Provider));
            var db = container.Resolve<IDbConnectionFactory>().Open();
            db.CreateTableIfNotExists<User>();
            db.CreateTableIfNotExists<Message>();
        }
    }
}
