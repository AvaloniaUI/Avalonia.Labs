using System;
using System.Globalization;
using Avalonia.Controls.Primitives;
using Avalonia.Data.Converters;

namespace Avalonia.Labs.Catalog.Converters;

public class IsEqualToScrollbarVisibilityConverter : IValueConverter
{
    public static IsEqualToScrollbarVisibilityConverter Instance { get; } = new();
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.Equals(parameter) ?? false ? ScrollBarVisibility.Visible : ScrollBarVisibility.Disabled;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
