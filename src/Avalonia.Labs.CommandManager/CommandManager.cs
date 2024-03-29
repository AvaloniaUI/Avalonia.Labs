using System;
using System.Collections.Generic;
using System.Windows.Input;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.Utilities;

namespace Avalonia.Labs.Input;

/// <summary>
/// Provides command related utility methods that register <see cref="CommandBinding"/> objects for class owners and commands,
/// add and remove command event handlers, and provides services for querying the status of a command.
/// </summary>
public sealed class CommandManager : AvaloniaObject
{
    [ThreadStatic]
    private static CommandManager? s_commandManager;
    private static readonly WeakReference<IInputElement?> s_inputElement = new(default);

    private DispatcherOperation? _requerySuggestedOperation;
    private event EventHandler? PrivateRequerySuggested;

    /// <summary>
    /// Identifies the CanExecute attached event.
    /// </summary>
    public static RoutedEvent<CanExecuteRoutedEventArgs> CanExecuteEvent =
        RoutedEvent.Register<CanExecuteRoutedEventArgs>("CanExecute", RoutingStrategies.Bubble | RoutingStrategies.Tunnel, typeof(CommandManager));

    /// <summary>
    /// Identifies the Executed attached event.
    /// </summary>
    public static RoutedEvent<ExecutedRoutedEventArgs> ExecutedEvent =
        RoutedEvent.Register<ExecutedRoutedEventArgs>("Executed", RoutingStrategies.Bubble | RoutingStrategies.Tunnel, typeof(CommandManager));

    /// <summary>
    /// Defines the <see cref="CommandBindings"/> property.
    /// </summary>
    public static readonly AttachedProperty<IList<CommandBinding>?> CommandBindingsProperty =
        AvaloniaProperty.RegisterAttached<CommandManager, InputElement, IList<CommandBinding>?>("CommandBindings");

    /// <summary>
    /// Occurs when the <see cref="CommandManager"/> detects conditions that might change the ability of a command to execute.
    /// </summary>
    public static event EventHandler RequerySuggested
    {
        // WeakHandlerWrapper will ensure, that add/remove with the same handler will work.
        add => PrivateRequerySuggestedEvent.Subscribe(Current, new WeakHandlerWrapper(value));
        remove => PrivateRequerySuggestedEvent.Unsubscribe(Current, new WeakHandlerWrapper(value));
    }

    internal static readonly WeakEvent<CommandManager, EventArgs> PrivateRequerySuggestedEvent =
        WeakEvent.Register<CommandManager>(
            (m, v) => m.PrivateRequerySuggested += v,
            (m, v) => m.PrivateRequerySuggested -= v);

    /// <summary>
    /// Invokes RequerySuggested listeners registered on the current thread.
    /// </summary>
    public static void InvalidateRequerySuggested()
    {
        Current.RaiseRequerySuggested();
    }

    /// <summary>
    /// Gets a collection of CommandBinding objects associated with this element. A CommandBinding enables command handling for this element, and declares the linkage between a command, its events, and the handlers attached by this element.
    /// </summary>
    public static IList<CommandBinding> GetCommandBindings(InputElement element)
    {
        var commands = element.GetValue(CommandBindingsProperty);
        if (commands is null)
        {
            commands = new List<CommandBinding>();
            element.SetValue(CommandBindingsProperty, commands);
        }
        return commands;
    }

    /// <summary>
    /// Sets a collection of CommandBinding objects associated with this element. A CommandBinding enables command handling for this element, and declares the linkage between a command, its events, and the handlers attached by this element.
    /// </summary>
    public static void SetCommandBindings(InputElement element, IList<CommandBinding> commands) =>
        element.SetValue(CommandBindingsProperty, commands);

    static CommandManager()
    {
        CanExecuteEvent.AddClassHandler<InputElement>(CanExecuteEventHandler);
        ExecutedEvent.AddClassHandler<InputElement>(ExecutedEventHandler);
        InputElement.GotFocusEvent.AddClassHandler<InputElement>(GotFocusEventHandler);
        InputElement.KeyDownEvent.AddClassHandler<InputElement>(KeyDownEventHandler, RoutingStrategies.Tunnel);
    }

    private CommandManager()
    {
    }

    internal static CommandManager Current => s_commandManager ??= new CommandManager();

    internal static IInputElement? FocusedElement => s_inputElement.TryGetTarget(out var element) ? element : default;

