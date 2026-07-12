using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Device.RS232Device;
using System.IO.Ports;
using System.Diagnostics;

namespace FALibrary.Device.CAS
{
    public class FACI1500Device : FASerialPortDevice
    {
        private readonly Byte CR = 13;
        private readonly Byte LF = 10;

        private Stopwatch _commandSendWatch = new Stopwatch();
        private Queue<byte[]> _commandQueue = new Queue<byte[]>();
        private List<byte> _readData = new List<byte>();

        public byte DeviceNo { get; set; }
        public DateTime LastReadTime { get; set; }

        public string Status { get; set; }
        public string WeightType { get; set; }
        public double Weight { get; set; }

        public FACI1500Device()
        {
            Port.DataReceived += OnDataReceived;
            LastReadTime = DateTime.Now;
        }

        public override void Open()
        {
            Port.Open();
            _commandSendWatch.Start();
        }

        public override void Close()
        {
            Port.Close();
        }

        public override void ReadWrite()
        {
            try
            {
                if (_commandSendWatch.ElapsedMilliseconds > 100)
                {
                    if (_commandQueue.Count > 0)
                    {
                        byte[] bytes = _commandQueue.Dequeue();
                        Port.Write(bytes, 0, bytes.Length);
                    }
                    else
                    {
                        ReadStatus();
                    }

                    _commandSendWatch.Restart();
                }
            }
            catch
            {
            }
        }

        public void ReadStatus()
        {
            byte[] command = { DeviceNo };
            _commandQueue.Enqueue(command);
        }

        protected virtual void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            LastReadTime = DateTime.Now;

            int len = Port.BytesToRead;
            byte[] buffer;

            try
            {
                buffer = new byte[len];
                Port.Read(buffer, 0, len);

                _readData.AddRange(buffer);

                if (_readData.Count == 22)
                {
                    if (_readData[20] != CR ||
                        _readData[21] != LF)
                    {
                        return;
                    }

                    byte[] status = { _readData[0], _readData[1] };
                    byte[] weightType = { _readData[3], _readData[4] };
                    byte[] deviceNo = { _readData[6] };
                    byte[] weight = { _readData[9], _readData[10], _readData[11], _readData[12], _readData[13], _readData[14], _readData[15], _readData[16] };
                    Status = Encoding.ASCII.GetString(status);
                    WeightType = Encoding.ASCII.GetString(weightType);
                    Weight = double.Parse(Encoding.ASCII.GetString(weight));
                }
                else if (_readData.Count > 22)
                {
                    _readData.Clear();
                    Port.DiscardInBuffer();
                }
            }
            catch
            {
                Port.DiscardInBuffer();
                return;
            }
            finally
            {
                if (_readData.Count > 200)
                    _readData.Clear();
                Port.DiscardInBuffer();
            }
        }
    }
}
