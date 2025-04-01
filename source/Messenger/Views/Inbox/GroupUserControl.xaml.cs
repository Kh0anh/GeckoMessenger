using Messenger.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Messenger.Views.Inbox
{
    /// <summary>
    /// Interaction logic for GroupUserControl.xaml
    /// </summary>
    public partial class GroupUserControl : UserControl
    {
        public GroupUserControl(GroupViewModel viewmodel)
        {
            InitializeComponent();
            DataContext = viewmodel;
            if (DataContext is GroupViewModel viewModel)
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
