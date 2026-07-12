using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows;
using System.IO;

namespace FALibrary.Device.Inverter
{
    public class modbus
    {
        private readonly Byte STX = 0x02;
        private readonly Byte ETX = 0x03;
        private readonly Byte ENQ = 0x05;
        private readonly Byte ACK = 0x06;
        private readonly Byte LF = 0x0A; //(10)
        private readonly Byte CR = 0x0D; //(13);
        private readonly Byte NAK = 0x15;

        public SerialPort Port = new SerialPort();
        public string modbusStatus;

        #region Constructor / Deconstructor
        public modbus()
        {
        }
        ~modbus()
        {
        }
        #endregion

        #region Open / Close Procedures
        public bool Open(string portName, int baudRate, int databits, Parity parity, StopBits stopBits)
        {
            //Ensure port isn't already opened:
            if (!Port.IsOpen)
            {
                //Assign desired settings to the serial port:
                Port.PortName = portName;
                Port.BaudRate = baudRate;
                Port.DataBits = databits;
                Port.Parity = parity;
                Port.StopBits = stopBits;
                //These timeouts are default and cannot be editted through the class at this point:
                Port.ReadTimeout = 1000;
                Port.WriteTimeout = 1000;
                Port.NewLine = "\r";//"\r\n"

                try
                {
                    Port.Open();
                }
                catch (Exception err)
                {
                    modbusStatus = "Error opening " + portName + ": " + err.Message;
                    InverterLogWrite(modbusStatus);
                    return false;
                }
                modbusStatus = portName + " opened successfully";
                InverterLogWrite(modbusStatus);
                return true;
            }
            else
            {
                modbusStatus = portName + " already opened";
                InverterLogWrite(modbusStatus);
                return false;
            }
        }
        public bool Close()
        {
            //Ensure port is opened before attempting to close:
            if (Port.IsOpen)
            {
                try
                {
                    Port.Close();
                }
                catch (Exception err)
                {
                    modbusStatus = "Error closing " + Port.PortName + ": " + err.Message;
                    InverterLogWrite(modbusStatus);
                    return false;
                }
                modbusStatus = Port.PortName + " closed successfully";
                InverterLogWrite(modbusStatus);
                return true;
            }
            else
            {
                modbusStatus = Port.PortName + " is not open";
                return false;
            }
        }
        #endregion

