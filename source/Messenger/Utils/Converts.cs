using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Messenger.Utils
{
    public static class Converts
    {
        public static string ConvertImageToBase64(ImageSource imageSource)
        {
            if (imageSource is BitmapSource bitmapSource)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                    encoder.Save(memoryStream);
                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
            return null;
        }
        public static ImageSource ConvertBase64ToImage(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            using (MemoryStream memoryStream = new MemoryStream(imageBytes))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = memoryStream;
                bitmap.EndInit();
                bitmap.Freeze(); // Tránh lỗi multi-threading
                return bitmap;
            }
        }
    }
}
