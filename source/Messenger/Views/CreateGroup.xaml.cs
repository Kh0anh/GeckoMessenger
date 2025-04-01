using Messenger.ViewModels;
using System.Windows;

namespace Messenger.Views
{
    /// <summary>
    /// Interaction logic for CreateGroup.xaml
    /// </summary>
    public partial class CreateGroup : Window
    {
        public CreateGroup(CreateGroupViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

            if (DataContext is CreateGroupViewModel viewModel)
            {
                viewModel.Close += () =>
                {
                    this.Close();
                };
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
