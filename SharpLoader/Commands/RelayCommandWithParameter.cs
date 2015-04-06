using System;
using System.Windows.Input;

namespace SharpLoader.Commands
{
    public class RelayCommandWithParameter<T> : ICommand
    {
        private Action<T> execute;
        private Predicate<T> canExecute;

        public RelayCommandWithParameter(Action<T> execute, Predicate<T> canExecute)
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
