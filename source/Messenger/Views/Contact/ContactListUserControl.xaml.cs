using Messenger.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Messenger.Views.Contact
{
    /// <summary>
    /// Interaction logic for ContactListUserControl.xaml
    /// </summary>
    public partial class ContactListUserControl : UserControl
    {
        public ContactListUserControl(ContactListViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void ContactContextOpen_OnClick(object sender, RoutedEventArgs e)
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
