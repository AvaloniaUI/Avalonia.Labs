using System;
using System.Windows.Input;
using Avalonia.Input;

namespace Avalonia.Labs.Input;

public partial class RoutedCommand : ICommand
{
    public string Name { get; }
    public KeyGesture? Gesture { get; }

    public RoutedCommand(string name, KeyGesture? keyGesture = default)
    {
        Name = name;
        Gesture = keyGesture;
    }

    event EventHandler ICommand.CanExecuteChanged
    {
        add
        {
            RoutedCommandsManager.Invalidate += value;
        }
        remove
        {
            RoutedCommandsManager.Invalidate -= value;
        }
    }

    public bool CanExecute(object? parameter, IInputElement? target)
    {
        if (target == null)
            return false;

        var args = new CanExecuteRoutedEventArgs(this, parameter);
        target.RaiseEvent(args);

        return args.CanExecute;
    }

    public void Execute(object? parameter, IInputElement? target)
    {
        if (target == null)
            return;

        var args = new ExecutedRoutedEventArgs(this, parameter);
        target.RaiseEvent(args);
    }

    bool ICommand.CanExecute(object parameter) =>
        CanExecute(parameter, RoutedCommandsManager.CurrentElement);

    void ICommand.Execute(object parameter) =>
        Execute(parameter, RoutedCommandsManager.CurrentElement);
}
