using Messenger.ViewModels;
using System.Windows.Controls;

namespace Messenger.Views.Contact
{
    /// <summary>
    /// Interaction logic for SuggestContactUserControl.xaml
    /// </summary>
    public partial class SuggestContactListUserControl : UserControl
    {
        public SuggestContactListUserControl(SuggestContactListViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
