using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FALibrary.Device.Network
{
    public class FARemoteObject : MarshalByRefObject
    {
        private DateTime _lastAccessTimeAsMessage;
        public DateTime LastAccessTimeAsMessage
        {
            get { return _lastAccessTimeAsMessage; }
            set
            {
                _lastAccessTimeAsMessage = value;
            }
        }

        private string _data;
        public string Data
        {
            get { return _data; }
            set
            {
                _data = value;
            }
        }

        public void ConnectTest()
        {
        }
    }  
}
