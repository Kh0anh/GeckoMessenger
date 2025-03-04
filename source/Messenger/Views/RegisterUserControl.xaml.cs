using Messenger.ViewModels;
using System.Windows.Controls;

namespace Messenger.Views
{
    /// <summary>
    /// Interaction logic for RegisterUserControl.xaml
    /// </summary>
    public partial class RegisterUserControl : UserControl
    {
        public RegisterUserControl()
        {
            InitializeComponent();
        }
        public RegisterUserControl(RegisterViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
