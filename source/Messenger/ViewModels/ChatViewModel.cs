using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Messenger.ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        public string OtherUserAvatar { get; set; }
        public string OtherUserFullName { get; set; }
        private ObservableCollection<Message> _messages;
        public ObservableCollection<Message> Messages
        {
            get => _messages;
            set
            {
                _messages = value;
                OnPropertyChanged();
            }
        }

        private string _newMessageText;
        public string NewMessageText
        {
            get => _newMessageText;
            set
            {
                _newMessageText = value;
                OnPropertyChanged();
            }
        }

        public ICommand SendMessageCommand { get; }

        public ChatViewModel(string userAvatar, string userFullName)
        {
            OtherUserAvatar = userAvatar;
            OtherUserFullName = userFullName;

            Messages = new ObservableCollection<Message>();
            SendMessageCommand = new RelayCommand(_ => SendMessage());
        }

        private void SendMessage()
        {
            if (!string.IsNullOrWhiteSpace(NewMessageText))
            {
                var newMessage = new Message
                {
                    Text = NewMessageText,
                    Timestamp = DateTime.Now,
                    IsSentByMe = true
                };
                Messages.Add(newMessage);
                NewMessageText = string.Empty;
            }
        }
    }
    public class Message
    {
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsSentByMe { get; set; }
    }
}
