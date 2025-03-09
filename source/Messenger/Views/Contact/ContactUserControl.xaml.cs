using Messenger.ViewModels;
using System.Windows.Controls;

namespace Messenger.Views.Contact
{
    /// <summary>
    /// Interaction logic for Contact.xaml
    /// </summary>
    public partial class ContactUserControl : UserControl
    {
        public ContactUserControl(ContactViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
