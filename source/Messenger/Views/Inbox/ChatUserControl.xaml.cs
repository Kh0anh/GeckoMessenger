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

        private void ChatContextOpen_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (btChatContextOpen.ContextMenu != null)
            {
                btChatContextOpen.ContextMenu.IsOpen = true;
                btChatContextOpen.ContextMenu.PlacementTarget = btChatContextOpen;
                btChatContextOpen.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            }
        }
    }
}
