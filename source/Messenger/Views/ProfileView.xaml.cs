using Messenger.ViewModels;
using System.Windows;

namespace Messenger.Views
{
    /// <summary>
    /// Interaction logic for ProfileView.xaml
    /// </summary>
    public partial class ProfileView : Window
    {
        public ProfileView(ProfileViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
