using HandyControl.Tools;
using Messenger.Services;
using System.Windows;

namespace Messenger
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ConfigHelper.Instance.SetWindowDefaultStyle();
            ConfigHelper.Instance.SetNavigationWindowDefaultStyle();
            ServiceLocator.ConfigureServices();
        }
    }
}
