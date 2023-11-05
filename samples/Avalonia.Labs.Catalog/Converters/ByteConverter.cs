using System;
using System.Globalization;

using Avalonia.Data.Converters;

namespace Avalonia.Labs.Catalog.Converters;

internal class ByteConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not null)
        {
            return System.Convert.ToByte(value);
        }
        return 128;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        Data.BindingOperations.DoNothing;
}
