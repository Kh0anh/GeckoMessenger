using HandyControl.Tools.Command;
using Messenger.Services;
using Messenger.Utils;
using Messenger.Views.Inbox;
using ServiceStack;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Messenger.ViewModels
{
    public class InboxViewModel : BaseViewModel
    {
        private object _CurrentChat;
        public object CurrentChat
        {
            get => _CurrentChat;
            set
            {
                _CurrentChat = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Conversation> _Conversations;
        public ObservableCollection<Conversation> Conversations
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

        private string _Search;
        public string Search
        {
            get => _Search;
            set
            {
                _Search = value;
                OnPropertyChanged(nameof(Search));
            }
        }

        public ICommand SearchSelectionChangedCommand { get; }

        public InboxViewModel()
        {
            SearchSelectionChangedCommand = new RelayCommand<Conversation>(SearchSelectionChanged);
            Conversations = new ObservableCollection<Conversation>();

            Task.Run(TaskLoadConversation);
        }

        private async void TaskLoadConversation()
        {
            while (true)
            {
                await Task.Delay(1000);

                var userService = ServiceLocator.GetService<IUserService>();
                if (userService.User == null)
                {
                    continue;
                }

                var client = new JsonServiceClient(ConfigurationManager.AppSettings["APIUrl"]);
                client.BearerToken = userService.User.AuthToken;

                var getConversations = new DTOs.GetConversations();

                try
                {
                    var response = await client.GetAsync(getConversations);
                    if (response?.Conversations != null)
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            var selectedId = SelectedConversation?.ConversationID;

                            foreach (var conversationResponse in response.Conversations)
                            {
                                var existingConversation = Conversations.FirstOrDefault(c => c.ConversationID == conversationResponse.ConversationID);

                                if (existingConversation != null)
                                {
                                    if (existingConversation.LatestMessage != conversationResponse.LatestMessageTime)
                                    {
                                        existingConversation.LatestMessageContent = conversationResponse.LatestMessage;
                                        existingConversation.LatestMessage = conversationResponse.LatestMessageTime;
                                    }
                                }
                                else
                                {
                                    Debug.WriteLine(conversationResponse.LatestMessage);
                                    var newConversation = new Conversation
                                    {
                                        ConversationID = conversationResponse.ConversationID,
                                        LatestMessageContent = conversationResponse.LatestMessage,
                                        LatestMessage = conversationResponse.LatestMessageTime,
                                        ChatView = new ChatUserControl(new ChatViewModel(conversationID: conversationResponse.ConversationID))
                                    };

                                    if (conversationResponse.ConversationType == "CHAT")
                                    {
                                        DTOs.ParticipantResponse participant = conversationResponse.Participants
                                            .FirstOrDefault(p => p.UserID != userService.User.UserID);

                                        if (participant != null)
                                        {
                                            LoadUserInfoTask(participant, newConversation);
                                        }
                                    }

                                    Conversations.Add(newConversation);
                                }
                            }

                            var sortedConversations = Conversations.OrderByDescending(c => c.LatestMessage).ToList();
                            for (int i = 0; i < sortedConversations.Count; i++)
                            {
                                if (Conversations[i] != sortedConversations[i])
                                {
                                    Conversations.Move(Conversations.IndexOf(sortedConversations[i]), i);
                                }
                            }

                            SelectedConversation = Conversations.FirstOrDefault(c => c.ConversationID == selectedId);
                        });
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }

        private async void LoadUserInfoTask(DTOs.ParticipantResponse participant, Conversation conversation)
        {
            var userService = ServiceLocator.GetService<IUserService>();
            if (userService.User == null) return;

            var client = new JsonServiceClient(ConfigurationManager.AppSettings["APIUrl"]);
            client.BearerToken = userService.User.AuthToken;

            var getInfo = new DTOs.GetInfo { UserID = participant.UserID };

            try
            {
                var response = await client.GetAsync(getInfo);
                if (!string.IsNullOrEmpty(response.Error))
                {
                    throw new Exception(response.Message);
                }

                var avatarUrl = ConfigurationManager.AppSettings["APIUrl"] + response.Data.Avatar;
                var avatarImage = LoadImage.LoadImageFromUrl(avatarUrl);
                var fullName = $"{response.Data.LastName} {response.Data.FirstName}";

                App.Current.Dispatcher.Invoke(() =>
                {
                    conversation.ConversationName = fullName;
                    conversation.ConversationAvatar = avatarImage;
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void SearchSelectionChanged(Conversation conversation)
        {
            Debug.WriteLine(conversation?.ConversationName);
        }

        private void UpdateCurrentChat()
        {
            CurrentChat = SelectedConversation?.ChatView;
        }
    }

    public class Conversation : BaseViewModel
    {
        public int ConversationID { get; set; }
        private string _conversationName;
        public string ConversationName
        {
            get => _conversationName;
            set
            {
                _conversationName = value;
                OnPropertyChanged(nameof(ConversationName));
            }
        }

        private ImageSource _conversationAvatar;
        public ImageSource ConversationAvatar
        {
            get => _conversationAvatar;
            set
            {
                _conversationAvatar = value;
                OnPropertyChanged(nameof(ConversationAvatar));
            }
        }

        private string _LatestMessageContent;
        public string LatestMessageContent
        {
            get => _LatestMessageContent;
            set
            {
                _LatestMessageContent = value;
                OnPropertyChanged(nameof(LatestMessageContent));
            }
        }

        private DateTime _LatestMessage;
        public DateTime LatestMessage
        {
            get => _LatestMessage;
            set
            {
                _LatestMessage = value;
                OnPropertyChanged(nameof(LatestMessage));
            }
        }
        public ChatUserControl ChatView { get; set; }
    }
}
