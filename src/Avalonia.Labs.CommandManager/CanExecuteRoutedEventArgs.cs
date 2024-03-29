using System;
using Avalonia.Interactivity;
using System.Windows.Input;

namespace Avalonia.Labs.Input;

/// <summary>
/// Provides data for the <see cref="CommandManager.CanExecuteEvent"/> routed events.
/// </summary>
public sealed class CanExecuteRoutedEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Gets the routed command that was invoked.
    /// </summary>
    public ICommand Command { get; }

    /// <summary>
    /// Gets data parameter of the command.
    /// </summary>
    public object? Parameter { get; }

    /// <summary>
    /// Gets or sets a value that indicates whether the <see cref="RoutedCommand"/> associated with this event can be executed on the command target.
    /// </summary>
    public bool CanExecute { get; set; }

    internal CanExecuteRoutedEventArgs(ICommand command, object? parameter)
    {
        Command = command ?? throw new ArgumentNullException(nameof(command));
        Parameter = parameter;
        RoutedEvent = CommandManager.CanExecuteEvent;
    }
}
