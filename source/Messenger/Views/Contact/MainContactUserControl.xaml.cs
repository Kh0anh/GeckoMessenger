using Messenger.ViewModels;
using System.Windows.Controls;

namespace Messenger.Views.Contact
{
    /// <summary>
    /// Interaction logic for Contact.xaml
    /// </summary>
    public partial class MainContactUserControl : UserControl
    {
        public MainContactUserControl(MainContactViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
