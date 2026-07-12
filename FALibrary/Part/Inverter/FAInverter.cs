using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Device.Inverter;
using System.Windows;

namespace FALibrary.Part.Inverter
{
    public class FAInverter : FAPart
    {
        #region Field
        //---------------------------------
        private bool _communicationOn;
        //---------------------------------
        private double _read_SetSpeed;
        private double _read_GetSpeed;
        private double _read_Current;
        private double _read_Volt;
        private double _read_TorqueRate;
        private double _read_AccelTime;
        private double _read_DecelTime;
        //---------------------------------
        private bool _read_Bit00_Run;
        private bool _read_Bit01_ForwardRun;
        private bool _read_Bit02_ReverseRun;
        private bool _read_Bit03_SU;
        private bool _read_Bit04_OL;
        private bool _read_Bit05;
        private bool _read_Bit06;
        private bool _read_Bit07;
        private bool _read_Bit15_ERR;
        //---------------------------------
        private double _write_SetSpeed = 0;
        private int _write_AccelTime = 0;
        private int _write_DecelTime = 0;
        //---------------------------------
        #endregion

        #region Status
        [FAAttribute("Status")]
        public bool CommunicationOn
        {
            get { return _communicationOn; }
            set
            {
                if (value == _communicationOn) return;

                _communicationOn = value;
                NotifyPropertyChanged("CommunicationOn");
            }
        }
        #endregion

        #region Time
        [FAAttribute("Time")]
        public FALibrary.Utility.FATime TimeCommunicationReadTimeLimit { get; set; }
        #endregion

        #region Alarm
        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int AlarmCommunicationError { get; set; }
        #endregion

        public FAMitsubishiInverterDevice Device
        {
            get;
            protected set;
        }

        #region Status
        //------------------------------------------------------------------        
        [FAAttribute("Status")]
        [FAPropertyAttribute]
        public double Write_SetSpeed
        {
            get { return _write_SetSpeed; }
            set
            {
                if (_write_SetSpeed != value)
                {
                    _write_SetSpeed = value;
                    NotifyPropertyChanged("Write_SetSpeed");

                    if (SimulationMode == false)
                        Device.WriteSpeed(value);
                }
            }
        }

        //------------------------------------------------------------------
        [FAAttribute("Status")]
        public double Read_SetSpeed
        {
            get { return _read_SetSpeed; }
            set
            {
                if (_read_SetSpeed != value)
                {
                    _read_SetSpeed = value;
                    NotifyPropertyChanged("Read_SetSpeed");
                }
            }
        }

        [FAAttribute("Status")]
        public double Read_GetSpeed
        {
            get { return _read_GetSpeed; }
            set
            {
                if (_read_GetSpeed != value)
                {
                    _read_GetSpeed = value;
                    NotifyPropertyChanged("Read_GetSpeed");
                }
            }
        }
        //------------------------------------------------------------------
        [FAAttribute("Status")]
        public bool Read_Bit00_Run
        {
            get { return _read_Bit00_Run; }
            set
            {
                if (_read_Bit00_Run != value)
                {
                    _read_Bit00_Run = value;
                    NotifyPropertyChanged("Read_Bit00_Run");
                }
            }
        }

        [FAAttribute("Status")]
        public bool Read_Bit01_ForwardRun
        {
            get { return _read_Bit01_ForwardRun; }
            set
            {
                if (_read_Bit01_ForwardRun != value)
                {
                    _read_Bit01_ForwardRun = value;
                    NotifyPropertyChanged("Read_Bit01_ForwardRun");
                }
            }
        }

        [FAAttribute("Status")]
        public bool Read_Bit02_ReverseRun
        {
            get { return _read_Bit02_ReverseRun; }
            set
            {
                if (_read_Bit02_ReverseRun != value)
                {
                    _read_Bit02_ReverseRun = value;
                    NotifyPropertyChanged("Read_Bit02_ReverseRun");
                }
            }
        }

        [FAAttribute("Status")]
        public bool Read_Bit03_SU   //주파수 도달
        {
            get { return _read_Bit03_SU; }
            set
            {
                if (_read_Bit03_SU != value)
                {
                    _read_Bit03_SU = value;
                    NotifyPropertyChanged("Read_Bit03_SU");
                }
            }
        }

        [FAAttribute("Status")]
        public bool Read_Bit04_OL   //과부하
        {
            get { return _read_Bit04_OL; }
            set
            {
                if (_read_Bit04_OL != value)
                {
                    _read_Bit04_OL = value;
                    NotifyPropertyChanged("Read_Bit04_OL");
                }
            }
        }

