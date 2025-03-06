using Messenger.ViewModels;
using System.Windows.Controls;

namespace Messenger.Views.Inbox
{
    /// <summary>
    /// Interaction logic for Chat.xaml
    /// </summary>
    public partial class ChatUserControl : UserControl
    {
        public ChatUserControl(ChatViewModel viewmodel)
        {
            InitializeComponent();
            DataContext = viewmodel;
        }
    }
}
