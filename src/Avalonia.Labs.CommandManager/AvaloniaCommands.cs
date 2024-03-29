namespace Avalonia.Labs.Input;

/// <summary>
/// Provides a standard set of application related commands.
/// </summary>
public static class AvaloniaCommands
{
    /// <summary>
    /// Represents a command which is always ignored.
    /// </summary>
    public static RoutedCommand NotACommand { get; } = new(nameof(NotACommand));
}
