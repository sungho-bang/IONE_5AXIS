using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Device.RS232Device;

namespace FALibrary.Part.Daincube
{
    public class DTP7PD : FAPart
    {
        private enum DTP7MOD
        {
            GET = 0x10,
            SET = 0x11
        }

        private enum DTP7SEL
        {
            LED = 0x3A,
            BUZZER = 0x3B,
            KEYPAD = 0x3D
        }

        public enum DTP7LED
        {
            LEFT_LED1 = 0x41,
            LEFT_LED2 = 0x42,
            LEFT_LED3 = 0x43,
            RIGHT_LED1 = 0x61,
            RIGHT_LED2 = 0x62,
            RIGHT_LED3 = 0x63
        }

        public enum DTP7LEDColor
        {
            OFF = 0x30,
            BLUE = 0x31,
            RED = 0x32,
            ALL = 0x33
        }

        private enum DTP7Buzzer
        {
            ON = 0x30,
            OFF = 0x31
        }

        private enum DTP7KeyStatus
        {
            NONE = 0x00,
            UP = 0x30,
            DOWN = 0x31
        }

        public enum DTP7Key
        {
            A = 0x41,
            B = 0x42,
            C = 0x43,
            D = 0x44,
            E = 0x45,
            F = 0x46,
            G = 0x47,
            H = 0x48,
            I = 0x49,
            J = 0x4A,
            K = 0x4B,
            L = 0x4C,
            UP = 0x26,
            DOWN = 0x28,
            LEFT = 0x25,
            RIGHT = 0x27,
            F1 = 0x70,
            F2 = 0x71,
            F3 = 0x72
        }

        private byte STX = 0x02;
        private byte ETX = 0x03;
        private byte DATA_RESERVED = 0x20;
        
        private Dictionary<byte, DTP7KeyStatus> _keysStatus = new Dictionary<byte, DTP7KeyStatus>();

        public event EventHandler<FAGenericEventArgs<DTP7Key>> KeyDown;
        public event EventHandler<FAGenericEventArgs<DTP7Key>> KeyPressed;
        public event EventHandler<FAGenericEventArgs<DTP7Key>> KeyUp;

        public DTP7PD()
        {
            foreach (byte value in Enum.GetValues(typeof(DTP7Key)))
            {
                _keysStatus.Add(value, DTP7KeyStatus.NONE);
            }
        }

        public FACommonSerialPortDevice Device { get; private set; }

        public override void SetDevice(Device.FADevice aDevice)
        {
            if (aDevice is FACommonSerialPortDevice)
            {
                Device = aDevice as FACommonSerialPortDevice;
                Device.DataReceived += DataReceived;
            }
            else
                throw new Exception("Device Type is not correct." + " Device Name : " + Name + " Device Type : " + aDevice.GetType().ToString());
        }

        public void OnLED(DTP7LED led, DTP7LEDColor color)
        {
            SendPacket(DTP7MOD.SET,
                DTP7SEL.LED,
                (byte)led,
                (byte)color,
                DATA_RESERVED);
        }

        public void OffLED(DTP7LED led)
        {
            SendPacket(DTP7MOD.SET,
                DTP7SEL.LED,
                (byte)led,
                (byte)DTP7LEDColor.OFF,
                DATA_RESERVED);
        }

        public void OnBuzzer()
        {
            SendPacket(DTP7MOD.SET,
                DTP7SEL.BUZZER,
                (byte)DTP7Buzzer.ON,
                DATA_RESERVED,
                DATA_RESERVED);
        }

        public void OffBuzzer()
        {
            SendPacket(DTP7MOD.SET,
                DTP7SEL.BUZZER,
                (byte)DTP7Buzzer.OFF,
                DATA_RESERVED,
                DATA_RESERVED);
        }

        private void SendPacket(DTP7MOD mod, DTP7SEL sel, byte data1, byte data2, byte data3)
        {
            byte[] crcBuf =
            {
                STX,
                (byte)mod,
                (byte)sel,
                data1,
                data2,
                data3,
            };

            var crc = Utility.Crc16.ComputeChecksumBytes(crcBuf);

            byte[] buf =
            {
                STX,
                (byte)mod,
                (byte)sel,
                data1,
                data2,
                data3,
                crc[0],
                crc[1],
                ETX,
                0
            };

            Device.SendData(buf, 0, buf.Length);
        }

        private void DataReceived(object sender, FAGenericEventArgs<byte[]> e)
        {
            var buf = e.Value;
            if (buf.Length != 8)
                return;

            if (buf[0] != STX || buf[8] != ETX)
                return;

            var crc = Utility.Crc16.ComputeChecksumBytes(buf, 0, 6);
            if (crc[0] != buf[6] || crc[1] != buf[7])
                return;

            if (buf[2] == (byte)DTP7SEL.KEYPAD)
                ParseKeypadEvent(buf);
        }

        private void ParseKeypadEvent(byte[] data)
        {            
            DTP7KeyStatus keyStatus = ByteToKeyStatus(data[3]);
            if (keyStatus == DTP7KeyStatus.NONE)
                return;

            if (_keysStatus.ContainsKey(data[4]))
            {
                var key = data[4];
                var oldValue = _keysStatus[key];
                if (oldValue != keyStatus)
                {
                    var keyEnum = (DTP7Key)Enum.ToObject(typeof(DTP7Key), key);
                    _keysStatus[key] = keyStatus;
                    if (keyStatus == DTP7KeyStatus.DOWN)
                    {
                        KeyDown(this, new FAGenericEventArgs<DTP7Key>(keyEnum));
                    }
                    else if (keyStatus == DTP7KeyStatus.UP)
                    {
                        if (oldValue == DTP7KeyStatus.DOWN)
                            KeyPressed(this, new FAGenericEventArgs<DTP7Key>(keyEnum));
                        
                        KeyUp(this, new FAGenericEventArgs<DTP7Key>(keyEnum));
                    }
                }
            }
        }

        private DTP7KeyStatus ByteToKeyStatus(byte value)
        {
            if (value == (byte)DTP7KeyStatus.DOWN)
                return DTP7KeyStatus.DOWN;
            else if (value == (byte)DTP7KeyStatus.UP)
                return DTP7KeyStatus.UP;
            else
                return DTP7KeyStatus.NONE;
        }
    }
}
