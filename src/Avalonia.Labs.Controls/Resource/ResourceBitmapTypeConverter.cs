using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using Avalonia.Controls;
using Avalonia.Labs.Controls.Resource;
using Avalonia.Media.Imaging;

namespace Avalonia.Labs.Controls
{
    public class ResourceBitmapTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(ResourceItem);
        }

        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
        {
            return destinationType == typeof(Bitmap) || destinationType == typeof(ResourceBitmap);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            var item = value as ResourceItem;
            if (item != null)
            {
                return new ResourceBitmap(item);
            }

            return null;
        }
    }
}
