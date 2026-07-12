using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Diagnostics;
using System.Windows;

namespace FALibrary.Device.Inverter
{
    public class FAMitsubishiInverterDevice : FADevice//RS232Device.FASerialPortDevice
    {
        private Stopwatch _commandSendWatch = new Stopwatch();

        //---------------------------------------------------
        modbus mb = new modbus();
        public UInt16 Address { get; protected set; }

        //---------------------------------------------------
        public ushort usInvertorMonitorBits;
        public ushort usRead_SetSpeed;
        public ushort usRead_GetSpeed;
        //---------------------------------------------------
        //public ushort[] SystemStatusValues      = new ushort[15]; //40009~14
        //public ushort[] RealTimeMonitorValues   = new ushort[25]; //40201~25
        //public ushort[] BasicParameterValues    = new ushort[10]; //41000~09
        //---------------------------------------------------
        
        //public ushort RunCommandBits { get; set; }
        //public ushort StatusMonitorBits { get; set; }
        //---------------------------------------------------

        public DateTime LastReadTime { get; set; }

        private int nStep = 0;
        private ushort WriteSpeed_data;
        private ushort oldWriteSpeed_data;

        private ushort WriteControlBits_data;
        private ushort oldWriteControlBits_data;
        public bool bSaveFlag { get; private set; }

        bool _threadStop = false;

        public FAMitsubishiInverterDevice()
        {
            Address = 1;
            LastReadTime = DateTime.Now;
        }

        public override void LoadParameters(System.Xml.Linq.XElement xml)
        {
            try
            {
                base.LoadParameters(xml);

                if (xml.Element("PortName") != null)
                    mb.Port.PortName = xml.Element("PortName").Value.ToString().Trim();
                else
                    throw new Exception("PortName is not exist. " + "DeviceName : " + Name);

                if (xml.Element("BaudRate") != null)
                {
                    int temp;
                    if (int.TryParse(xml.Element("BaudRate").Value.ToString(), out temp))
                        mb.Port.BaudRate = temp;
                    else
                        throw new Exception("BaudRate is not digit");
                }
                else
                    throw new Exception("BaudRate is not exit. " + "DeviceName : " + Name);

                if (xml.Element("Parity") != null)
                    mb.Port.Parity = (Parity)Enum.Parse(Parity.Even.GetType(),
                        xml.Element("Parity").Value.ToString().Trim());

                if (xml.Element("StopBits") != null)
                    mb.Port.StopBits = (StopBits)Enum.Parse(StopBits.None.GetType(),
                        xml.Element("StopBits").Value.Trim());

                if (xml.Element("Address") != null)
                {
                    UInt16 temp;
                    if (UInt16.TryParse(xml.Element("Address").Value.ToString().Trim(), out temp))
                        Address = temp;
                    else
                        throw new Exception("Address is not digit");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            //Open(); //jbpark_임시코드
        }

        public override void Open()
        {
            //if (Slave == false)
            {
                bool bRet = mb.Open(mb.Port.PortName, mb.Port.BaudRate, 
                                    mb.Port.DataBits, mb.Port.Parity, mb.Port.StopBits);
                if (bRet == false)
                {
                    System.Diagnostics.Trace.WriteLine($"{mb.Port.PortName} 시리얼 포트 Open Failed~! ");
                    // MessageBox.Show("시리얼 포트를 확인해주세요!!");
                }
                else
                {
                    // MessageBox.Show("시리얼 포트가 정상연결되었습니다", mb.Port.PortName);
                }

            }

            _commandSendWatch.Start();
            System.Threading.Thread.Sleep(100);

            System.Threading.Thread thread = new System.Threading.Thread(
                delegate (object obj)
                {
                    bool bRet = true;

                    while (_threadStop == false)
                    {
                        try
                        {
                            if (oldWriteSpeed_data != WriteSpeed_data)
                            {
                                oldWriteSpeed_data = WriteSpeed_data;
                                bRet = mb.WriteCommand_A(0xED, WriteSpeed_data);//설정주파수(RAM)
                                //bRet = mb.WriteCommand_A(0xEE, data);         //설정주파수(RAM, EEPROM)
                                System.Threading.Thread.Sleep(10);
                                bSaveFlag = true;
                            }

                            if (oldWriteControlBits_data != WriteControlBits_data)
                            {
                                oldWriteControlBits_data = WriteControlBits_data;

                                bRet = mb.WriteCommand_A(0xF9, WriteControlBits_data); //16bit
                                //bRet = mb.WriteCommand_A1(0xFA, (byte)WriteControlBits_data);//8bit

                                System.Threading.Thread.Sleep(10);
                                bSaveFlag = true;
                            }

                            if (bSaveFlag)
                            {
                                bSaveFlag = false;
                                _commandSendWatch.Restart();
                            }

                            if (_commandSendWatch.ElapsedMilliseconds >= 500)
                            {
                                int iData = 0;
                                switch (nStep)
                                {
                                    case 0:
                                        bRet = mb.ReadCommand_B(0x79, out iData); //16bit
                                        //bRet = mb.ReadCommand_B(0x7A, out InvertorMonitorBits); //8bit
                                        usInvertorMonitorBits = (ushort)iData;
                                        nStep++;
                                        break;
                                    case 1:
                                        bRet = mb.ReadCommand_B(0x6D, out iData);   //설정주파수(RAM)
                                        //bRet = mb.ReadCommand_B(0x6E, out Read_SetSpeed); //설정주파수(EEPROM)
                                        usRead_SetSpeed = (ushort)iData;
                                        nStep++;
                                        break;
                                    case 2:
                                        mb.ReadCommand_B(0x6F, out iData); //출력주파수 읽기
                                        usRead_GetSpeed = (ushort)iData;
                                        nStep = 0;
                                        break;
                                    default:
                                        nStep = 0;
                                        break;
                                }
                                _commandSendWatch.Restart();
                            }
                        }
                        catch
                        {
                        }

                        if (bRet == false)
                            System.Threading.Thread.Sleep(3000);
                        else
                            System.Threading.Thread.Sleep(30);
                    }
                });

            thread.Start();

        }
        public override void Close()
        {
            _threadStop = true;

            //if (Slave == false)
                mb.Close();
        }

        public override void ReadWrite()
        {
        }

        public bool WriteSpeed(double data)
        {
            // 0~60Hz (0~6000)
            WriteSpeed_data = (ushort)(data * 100);
            
            return true;
        }

        public bool WriteControlBits(ushort data)
        {
            // 인버터 제어 입력 명령 16bit
            WriteControlBits_data = data;
            
            return true;
        }
    }
}
