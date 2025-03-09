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

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (chatContextOpen.ContextMenu != null)
            {
                chatContextOpen.ContextMenu.IsOpen = true;
                chatContextOpen.ContextMenu.PlacementTarget = chatContextOpen;
                chatContextOpen.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            }
        }
    }
}
