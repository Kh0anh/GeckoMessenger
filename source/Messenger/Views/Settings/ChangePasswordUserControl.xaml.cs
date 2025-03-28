using Messenger.ViewModels;
using System.Windows.Controls;

namespace Messenger.Views.Settings
{
    /// <summary>
    /// Interaction logic for ChangePasswordUserControl.xaml
    /// </summary>
    public partial class ChangePasswordUserControl : UserControl
    {
        public ChangePasswordUserControl(ChangePasswordViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
