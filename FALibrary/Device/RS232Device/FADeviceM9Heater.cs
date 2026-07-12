using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Diagnostics;
using FALibrary.Part.HeaterPart;

namespace FALibrary.Device.RS232Device
{
    public class FADeviceM9Heater : FASerialPortDevice
    {
        public struct ChannelSetTemperatureInfo
        {
            public ushort ChannelNo { get; set; }
            public double Temperature { get; set; }
        }

        protected readonly byte STX = 2;
        protected readonly byte ETX = 3;


        public double PVCH1 { get; set; }
        public double PVCH2 { get; set; }
        public double PVCH3 { get; set; }
        public double PVCH4 { get; set; }

        public double SVCH1 { get; set; }
        public double SVCH2 { get; set; }
        public double SVCH3 { get; set; }
        public double SVCH4 { get; set; }

        public FAM9Heater PartHeater { get; set; }

        public double CurrentTemperature { get; set; }
        public double TargetTemperature { get; set; }

        public double Ch1Temperature { get; set; }
        public double Ch2Temperature { get; set; }
        public double Ch3Temperature { get; set; }
        public double Ch4Temperature { get; set; }

        public DateTime LastReadTime { get; set; }
        private List<byte> _readData = new List<byte>();
        private bool _readType = false;
        private Stopwatch _commandSendWatch = new Stopwatch();
        private Queue<byte[]> _commandQueue = new Queue<byte[]>();


        public FADeviceM9Heater()
        {
            Address = 1;
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
                if (_commandSendWatch.ElapsedMilliseconds > 500)
                {
                    if (_commandQueue.Count > 0)
                    {
                        byte[] bytes = _commandQueue.Dequeue();
                        Port.Write(bytes, 0, bytes.Length);
                    }
                    if(_readType == true)
                    {
                        ReadTemperature();
                    }
                    else
                    {
                        ReadTargetTemperature();
                    }
                                      
                    _commandSendWatch.Restart();     
                }
            }
            catch
            {
            }
        }

