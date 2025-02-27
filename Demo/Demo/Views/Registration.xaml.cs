using ServiceStack;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Demo.Views
{
    /// <summary>
    /// Interaction logic for Registration.xaml
    /// </summary>
    public partial class Registration : UserControl
    {
        public Registration()
        {
            InitializeComponent();
        }
        private void btnSwitchLogin_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            App.MainWinow.ContentArea.Content = new Login();
        }

        private void btnSignUp_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;
            Task.Run(() =>
            {
                var client = new JsonHttpClient(App.APIUrl);

                Models.Register registerData = new Models.Register
                {
                    Username = username,
                    Password = password
                };

                try
                {
                    Models.RegisterResponse response = client.Post<Models.RegisterResponse>("/auth/register", registerData);

                    Dispatcher.Invoke(() =>
                    {
                        Dispatcher.Invoke(() => status.Text = "Registration successful");
                    });
                }
                catch (Exception err)
                {
                    Dispatcher.Invoke(() => status.Text = err.Message);
                }
            });
        }
    }
}
