using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FAFramework.Utility.PackingLog
{
    public class LogBase
    {
        public DateTime Date { get; set; }
        private List<string> _elements = new List<string>();

        public LogBase()
        {
            Date = DateTime.Now;
            AppendElement(Date.ToString("yyyy/MM/dd HH:mm:ss.fff"));
        }

        protected void AppendElement(string str)
        {
            if (str == null)
                str = string.Empty;

            _elements.Add(str);
        }

        protected string ToLog()
        {
            return string.Join(", ", _elements);
        }
    }
}
