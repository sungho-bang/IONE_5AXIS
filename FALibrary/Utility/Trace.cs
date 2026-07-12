using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FALibrary.Utility
{
    public class TraceEventArgs : EventArgs
    {
        public string MessageType { get; set; }
        public string Message { get; set; }

        public TraceEventArgs(string msgType, string msg)
        {
            MessageType = msgType;
            Message = msg;
        }
    }

    public static class Trace
    {
        public static event EventHandler<TraceEventArgs> OnWriteLine;

        public static void WriteLine(object sender, string msgType, string msg)
        {
            if (OnWriteLine != null)
                OnWriteLine(sender, new TraceEventArgs(msgType, msg));
        }
    }
}
