using HandyControl.Tools.Command;
using Messenger.DTOs;
using Messenger.Services;
using Messenger.Utils;
using Messenger.Views;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        public int PhotoCount { get; set; }
        public int FileCount { get; set; }

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

        private ObservableCollection<Message> _Messages;
        public ObservableCollection<Message> Messages
        {
            get => _Messages;
            set
            {
                _Messages = value;
                OnPropertyChanged(nameof(Messages));
            }
        }


        private ObservableCollection<Attachment> _Attachments;
        public ObservableCollection<Attachment> Attachments
        {
            get => _Attachments;
            set
            {
                _Attachments = value;
                OnPropertyChanged(nameof(Attachments));
            }
        }

        private string _newMessageText = string.Empty;
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
        public ICommand GiveFileCommand { get; }
        public ICommand GivePhotoCommand { get; }
        public ICommand RemoveAttachmentCommand { get; }
        public ICommand OpenProfileCommand { get; }
        public ChatViewModel(int? userID = null, int? conversationID = null)
        {
            Messages = new ObservableCollection<Message>();
            Attachments = new ObservableCollection<Attachment>();

            SendMessageCommand = new RelayCommand(_ => SendMessage());
            StartNewChatCommand = new RelayCommand(_ => StartNewChat());
            GiveFileCommand = new RelayCommand(_ => GiveFile());
            OpenProfileCommand = new RelayCommand(_ => OpenProfile());
            GivePhotoCommand = new RelayCommand(_ => GivePhoto());
            RemoveAttachmentCommand = new RelayCommand<Object>(a => RemoveAttachment(a));

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

        private void GiveFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Chọn một tệp",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Task.Run(() =>
                {
                    var attachment = new Attachment
                    {
                        FileName = Path.GetFileName(openFileDialog.FileName),
                        FileType = "FILE",
                        Data = System.IO.File.ReadAllBytes(openFileDialog.FileName),
                    };
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        Attachments.Add(attachment);
                    });
                });
            }
        }

        private void OpenProfile()
        {
            ProfileView settingsView = new ProfileView(new ProfileViewModel(UserID));
            settingsView.ShowDialog();
        }

        private void GivePhoto()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Chọn một hình ảnh",
                Filter = "Hình ảnh|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Task.Run(() =>
                {
                    var attachment = new Attachment
                    {
                        FileName = Path.GetFileName(openFileDialog.FileName),
                        FileType = "PHOTO",
                        Data = System.IO.File.ReadAllBytes(openFileDialog.FileName),
                    };
                    attachment.Thumnail = Utils.LoadImage.LoadImageFromBytes(attachment.Data);
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        Attachments.Add(attachment);
                    });
                });
            }
        }
        private void RemoveAttachment(Object attachment)
        {
            Debug.WriteLine("work");
            if (attachment is Attachment a && Attachments.Contains(a))
            {
                Attachments.Remove(a);
            }
        }
        private async void TaskLoadMessage()
        {
            while (true)
            {
                await Task.Delay(300);

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

                                var photoList = new List<Photo>();
                                var fileList = new List<File>();
                                if (message.Attachments.Length > 0)
                                {
                                    foreach (var attachment in message.Attachments)
                                    {
                                        if (attachment.AttachmentType == "PHOTO")
                                        {
                                            try
                                            {
                                                photoList.Add(new Photo
                                                {
                                                    Image = LoadImage.LoadImageFromUrl(ConfigurationManager.AppSettings["APIUrl"] + "/storages/" + attachment.FileURL),
                                                });
                                                PhotoCount++;
                                                OnPropertyChanged(nameof(PhotoCount));
                                            }
                                            catch (Exception err)
                                            {
                                                Debug.WriteLine(err);
                                            }
                                        }
                                        if (attachment.AttachmentType == "FILE")
                                        {
                                            try
                                            {
                                                fileList.Add(new File
                                                {
                                                    FileName = attachment.FileURL,
                                                    Url = ConfigurationManager.AppSettings["APIUrl"] + "/storages/" + attachment.FileURL,
                                                });
                                                FileCount++;
                                                OnPropertyChanged(nameof(FileCount));
                                            }
                                            catch (Exception err)
                                            {
                                                Debug.WriteLine(err);
                                            }
                                        }
                                    }
                                }

                                var newMessage = new Message
                                {
                                    MessageID = message.MessageID,
                                    UserID = message.Sender.UserID,
                                    UserFullName = message.Sender.FirstName + " " + message.Sender.LastName,
                                    UserAvatar = AvatarData[avatarUrl],
                                    Content = message.Content,
                                    Photos = photoList.ToArray(),
                                    Files = fileList.ToArray(),
                                    IsSentByMe = message.Sender.UserID == userService.User.UserID,
                                    Timestamp = message.CreatedAt
                                };

                                App.Current.Dispatcher.Invoke(() =>
                                {
                                    Messages.Add(newMessage);
                                });

                                Messages.CollectionChanged += Messages_CollectionChanged;
                            }
                        }

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


                var newContact = new DTOs.AddContact
                {
                    UserID = UserID,
                };

                var newChat = new DTOs.NewChat
                {
                    ParticipantID = UserID
                };

                try
                {
                    var newContactResponse = client.Post(newContact);

                    var newChatResponse = client.Post(newChat);

                    IsStarted = true;
                    LoadInfoConversation(newChatResponse.Conversation);
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

            UserID = userID;

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
            if (!string.IsNullOrWhiteSpace(NewMessageText) || Attachments.Count > 0)
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

                    if (Attachments.Count > 0)
                    {
                        var attachmentList = new List<DTOs.InAttachment>();
                        foreach (var attachment in Attachments)
                        {
                            attachmentList.Add(new DTOs.InAttachment() { AttachmentType = attachment.FileType, FileData = attachment.Data, FileName = attachment.FileName });
                        }

                        sendMessage.Attachments = attachmentList.ToArray();
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            Attachments.Clear();
                        });
                    }

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
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    ScrollToEnd?.Invoke();
                });
            }
        }

        public event Action ScrollToEnd;
    }

    public class Attachment
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public ImageSource Thumnail { get; set; }
        public byte[] Data { get; set; }
    }

    public class Message
    {
        public int MessageID { get; set; }
        public int UserID { get; set; }
        public string UserFullName { get; set; }
        public ImageSource UserAvatar { get; set; }
        public Photo[] Photos { get; set; }
        public File[] Files { get; set; }
        public bool IsSentByMe { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { set; get; }
    }

    public class Photo
    {
        public string FileName { get; set; }
        public ImageSource Image { get; set; }
    }
    public class File
    {
        public string FileName { get; set; }
        public string Url { get; set; }
    }
}