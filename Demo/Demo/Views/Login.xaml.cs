using Demo.Models;
using ServiceStack;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Demo.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : UserControl
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;
            Task.Run(() =>
            {
                var client = new JsonHttpClient(App.APIUrl);

                Models.Login loginData = new Models.Login
                {
                    Username = username,
                    Password = password
                };

                try
                {
                    LoginResponse response = client.Post<LoginResponse>("/auth/login", loginData);

                    Dispatcher.Invoke(() =>
                    {
                        App.UserToken = response.Token;
                        App.MainWinow.ContentArea.Content = new Home();
                    });
                }
                catch (Exception err)
                {
                    Dispatcher.Invoke(() => status.Text = err.Message);
                }
            });
        }

        private void btnSwitchSignUp_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            App.MainWinow.ContentArea.Content = new Registration();
        }
    }
}
