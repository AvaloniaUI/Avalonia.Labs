using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.Utilities;

namespace Avalonia.Labs.Input;

/// <summary>
/// Provides command related utility methods that register <see cref="RoutedCommandBinding"/> objects for class owners and commands,
/// add and remove command event handlers, and provides services for querying the status of a command.
/// </summary>
public sealed class RoutedCommandManager : AvaloniaObject
{
    [ThreadStatic]
    private static RoutedCommandManager? s_commandManager;
    private DispatcherOperation? _requerySuggestedOperation;

    /// <summary>
    /// Identifies the CanExecute attached event.
    /// </summary>
    public static RoutedEvent<CanExecuteRoutedEventArgs> CanExecuteEvent =
        RoutedEvent.Register<CanExecuteRoutedEventArgs>("CanExecute", RoutingStrategies.Bubble | RoutingStrategies.Tunnel, typeof(RoutedCommandManager));

    /// <summary>
    /// Identifies the Executed attached event.
    /// </summary>
    public static RoutedEvent<ExecutedRoutedEventArgs> ExecutedEvent =
        RoutedEvent.Register<ExecutedRoutedEventArgs>("Executed", RoutingStrategies.Bubble | RoutingStrategies.Tunnel, typeof(RoutedCommandManager));

    public static readonly AttachedProperty<IList<RoutedCommandBinding>?> CommandsProperty =
        AvaloniaProperty.RegisterAttached<RoutedCommandManager, InputElement, IList<RoutedCommandBinding>?>("Commands");

    private static readonly WeakReference<IInputElement?> s_inputElement = 
        new WeakReference<IInputElement?>(default);

    internal static readonly WeakEvent<RoutedCommandManager, EventArgs> PrivateRequerySuggestedEvent =
        WeakEvent.Register<RoutedCommandManager>(
            (m, v) => m.PrivateRequerySuggested += v,
            (m, v) => m.PrivateRequerySuggested -= v);

    private event EventHandler? PrivateRequerySuggested;
    public static event EventHandler RequerySuggested
    {
        // WeakHandlerWrapper will ensure, that add/remove with the same handler will work.
        add => PrivateRequerySuggestedEvent.Subscribe(Current, new WeakHandlerWrapper(value));
        remove => PrivateRequerySuggestedEvent.Unsubscribe(Current, new WeakHandlerWrapper(value));
    }

    static RoutedCommandManager()
    {
        CanExecuteEvent.AddClassHandler<InputElement>(CanExecuteEventHandler);
        ExecutedEvent.AddClassHandler<InputElement>(ExecutedEventHandler);
        InputElement.GotFocusEvent.AddClassHandler<Interactive>(GotFocusEventHandler);
    }

    private RoutedCommandManager()
    {
    }

    internal static RoutedCommandManager Current => s_commandManager ??= new RoutedCommandManager();

    internal static IInputElement? FocusedElement => s_inputElement.TryGetTarget(out var element) ? element : default;

    private static void GotFocusEventHandler(Interactive control, GotFocusEventArgs args)
    {
        s_inputElement.SetTarget(args.Source as IInputElement);
        Current.RaiseRequerySuggested();
    }

    /// <summary>
    /// Invokes RequerySuggested listeners registered on the current thread.
    /// </summary>
    public static void InvalidateRequerySuggested()
    {
        Current.RaiseRequerySuggested();
    }
    
    public static IList<RoutedCommandBinding> GetCommands(InputElement element)
    {
        var commands = element.GetValue(CommandsProperty);
        if (commands is null)
        {
            commands = new List<RoutedCommandBinding>();
            element.SetValue(CommandsProperty, commands);
        }
        return commands;
    }

    public static void SetCommands(InputElement element, IList<RoutedCommandBinding> commands) =>
        element.SetValue(CommandsProperty, commands);

    private static void CanExecuteEventHandler(InputElement inputElement, CanExecuteRoutedEventArgs args)
    {
        if (GetCommands(inputElement) is { } commands)
        {
            var binding = commands
                .Where(c => c != null)
                .FirstOrDefault(c => c.RoutedCommand == args.Command && c.DoCanExecute(inputElement, args));
            if(!args.Handled)
                args.CanExecute = binding != null;
        }
    }

    private static void ExecutedEventHandler(InputElement inputElement, ExecutedRoutedEventArgs args)
    {
        if (GetCommands(inputElement) is { } commands)
        {
            // ReSharper disable once UnusedVariable
            var binding = commands
                .Where(c => c != null)
                .FirstOrDefault(c => c.RoutedCommand == args.Command && c.DoExecuted(inputElement, args));
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
