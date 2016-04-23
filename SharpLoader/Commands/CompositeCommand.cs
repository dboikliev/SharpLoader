using System;
using System.Linq;
using System.Windows.Input;
using Microsoft.Practices.ObjectBuilder2;
using System.Collections.Generic;

namespace SharpLoader.Commands
{
    public class CompositeCommand : ICommand
    {
        private readonly IEnumerable<ICommand> _commands;

        public CompositeCommand(params ICommand[] commands)
        {
            _commands = commands;
        }

        public bool CanExecute(object parameter)
        {
            var canExecute = _commands.All(command => command.CanExecute(parameter));
            return canExecute;
        }

        public void Execute(object parameter)
        {
            _commands.ForEach(command => command.Execute(parameter));
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }
    }
}
