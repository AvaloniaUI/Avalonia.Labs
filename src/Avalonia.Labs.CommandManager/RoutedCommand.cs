using System;
using System.Collections.Generic;
using System.Windows.Input;
using Avalonia.Input;
using Avalonia.Utilities;

namespace Avalonia.Labs.Input;

/// <summary>
/// Defines a command that implements <see cref="ICommand"/> and is routed through the element tree.
/// </summary>
public class RoutedCommand : ICommand
{
    /// <summary>
    /// The weak collection of delegates for <see cref="ICommand.CanExecuteChanged"/>.
    /// </summary>
    private readonly WeakCollection<EventHandler> _canExecuteChanged = new();

    private RoutedCommandRequeryHandler? _handler;
    private IList<KeyGesture>? _gestures;

    /// <summary>
    /// Gets the name of the command.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the list of <see cref="KeyGesture"/> object that are associated with this command.
    /// </summary>
    public IList<KeyGesture> Gestures
    {
        get => _gestures ??= new List<KeyGesture>();
        set => _gestures = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RoutedCommand"/> class with the specified name.
    /// </summary>
    /// <param name="name">Declared name.</param>
    public RoutedCommand(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RoutedCommand"/> class with the specified name.
    /// </summary>
    /// <param name="name">Declared name.</param>
    /// <param name="gestures">Default input gestures associated with this command.</param>
    public RoutedCommand(string name, IList<KeyGesture> gestures)
        : this(name)
    {
        Gestures = gestures;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RoutedCommand"/> class with the specified name.
    /// </summary>
    /// <param name="name">Declared name.</param>
    /// <param name="gesture">Default input gestures associated with this command.</param>
    public RoutedCommand(string name, KeyGesture gesture)
        : this(name, [gesture])
    {
    }

    event EventHandler? ICommand.CanExecuteChanged
    {
        add
        {
            _canExecuteChanged.Add(value!);
            if (_handler is null)
            {
                _handler ??= new RoutedCommandRequeryHandler(this);
                CommandManager.PrivateRequerySuggestedEvent.Subscribe(CommandManager.Current, _handler);
            }
        }
        remove
        {
            _canExecuteChanged.Remove(value!);
            if (_handler is not null && _canExecuteChanged is null)
            {
                CommandManager.PrivateRequerySuggestedEvent.Unsubscribe(CommandManager.Current, _handler);
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

        return CanExecuteCore(parameter, target, out _);
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

        ExecuteCore(parameter, target);
    }

    bool ICommand.CanExecute(object? parameter) =>
        CanExecuteCore(parameter, CommandManager.FocusedElement, out _);

    void ICommand.Execute(object? parameter) =>
        ExecuteCore(parameter, CommandManager.FocusedElement);

    internal bool CanExecuteCore(object? parameter, IInputElement? target, out bool continueRouting)
    {
        if (target != null)
        {
            var args = new CanExecuteRoutedEventArgs(this, parameter)
            {
                RoutedEvent = CommandManager.CanExecuteEvent,
                CanExecute = true // initial value
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

    internal bool ExecuteCore(object? parameter, IInputElement? target)
    {
        if (target is not null)
        {
            var args = new ExecutedRoutedEventArgs(this, parameter)
            {
                RoutedEvent = CommandManager.ExecutedEvent
            };
            target.RaiseEvent(args);

            return args.Handled;
        }

        return false;
    }

    private class RoutedCommandRequeryHandler(RoutedCommand command) : IWeakEventSubscriber<EventArgs>
    {
        public void OnEvent(object? sender, WeakEvent ev, EventArgs e)
        {
            var list = command._canExecuteChanged.GetLiveItems();
            foreach (var canExecuteChanged in list)
                canExecuteChanged(sender, EventArgs.Empty);
        }
    }
}
