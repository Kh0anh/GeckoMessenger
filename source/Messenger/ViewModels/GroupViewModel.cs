using HandyControl.Tools.Command;
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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

namespace Messenger.ViewModels
{
    public class GroupViewModel : BaseViewModel
    {
        public ImageSource Avatar { get; set; }
        public string GroupTitle { get; set; }
        public int PhotoCount { get; set; }
        public int FileCount { get; set; }
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

        private ObservableCollection<Participant> _Participants;
        public ObservableCollection<Participant> Participants
        {
            get => _Participants;
            set
            {
                _Participants = value;
                OnPropertyChanged(nameof(Participants));
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

        public ICommand SendMessageCommand { get; }
        public ICommand GiveFileCommand { get; }
        public ICommand GivePhotoCommand { get; }
        public ICommand RemoveAttachmentCommand { get; }
        public GroupViewModel(int conversationID)
        {
            Messages = new ObservableCollection<Message>();
            Attachments = new ObservableCollection<Attachment>();
            Participants = new ObservableCollection<Participant>();

            SendMessageCommand = new RelayCommand(_ => SendMessage());
            GiveFileCommand = new RelayCommand(_ => GiveFile());
            GivePhotoCommand = new RelayCommand(_ => GivePhoto());
            RemoveAttachmentCommand = new RelayCommand<Object>(a => RemoveAttachment(a));

            LoadGroupInfo(conversationID);

            Task.Run(TaskLoadMessage);
        }

        private void LoadGroupInfo(int conversationID)
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

                ConversationInfo = response.Conversation;

                var avatarUrl = ConfigurationManager.AppSettings["APIUrl"] + "/storages/" + response.Conversation.ConversationName + ".png";
                Avatar = LoadImage.LoadImageFromUrl(avatarUrl);
                GroupTitle = response.Conversation.ConversationTitle;

                foreach (var participant in response.Conversation.Participants)
                {
                    Participant p = new Participant
                    {
                        UserID = participant.UserID,
                        UserFullName = participant.NickName,
                    };
                    if (participant.ConversationRole == "OWNER")
                    {
                        p.UserRole = "Trưởng nhóm";
                    }
                    else if (participant.ConversationRole == "STAFF")
                    {
                        p.UserRole = "Phó nhóm";
                    }
                    else if (participant.ConversationRole == "USER")
                    {
                        p.UserRole = "Thành viên";
                    }
                    Participants.Add(p);
                    Task.Run(() =>
                    {

                        var getInfo = new DTOs.GetInfo
                        {
                            UserID = participant.UserID
                        };
                        var getInfoResponse = client.Get(getInfo);

                        if (string.IsNullOrEmpty(participant.NickName))
                        {
                            p.UserFullName = getInfoResponse.Data.LastName + " " + getInfoResponse.Data.FirstName;
                        }
                        p.UserAvatar = LoadImage.LoadImageFromUrl(ConfigurationManager.AppSettings["APIUrl"] + getInfoResponse.Data.Avatar);

                        App.Current.Dispatcher.Invoke(() =>
                        {
                            OnPropertyChanged(nameof(p.UserFullName));
                            OnPropertyChanged(nameof(p.UserAvatar));
                        });
                    });
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err);
            }
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

        public class Participant
        {
            public int UserID { get; set; }
            public ImageSource UserAvatar { get; set; }
            public string UserFullName { get; set; }
            public string UserRole { get; set; }
        }
    }
}
