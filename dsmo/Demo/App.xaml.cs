using System.Windows;

namespace Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly string APIUrl = "http://localhost:8080/";
        public static string UserToken;
        public static MainWindow MainWinow;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWinow = new MainWindow();
            MainWinow.Show();
        }
    }
}
