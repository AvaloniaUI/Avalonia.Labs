using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Labs.Catalog.ViewModels;
using Avalonia.Labs.Catalog.Views;
using Avalonia.Labs.Controls;

namespace Avalonia.Labs.Catalog;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                Content = new PageNavigationHost()
                {
                    Page = new MainView()
                    {
                        DataContext = new MainViewModel()
                    }
                }
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new PageNavigationHost()
            {
                Page = new MainView()
                {
                    DataContext = new MainViewModel()
                }
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
