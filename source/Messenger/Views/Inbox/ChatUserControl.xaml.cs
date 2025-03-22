using Messenger.ViewModels;
using System.Windows;
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
            if (DataContext is ChatViewModel viewModel)
            {
                viewModel.ScrollToEnd += () =>
                {
                    MessagesScrollViewer.ScrollToEnd();
                };
            }
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

        private void SearchMessageOpen_OnClick(object sender, RoutedEventArgs e)
        {
            ToggleSearchMessage.IsChecked = !ToggleSearchMessage.IsChecked;
        }

        private void MessageContextOpen_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.ContextMenu != null)
            {
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                button.ContextMenu.IsOpen = true;
            }
        }
    }
}
