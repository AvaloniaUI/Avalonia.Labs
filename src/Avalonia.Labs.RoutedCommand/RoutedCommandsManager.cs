using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Labs.Input;

public sealed class RoutedCommandsManager : AvaloniaObject
{
    public static RoutedEvent<CanExecuteRoutedEventArgs> CanExecuteEvent { get; } =
        RoutedEvent.Register<CanExecuteRoutedEventArgs>("CanExecute", RoutingStrategies.Bubble, typeof(RoutedCommandsManager));

    public static RoutedEvent<ExecutedRoutedEventArgs> ExecutedEvent { get; } =
        RoutedEvent.Register<ExecutedRoutedEventArgs>("Executed", RoutingStrategies.Bubble, typeof(RoutedCommandsManager));

    public static readonly AttachedProperty<IList<RoutedCommandBinding>> CommandsProperty =
        AvaloniaProperty.RegisterAttached<RoutedCommandsManager, InputElement, IList<RoutedCommandBinding>>("Commands");

    private static readonly WeakReference<IInputElement?> _inputElement = 
        new WeakReference<IInputElement?>(default);

    internal static event EventHandler? Invalidate = default;

    static RoutedCommandsManager()
    {
        CanExecuteEvent.AddClassHandler<InputElement>(CanExecuteEventHandler);
        ExecutedEvent.AddClassHandler<InputElement>(ExecutedEventHandler);
        InputElement.GotFocusEvent.AddClassHandler<Interactive>(GotFocusEventHandler);
    }
    
    internal static IInputElement? CurrentElement
    {
        get
        {
            if(_inputElement.TryGetTarget(out var element))
            {
                return element;
            }
            return default;
        }
    }

    private static void GotFocusEventHandler(Interactive control, GotFocusEventArgs args)
    {
        _inputElement.SetTarget(args.Source as IInputElement);
        Invalidate?.Invoke(control, EventArgs.Empty);
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
}
