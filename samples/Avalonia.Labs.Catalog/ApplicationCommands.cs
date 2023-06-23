using Avalonia.Labs.Input;

namespace Avalonia.Labs.Catalog;

public static class ApplicationCommands
{
    public readonly static RoutedCommand Open = new RoutedCommand(nameof(Open));
    public readonly static RoutedCommand Save = new RoutedCommand(nameof(Save));
    public readonly static RoutedCommand Delete = new RoutedCommand(nameof(Delete));
}
