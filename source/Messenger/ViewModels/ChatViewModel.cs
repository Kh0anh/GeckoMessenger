using Messenger.DTOs;
using Messenger.Services;
using Messenger.Utils;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Messenger.ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        public ImageSource Avatar { get; set; }
        public int UserID { get; set; }
        public string FullName { get; set; }
        public string Activity { get; set; }

        private bool _IsStarted;
        public bool IsStarted
        {
            get => _IsStarted; set
            {
                _IsStarted = value;
                OnPropertyChanged(nameof(IsStarted));
            }
        }
        private ConversationResponse ConversationInfo { get; set; }

        private ObservableCollection<Message> _messages;
        public ObservableCollection<Message> Messages
        {
            get => _messages;
            set
            {
                _messages = value;
                OnPropertyChanged(nameof(Messages));
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

        private Dictionary<string, ImageSource> AvatarData = new Dictionary<string, ImageSource>();

        public ICommand StartNewChatCommand { get; }
        public ICommand SendMessageCommand { get; }

        public ChatViewModel(int? userID = null, int? conversationID = null)
        {
            Messages = new ObservableCollection<Message>();
            SendMessageCommand = new RelayCommand(_ => SendMessage());
            StartNewChatCommand = new RelayCommand(_ => StartNewChat());

            if (userID != null)
            {
                LoadUserConversation(userID.Value);
            }
            else if (conversationID != null)
            {
                LoadConversation(conversationID.Value);
            }


            Task.Run(TaskLoadMessage);
        }

        private async void TaskLoadMessage()
        {
            while (true)
            {
                await Task.Delay(500);

                if (ConversationInfo == null)
                {
                    continue;
                }

                var userService = ServiceLocator.GetService<IUserService>();
                if (userService.User == null)
                {
                    break;
                }

                var client = new JsonServiceClient(ConfigurationManager.AppSettings["APIUrl"]);
                client.BearerToken = userService.User.AuthToken;

                var getMessages = new DTOs.GetMessages
                {
                    ConversationID = ConversationInfo.ConversationID,
                };
                try
                {
                    var response = await client.GetAsync(getMessages);

                    if (response?.Messages != null)
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            foreach (var message in response.Messages)
                            {
                                var existingMessage = Messages.FirstOrDefault(c => c.MessageID == message.MessageID);

                                if (existingMessage == null)
                                {
                                    var avatarUrl = ConfigurationManager.AppSettings["APIUrl"] + message.Sender.Avatar;

                                    if (!AvatarData.ContainsKey(avatarUrl))
                                    {
                                        AvatarData[avatarUrl] = LoadImage.LoadImageFromUrl(avatarUrl);
                                    }

                                    var newMessage = new Message
                                    {
                                        MessageID = message.MessageID,
                                        UserID = message.Sender.UserID,
                                        UserFullName = message.Sender.FirstName + " " + message.Sender.LastName,
                                        UserAvatar = AvatarData[avatarUrl],
                                        Content = message.Content,
                                        IsSentByMe = message.Sender.UserID == userService.User.UserID,
                                        Timestamp = message.CreatedAt
                                    };

                                    Messages.Add(newMessage);

                                    Messages.CollectionChanged += Messages_CollectionChanged;
                                }
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }

        private void StartNewChat()
        {
            Task.Run(() =>
            {
                var userService = ServiceLocator.GetService<IUserService>();

                var client = new JsonServiceClient(ConfigurationManager.AppSettings["APIUrl"]);
                client.BearerToken = userService.User.AuthToken;

                var chat = new DTOs.NewChat
                {
                    ParticipantID = UserID
                };

                try
                {
                    var response = client.Post(chat);

                    IsStarted = true;
                    LoadInfoConversation(response.Conversation);
                }
                catch (Exception err)
                {
                    Debug.WriteLine(err);
                }
            });
        }

        private void LoadUserConversation(int userID)
        {
            var userService = ServiceLocator.GetService<IUserService>();
            if (userService == null)
                return;

            var client = new JsonServiceClient(ConfigurationManager.AppSettings["APIUrl"]);
            client.BearerToken = userService.User.AuthToken;

            UserID = userID;

            var findChat = new DTOs.FindChat
            {
                UserID = userID
            };

            try
            {
                var response = client.Get(findChat);

                if (response.Conversation == null)
                {
                    LoadUserInfo(userID);

                    IsStarted = false;
                    return;
                }

                IsStarted = true;
                LoadInfoConversation(response.Conversation);
            }
            catch (Exception err)
            {
                Debug.WriteLine(err);
            }
        }

        private void LoadConversation(int conversationID)
        {
            var userService = ServiceLocator.GetService<IUserService>();

            if (userService == null)
                return;

            var client = new JsonServiceClient(ConfigurationManager.AppSettings["APIUrl"]);
            client.BearerToken = userService.User.AuthToken;

            var findConversation = new DTOs.FindConversation
            {
                ConversationID = conversationID
            };

            try
            {
                var response = client.Get(findConversation);

                if (response.Conversation == null)
                {
                    IsStarted = false;
                    return;
                }

                IsStarted = true;
                LoadInfoConversation(response.Conversation);
            }
            catch (Exception err)
            {
                Debug.WriteLine(err);
            }
        }

        private void LoadUserInfo(int userID)
        {
            var userService = ServiceLocator.GetService<IUserService>();

            var client = new JsonServiceClient(ConfigurationManager.AppSettings["APIUrl"]);
            client.BearerToken = userService.User.AuthToken;

            var getInfo = new DTOs.GetInfo
            {
                UserID = userID,
            };

            var response = client.Get(getInfo);

            if (!string.IsNullOrEmpty(response.Error))
            {
                throw new Exception(response.Message);
            }

            Task.Run(() =>
            {
                Avatar = LoadImage.LoadImageFromUrl(ConfigurationManager.AppSettings["APIUrl"] + response.Data.Avatar);
            });

            FullName = response.Data.FirstName + " " + response.Data.LastName;
        }

        private void LoadInfoConversation(DTOs.ConversationResponse conversation)
        {
            ConversationInfo = conversation;

            var userService = ServiceLocator.GetService<IUserService>();
            var client = new JsonServiceClient(ConfigurationManager.AppSettings["APIUrl"]);
            client.BearerToken = userService.User.AuthToken;

            if (conversation.ConversationType == "GROUP")
            {

            }
            else
            {
                DTOs.ParticipantResponse participant;

                if (conversation.Participants[0].UserID != userService.User.UserID)
                {
                    participant = conversation.Participants[0];
                }
                else
                {
                    participant = conversation.Participants[1];
                }

                LoadUserInfo(participant.UserID);

                if (!string.IsNullOrEmpty(participant.NickName))
                {
                    FullName = participant.NickName;
                }
            }
        }

        private void SendMessage()
        {
            if (!string.IsNullOrWhiteSpace(NewMessageText))
            {
                var MessageContent = NewMessageText;
                NewMessageText = string.Empty;

                Task.Run(() =>
                {
                    var userService = ServiceLocator.GetService<IUserService>();

                    var client = new JsonServiceClient(ConfigurationManager.AppSettings["APIUrl"]);
                    client.BearerToken = userService.User.AuthToken;

                    var sendMessage = new DTOs.SendMessage
                    {
                        ConversationID = ConversationInfo.ConversationID,
                        Content = MessageContent,
                        MessageType = "TEXT",
                    };

                    try
                    {
                        var response = client.Post(sendMessage);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                });
            }
        }

        private void Messages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ScrollToEnd?.Invoke();
                });
            }
        }

        public event Action ScrollToEnd;
    }
    public class Message
    {
        public int MessageID { get; set; }
        public int UserID { get; set; }
        public string UserFullName { get; set; }
        public ImageSource UserAvatar { get; set; }
        public bool IsSentByMe { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { set; get; }
    }
}
