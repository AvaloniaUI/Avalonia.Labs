using System;
using System.Windows.Input;
using Avalonia.Input;
using Avalonia.Utilities;

namespace Avalonia.Labs.Input;

/// <summary>
/// Defines a command that implements <see cref="ICommand"/> and is routed through the element tree.
/// </summary>
public class RoutedCommand : ICommand
{
    private EventHandler? _canExecuteChanged;
    private RoutedCommandRequeryHandler? _handler;

    /// <summary>
    /// Gets the name of the command.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RoutedCommand"/> class with the specified name.
    /// </summary>
    /// <param name="name"></param>
    public RoutedCommand(string name)
    {
        Name = name;
    }

    event EventHandler ICommand.CanExecuteChanged
    {
        add
        {
            _canExecuteChanged += value;

            if (_handler is null)
            {
                _handler ??= new RoutedCommandRequeryHandler(this);
                RoutedCommandManager.PrivateRequerySuggestedEvent.Subscribe(RoutedCommandManager.Current, _handler);
            }
        }
        remove
        {
            _canExecuteChanged -= value;
            
            if (_handler is not null && _canExecuteChanged is null)
            {
                RoutedCommandManager.PrivateRequerySuggestedEvent.Unsubscribe(RoutedCommandManager.Current, _handler);
                _handler = null;
            }
        }
    }

    /// <summary>
    /// Determines whether this <see cref="RoutedCommand"/> can execute in its current state.
    /// </summary>
    /// <param name="parameter">A user defined data type.</param>
    /// <param name="target">The command target.</param>
    /// <returns>true if the command can execute on the current command target; otherwise, false.</returns>
    public bool CanExecute(object? parameter, IInputElement target)
    {
        if (target == null)
            throw new ArgumentNullException(nameof(target));

        return CanExecuteImpl(parameter, target, out _);
    }

    /// <summary>
    /// Executes the RoutedCommand on the current command target.
    /// </summary>
    /// <param name="parameter">User defined parameter to be passed to the handler.</param>
    /// <param name="target">Element at which to begin looking for command handlers.</param>
    public void Execute(object? parameter, IInputElement target)
    {
        if (target == null)
            throw new ArgumentNullException(nameof(target));

        ExecuteImpl(parameter, target);
    }

    bool ICommand.CanExecute(object parameter) =>
        CanExecuteImpl(parameter, RoutedCommandManager.FocusedElement, out _);

    void ICommand.Execute(object parameter) =>
        ExecuteImpl(parameter, RoutedCommandManager.FocusedElement);

    private bool CanExecuteImpl(object? parameter, IInputElement? target, out bool continueRouting)
    {
        if (target != null)
        {
            var args = new CanExecuteRoutedEventArgs(this, parameter)
            {
                RoutedEvent = RoutedCommandManager.CanExecuteEvent
            };
            target.RaiseEvent(args);

            continueRouting = args.Handled;
            return args.CanExecute;
        }
        else
        {
            continueRouting = false;
            return false;
        }
    }

    private bool ExecuteImpl(object? parameter, IInputElement? target)
    {
        if (target is not null)
        {
            var args = new ExecutedRoutedEventArgs(this, parameter)
            {
                RoutedEvent = RoutedCommandManager.ExecutedEvent
            };
            target.RaiseEvent(args);

            return args.Handled;
        }

        return false;
    }

    private class RoutedCommandRequeryHandler(RoutedCommand command) : IWeakEventSubscriber<EventArgs>
    {
        public void OnEvent(object? sender, WeakEvent ev, EventArgs e) => command._canExecuteChanged?.Invoke(sender, e);
    }
}
