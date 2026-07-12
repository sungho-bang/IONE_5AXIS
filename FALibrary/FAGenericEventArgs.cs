using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FALibrary
{
    public class FAGenericEventArgs<T> : EventArgs
    {
        public FAGenericEventArgs(T value)
        {
            _value = value;
        }

        private T _value;

        public T Value
        {
            get { return _value; }
        }
    }
}
