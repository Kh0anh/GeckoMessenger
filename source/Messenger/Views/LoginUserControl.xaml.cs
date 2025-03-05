using Messenger.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Messenger.Views
{
    /// <summary>
    /// Interaction logic for LoginUserControl.xaml
    /// </summary>
    public partial class LoginUserControl : UserControl
    {
        private readonly LoginViewModel _viewmodel;
        public LoginUserControl()
        {
            InitializeComponent();
        }
        public LoginUserControl(LoginViewModel viewmodel)
        {
            InitializeComponent();
            _viewmodel = viewmodel;
            DataContext = viewmodel;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                _viewmodel.password = pwdPassword.Password;
            }
        }
    }
}