        public bool ReadTemperature()
        {
            try
            {
                if (_commandQueue.Count > 0) return false;

                byte[] command = { 0x02, 0x30, 0x31, 0x44, 0x52, 0x53, 0x2C, 0x31, 0x34, 0x2C, 0x30, 0x30, 0x30, 0x31, 0x0D, 0x0A };

                _commandQueue.Enqueue(command);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool ReadTargetTemperature()
        {
            try
            {
                if (_commandQueue.Count > 0) return false;

                byte[] command = { 0x02, 0x30, 0x31, 0x44, 0x52, 0x53, 0x2C, 0x31, 0x34, 0x2C, 0x30, 0x30, 0x30, 0x31, 0x0D, 0x0A };

                if (_commandQueue.Count == 0)
                    _commandQueue.Enqueue(command);

                //char STX, CR, LF;
                //STX = Convert.ToChar(0x02);
                //CR = Convert.ToChar(0x0D);
                //LF = Convert.ToChar(0x0A);

                //string data = STX + "01DRS,14,0001" + CR + LF;
                //byte[] command;
                //command = Encoding.ASCII.GetBytes(data);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool WriteTemperature(ushort channel, double temperature)
        {
            try
            {
                if (_commandQueue.Count > 1) return false;

                char STX, CR, LF;
                STX = Convert.ToChar(0x02);
                CR = Convert.ToChar(0x0D);
                LF = Convert.ToChar(0x0A);
                var strTemperature = String.Format("{0:X4}", (int)(temperature * 10));
                var strChannel = channel.ToString("0000");
                string data = STX + "01DWR,03,0301,0001,0302," + strChannel + "0401," + strTemperature + CR + LF;
                byte[] command;
                command = Encoding.ASCII.GetBytes(data);

                _commandQueue.Enqueue(command);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool WriteTemperatureToMultiChannel(params ChannelSetTemperatureInfo[] arr)
        {
            try
            {
                if (arr == null) return false;
                if (arr.Length < 1) return false;
                if (_commandQueue.Count > 1) return false;

                string[] svSetDataArr = new string[arr.Length];                
                var strDataCount = (arr.Length * 3).ToString();
                for (int i = 0; i < arr.Length; i++)
                {
                    var strTemperature = String.Format("{0:X4}", (int)(arr[i].Temperature * 10));
                    svSetDataArr[i] = string.Format("0301,0001,0302,{0},0401,{1}", (i + 1).ToString("0000"), strTemperature);
                }

                string svSetData = string.Join(",", svSetDataArr);

                char STX, CR, LF;
                STX = Convert.ToChar(0x02);
                CR = Convert.ToChar(0x0D);
                LF = Convert.ToChar(0x0A);

                string data = STX + "01DWR," + strDataCount + ","+ svSetData + CR + LF;
                byte[] command;
                command = Encoding.ASCII.GetBytes(data);

                _commandQueue.Enqueue(command);
            }
            catch
            {
                return false;
            }

            return true;
        }

        protected virtual void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            LastReadTime = DateTime.Now;
            int len = Port.BytesToRead;
            byte[] buffer;
            //string str = string.Empty;
            //str = Port.ReadExisting();

            
            try
            {                
                //buffer = Encoding.ASCII.GetBytes(str);
                buffer = new byte[len];
                Port.Read(buffer,0,len);
                _readData.AddRange(buffer);

                int stxpos = _readData.IndexOf(0x02);
                if(stxpos < 0) return;
                
                int lf = _readData.IndexOf(0x0A, stxpos);
                if (lf < 0 || lf < stxpos) return;
               
                int dataLen = lf - stxpos + 1;
                byte[] data = new byte[dataLen];
                _readData.CopyTo(stxpos, data, 0, dataLen);
                var command = Encoding.ASCII.GetString(data, 3, 3);

                if (command == "DRS")
                {
                    ParseDRSReturnValue(data);
                }
               
                _readData.Clear();
            }
            catch
            {
                Port.DiscardInBuffer();
                _readData.Clear();                
            }
            finally
            {
                if (_readData.Count > 200)
                    _readData.Clear();
            }            
        }

        private void ParseDRSReturnValue(byte[] data)
        {
            if (data.Length < 81) return;

            byte[] Ch1Pv = { data[10], data[11], data[12], data[13] };
            byte[] Ch2Pv = { data[15], data[16], data[17], data[18] };
            byte[] Ch3Pv = { data[20], data[21], data[22], data[23] };
            byte[] Ch4Pv = { data[25], data[26], data[27], data[28] };

            byte[] Ch1Sv = { data[60], data[61], data[62], data[63] };
            byte[] Ch2Sv = { data[65], data[66], data[67], data[68] };
            byte[] Ch3Sv = { data[70], data[71], data[72], data[73] };
            byte[] Ch4Sv = { data[75], data[76], data[77], data[78] };

            string PvCh1 = Encoding.ASCII.GetString(Ch1Pv);
            PVCH1 = (double)(Convert.ToInt32(PvCh1, 16)) / 10.0;

            string PvCh2 = Encoding.ASCII.GetString(Ch2Pv);
            PVCH2 = (double)(Convert.ToInt32(PvCh2, 16)) / 10.0;

            string PvCh3 = Encoding.ASCII.GetString(Ch3Pv);
            PVCH3 = (double)(Convert.ToInt32(PvCh3, 16)) / 10.0;

            string PvCh4 = Encoding.ASCII.GetString(Ch4Pv);
            PVCH4 = (double)(Convert.ToInt32(PvCh4, 16)) / 10.0;

            string SvCh1 = Encoding.ASCII.GetString(Ch1Sv);
            SVCH1 = (double)(Convert.ToInt32(SvCh1, 16)) / 10.0;

            string SvCh2 = Encoding.ASCII.GetString(Ch2Sv);
            SVCH2 = (double)(Convert.ToInt32(SvCh2, 16)) / 10.0;

            string SvCh3 = Encoding.ASCII.GetString(Ch3Sv);
            SVCH3 = (double)(Convert.ToInt32(SvCh3, 16)) / 10.0;

            string SvCh4 = Encoding.ASCII.GetString(Ch4Sv);
            SVCH4 = (double)(Convert.ToInt32(SvCh4, 16)) / 10.0;

            if (PVCH1 > 160 ||
                PVCH2 > 160 ||
                PVCH3 > 160 ||
                PVCH4 > 160)
            {
                string s = Encoding.ASCII.GetString(data);
            }

            Ch1Temperature = PVCH1;
            Ch2Temperature = PVCH2;
            Ch3Temperature = PVCH3;
            Ch4Temperature = PVCH4;
        }

        protected byte GetBCC(byte[] arr, int lastIndex)
        {
            try
            {
                if (arr == null) return 0;
                byte result = arr[0];
                for (int i = 1; i <= lastIndex; i++)
                {
                    result = (byte)(result ^ arr[i]);
                }

                return result;
            }
            catch
            {
                return 0;
            }
        } 
    }    
}
