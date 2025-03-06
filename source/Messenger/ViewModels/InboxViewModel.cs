using Messenger.Views.Inbox;

namespace Messenger.ViewModels
{
    public class InboxViewModel : BaseViewModel
    {
        public object CurrentChat { get; set; }
        public InboxViewModel()
        {
            CurrentChat = new ChatUserControl(new ChatViewModel("UserTest", "My Test Name"));
        }
    }
}