    private static void GotFocusEventHandler(InputElement targetElement, GotFocusEventArgs args)
    {
        s_inputElement.SetTarget(args.Source as IInputElement);
        Current.RaiseRequerySuggested();
    }

    private static void KeyDownEventHandler(InputElement targetElement, KeyEventArgs inputEventArgs)
    {
        ICommand? command = null;
        IInputElement? target = null;
        object? parameter = null;

        // Step 1: Check local input bindings
        // TODO
        
        // Step 2: If no command, check class input bindings
        // TODO
        
        // Step 3: If no command, check local command bindings
        if (GetCommandBindings(targetElement) is { Count :> 0 } bindings)
        {
            command = FindMatch();

            ICommand? FindMatch()
            {
                foreach (var binding in bindings)
                {
                    if (binding.Command is RoutedCommand routedCommand)
                    {
                        foreach (var gesture in routedCommand.Gestures)
                        {
                            if (gesture.Matches(inputEventArgs))
                            {
                                // TODO: wpf doesn't set DataContext here as it seems, should we?
                                parameter = (inputEventArgs.Source as InputElement)?.DataContext;
                                return routedCommand;
                            }
                        }
                    }
                }
                return null;
            }
        }

        // Step 4: If no command, look at class command bindings
        // TODO
        
        // Step 5: If found a command, then execute it (unless it is
        // the special "NotACommand" command, which we simply ignore without
        // setting Handled=true, so that the input bubbles up to the parent)
        if (command != null && command != AvaloniaCommands.NotACommand)
        {
            // We currently do not support declaring the element with focus as the target
            // element by setting target == null.  Instead, we interpret a null target to indicate
            // the element that we are routing the event through, e.g. the targetElement parameter.
            if (target == null)
            {
                target = targetElement;
            }

            bool continueRouting = false;

            if (command is RoutedCommand routedCommand)
            {
                if (routedCommand.CanExecuteCore(parameter, target, out continueRouting))
                {
                    // If the command can be executed, we never continue to route the
                    // input event.
                    continueRouting = false;

                    routedCommand.ExecuteCore(parameter, target);
                }
            }
            else
            {
                if (command.CanExecute(parameter))
                {
                    command.Execute(parameter);
                }
            }

            // If we mapped an input event to a command, we should always
            // handle the input event - regardless of whether the command
            // was executed or not.  Unless the CanExecute handler told us
            // to continue the route.
            inputEventArgs.Handled = !continueRouting;
        }
    }

    private static void CanExecuteEventHandler(InputElement inputElement, CanExecuteRoutedEventArgs args)
    {
        if (GetCommandBindings(inputElement) is { Count :> 0 } commands)
        {
            foreach (var command in commands)
            {
                if (command.Command == args.Command)
                {
                    command.OnCanExecute(inputElement, args);
                    if (args.Handled)
                    {
                        break;
                    }
                }
            }
        }
    }

    private static void ExecutedEventHandler(InputElement inputElement, ExecutedRoutedEventArgs args)
    {
        if (GetCommandBindings(inputElement) is { Count :> 0 } commands)
        {
            foreach (var command in commands)
            {
                if (command.Command == args.Command)
                {
                    command.OnExecuted(inputElement, args);
                    if (args.Handled)
                    {
                        break;
                    }
                }
            }
        }
    }
    
    private void RaiseRequerySuggested()
    {
        if (_requerySuggestedOperation == null)
        {
            var dispatcher = Dispatcher.UIThread; // should be CurrentDispatcher
            _requerySuggestedOperation = dispatcher.InvokeAsync(RaiseRequerySuggestedImpl, DispatcherPriority.Background);
        }

        static void RaiseRequerySuggestedImpl()
        {
            var current = Current;

            // Call the RequerySuggested handlers
            current._requerySuggestedOperation = null;
            current.PrivateRequerySuggested?.Invoke(null, EventArgs.Empty);
        }
    }

    private sealed class WeakHandlerWrapper : IWeakEventSubscriber<EventArgs>
    {
        private readonly EventHandler _handler;
        public WeakHandlerWrapper(EventHandler handler) => _handler = handler;
        public void OnEvent(object? sender, WeakEvent ev, EventArgs e) => _handler(sender, e);

        public override int GetHashCode() => _handler.GetHashCode();
        public override bool Equals(object? obj) => obj is WeakHandlerWrapper other && other._handler == _handler;
    }
}
