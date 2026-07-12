using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace FAFramework.Utility
{
    public class NoActionRoutedCommand : ICommand
    {
        public event EventHandler CanExecuteChanged = null;

        public void Execute(object obj)
        {
        }

        public bool CanExecute(object obj)
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(obj, EventArgs.Empty);

            return true;
        }
    }
}
