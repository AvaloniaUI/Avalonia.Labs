using System.ComponentModel;
using Avalonia.Media.Imaging;

namespace Avalonia.Labs.Controls.Resource
{
    [TypeConverter(typeof(ResourceBitmapTypeConverter))]
    public class ResourceBitmap : Bitmap
    {
        private readonly ResourceItem _item;

        public ResourceBitmap(ResourceItem item) : base(item.Open())
        {
            _item = item;
        }
    }
}
