using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace Avalonia.Labs.Controls;

internal static class Utilities
{
    // copied from https://github.com/AvaloniaUI/Avalonia.Labs/blob/9de6a830045c9783c0f6d69e9396c4972d43a213/src/Avalonia.Labs.Controls/ContentDialog/ContentDialog.cs#L190
    internal static TopLevel GetTopLevel(Window? window)
    {
        return window ??
               Application.Current!.ApplicationLifetime switch
               {
                   IClassicDesktopStyleApplicationLifetime cls when cls.MainWindow is not null =>
                       cls.Windows.FirstOrDefault(x => x.IsActive)
                       ?? cls.MainWindow,
                   ISingleViewApplicationLifetime sl when sl.MainView is not null =>
                       TopLevel.GetTopLevel(sl.MainView)
                       ?? throw new InvalidOperationException(),
                   _ => throw new InvalidOperationException()
               };
    }

}
