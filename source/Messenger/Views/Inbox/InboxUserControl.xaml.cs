using Messenger.ViewModels;
using System.Windows;
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

        private void ConversationContext_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.ContextMenu != null)
            {
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                button.ContextMenu.IsOpen = true;
            }
        }

        private void SearchBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is HandyControl.Controls.AutoCompleteTextBox autoCompleteTextBox)
            {
                var selectedItem = autoCompleteTextBox.SelectedItem as ViewModels.Conversation;
                if (selectedItem != null)
                {
                    var viewModel = DataContext as InboxViewModel;
                    viewModel?.SearchSelectionChangedCommand.Execute(selectedItem);
                }
            }
        }
    }
}
