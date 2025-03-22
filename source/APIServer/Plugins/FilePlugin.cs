using APIServer.Utils;
using ServiceStack;
using ServiceStack.IO;
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
