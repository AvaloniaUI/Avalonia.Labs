using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Avalonia.Data.Converters;

namespace Avalonia.Labs.Catalog.Converters;

internal class CompositeCommandConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        var commands = values.OfType<ICommand>();
        return new CompositeCommand(commands);
    }
}
