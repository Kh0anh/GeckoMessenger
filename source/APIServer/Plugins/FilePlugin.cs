using ServiceStack.IO;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using APIServer.Utils;
using System.IO;

namespace APIServer.Plugins
{
    public class FilePlugin : IPlugin
    {
        private static readonly string rootDir = "storages";

        public void Register(IAppHost appHost)
        {
            if (!Directory.Exists(rootDir))
            {
                Directory.CreateDirectory(rootDir);
            }

            appHost.VirtualFiles = new FileSystemVirtualFiles(rootDir);

            var vfs = appHost.VirtualFiles;
            if (!vfs.FileExists("DefaultAvatar.png"))
            {
                vfs.WriteFile("DefaultAvatar.png", ResourceHelper.GetEmbeddedResource("APIServer.Resources.DefaultAvatar.png"));
            }
        }
    }

}
