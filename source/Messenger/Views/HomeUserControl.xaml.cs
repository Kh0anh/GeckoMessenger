using Messenger.ViewModels;
using System.Windows.Controls;

namespace Messenger.Views
{
    /// <summary>
    /// Interaction logic for HomeUserControl.xaml
    /// </summary>
    public partial class HomeUserControl : UserControl
    {
        public HomeUserControl(HomeViewModel viewmodel)
        {
            InitializeComponent();
            DataContext = viewmodel;
        }

        private void UserInfo_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (UserInfoOpen.ContextMenu != null)
            {
                UserInfoOpen.ContextMenu.IsOpen = true;
                UserInfoOpen.ContextMenu.PlacementTarget = UserInfoOpen;
                UserInfoOpen.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            }
        }

        private void Settings_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (btSetting.ContextMenu != null)
            {
                btSetting.ContextMenu.IsOpen = true;
                btSetting.ContextMenu.PlacementTarget = btSetting;
                btSetting.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            }
        }

        private void Close_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}
