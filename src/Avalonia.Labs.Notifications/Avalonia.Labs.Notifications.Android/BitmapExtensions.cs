using Android.Graphics;
using AndroidBitmap = Android.Graphics.Bitmap;
using AvaloniaBitmap = Avalonia.Media.Imaging.Bitmap;

namespace Avalonia.Labs.Notifications.Android
{
    internal static class BitmapExtensions
    {
        public static AndroidBitmap? ToAndroid(this AvaloniaBitmap avaloniaBitmap)
        {
            using var stream = new MemoryStream();
            avaloniaBitmap.Save(stream);

            return BitmapFactory.DecodeStream(stream);
        }
    }
}
