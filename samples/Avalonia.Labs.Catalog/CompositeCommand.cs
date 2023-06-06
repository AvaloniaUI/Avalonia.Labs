using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Avalonia.Labs.Catalog;

internal class CompositeCommand : AvaloniaObject, ICommand
{
    readonly List<ICommand> _commands;
    public CompositeCommand(IEnumerable<ICommand> commands)
    {

        _commands = new(commands);
        foreach (ICommand command in _commands)
        {
            command.CanExecuteChanged += Command_CanExecuteChanged;
            ;
        }
    }

    private void Command_CanExecuteChanged(object? sender, EventArgs e)
    {
        CanExecuteChanged?.Invoke(sender, e);
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        return _commands.All(x => x.CanExecute(parameter));
    }

    public void Execute(object? parameter)
    {
        foreach (var item in _commands)
        {
            item.Execute(parameter);
        }
    }
}
