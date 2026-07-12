using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace FALibrary.Utility
{
    public class CreatePropertyEventArgs : EventArgs
    {
        [FAAttribute("")]
        public PropertyInfo PropertyInfo { get; set; }
        public CreatePropertyEventArgs() { }
        public CreatePropertyEventArgs(PropertyInfo info)
        {
            PropertyInfo = info;
        }
    }
}
