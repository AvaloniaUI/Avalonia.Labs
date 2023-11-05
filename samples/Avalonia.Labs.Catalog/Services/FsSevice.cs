using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace Avalonia.Labs.Catalog.Services;

internal static class FsSevice
{
    public static IStorageProvider? GetStorageProvider()
    {
        if (App.Current is App app)
        {
            TopLevel? root = app.ApplicationLifetime switch
            {
                IClassicDesktopStyleApplicationLifetime cdl => cdl.MainWindow,
                ISingleViewApplicationLifetime sv => TopLevel.GetTopLevel(sv.MainView),
                _ => default
            };

            return root?.StorageProvider;
        }
        return default;
    }
}
