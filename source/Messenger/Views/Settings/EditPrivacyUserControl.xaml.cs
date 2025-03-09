using Messenger.ViewModels;
using System.Windows.Controls;

namespace Messenger.Views.Settings
{
    /// <summary>
    /// Interaction logic for EditPrivacyUserControl.xaml
    /// </summary>
    public partial class EditPrivacyUserControl : UserControl
    {
        public EditPrivacyUserControl(EditPrivacyViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
