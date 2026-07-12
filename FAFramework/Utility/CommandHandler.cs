using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace FAFramework.Utility
{
    public class CommandHandler : ICommand
    {
        private Action<object> _action;
        private bool _canExecute;

        public CommandHandler(Action<object> action, bool canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action(parameter);
        }

        public void SetCanExecute(bool flag)
        {
            _canExecute = flag;
            RaiseCanExecuteChanged();
        }

        private void RaiseCanExecuteChanged()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(
                new Action(delegate ()
                {
                    if (CanExecuteChanged != null)
                        CanExecuteChanged(this, EventArgs.Empty);
                }), null);
        }
    }
}
