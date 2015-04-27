using System;
using System.Windows.Input;

namespace SharpLoader.Commands
{
    public class CommandWithParameter<T> : ICommand
    {
        private readonly Action<T> execute;
        private readonly Predicate<T> canExecute;

        public CommandWithParameter(Action<T> execute, Predicate<T> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(T parameter)
        {
            if (canExecute == null)
            {
                return true;
            }
            return canExecute(parameter);
        }

        bool ICommand.CanExecute(object parameter)
        {
            if (canExecute == null)
            {
                return true;
            }
            return canExecute((T)parameter);
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

        public void Execute(T parameter)
        {
            execute(parameter);
        }

        void ICommand.Execute(object parameter)
        {
            execute((T)parameter);
        }
    }
}
