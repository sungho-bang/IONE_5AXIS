using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FALibrary.Device.LanDevice
{
    public class FAPrintronix5000TRDevice : FAAsyncSocketDevice
    {
        private readonly byte STX = 2;
        //private readonly byte ETX = 3;

        private List<byte> _sumReceivedData = new List<byte>();

        public bool StatusOnlineError { get; private set; }        
        public bool StatusPaperOutError { get; private set; }
        public bool StatusHeadOpenError { get; private set; }
        public bool StatusBufferOverflow { get; private set; }
        public bool StatusRibbonOut { get; private set; }
        
        public FAPrintronix5000TRDevice()
        {
            StreamEncoding = Encoding.ASCII;
            OnRead += Read;
        }

        public void SendStatusCheckCommand()
        {
            try
            {
                InitialStatus();
                byte[] command = { 2, 3, (byte)'~', (byte)'H', (byte)'S', 12, 10 };
                Write(command);
            }
            catch
            {
                StatusOnlineError = true;
            }
        }

        public void PrintScript(string script)
        {
            try
            {
                WriteString(script);
            }
            catch
            {
                StatusOnlineError = true;
            }
        }

        public override void LoadParameters(System.Xml.Linq.XElement xml)
        {
            base.LoadParameters(xml);
        }

        public virtual void Read(object sender, FAGenericEventArgs<byte[]> e)
        {
            try
            {
                _sumReceivedData.AddRange(e.Value);

                if (_sumReceivedData.Count >= 82)
                {
                    string s = StreamEncoding.GetString(e.Value);

                    int stxIndex = 0;
                    for (stxIndex = 0; stxIndex < _sumReceivedData.Count; stxIndex++)
                    {
                        if (_sumReceivedData[stxIndex] == STX) break;
                    }

                    if (_sumReceivedData[stxIndex] == STX &&
                        _sumReceivedData[stxIndex + 36] == STX &&
                        _sumReceivedData[stxIndex + 69] == STX &&
                        _sumReceivedData[stxIndex + 72] == STX &&
                        _sumReceivedData[stxIndex + 79] == STX)
                    {
                    }

                    string[] buffer = s.Split(',');

                    if (buffer[1] == "1")
                        StatusPaperOutError = true;
                    else
                        StatusPaperOutError = false;

                    if (buffer[2] == "1")
                        StatusOnlineError = true;
                    else
                        StatusOnlineError = false;

                    if (buffer[5] == "1")
                        StatusBufferOverflow = true;
                    else
                        StatusBufferOverflow = false;

                    if (buffer[13] == "1")
                        StatusHeadOpenError = true;
                    else
                        StatusHeadOpenError = false;

                    if (buffer[14] == "1")
                        StatusRibbonOut = true;
                    else
                        StatusRibbonOut = false;

                    _sumReceivedData.Clear();
                }
            }
            catch
            {
            }
        }

        private void InitialStatus()
        {
            StatusOnlineError = false;            
            StatusPaperOutError = false;
            StatusHeadOpenError = false;
            StatusBufferOverflow = false;
            StatusRibbonOut = false;
        }
    }
}
