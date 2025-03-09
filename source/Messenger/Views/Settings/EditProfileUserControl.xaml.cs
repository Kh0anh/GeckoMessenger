using Messenger.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Messenger.Views.Settings
{
    /// <summary>
    /// Interaction logic for EditProfileUserControl.xaml
    /// </summary>
    public partial class EditProfileUserControl : UserControl
    {
        public EditProfileUserControl(EditProfileViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void OpenEditImageContext_Click(object sender, RoutedEventArgs e)
        {
            if (OpenEditImageContext.ContextMenu != null)
            {
                OpenEditImageContext.ContextMenu.IsOpen = true;
                OpenEditImageContext.ContextMenu.PlacementTarget = OpenEditImageContext;
                OpenEditImageContext.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            }
        }
    }
}
