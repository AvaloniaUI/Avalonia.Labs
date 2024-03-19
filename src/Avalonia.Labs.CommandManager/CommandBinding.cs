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

    /// <summary>
    /// Defines the <see cref="ExecutingCommand"/> property.
    /// </summary>
    public static readonly DirectProperty<CommandBinding, ICommand?> ExecutingCommandProperty =
        AvaloniaProperty.RegisterDirect<CommandBinding, ICommand?>(nameof(ExecutingCommand),
            o => o.ExecutingCommand,
            (o, v) => o.ExecutingCommand = v
        );

    private ICommand? _command;
    private ICommand? _executingCommand;

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
    /// <see cref="ICommand"/> handler for the routed command.
    /// </summary>
    public ICommand? ExecutingCommand
    {
        get => _executingCommand;
        set => SetAndRaise(ExecutingCommandProperty, ref _executingCommand, value);
    }

    /// <summary>
    /// Called when the command is executed.
    /// </summary>
    public event EventHandler<ExecutedRoutedEventArgs>? Executed;

    /// <summary>
    /// Called to determine if the command can be executed.
    /// </summary>
    public event EventHandler<CanExecuteRoutedEventArgs>? CanExecute;

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

    internal void OnExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Handled) return;
        if (Executed is null) return;
        if (!CheckCanExecute(sender, e)) return;
        Executed(sender, e);
        e.Handled = true;
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ExecutingCommandProperty)
        {
            if (change.GetOldValue<ICommand?>() is not null)
            {
                CanExecute -= OnCanExecuteRedirect;
                Executed -= OnExecutedRedirect;
            }

            if (change.GetNewValue<ICommand?>() is not null)
            {
                CanExecute += OnCanExecuteRedirect;
                Executed += OnExecutedRedirect;
            }
        }
    }

    private void OnExecutedRedirect(object? sender, ExecutedRoutedEventArgs e)
    {
        if (!e.Handled && (ExecutingCommand is { } command))
        {
            command.Execute(e.Parameter);
            e.Handled = true;
        }
    }

    private void OnCanExecuteRedirect(object? sender, CanExecuteRoutedEventArgs e)
    {
        if (!e.Handled && (ExecutingCommand is { } command))
        {
            e.CanExecute = command.CanExecute(e.Parameter);
            e.Handled = true;
        }
    }
}
