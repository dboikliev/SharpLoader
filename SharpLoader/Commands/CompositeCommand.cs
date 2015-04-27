using System;
using System.Linq;
using System.Windows.Input;
using Microsoft.Practices.ObjectBuilder2;
using System.Collections.Generic;

namespace SharpLoader.Commands
{
    public class CompositeCommand : ICommand
    {
        private readonly IEnumerable<ICommand> commands;

        public CompositeCommand(params ICommand[] commands)
        {
            this.commands = commands;
        }

        public bool CanExecute(object parameter)
        {
            var canExecute = commands.All(command => command.CanExecute(parameter));
            return canExecute;
        }

        public void Execute(object parameter)
        {
            commands.ForEach(command => command.Execute(parameter));
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
