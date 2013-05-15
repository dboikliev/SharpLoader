using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace DownloaderWPF.Commands
{
    class RelayCommandWithParameter<T> : ICommand
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
            return this.canExecute(parameter);
        }

        bool ICommand.CanExecute(object parameter)
        {
            if (canExecute == null)
            {
                return true;
            }
            return this.canExecute((T)parameter);
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
            this.execute(parameter);
        }

        void ICommand.Execute(object parameter)
        {
            this.execute((T)parameter);
        }
    }
}
