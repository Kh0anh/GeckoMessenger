using Messenger.Views.Inbox;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Messenger.ViewModels
{
    public class InboxViewModel : BaseViewModel
    {
        public object _CurrentChat;
        public object CurrentChat
        {
            get => _CurrentChat;
            set
            {
                _CurrentChat = value;
                OnPropertyChanged();
            }
        }
        private List<Conversation> _Conversations;
        public List<Conversation> Conversations
        {
            get => _Conversations;
            set
            {
                _Conversations = value;
                OnPropertyChanged();
            }
        }
        private Conversation _SelectedConversation;
        public Conversation SelectedConversation
        {
            get => _SelectedConversation;
            set
            {
                _SelectedConversation = value;
                OnPropertyChanged();
                UpdateCurrentChat();
            }
        }
        public ICommand ViewChat;
        public InboxViewModel()
        {
            Conversations = new List<Conversation>
            {
                new Conversation
                {
                    ChatView =  new ChatUserControl(new ChatViewModel("UserTest", "User Test 1")),
                    ConversationName = "User Test 1",
                    LatestMessageContent = "Xin chào",
                    LatestMessage = DateTime.Now,
                },
                new Conversation
                {
                    ChatView =  new ChatUserControl(new ChatViewModel("UserTest2", "User Test 1")),
                    ConversationName = "User Test 2",
                    LatestMessageContent = "Baiii",
                    LatestMessage = DateTime.Now,
                }
            };
        }
        private void UpdateCurrentChat()
        {
            if (SelectedConversation != null)
            {
                CurrentChat = SelectedConversation.ChatView;
            }
            else
            {
                CurrentChat = null;
            }
        }
    }
    public class Conversation
    {
        public ChatUserControl ChatView { get; set; }
        public string ConversationName { get; set; }
        public string ConversationAvatar { get; set; }
        public string LatestMessageContent { get; set; }
        public DateTime LatestMessage { get; set; }
    }
}
