using System;
using Avalonia.Interactivity;
using System.Windows.Input;

namespace Avalonia.Labs.Input;

public sealed class ExecutedRoutedEventArgs : RoutedEventArgs
{
    public ICommand Command { get; }

    public object? Parameter { get; }

    internal ExecutedRoutedEventArgs(ICommand command, object? parameter)
    {
        Command = command ?? throw new ArgumentNullException(nameof(command));
        Parameter = parameter;
        RoutedEvent = RoutedCommandsManager.ExecutedEvent;
    }
}
