using Messenger.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Messenger.ViewModels
{
    public class GroupViewModel : BaseViewModel
    {
        public ImageSource Avatar { get; set; }
        public GroupViewModel() {
            Avatar = LoadImage.LoadImageFromUrl(ConfigurationManager.AppSettings["APIUrl"] + "storages/DefaultAvatar.png");
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
}