        #region CRC Computation
        private void GetCRC(byte[] message, ref byte[] CRC)
        {
            //Function expects a modbus message of any length as well as a 2 byte CRC array in which to 
            //return the CRC values:

            ushort CRCFull = 0xFFFF;
            byte CRCHigh = 0xFF, CRCLow = 0xFF;
            char CRCLSB;

            for (int i = 0; i < (message.Length) - 2; i++)
            {
                CRCFull = (ushort)(CRCFull ^ message[i]);

                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);

                    if (CRCLSB == 1)
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                }
            }
            CRC[1] = CRCHigh = (byte)((CRCFull >> 8) & 0xFF);
            CRC[0] = CRCLow = (byte)(CRCFull & 0xFF);
        }
        #endregion

        #region Build Message
        private void BuildMessage(byte address, byte type, ushort start, ushort registers, ref byte[] message)
        {
            //Array to receive CRC bytes:
            byte[] CRC = new byte[2];

            message[0] = address;
            message[1] = type;
            message[2] = (byte)(start >> 8);
            message[3] = (byte)start;
            message[4] = (byte)(registers >> 8);
            message[5] = (byte)registers;

            GetCRC(message, ref CRC);
            message[message.Length - 2] = CRC[0];
            message[message.Length - 1] = CRC[1];
        }
        #endregion

        #region Check Response
        private bool CheckResponse(byte[] response)
        {
            //Perform a basic CRC check:
            byte[] CRC = new byte[2];
            GetCRC(response, ref CRC);
            if (CRC[0] == response[response.Length - 2] && CRC[1] == response[response.Length - 1])
                return true;
            else
                return false;
        }
        #endregion

        #region Get Response
        private void GetResponse(ref byte[] response)
         {
            //There is a bug in .Net 2.0 DataReceived Event that prevents people from using this
            //event as an interrupt to handle data (it doesn't fire all of the time).  Therefore
            //we have to use the ReadByte command for a fixed length as it's been shown to be reliable.
            int len = Port.BytesToRead;
            for (int i = 0; i < response.Length; i++)
            {
                len = Port.BytesToRead;
                if(len <= 0)
                {
                    ;
                }

                response[i] = (byte)(Port.ReadByte());
            }
        }
        #endregion

        #region Function 16 - Write Multiple Registers
        public bool SendFc16(byte address, ushort start, ushort registers, short[] values)
        {
            //Ensure port is open:
            if (Port.IsOpen)
            {
                //Clear in/out buffers:
                Port.DiscardOutBuffer();
                Port.DiscardInBuffer();
                //Message is 1 addr + 1 fcn + 2 start + 2 reg + 1 count + 2 * reg vals + 2 CRC
                byte[] message = new byte[9 + 2 * registers];
                //Function 16 response is fixed at 8 bytes
                byte[] response = new byte[8];

                //Add bytecount to message:
                message[6] = (byte)(registers * 2);
                //Put write values into message prior to sending:
                for (int i = 0; i < registers; i++)
                {
                    message[7 + 2 * i] = (byte)(values[i] >> 8);
                    message[8 + 2 * i] = (byte)(values[i]);
                }
                //Build outgoing message:
                BuildMessage(address, (byte)16, start, registers, ref message);

                //Send Modbus message to Serial Port:
                try
                {
                    Port.Write(message, 0, message.Length);
                    GetResponse(ref response);
                }
                catch (Exception err)
                {
                    modbusStatus = "Error in write event: " + err.Message;
                    return false;
                }
                //Evaluate message:
                if (CheckResponse(response))
                {
                    modbusStatus = "Write successful";
                    return true;
                }
                else
                {
                    modbusStatus = "CRC error";
                    return false;
                }
            }
            else
            {
                modbusStatus = "Serial port not open";
                return false;
            }
        }
        #endregion


        #region Function 6 - Write Registers
        public bool SendFc6(byte address, ushort start, ushort registers)
        {
            ushort len = 1;
            //Ensure port is open:
            if (Port.IsOpen)
            {
                //Clear in/out buffers:
                Port.DiscardOutBuffer();
                Port.DiscardInBuffer();
                //Message is 1 addr + 1 fcn + 2 start + 2 reg + 1 count + 2 * reg vals + 2 CRC
                byte[] message = new byte[7 + 2 * len];
                //Function 6 response is fixed at 8 bytes
                byte[] response = new byte[8];

                //Build outgoing message:
                BuildMessage(address, (byte)6, start, registers, ref message);

                //Send Modbus message to Serial Port:
                try
                {
                    Port.Write(message, 0, message.Length);
                    GetResponse(ref response);
                }
                catch (Exception err)
                {
                    modbusStatus = "Error in write event: " + err.Message;
                    return false;
                }
                //Evaluate message:
                if (CheckResponse(response))
                {
                    modbusStatus = "Write successful";

                    //MessageBox.Show("시리얼 포트가 정상연결되었습니다");
                    return true;
                }
                else
                {
                    modbusStatus = "CRC error";
                    return false;
                }
            }
            else
            {
                modbusStatus = "Serial port not open";
                //MessageBox.Show("시리얼 포트가 정상연결 안되었습니다");
                return false;
            }
        }
        #endregion

        #region Function 3 - Read Registers
        public bool SendFc3(byte address, ushort start, ushort registers, ref ushort[] values)
        {
            //Ensure port is open:
            if (Port.IsOpen)
            {
                //Clear in/out buffers:
                Port.DiscardOutBuffer();
                Port.DiscardInBuffer();
                //Function 3 request is always 8 bytes:
                byte[] message = new byte[8];
                //Function 3 response buffer:
                byte[] response = new byte[5 + 2 * registers];
                //Build outgoing modbus message:
                BuildMessage(address, (byte)3, start, registers, ref message);
                //Send modbus message to Serial Port:
                try
                {
                    Port.Write(message, 0, message.Length);
                    GetResponse(ref response);
                }
                catch (Exception err)
                {
                    modbusStatus = "Error in read event: " + err.Message;
                    //System.Threading.Thread.Sleep(500);
                    return false;
                }
                //Evaluate message:
                if (CheckResponse(response))
                {
                    //Return requested register values:
                    for (int i = 0; i < (response.Length - 5) / 2; i++)
                    {
                        values[i] = response[2 * i + 3];
                        values[i] <<= 8;
                        values[i] += response[2 * i + 4];
                    }
                    modbusStatus = "Read successful";
                    return true;
                }
                else
                {
                    modbusStatus = "CRC error";
                    return false;
                }
            }
            else
            {
                modbusStatus = "Serial port not open";
                return false;
            }

        }
        #endregion

        //--------------------------------
        // Mitsubishi Inverter용 프로토콜 rs485
        public bool ReadCommand_B(byte nCmdCode, out int data) // 0x79(16bit),0x7A(8bit)
        {
            data = 0;
            if (Port.IsOpen)
            {
                //Clear in/out buffers:
                Port.DiscardOutBuffer();
                Port.DiscardInBuffer();

                byte[] command = { ENQ, 0x30, 0x31, 0x00, 0x00, 0x31, 0x00, 0x00, CR };
                int Address = 01;
                command[01] = System.Text.ASCIIEncoding.ASCII.GetBytes((Address >> 8).ToString())[0];
                command[02] = System.Text.ASCIIEncoding.ASCII.GetBytes((Address & 0xFF).ToString())[0];

                string szCmdCode = string.Format("{0:X2}", nCmdCode);
                command[03] = System.Text.ASCIIEncoding.ASCII.GetBytes(szCmdCode)[0];
                command[04] = System.Text.ASCIIEncoding.ASCII.GetBytes(szCmdCode)[1];

                int nWaitTime = 1;
                command[05] = System.Text.ASCIIEncoding.ASCII.GetBytes(nWaitTime.ToString())[0];

                var chksum = ChecksumBytes(command, 1, 5);
                command[06] = System.Text.ASCIIEncoding.ASCII.GetBytes((chksum[0] >> 4).ToString("X"))[0];
                command[07] = System.Text.ASCIIEncoding.ASCII.GetBytes((chksum[0] & 0xF).ToString("X"))[0];
                command[08] = CR;
                //------------------------------------------------------------------------------------------------------
                //string szCommand = Encoding.Default.GetString(command); //Log출력 사용
                string szCommand = Encoding.Default.GetString(command);
                //InverterLogWrite("Inverter Command (len=" + command.Length.ToString("D2") + ") Send=" + szCommand);
                //------------------------------------------------------------------------------------------------------
                //byte[] response = new byte[200];
                string szRead = "";
                try
                {
                    Port.Write(command, 0, command.Length);
                    szRead = Port.ReadLine();
                    byte[] response = Encoding.ASCII.GetBytes(szRead);
                    //System.Threading.Thread.Sleep(50);
                    //int len = Port.BytesToRead;
                    //Port.Read(response, 0, len);
                    //string szRead = Encoding.Default.GetString(response);

                    if (response[0] == STX)
                    {
                        int iETX_POS = 0;
                        for (int i = 0; i < response.Length; i++)
                        {
                            if (response[i] == ETX)
                            {
                                iETX_POS = i + 1;
                                break;
                            }
                        }

                        if (iETX_POS == 6)
                        {
                            //data = response[3];
                            //data = (ushort)((ushort)(data << 8) | (ushort)(response[4]));
                            //data = (ushort)(response[3] | response[4] << 8);
                            byte[] byteData = { 0x00, 0x00 };
                            byteData[0] = response[3];
                            byteData[1] = response[4];
                            string szData = Encoding.Default.GetString(byteData);
                            data = Convert.ToInt32(szData, 16);
                            InverterLogWrite("[STX] 16bit Data Response OK");
                        }
                        else if (iETX_POS == 8)
                        {
                            //int iData = (int)(response[3] | (response[4] << 8) | (response[5] << 16) | (response[6] << 24));
                            byte[] byteData = { 0x00, 0x00, 0x00, 0x00 };
                            byteData[0] = response[3];
                            byteData[1] = response[4];
                            byteData[2] = response[5];
                            byteData[3] = response[6];
                            string szData = Encoding.Default.GetString(byteData);
                            data = Convert.ToInt32(szData, 16);
                            InverterLogWrite("[STX] 32bit Data Response OK");
                        }
                        else
                        {
                            data = 0;
                            InverterLogWrite("[STX] Size Error Data Response OK  Len=" + response.Length.ToString());
                            InverterLogWrite("Read data=" + szRead);
                            InverterLogWrite("Read data=" + str2hex(szRead));
                        }

                        //인버터 측에 Ack전송
                        byte[] AckCmd = { ACK, 0x30, 0x31, CR };
                        AckCmd[01] = System.Text.ASCIIEncoding.ASCII.GetBytes((Address >> 8).ToString())[0];
                        AckCmd[02] = System.Text.ASCIIEncoding.ASCII.GetBytes((Address & 0xFF).ToString())[0];
                        AckCmd[03] = CR;
                        Port.Write(AckCmd, 0, AckCmd.Length);
                    }
                    else if (response[0] == NAK)
                    {
                        int iErrorCode = response[3];
                        InverterLogWrite("[NAK] ErrorCode=" + iErrorCode.ToString());
                        InverterLogWrite("Read data=" + szRead);
                        InverterLogWrite("Read data=" + str2hex(szRead));
                    }
                    else
                    {
                        InverterLogWrite("[ERR] Response=" + szRead);
                        InverterLogWrite("Read data=" + szRead);
                        InverterLogWrite("Read data=" + str2hex(szRead));
                    }
                    //string szRead = Port.ReadLine();
                    //string szRead = Port.ReadByte().ToString();
                    //GetResponse(ref response);
                }
                catch (Exception err)
                {
                    modbusStatus = "Error in read event: " + err.Message;
                    InverterLogWrite("[ERR] " + modbusStatus);
                    InverterLogWrite(err.ToString());
                    //System.Threading.Thread.Sleep(100);
                    InverterLogWrite("Read data=" + szRead);
                    InverterLogWrite("Read data=" + str2hex(szRead));
                    return false;
                }
            }

            return true;
        }

        public bool WriteCommand_A(byte nCmdCode = 0xF9, ushort nData = 0x0000)
        {
            if (Port.IsOpen)
            {
                //Clear in/out buffers:
                Port.DiscardOutBuffer();
                Port.DiscardInBuffer();

                UInt16 Address = 0x01;
                byte[] command = { ENQ, 0x30, 0x31, 0x00, 0x00, 0x31, 0x00, 0x01, 0x02, 0x03, 0x00, 0x00, CR };
                command[01] = System.Text.ASCIIEncoding.ASCII.GetBytes((Address >> 8).ToString())[0];
                command[02] = System.Text.ASCIIEncoding.ASCII.GetBytes((Address & 0xFF).ToString())[0];

                string szCmdCode = string.Format("{0:X2}", nCmdCode);
                command[03] = System.Text.ASCIIEncoding.ASCII.GetBytes(szCmdCode)[0];
                command[04] = System.Text.ASCIIEncoding.ASCII.GetBytes(szCmdCode)[1];

                int nWaitTime = 1;
                command[05] = System.Text.ASCIIEncoding.ASCII.GetBytes(nWaitTime.ToString())[0];

                string szData = string.Format("{0:X4}", nData);
                command[06] = System.Text.ASCIIEncoding.ASCII.GetBytes(szData)[0];
                command[07] = System.Text.ASCIIEncoding.ASCII.GetBytes(szData)[1];
                command[08] = System.Text.ASCIIEncoding.ASCII.GetBytes(szData)[2];
                command[09] = System.Text.ASCIIEncoding.ASCII.GetBytes(szData)[3];

                var chksum = ChecksumBytes(command, 1, 9);
                command[10] = System.Text.ASCIIEncoding.ASCII.GetBytes((chksum[0] >> 4).ToString("X"))[0];
                command[11] = System.Text.ASCIIEncoding.ASCII.GetBytes((chksum[0] & 0xF).ToString("X"))[0];
                command[12] = CR;
                //------------------------------------------------------------------------------------------------------
                //string szCommand = Encoding.Default.GetString(command); //Log출력 사용
                string szCommand = Encoding.Default.GetString(command);
                //InverterLogWrite("Inverter Command (len=" + command.Length.ToString("D2") +") Send=" + szCommand);
                //------------------------------------------------------------------------------------------------------
                //_sharedPorts.AddCommand(Port.PortName, command);
                //byte[] response = new byte[200];
                string szRead = "";
                try
                {
                    Port.Write(command, 0, command.Length);
                    szRead = Port.ReadLine();
                    byte[] response = Encoding.ASCII.GetBytes(szRead);
                    //System.Threading.Thread.Sleep(50);
                    //int len = Port.BytesToRead;
                    //Port.Read(response, 0, len);
                    //string szRead = Encoding.Default.GetString(response);

                    if (response[0] == ACK)
                    {
                        InverterLogWrite("[ACK] Response OK");
                    }
                    else if (response[0] == NAK)
                    {
                        int iErrorCode = response[3];
                        InverterLogWrite("[NAK] ErrorCode=" + iErrorCode.ToString());
                        InverterLogWrite("Read data=" + szRead);
                        InverterLogWrite("Read data=" + str2hex(szRead));
                    }
                    else
                    {
                        InverterLogWrite("[ERR] Response=" + szRead);
                        InverterLogWrite("Read data=" + szRead);
                        InverterLogWrite("Read data=" + str2hex(szRead));
                    }
                    //string szRead = Port.ReadLine();
                    //string szRead = Port.ReadByte().ToString();
                    //GetResponse(ref response);
                }
                catch (Exception err)
                {
                    modbusStatus = "Error in wirte event: " + err.Message;
                    InverterLogWrite("[ERR] " + modbusStatus);
                    InverterLogWrite(err.ToString());
                    //System.Threading.Thread.Sleep(100);
                    InverterLogWrite("Read data=" + szRead);
                    InverterLogWrite("Read data=" + str2hex(szRead));
                    return false;
                }
            }
            return true;
        }


        public bool WriteCommand_A1(byte nCmdCode = 0xFA, byte nData = 0x00)
        {
            if (Port.IsOpen)
            {
                //Clear in/out buffers:
                Port.DiscardOutBuffer();
                Port.DiscardInBuffer();

                UInt16 Address = 0x01;
                byte[] command = { ENQ, 0x30, 0x31, 0x00, 0x00, 0x31, 0x00, 0x01, 0x00, 0x00, CR };
                command[01] = System.Text.ASCIIEncoding.ASCII.GetBytes((Address >> 8).ToString())[0];
                command[02] = System.Text.ASCIIEncoding.ASCII.GetBytes((Address & 0xFF).ToString())[0];

                string szCmdCode = string.Format("{0:X2}", nCmdCode);
                command[03] = System.Text.ASCIIEncoding.ASCII.GetBytes(szCmdCode)[0];
                command[04] = System.Text.ASCIIEncoding.ASCII.GetBytes(szCmdCode)[1];

                int nWaitTime = 1;
                command[05] = System.Text.ASCIIEncoding.ASCII.GetBytes(nWaitTime.ToString())[0];

                string szData = string.Format("{0:X2}", nData);
                command[06] = System.Text.ASCIIEncoding.ASCII.GetBytes(szData)[0];
                command[07] = System.Text.ASCIIEncoding.ASCII.GetBytes(szData)[1];

                var chksum = ChecksumBytes(command, 1, 7);
                command[8] = System.Text.ASCIIEncoding.ASCII.GetBytes((chksum[0] >> 4).ToString("X"))[0];
                command[9] = System.Text.ASCIIEncoding.ASCII.GetBytes((chksum[0] & 0xF).ToString("X"))[0];
                command[10] = CR;
                //------------------------------------------------------------------------------------------------------
                //string szCommand = Encoding.Default.GetString(command); //Log출력 사용
                string szCommand = Encoding.Default.GetString(command);
                //InverterLogWrite("Inverter Command (len=" + command.Length.ToString("D2") +") Send=" + szCommand);
                //------------------------------------------------------------------------------------------------------
                //_sharedPorts.AddCommand(Port.PortName, command);
                //byte[] response = new byte[200];
                string szRead = "";
                try
                {
                    Port.Write(command, 0, command.Length);
                    szRead = Port.ReadLine();
                    byte[] response = Encoding.ASCII.GetBytes(szRead);
                    //System.Threading.Thread.Sleep(50);
                    //int len = Port.BytesToRead;
                    //Port.Read(response, 0, len);
                    //string szRead = Encoding.Default.GetString(response);
                    if (response[0] == ACK)
                    {
                        InverterLogWrite("[ACK] Response OK");
                    }
                    else if (response[0] == NAK)
                    {
                        int iErrorCode = response[3];
                        InverterLogWrite("[NAK] ErrorCode=" + iErrorCode.ToString());
                        InverterLogWrite("Read data=" + szRead);
                        InverterLogWrite("Read data=" + str2hex(szRead));
                    }
                    else
                    {
                        InverterLogWrite("[ERR] Response=" + szRead);
                        InverterLogWrite("Read data=" + szRead);
                        InverterLogWrite("Read data=" + str2hex(szRead));
                    }
                    //string szRead = Port.ReadLine();
                    //string szRead = Port.ReadByte().ToString();
                    //GetResponse(ref response);
                }
                catch (Exception err)
                {
                    modbusStatus = "Error in wirte event: " + err.Message;
                    InverterLogWrite("[ERR] " + modbusStatus);
                    InverterLogWrite(err.ToString());
                    //System.Threading.Thread.Sleep(100);
                    InverterLogWrite("Read data=" + szRead);
                    InverterLogWrite("Read data=" + str2hex(szRead));
                    return false;
                }
            }
            return true;
        }

        public byte[] ChecksumBytes(byte[] bytes, int offset, int length)
        {
            ushort checksum = 0;
            for (int i = offset; i < offset + length; i++)
            {
                checksum += bytes[i];
            }
            return BitConverter.GetBytes(checksum);
        }
        //--------------------------------
        public string str2hex(string strData)
        {
            string resultHex = string.Empty;
            byte[] arr_byteStr = Encoding.Default.GetBytes(strData);

            foreach (byte byteStr in arr_byteStr)
                resultHex += string.Format("{0:x2} ", byteStr);

            return resultHex;
        }
        private void InverterLogWrite(string message)
        {
            string dir;
            string path;
            string wMessage;

            DateTime dt = DateTime.Now;
            StreamWriter sw;

            dir = string.Format(@"{0}\InverterLog\{1:0000}\{2:00}\{3:00}\", ".\\LOG", dt.Year, dt.Month, dt.Day); //DefaultInfo.PATH

            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }

            path = string.Format(@"{0}{1}_{2:0000}{3:00}{4:00}{5:00}.log",
                                dir, "InverterLog", dt.Year, dt.Month, dt.Day, dt.Hour);

            sw = new StreamWriter(path, true, System.Text.Encoding.UTF8);

            wMessage = string.Format("{0:0000}/{1:00}/{2:00} {3:00}:{4:00}:{5:00}.{6:000}\t{7}\r\n",
                                        dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond, message);

            try
            {
                sw.Write(wMessage);
                sw.Flush();
            }
            /*
                        catch (Exception e)
                        {
                            //MessageBox.Show(e.StackTrace);
                        }
            */
            finally
            {
                sw.Close();
            }
        }
    }
}
