using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ML
{
    public class Command : ICommand
    {
        private Action<object> _exect;

        private Func<object, bool> _canExect;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public Command(Action<object> exec, Func<object, bool> canExec = null)
        {
            _exect = exec;
            _canExect = canExec;
        }

        public bool CanExecute(object parameter) => _canExect == null || _canExect.Invoke(parameter);
        public void Execute(object parameter) => _exect.Invoke(parameter);
    }
}
