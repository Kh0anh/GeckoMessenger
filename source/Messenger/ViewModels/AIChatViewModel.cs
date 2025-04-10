using Messenger.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Messenger.ViewModels
{
    public class AIChatViewModel : BaseViewModel
    {
        private ObservableCollection<AIMessage> _Messages;
        public ObservableCollection<AIMessage> Messages
        {
            get => _Messages;
            set
            {
                _Messages = value;
                OnPropertyChanged(nameof(Messages));
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

        private string _LatestMessage;
        public string LatestMessage
        {
            get => _LatestMessage; set
            {
                _LatestMessage = value;
                OnPropertyChanged(nameof(LatestMessage));
            }
        }

        public ImageSource Avatar { get; set; } = LoadImage.LoadImageFromBytes(ResourceHelper.GetEmbeddedResource("Messenger.Resources.gecko.png"));
        public ICommand SendMessageCommand { get; }
        public AIChatViewModel()
        {
            Messages = new ObservableCollection<AIMessage> { };

            SendMessageCommand = new RelayCommand(async _ => await SendMessage());
        }

        private async Task SendMessage()
        {
            if (!string.IsNullOrWhiteSpace(NewMessageText))
            {
                var apiKey = "API";
                var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";

                var userMsg = new AIMessage { Role = "user", Content = NewMessageText };
                Messages.Add(userMsg);
                NewMessageText = "";

                var contents = new List<object>();

                contents.Add(new
                {
                    role = "user",
                    parts = new[] {
                        new { text = "Bạn là trợ lý ảo Gecko có chức năng trả lời và thực hiện các yêu cầu, câu trả lời ngắn gọn và không thêm các icon." }
                    }
                });

                contents.AddRange(Messages.Select(m => new
                {
                    role = m.Role,
                    parts = new[] { new { text = m.Content } }
                }));

                var modelMsg = new AIMessage { Role = "model", Content = "" };
                Messages.Add(modelMsg);

                var requestBody = new { contents };

                using (var client = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(requestBody);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(url, content);
                    var result = await response.Content.ReadAsStringAsync();


                    var root = JObject.Parse(result);
                    var reply = (string)root["candidates"]?[0]?["content"]?["parts"]?[0]?["text"];

                    if (!string.IsNullOrEmpty(reply))
                    {
                        LatestMessage = reply.Trim();
                        modelMsg.Content = reply.Trim();
                    }
                    else
                    {
                        LatestMessage = "Không có phản hồi từ Gecko";
                        modelMsg.Content = "[Không có phản hồi từ Gecko]";
                    }

                    Messages.CollectionChanged += Messages_CollectionChanged;
                }
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
        public class AIMessage : BaseViewModel
        {
            public string Role { get; set; }

            private string _Content;
            public string Content { get => _Content; set
                {
                    _Content = value;
                    OnPropertyChanged(nameof(Content));
                }
            }
        }
    }
}