        [FAAttribute("Status")]
        public bool Read_Bit05
        {
            get { return _read_Bit05; }
            set
            {
                if (_read_Bit05 != value)
                {
                    _read_Bit05 = value;
                    NotifyPropertyChanged("Read_Bit05");
                }
            }
        }

        [FAAttribute("Status")]
        public bool Read_Bit06
        {
            get { return _read_Bit06; }
            set
            {
                if (_read_Bit06 != value)
                {
                    _read_Bit06 = value;
                    NotifyPropertyChanged("Read_Bit06");
                }
            }
        }

        [FAAttribute("Status")]
        public bool Read_Bit07
        {
            get { return _read_Bit07; }
            set
            {
                if (_read_Bit07 != value)
                {
                    _read_Bit07 = value;
                    NotifyPropertyChanged("Read_Bit07");
                }
            }
        }

        [FAAttribute("Status")]
        public bool Read_Bit15_ERR
        {
            get { return _read_Bit15_ERR; }
            set
            {
                if (_read_Bit15_ERR != value)
                {
                    _read_Bit15_ERR = value;
                    NotifyPropertyChanged("Read_Bit15_ERR");
                }
            }
        }
        //------------------------------------------------------------------
        #endregion

        public override void SetDevice(Device.FADevice aDevice)
        {
            if (aDevice is FAMitsubishiInverterDevice)
                Device = aDevice as FAMitsubishiInverterDevice;
            else
                throw new Exception("Device Type is not correct." + " Device Name : " + Name + " Device Type : " + aDevice.GetType().ToString());
        }

        //-------------------------------------------------------------------------
        public static int SetBit(int data, int iBitPos, bool value)
        {
            if (value == true)
                data |= 1 << iBitPos;
            else
                data &= ~(1 << iBitPos);
            return data;
        }

        public static bool GetBit(int data, int iBitPos)
        {
            data &= 1 << iBitPos;
            return (data > 0) ? true : false;
        }
        //-------------------------------------------------------------------------
        public override void Validate()
        {
            if (SimulationMode == false)
            {
                if (DateTime.Now - Device.LastReadTime > TimeCommunicationReadTimeLimit.Time)
                    CommunicationOn = false;
                else
                    CommunicationOn = true;

                Device.ReadWrite();
                base.Validate();

                //--------------------------------------------------------
                Read_Bit00_Run          = GetBit(Device.usInvertorMonitorBits, 00);
                Read_Bit01_ForwardRun   = GetBit(Device.usInvertorMonitorBits, 01);
                Read_Bit02_ReverseRun   = GetBit(Device.usInvertorMonitorBits, 02);
                Read_Bit03_SU           = GetBit(Device.usInvertorMonitorBits, 03);
                Read_Bit04_OL           = GetBit(Device.usInvertorMonitorBits, 04);
                Read_Bit05              = GetBit(Device.usInvertorMonitorBits, 05);
                Read_Bit06              = GetBit(Device.usInvertorMonitorBits, 06);
                Read_Bit07              = GetBit(Device.usInvertorMonitorBits, 07);
                Read_Bit15_ERR          = GetBit(Device.usInvertorMonitorBits, 15);
                //--------------------------------------------------------
                Read_SetSpeed = Device.usRead_SetSpeed * 0.01; //(~60Hz)
                Read_GetSpeed = Device.usRead_GetSpeed * 0.01; //(~60Hz)
                //--------------------------------------------------------
            }
        }

        [FAAttribute("Operation")]
        public void Run()
        {
            ushort data = 0;
            data = (ushort)SetBit(data, 02, true);
            Device.WriteControlBits(data);
            //MessageBox.Show("데이터를 보냄");
            System.Diagnostics.Trace.WriteLine($"data : {data} ");
        }

        [FAAttribute("Operation")]
        public void ReverseRun()
        {
            ushort data = 0;
            data = (ushort)SetBit(data, 01, true);
            if (SimulationMode == false)
                Device.WriteControlBits(data);
        }
        [FAAttribute("Operation")]
        public void Stop()
        {
            ushort data = 0;
            data = (ushort)SetBit(data, 00, true);
            if (SimulationMode == false)
                Device.WriteControlBits(data);
        }
        //[FAAttribute("Operation")]
        //public void WriteSpeed(double speed) //0~60Hz
        //{
        //    if (SimulationMode == false)
        //        Device.WriteSpeed(speed);
        //}
    }
}
