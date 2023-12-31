using System;
using System.Windows.Input;

namespace StartStopServices.ViewModels
{
    public class DelegateCommand : ICommand
    {
        
        private readonly Action command;
        private readonly Func<bool> canExecute;

       
        public DelegateCommand(Action command,Func<bool> canExecute = null)
        {
            this.canExecute = canExecute;
            this.command = command ?? throw new ArgumentException();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) 
        {
            return canExecute == null || canExecute();
        }


        public void Execute(object parameter) => command();
    }
}
