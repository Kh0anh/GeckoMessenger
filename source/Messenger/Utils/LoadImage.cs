using System;
using System.IO;
using System.Net;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Messenger.Utils
{
    public static class LoadImage
    {
        public static ImageSource LoadImageFromUrl(string url)
        {
            using (WebClient webClient = new WebClient())
            {
                byte[] imageBytes = webClient.DownloadData(url);
                return LoadImageFromBytes(imageBytes);
            }
        }
        public static string LoadImageFromUrlAsBase64(string url)
        {
            using (WebClient webClient = new WebClient())
            {
                byte[] imageBytes = webClient.DownloadData(url);
                return Convert.ToBase64String(imageBytes);
            }
        }
        public static ImageSource LoadImageFromBytes(byte[] imageBytes)
        {
            BitmapImage bitmap = new BitmapImage();
            using (MemoryStream stream = new MemoryStream(imageBytes))
            {
                stream.Position = 0;
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                bitmap.Freeze(); // Để tránh lỗi trong luồng UI
            }
            return bitmap;
        }
    }
}
