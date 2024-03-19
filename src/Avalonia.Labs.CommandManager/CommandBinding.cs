using System;
using System.Windows.Input;

namespace Avalonia.Labs.Input;

/// <summary>
/// Binds a <see cref="RoutedCommand"/> to the event handlers that implement the command.
/// </summary>
public class CommandBinding : AvaloniaObject
{
    /// <summary>
    /// Defines the <see cref="RoutedCommand"/> property.
    /// </summary>
    public static readonly DirectProperty<CommandBinding, ICommand?> CommandProperty =
        AvaloniaProperty.RegisterDirect<CommandBinding, ICommand?>(nameof(Command),
            o => o.Command,
            (o, v) => o.Command = v);

    private ICommand? _command;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandBinding"/> class.
    /// </summary>
    public CommandBinding()
    {
        
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandBinding"/> class by using the specified handler.
    /// </summary>
    /// <param name="command"><see cref="ICommand"/> associated with this CommandBinding</param>
    /// <param name="executed">Executed handler for the routed command.</param>
    public CommandBinding(ICommand command,
        EventHandler<ExecutedRoutedEventArgs>? executed)
        : this(command, executed, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandBinding"/> class by using the specified handler.
    /// </summary>
    /// <param name="command"><see cref="ICommand"/> associated with this CommandBinding</param>
    /// <param name="executed">Executed handler for the routed command.</param>
    /// <param name="canExecute">Can execute handler for the routed command.</param>
    public CommandBinding(ICommand command,
        EventHandler<ExecutedRoutedEventArgs>? executed,
        EventHandler<CanExecuteRoutedEventArgs>? canExecute)
    {
        Command = command;
        if (executed is not null)
        {
            Executed += executed;
        }
        if (canExecute is not null)
        {
            CanExecute += canExecute;
        }
    }
    
    /// <summary>
    /// Gets or sets the <see cref="ICommand"/> associated with this CommandBinding.
    /// </summary>
    public ICommand? Command
    {
        get => _command;
        set => SetAndRaise(CommandProperty, ref _command, value);
    }

    /// <summary>
    /// Called when the command is executed.
    /// </summary>
    public event EventHandler<ExecutedRoutedEventArgs>? Executed;

    /// <summary>
    /// Called to determine if the command can be executed.
    /// </summary>
    public event EventHandler<CanExecuteRoutedEventArgs>? CanExecute;

    /// <summary>
    ///     Calls the CanExecute or PreviewCanExecute event based on the event argument's RoutedEvent.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">Event arguments.</param>
    internal void OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        if (e.Handled) return;
        if (CanExecute is null)
        {
            if (e.CanExecute) return;
            // If there is an Executed handler, then the command can be executed.
            if (Executed is null) return;
            e.CanExecute = true;
            e.Handled = true;
        }
        else
        {
            CanExecute(sender, e);
            if (e.CanExecute)
            {
                e.Handled = true;
            }
        }
    }

    private bool CheckCanExecute(object sender, ExecutedRoutedEventArgs e)
    {
        CanExecuteRoutedEventArgs canExecuteArgs = new(e.Command, e.Parameter)
        {
            RoutedEvent = CommandManager.CanExecuteEvent,
            // Since we don't actually raise this event, we have to explicitly set the source.
            Source = e.Source
        };

        OnCanExecute(sender, canExecuteArgs);

        return canExecuteArgs.CanExecute;
    }

    /// <summary>
    ///     Calls Executed or PreviewExecuted based on the event argument's RoutedEvent.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">Event arguments.</param>
    internal void OnExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Handled) return;
        if (Executed is null) return;
        if (!CheckCanExecute(sender, e)) return;
        Executed(sender, e);
        e.Handled = true;
    }
}
