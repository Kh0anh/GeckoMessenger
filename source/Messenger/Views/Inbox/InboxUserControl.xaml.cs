using Messenger.ViewModels;
using System.Windows.Controls;

namespace Messenger.Views.Inbox
{
    /// <summary>
    /// Interaction logic for Inbox.xaml
    /// </summary>
    public partial class InboxUserControl : UserControl
    {
        public InboxUserControl(InboxViewModel viewmodel)
        {
            InitializeComponent();
            DataContext = viewmodel;
        }
    }
}
