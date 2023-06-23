using System.Windows.Input;

namespace Avalonia.Labs.Input
{
    public sealed class RoutedCommandBinding : AvaloniaObject
    {

        public static readonly DirectProperty<RoutedCommandBinding, RoutedCommand?> RoutedCommandProperty =
            AvaloniaProperty.RegisterDirect<RoutedCommandBinding, RoutedCommand?>(nameof(RoutedCommand),
                o => o.RoutedCommand,
                (o, v) => o.RoutedCommand = v
                );

        public static readonly DirectProperty<RoutedCommandBinding, ICommand?> CommandProperty =
            AvaloniaProperty.RegisterDirect<RoutedCommandBinding, ICommand?>(nameof(Command),
                o => o.Command,
                (o, v) => o.Command = v
                );

        public static readonly AvaloniaProperty<object?> CommandParameterProperty =
            AvaloniaProperty.Register<RoutedCommandBinding, object?>(nameof(CommandParameter));


        private RoutedCommand? _routedCommand;

        private ICommand? _command;

        public RoutedCommandBinding()
        {

        }
        public RoutedCommandBinding(RoutedCommand routedCommand, ICommand? command)
        {
            RoutedCommand = routedCommand;
            Command = command;
        }

        public RoutedCommand? RoutedCommand
        {
            get => _routedCommand;
            set => SetAndRaise(RoutedCommandProperty, ref _routedCommand, value);
        }

        public ICommand? Command
        {
            get => _command;
            set => SetAndRaise(CommandProperty, ref _command, value);
        }

        public object? CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        internal bool DoCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Handled)
                return true;

            if (Command is { } command)
            {
                var parameter = e.Parameter;
                if (IsSet(CommandParameterProperty))
                {
                    parameter = CommandParameter;
                }
                e.CanExecute = command.CanExecute(parameter);
                e.Handled = true;
            }

            return e.CanExecute;
        }

        internal bool DoExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (!e.Handled && (Command is { } command))
            {
                var parameter = e.Parameter;
                if (IsSet(CommandParameterProperty))
                {
                    parameter = CommandParameter;
                }
                command.Execute(parameter);
                e.Handled = true;
                return true;
            }

            return false;
        }
    }
}
