using Messenger.ViewModels;
using System.Windows;

namespace Messenger.Views.Settings
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class MainSettingsView : Window
    {
        public MainSettingsView(MainSettingsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
