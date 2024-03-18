using System.Windows.Input;

namespace Avalonia.Labs.Input;

/// <summary>
/// Binds a <see cref="RoutedCommand"/> to the event handlers that implement the command.
/// </summary>
public sealed class RoutedCommandBinding : AvaloniaObject
{
    /// <summary>
    /// Defines the <see cref="RoutedCommand"/> property.
    /// </summary>
    public static readonly DirectProperty<RoutedCommandBinding, RoutedCommand?> RoutedCommandProperty =
        AvaloniaProperty.RegisterDirect<RoutedCommandBinding, RoutedCommand?>(nameof(RoutedCommand),
            o => o.RoutedCommand,
            (o, v) => o.RoutedCommand = v);

    /// <summary>
    /// Defines the <see cref="ExecutingCommand"/> property.
    /// </summary>
    public static readonly DirectProperty<RoutedCommandBinding, ICommand?> ExecutingCommandProperty =
        AvaloniaProperty.RegisterDirect<RoutedCommandBinding, ICommand?>(nameof(ExecutingCommand),
            o => o.ExecutingCommand,
            (o, v) => o.ExecutingCommand = v
        );

    /// <summary>
    /// Defines the <see cref="ExecutingCommandParameter"/> property.
    /// </summary>
    public static readonly AvaloniaProperty<object?> ExecutingCommandParameterProperty =
        AvaloniaProperty.Register<RoutedCommandBinding, object?>(nameof(ExecutingCommandParameter));

    private RoutedCommand? _routedCommand;
    private ICommand? _executingCommand;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoutedCommandBinding"/> class.
    /// </summary>
    public RoutedCommandBinding()
    {

    }
        
    /// <summary>
    /// Initializes a new instance of the <see cref="RoutedCommandBinding"/> class by using the specified handler.
    /// </summary>
    /// <param name="routedCommand"><see cref="RoutedCommand"/> associated with this CommandBinding</param>
    /// <param name="executingCommand"><see cref="ICommand"/> handler for the routed command.</param>
    public RoutedCommandBinding(RoutedCommand routedCommand, ICommand? executingCommand)
    {
        RoutedCommand = routedCommand;
        ExecutingCommand = executingCommand;
    }

    /// <summary>
    /// Gets or sets the <see cref="ICommand"/> associated with this CommandBinding.
    /// </summary>
    public RoutedCommand? RoutedCommand
    {
        get => _routedCommand;
        set => SetAndRaise(RoutedCommandProperty, ref _routedCommand, value);
    }

    /// <summary>
    /// <see cref="ICommand"/> handler for the routed command.
    /// </summary>
    public ICommand? ExecutingCommand
    {
        get => _executingCommand;
        set => SetAndRaise(ExecutingCommandProperty, ref _executingCommand, value);
    }

    /// <summary>
    /// Parameter of <see cref="ICommand"/> handler for the routed command.
    /// </summary>
    public object? ExecutingCommandParameter
    {
        get => GetValue(ExecutingCommandParameterProperty);
        set => SetValue(ExecutingCommandParameterProperty, value);
    }

    internal bool DoCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        if (e.Handled)
            return true;

        if (ExecutingCommand is { } command)
        {
            var parameter = e.Parameter;
            if (IsSet(ExecutingCommandParameterProperty))
            {
                parameter = ExecutingCommandParameter;
            }
            e.CanExecute = command.CanExecute(parameter);
            e.Handled = true;
        }

        return e.CanExecute;
    }

    internal bool DoExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        if (!e.Handled && (ExecutingCommand is { } command))
        {
            var parameter = e.Parameter;
            if (IsSet(ExecutingCommandParameterProperty))
            {
                parameter = ExecutingCommandParameter;
            }
            command.Execute(parameter);
            e.Handled = true;
            return true;
        }

        return false;
    }
}
