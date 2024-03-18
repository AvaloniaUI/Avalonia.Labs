using System;
using Avalonia.Interactivity;
using System.Windows.Input;

namespace Avalonia.Labs.Input;

/// <summary>
/// Provides data for the <see cref="RoutedCommandManager.ExecutedEvent"/> routed event.
/// </summary>
public sealed class ExecutedRoutedEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Gets the routed command that was invoked.
    /// </summary>
    public ICommand Command { get; }

    /// <summary>
    /// Gets data parameter of the command.
    /// </summary>
    public object? Parameter { get; }

    internal ExecutedRoutedEventArgs(ICommand command, object? parameter)
    {
        Command = command ?? throw new ArgumentNullException(nameof(command));
        Parameter = parameter;
        RoutedEvent = RoutedCommandManager.ExecutedEvent;
    }
}
