using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FAFramework.Utility.PackingLog
{
    public class EventLog : LogBase
    {
        public enum EState
        {
            Run, Stop, Error
        }

        public EState State { get; set; }
        public string Event { get; set; }

        public override string ToString()
        {
            AppendElement(State.ToString());
            AppendElement(Event);

            return ToLog();
        }
    }
}
