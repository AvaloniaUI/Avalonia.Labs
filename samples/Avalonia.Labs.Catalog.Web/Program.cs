using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Avalonia.ReactiveUI;
using Avalonia.Labs.Catalog;

[assembly: SupportedOSPlatform("browser")]

internal partial class Program
{
    private static Task Main(string[] args) => BuildAvaloniaApp()
        .UseReactiveUI()
        .StartBrowserAppAsync("out");

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}
