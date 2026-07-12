using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Device.MemoryBaseDevice;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace FALibrary.Device.Hilscher
{
    public class FACIF50DNM : FAMemoryBaseDevice
    {
        private class CIF50DNM
        {
            public const int MAX_DEV_BOARDS = 4;
            //  maximum numbers of boards
            //  ====================================================================================
            //   driver errors
            //  ====================================================================================
            public const int DRV_NO_ERROR = 0;
            //  no error
            public const int DRV_BOARD_NOT_INITIALIZED = -1;
            //  DRIVER board not initialized
            public const int DRV_INIT_STATE_ERROR = -2;
            //  DRIVER error in internal init state
            public const int DRV_READ_STATE_ERROR = -3;
            //  DRIVER error in internal read state
            public const int DRV_CMD_ACTIVE = -4;
            //  DRIVER command on this chanal is activ
            public const int DRV_PARAMETER_UNKNOWN = -5;
            //  DRIVER unknown parameter in function occured
            public const int DRV_DEV_DPM_ACCESS_ERROR = -10;
            //  DEVICE dual port ram not accessable
            public const int DRV_DEV_NOT_READY = -11;
            //  DEVICE not ready (ready flag failed)
            public const int DRV_DEV_NOT_RUNNING = -12;
            //  DEVICE not running (running flag failed)
            public const int DRV_DEV_WATCHDOG_FAILED = -13;
            //  DEVICE watch dog test failed
            public const int DRV_DEV_OS_VERSION_ERROR = -14;
            //  DEVICE signals wrong OS version
            public const int DRV_DEV_SYSERR = -15;
            //  DEVICE error in dual port flags
            public const int DRV_DEV_MAILBOX_FULL = -16;
            //  DEVICE send mailbox is full
            public const int DRV_DEV_PUT_TIMEOUT = -17;
            //  DEVICE PutMessage timeout
            public const int DRV_DEV_GET_TIMEOUT = -18;
            //  DEVICE GetMessage timeout
            public const int DRV_DEV_GET_NO_MESSAGE = -19;
            //  DEVICE no message available
            public const int DRV_DEV_RESET_TIMEOUT = -20;
            //  DEVICE RESET command timeout
            public const int DRV_DEV_NO_COM_FLAG = -21;
            //  DEVICE COM=flag not set
            public const int DRV_DEV_EXCHANGE_FAILED = -22;
            //  DEVICE IO data exchange failed
            public const int DRV_DEV_EXCHANGE_TIMEOUT = -23;
            //  DEVICE IO data exchange timeout
            public const int DRV_DEV_COM_MODE_UNKNOWN = -24;
            //  DEVICE IO data mode unknown
            public const int DRV_DEV_FUNCTION_FAILED = -25;
            //  DEVICE Function call failed
            public const int DRV_DEV_DPMSIZE_MISMATCH = -26;
            //  DEVICE DPM size differs from configuration
            public const int DRV_DEV_STATE_MODE_UNKNOWN = -27;
            //  DEVICE COM state mode unknown
            //  Error from Interface functions
            public const int DRV_USR_OPEN_ERROR = -30;
            //  USER driver not opened
            public const int DRV_USR_INIT_DRV_ERROR = -31;
            //  USER can't connect with DEV board
            public const int DRV_USR_NOT_INITIALIZED = -32;
            //  USER board not initialized
            public const int DRV_USR_COMM_ERR = -33;
            //  USER IOCTRL function faild
            public const int DRV_USR_DEV_NUMBER_INVALID = -34;
            //  USER parameter for DEV number invalid
            public const int DRV_USR_INFO_AREA_INVALID = -35;
            //  USER parameter InfoArea unknown
            public const int DRV_USR_NUMBER_INVALID = -36;
            //  USER parameter Number invalid
            public const int DRV_USR_MODE_INVALID = -37;
            //  USER parameter Mode invalid
            public const int DRV_USR_MSG_BUF_NULL_PTR = -38;
            //  USER NULL pointer assignment
            public const int DRV_USR_MSG_BUF_TOO_SHORT = -39;
            //  USER Messagebuffer too short
            public const int DRV_USR_SIZE_INVALID = -40;
            //  USER size parameter invalid
            public const int DRV_USR_SIZE_ZERO = -42;
            //  USER size parameter with zero length
            public const int DRV_USR_SIZE_TOO_LONG = -43;
            //  USER size parameter too long
            public const int DRV_USR_DEV_PTR_NULL = -44;
            //  USER device address null pointer
            public const int DRV_USR_BUF_PTR_NULL = -45;
            //  USER pointer to buffer is a null pointer
            public const int DRV_USR_SENDSIZE_TOO_LONG = -46;
            //  USER SendSize parameter too long
            public const int DRV_USR_RECVSIZE_TOO_LONG = -47;
            //  USER ReceiveSize parameter too long
            public const int DRV_USR_SENDBUF_PTR_NULL = -48;
            //  USER pointer to buffer is a null pointer
            public const int DRV_USR_RECVBUF_PTR_NULL = -49;
            //  USER pointer to buffer is a null pointer
            public const int DRV_RCS_ERROR_OFFSET = 1000;
            //  RCS error number start
            //  max. length is 288 Bytes, max message length is 255 + 8 Bytes
            //  ====================================================================================
            //   INFO structure definitions
            //  ====================================================================================
            //  DEVRESET
            public const int COLDSTART = 2;
            public const int WARMSTART = 3;
            public const int BOOTSTART = 4;
            //  DEVMBXINFO
            public const int DEVICE_MBX_EMPTY = 0;
            public const int DEVICE_MBX_FULL = 1;
            public const int HOST_MBX_EMPTY = 0;
            public const int HOST_MBX_FULL = 1;
            //  TRIGGERWATCHDOG
            public const int WATCHDOG_STOP = 0;
            public const int WATCHDOG_START = 1;
            //  GETINFO InfoArea definitions
            public const int GET_DRIVER_INFO = 1;
            public const int GET_VERSION_INFO = 2;
            public const int GET_FIRMWARE_INFO = 3;
            public const int GET_TASK_INFO = 4;
            public const int GET_RCS_INFO = 5;
            public const int GET_DEV_INFO = 6;
            public const int GET_IO_INFO = 7;
            public const int GET_IO_SEND_DATA = 8;
            //  HOST mode definition
            public const int HOST_NOT_READY = 0;
            public const int HOST_READY = 1;
            // DEVREADWRITERAW
            public const int PARAMETER_READ = 1;
            public const int PARAMETER_WRITE = 2;
            // STATE definition
            public const int STATE_ERR_NON = 0;
            public const int STATE_ERR = 1;
            public const int STATE_MODE_0 = 0;
            public const int STATE_MODE_1 = 1;
            public const int STATE_MODE_2 = 2;
            //  state information in bLastFunction
            public const int FKT_OPEN = 1;
            public const int FKT_CLOSE = 2;
            public const int FKT_READ = 3;
            public const int FKT_WRITE = 4;
            public const int FKT_IO = 5;
            //  state information in bWriteState and bReadState
            public const int STATE_IN = 1;
            public const int STATE_WAIT = 2;
            public const int STATE_OUT = 3;
            public const int STATE_IN_IRQ = 4;

    
            //  ====================================================================================
            //   funcion protostructures
            //  ====================================================================================
            [DllImport("CIF32DLL.DLL")]
            public static extern short DevOpenDriver(ushort usDevNumber);
    
            [DllImport("CIF32DLL.DLL")]
            public static extern short DevCloseDriver(ushort usDevNumber);
    
            [DllImport("CIF32DLL.DLL")]
            public static extern short DevGetBoardInfo(ushort usDevNumber, ushort usSize, IntPtr pvData);
    
            [DllImport("CIF32DLL.DLL")]
            public static extern short DevInitBoard(ushort usDevNumber, IntPtr pDevAddress);
    
            [DllImport("CIF32DLL.DLL")]
            public static extern short DevExitBoard(ushort usDevNumber);
    
            [DllImport("CIF32DLL.DLL")]
            public static extern short DevPutTaskParameter(ushort usDevNumber, ushort usNumber, ushort usSize, IntPtr pvData);
    
            [DllImport("CIF32DLL.DLL")]
            public static extern short DevReset(ushort usDevNumber, ushort usMode, System.UInt32 ulTimeout);
    
            [DllImport("CIF32DLL.DLL")]
            public static extern short DevGetTaskState(ushort usDevNumber, ushort usNumber, ushort usSize, IntPtr pvData);
    
            [DllImport("CIF32DLL.DLL")]
            public static extern short DevGetMBXState(ushort usDevNumber, byte pusDevMBXState, byte pusHostMBXState);
    
            [DllImport("CIF32DLL.DLL")]
            public static extern short DevTriggerWatchDog(ushort usDevNumber, ushort usFunction, byte pusDevWatchDog);
    
            [DllImport("CIF32DLL.DLL")]
            public static extern short DevGetInfo(ushort usDevNumber, ushort usFunction, ushort usSize, IntPtr pvData);
    
            [DllImport("CIF32DLL.DLL")]
            public static extern short DevGetTaskParameter(ushort usDevNumber, ushort usNumber, ushort usSize, IntPtr pvData);
    
            [DllImport("CIF32DLL.DLL")]
            public static extern short DevExchangeIO(ushort usDevNumber, ushort usSendOffset, ushort usSendSize, ref byte pvSendData, ushort usReceiveOffset, ushort usReceiveSize, ref byte pvReceiveData, System.UInt32 ulTimeout);
    
            [DllImport("CIF32DLL.DLL")]
            public static extern short DevReadSendData(ushort usDevNumber, ushort usOffset, ushort usSize, ref IntPtr pvData);
    
            [DllImport("CIF32DLL.DLL")]
            public static extern short DevSetHostState(ushort usDevNumber, ushort usMode, System.UInt32 ulTimeout);
    
            // Special function do not use
            [DllImport("CIF32DLL.DLL")]
            public static extern short DevExtendedData(ushort usDevNumber, ushort usMode, ushort usSize, IntPtr pvData);
    
            // Special function do not use
            [DllImport("CIF32DLL.DLL")]
            public static extern short DevGetMBXData(ushort usDevNumber, ushort usHostSize, IntPtr pvHostData, ushort usDevSize, IntPtr pvDevData);
    
            // Special function do not use
            [DllImport("CIF32DLL.DLL")]
            public static extern short DevGetBoardInfoEx(ushort usDevNumber, ushort usSize, IntPtr pvData);
    
            [DllImport("CIF32DLL.DLL")]
            public static extern short DevExchangeIOEx(ushort usDevNumber, ushort usMode, ushort usSendOffset, ushort usSendSize, IntPtr pvSendData, ushort usReceiveOffset, int usReceiveSize, IntPtr pvReceiveData, System.UInt32 ulTimeout);
    
            [DllImport("CIF32DLL.DLL")]
            public static extern short DevReadWriteDPMRaw(ushort usDevNumber, ushort usMode, ushort usOffset, ushort usSize, IntPtr pvData);
        }

        protected struct NodeInfo
        {
            public ushort MacID { get; set; }
            public ushort InputSize { get; set; }
            public ushort OutputSize { get; set; }
            public byte[] InputIO { get; set; }
            public byte[] OutputIO { get; set; }
        };

        public ushort BoardNo { get; set; }
        public ushort InputSize { get; set; }
        public ushort OutputSize { get; set; }
        private byte[] _inputArray = null;
        private byte[] _outputArray = null;
        private Dictionary<int, NodeInfo> _nodeInfoList = new Dictionary<int, NodeInfo>();
        protected Dictionary<int, NodeInfo> NodeInfoList { get { return _nodeInfoList; } }

        public override void LoadParameters(System.Xml.Linq.XElement xml)
        {
            base.LoadParameters(xml);

            try
            {
                if (xml.Element("NodeInfoList") != null)
                    LoadNodeInfo(xml.Element("NodeInfoList"));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            InputSize = GetInputSize();
            OutputSize = GetOutputSize();

            // InputSize 또는 OutputSize가 0인 경우 1을 세팅한다.
            // 0인 경우 DevExchangeIO()를 호출할 때 IndexOutOfRangeException이 발생한다.
            if (InputSize < 1)
                InputSize = 1;
            if (OutputSize < 1)
                OutputSize = 1;

            _inputArray = new byte[InputSize];
            _outputArray = new byte[OutputSize];
        }

        public override bool GetInputIOValue(int index)
        {
            int macID = index / 10000;
            int byteIndex = (index - macID * 10000) / 10;
            int bitIndex = index % 10;
            bool result = false;
            try
            {
                result = Utility.FAUtility.CheckBit(NodeInfoList[macID].InputIO[byteIndex], bitIndex);
            }
            catch (IndexOutOfRangeException e)
            {
                throw new Exception(string.Format("Index out of range exception param index={0}, \n{1}", index, e.ToString()));
            }

            return result;
        }

        public override void SetInputIOValue(int index, bool value) //Simulation에서만 사용
        {
            int macID = (ushort)(index / 10000);
            int byteIndex = (index - macID * 10000) / 10;
            int bitIndex = index % 10;
            NodeInfoList[macID].InputIO[byteIndex] =
                Utility.FAUtility.SetBit(NodeInfoList[macID].InputIO[byteIndex], bitIndex, value);
        }

        public override bool GetOutputIOValue(int index)
        {
            int macID = index / 10000;
            int byteIndex = (index - macID * 10000) / 10;
            int bitIndex = index % 10;
            if (NodeInfoList.ContainsKey(macID) == false) return false;
            if (NodeInfoList[macID].OutputIO.Length <= byteIndex) return false;

            return Utility.FAUtility.CheckBit(NodeInfoList[macID].OutputIO[byteIndex], bitIndex);
        }

        public override void SetOutputIOValue(int index, bool value)
        {
            int macID = (ushort)(index / 10000);
            int byteIndex = (index - macID * 10000) / 10;
            int bitIndex = index % 10;
            NodeInfoList[macID].OutputIO[byteIndex] =
                Utility.FAUtility.SetBit(NodeInfoList[macID].OutputIO[byteIndex], bitIndex, value);
        }

        public override void GetInputIOBytes(int index, byte[] bytes)
        {
            int moduleID = index / 10000;
            int byteIndex = (index - moduleID * 10000) / 10;
            if (NodeInfoList.ContainsKey(moduleID) == false) return;
            if (NodeInfoList[moduleID].InputIO.Length <= byteIndex) return;

            for (int i = 0; i < bytes.Length; i++)
            {
                int currentIndex = byteIndex + i;
                if (NodeInfoList[moduleID].InputIO.Length <= currentIndex) break;
                bytes[i] = NodeInfoList[moduleID].InputIO[currentIndex];
            }
        }

        public override void SetOutputIOBytes(int index, byte[] bytes)
        {
            try
            {
                int moduleID = (ushort)(index / 10000);
                int currentIndex = (index - moduleID * 10000) / 10;

                for (int i = 0; i < bytes.Length; i++)
                {
                    if (NodeInfoList[moduleID].OutputIO.Length <= currentIndex) break;
                    NodeInfoList[moduleID].OutputIO[currentIndex] = bytes[i];
                }
            }
            catch
            {
            }
        }

        public override void Open()
        {
            if (OpenDriver() == false)
                throw new Exception("Do Not Open " + Name + " OpenDriver() Fail");

            if (CIF50DNM.DevInitBoard(BoardNo, IntPtr.Zero) != (ushort)CIF50DNM.DRV_NO_ERROR)
                throw new Exception("Do Not Open " + Name + " DevInitBoard() Fail");

            if (CIF50DNM.DevSetHostState(BoardNo, CIF50DNM.HOST_READY, 0) != (ushort)CIF50DNM.DRV_NO_ERROR)
                throw new Exception("Do Not Open " + Name + " DevSetHostState() Fail");            
        }

        public override void Close()
        {
            CIF50DNM.DevExitBoard(BoardNo);
            CIF50DNM.DevCloseDriver(BoardNo);
        }

        public override void ReadWrite()
        {
            NodeInfoListToOutputArray();

            CIF50DNM.DevExchangeIO(BoardNo, 0, OutputSize, ref _outputArray[0], 0, InputSize, ref _inputArray[0], 50);

            InputArrayToNodeInfoList();
        }

        private bool OpenDriver()
        {
            for (int i = 0; i < 3; i++)
            {
                if (CIF50DNM.DevOpenDriver(BoardNo) == (ushort)CIF50DNM.DRV_NO_ERROR)
                    return true;
            }
            
            return false;            
        }

        private void LoadNodeInfo(XElement xml)
        {
            foreach (XElement item in xml.Elements())
            {
                NodeInfo nodeInfo = new NodeInfo();
                nodeInfo.MacID = ushort.Parse(item.Element("MacID").Value.Trim());
                nodeInfo.InputSize = ushort.Parse(item.Element("InputSize").Value.Trim());
                nodeInfo.OutputSize = ushort.Parse(item.Element("OutputSize").Value.Trim());
                nodeInfo.InputIO = new byte[nodeInfo.InputSize];
                nodeInfo.OutputIO = new byte[nodeInfo.OutputSize];
                NodeInfoList.Add(nodeInfo.MacID, nodeInfo);
            }
        }

        private void NodeInfoListToOutputArray()
        {
            int index = 0;

            foreach (var node in NodeInfoList)
            {
                foreach (var ioByte in node.Value.OutputIO)
                {
                    _outputArray[index] = ioByte;
                    index++;
                }
            }
        }

        private void InputArrayToNodeInfoList()
        {
            int index = 0;

            foreach (var node in NodeInfoList)
            {
                for (int i = 0; i < node.Value.InputSize; i++)
                {
                    node.Value.InputIO[i] = _inputArray[index];
                    index++;
                }
            }
        }

        private ushort GetInputSize()
        {
            ushort size = 0;
            foreach (var item in NodeInfoList)
            {
                size += item.Value.InputSize;
            }

            return size;
        }

        private ushort GetOutputSize()
        {
            ushort size = 0;
            foreach (var item in NodeInfoList)
            {
                size += item.Value.OutputSize;
            }

            return size;
        }
    }
}
