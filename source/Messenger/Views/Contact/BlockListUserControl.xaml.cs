using Messenger.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Messenger.Views.Contact
{
    /// <summary>
    /// Interaction logic for BlockListUserControl.xaml
    /// </summary>
    public partial class BlockListUserControl : UserControl
    {
        public BlockListUserControl(BlockListViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
        private void BlockContextOpen_OnClick(object sender, RoutedEventArgs e)
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
