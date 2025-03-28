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
using System.Threading;
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
                OnPropertyChanged(nameof(CurrentChat));
            }
        }

        private ObservableCollection<Conversation> _Conversations;
        public ObservableCollection<Conversation> Conversations
        {
            get => _Conversations;
            set
            {
                _Conversations = value;
                OnPropertyChanged(nameof(Conversations));
            }
        }

        private object _SelectedConversation;
        public object SelectedConversation
        {
            get => _SelectedConversation;
            set
            {
                _SelectedConversation = value;
                OnPropertyChanged();
                UpdateCurrentChat();
            }
        }

        private CancellationTokenSource _cts;

        private string _SearchText = "";
        public string SearchText
        {
            get => _SearchText;
            set
            {
                _SearchText = value;
                OnPropertyChanged(nameof(SearchText));


                if (_SearchText.Length > 2)
                {
                    DoSearch(_SearchText);
                }
            }
        }

        private ObservableCollection<SearchResult> _SearchResults;
        public ObservableCollection<SearchResult> SearchResults
        {
            get => _SearchResults;
            set
            {
                _SearchResults = value;
                OnPropertyChanged(nameof(SearchResults));
            }
        }

        public ICommand SearchSelectionChangedCommand { get; }

        public InboxViewModel()
        {
            SearchSelectionChangedCommand = new RelayCommand<Conversation>(SearchSelectionChanged);
            Conversations = new ObservableCollection<Conversation>();
            SearchResults = new ObservableCollection<SearchResult>();
            Task.Run(TaskLoadConversation);
        }

        private async void DoSearch(string searchText)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            try
            {
                await Task.Delay(500, token);
                if (token.IsCancellationRequested) return;

                Search(searchText);
            }
            catch (TaskCanceledException) { }
        }

        private void Search(string searchText)
        {
            var userService = ServiceLocator.GetService<IUserService>();
            if (userService.User == null)
            {
                return;
            }

            var client = new JsonServiceClient(ConfigurationManager.AppSettings["APIUrl"]);
            client.BearerToken = userService.User.AuthToken;

            var searchUser = new DTOs.SearchUser
            {
                Query = searchText,
                MaxResult = 5
            };

            try
            {
                var response = client.Get(searchUser);

                App.Current.Dispatcher.Invoke(() =>
                {
                    SearchResults.Clear();
                    OnPropertyChanged(nameof(SearchResults));
                });

                foreach (var user in response.Users)
                {

                    var result = new SearchResult(user);
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        SearchResults.Add(result);
                        OnPropertyChanged(nameof(SearchResults));
                    });
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err);
            }
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
                            int selectedId = -1;
                            if (SelectedConversation is Conversation conversation)
                            {
                                selectedId = conversation.ConversationID;
                            }

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
                catch (Exception err)
                {
                    Debug.WriteLine(err);
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
            catch (Exception err)
            {
                Debug.WriteLine(err);
            }
        }

        private void SearchSelectionChanged(Conversation conversation)
        {
            Debug.WriteLine(conversation?.ConversationName);
        }

        private void UpdateCurrentChat()
        {
            if (SelectedConversation is SearchResult sr)
            {
                CurrentChat = sr.ChatView;
            }
            if (SelectedConversation is Conversation c)
            {
                CurrentChat = c.ChatView;
            }
        }
    }

    public class SearchResult : BaseViewModel
    {
        public SearchResult(DTOs.UserInfo userInfo)
        {
            this.FullName = userInfo.LastName + " " + userInfo.FirstName;

            var avatarUrl = ConfigurationManager.AppSettings["APIUrl"] + userInfo.Avatar;
            UserAvatar = LoadImage.LoadImageFromUrl(avatarUrl);

            ChatView = new ChatUserControl(new ChatViewModel(userID: userInfo.UserID));
        }

        private ImageSource _UserAvatar;
        public ImageSource UserAvatar
        {
            get => _UserAvatar;
            set
            {
                _UserAvatar = value;
                OnPropertyChanged(nameof(UserAvatar));
            }
        }

        private string _FullName;
        public string FullName
        {
            get => _FullName;
            set
            {
                _FullName = value;
                OnPropertyChanged(nameof(_FullName));
            }
        }
        public ChatUserControl ChatView { get; set; }
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
