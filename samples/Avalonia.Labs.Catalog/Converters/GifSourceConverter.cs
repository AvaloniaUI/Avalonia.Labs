using System;
using System.Globalization;
using System.IO;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Labs.Gif;

namespace Avalonia.Labs.Catalog.Converters
{
    public class GifSourceConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            switch (value)
            {
                case Stream gifSourceStream:
                    return GifStreamSource.FromStream(gifSourceStream);
                case string gifSourceUriString:
                    return GifStreamSource.FromUriString(gifSourceUriString);
                case Uri gifSourceUri:
                    return GifStreamSource.FromUri(gifSourceUri);
                default:
                    return null;
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return BindingOperations.DoNothing;
        }
    }
}
