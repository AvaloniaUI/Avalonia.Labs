using System;
using System.ComponentModel;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Labs.Controls.Resource;

namespace Avalonia.Labs.Controls
{
    public class ResourceWindowIconTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(ResourceItem);
        }

        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
        {
            return destinationType == typeof(WindowIcon) || destinationType == typeof(ResourceWindowIcon);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            var item = value as ResourceItem;
            if (item != null)
            {
                return new WindowIcon(item.Open());
            }

            return null;
        }
    }
}
