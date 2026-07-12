using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml.Linq;

namespace FALibrary.Device.MemoryBaseDevice
{
    public struct IO_DATA_CTL
    {
        public ushort DataSize;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 200)]
        public byte[] Data;
    }    

    public class FAOmronDeviceNet : FAMemoryBaseDevice
    {
        protected struct NodeInfo
        {
            public ushort MacID { get; set; }
            public ushort InputSize { get; set; }
            public ushort OutputSize { get; set; }
            public byte[] InputIO { get; set; }
            public byte[] OutputIO { get; set; }
        };

        #region "DllImport"
        [DllImport("DN3G8F7Card.dll")]
        public static extern int BusD_GetVersion(out uint version);

        [DllImport("DN3G8F7Card.dll")]
        public static extern int BusD_GetDriverVersion(uint handle, out uint version);

        [DllImport("DN3G8F7Card.dll")]
        public static extern int BusD_CheckBoard(uint deviceNo);

        [DllImport("DN3G8F7Card.dll")]
        public static extern int BusD_Open(uint deviceNo, uint moduleID, out uint handle);

        [DllImport("DN3G8F7Card.dll")]
        public static extern int BusD_Close(uint handle);

        [DllImport("DN3G8F7Card.dll")]
        public static extern int BusD_CheckHandle(uint handle);

        [DllImport("DN3G8F7Card.dll")]
        public static extern int BusD_GetIrqControl(uint handle, out byte irqReg);

        [DllImport("DN3G8F7Card.dll")]
        public static extern int BusD_SetIrqControl(uint handle, byte irqReg);

        [DllImport("DN3G8F7Card.dll")]
        public static extern int BusD_GetIrqStatus(uint handle, out byte irqStatus);

        [DllImport("DN3G8F7Card.dll")]
        public static extern int BusD_ClearIrq(uint handle, byte irqClrMask);

        [DllImport("DN3G8F7Card.dll")]
        public static extern int BusD_CmdInterrupt(uint handle);

        [DllImport("DN3G8F7Card.dll")]
        public static extern int BusD_CustomInterrupt(uint handle, uint customInt);

        [DllImport("DN3G8F7Card.dll")]
        public static extern int BusD_PcWdtInterrupt(uint handle);

        [DllImport("DN3G8F7Card.dll")]
        public static extern int BusD_Reset(uint handle);

        [DllImport("DN3G8F7Card.dll")]
        public static extern int BusD_SysGo(uint handle, uint execModule);

        [DllImport("DN3G8F7Card.dll")]
        public static extern int BusD_GetInterruptStatus(uint handle, out byte interruptStatus);

        [DllImport("DN3G8F7Card.dll")]
        public static extern int BusD_GetMemoryAddress(uint handle, out uint memAddress);

        [DllImport("DN3G8F7Card.dll")]
        public static extern int BusD_ReadMemoryByte(uint handle, uint offset, out byte data);

        [DllImport("DN3G8F7Card.dll")]
        public static extern int BusD_ReadMemoryWord(uint handle, uint offset, out ushort data);

        [DllImport("DN3G8F7Card.dll")]
        public static extern int BusD_ReadMemoryDword(uint handle, uint offset, out uint data);

        [DllImport("DN3G8F7Card.dll")]
        public static extern int BusD_ReadMemoryBlock(uint handle, uint offset, byte[] data, uint size);

        [DllImport("DN3G8F7Card.dll")]
        public static extern int BusD_WriteMemoryByte(uint handle, uint offset, byte data);

        [DllImport("DN3G8F7Card.dll")]
        public static extern int BusD_WriteMemoryWord(uint handle, uint offset, ushort data);

        [DllImport("DN3G8F7Card.dll")]
        public static extern int BusD_WriteMemoryDword(uint handle, uint offset, uint data);

        [DllImport("DN3G8F7Card.dll")]
        public static extern int BusD_WriteMemoryBlock(uint handle, uint offset, byte[] data, uint size);

        [DllImport("DN3G8F7Card.dll")]
        public static extern int BusD_GetNetworkStatus(uint handle, out ushort status);

        [DllImport("DN3G8F7Card.dll")]
        public static extern int BusD_SetNotifyThreadMessage(uint handle, uint threadID, uint msg);

        [DllImport("DN3G8F7Card.dll")]
        public static extern int BusD_SetNotifyMessage(uint handle, uint hWnd, uint msg);

        [DllImport("DN3G8F7Scanner.dll")]
        public static extern int SCAN_Open(uint device, out uint handle);

        [DllImport("DN3G8F7Scanner.dll")]
        public static extern int SCAN_Close(uint handle);

        [DllImport("DN3G8F7Scanner.dll")]
        public static extern int SCAN_Online(uint handle, ushort macID, ushort baudRate);

        [DllImport("DN3G8F7Scanner.dll")]
        public static extern int SCAN_Offline(uint Handle);

        [DllImport("DN3G8F7Scanner.dll")]
        public static extern int SCAN_GetInData(uint handle, ushort macId, ref IO_DATA_CTL InData1, IntPtr InData2);

        [DllImport("DN3G8F7Scanner.dll")]
        public static extern int SCAN_RegisterSlaveDevice(uint Handle, ushort MacId, ushort Outsize, ushort Insize);

        [DllImport("DN3G8F7Scanner.dll")]
        public static extern int SCAN_StartScan(uint Handle, int ErrStop);

        [DllImport("DN3G8F7Scanner.dll")]
        public static extern int SCAN_StopScan(uint Handle);

        [DllImport("DN3G8F7Scanner.dll")]
        public static extern int SCAN_LoadScanlist(uint Handle);

        [DllImport("DN3G8F7Scanner.dll")]
        public static extern int SCAN_GetSlaveDevice(uint Handle, ushort MacId, out ushort Outsize, out ushort Insize);

        [DllImport("DN3G8F7Scanner.dll")]
        public static extern int SCAN_GetSlaveDeviceStatus(uint Handle, ushort MacId, out ushort Status);

        [DllImport("DN3G8F7Scanner.dll")]
        public static extern int SCAN_IoRefresh(uint Handle);

        [DllImport("DN3G8F7Scanner.dll")]
        public static extern int SCAN_StoreSlaveScanlist(uint Handle);

        [DllImport("DN3G8F7Scanner.dll")]
        public static extern int SCAN_SetOutData(uint Handle, uint MacId, ref IO_DATA_CTL OutData1, IntPtr OutData2);

        [DllImport("DN3G8F7Scanner.dll")]
        public static extern int SCAN_ConnectSlaveDevice(uint Handle, ushort MacId);

        [DllImport("DN3G8F7Scanner.dll")]
        public static extern int SCAN_ClearScanlist(uint Handle);

        [DllImport("DN3G8F7Scanner.dll")]
        public static extern int SCAN_EnableScanlist(uint Handle, int Enable);

        [DllImport("DN3G8F7Scanner.dll")]
        public static extern int SCAN_ConnectMasterDevice(uint Handle, ushort ErrorOutData);

        [DllImport("DN3G8F7Scanner.dll")]
        public static extern int SCAN_StoreScanlist(uint Handle);

        [DllImport("DN3G8F7Scanner.dll")]
        public static extern int SCAN_IsScanlistSlaveDeviceRegist(uint Handle, ushort MacId);

        [DllImport("DN3G8F7Scanner.dll")]
        public static extern int SCAN_SetScanlist(uint Handle, string FilePath);
        #endregion
        
        private uint _handle;
        private IO_DATA_CTL _inputData;
        private IO_DATA_CTL _outputData;
        private Dictionary<int, NodeInfo> _nodeInfoList = new Dictionary<int, NodeInfo>();

        protected Dictionary<int, NodeInfo> NodeInfoList { get { return _nodeInfoList; } }

        public override bool GetInputIOValue(int index)
        {         
            int macID = index / 10000;
            int byteIndex = (index - macID * 10000) / 10;
            int bitIndex = index % 10;
            return Utility.FAUtility.CheckBit(NodeInfoList[macID].InputIO[byteIndex], bitIndex);
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
        }

        public override void Open()
        {
            for (int i = 0; i < 3; i++)
            {
                int result1 = SCAN_Open(0, out _handle);
                int result2 = RegisterSlaveDevice(_handle);                
                int result3 = SCAN_Online(_handle, 0, 0);
                int result4 = SCAN_StartScan(_handle, 0);

                if (result1 == 1 && result2 == 1 && result3 == 1 && result4 == 1) break;
                else if (i == 2) throw new Exception("Do Not Open " + Name);
            }

            while (true)
            {
                bool allStatusOk = true;
                ushort status;

                foreach (KeyValuePair<int, NodeInfo> item in NodeInfoList)
                {
                    SCAN_GetSlaveDeviceStatus(_handle, item.Value.MacID, out status);
                    if (Utility.FAUtility.CheckBit(status, 15) == true)
                    {
                        allStatusOk = false;
                        break;
                    }
                }

                if (allStatusOk == true) break;
                Thread.Sleep(10);
            }
            
            _inputData.Data = new byte[200];
            _outputData.Data = new byte[200];
        }
        
        public override void Close()
        {
            SCAN_StopScan(_handle);
            SCAN_Offline(_handle);
            SCAN_Close(_handle);            
        }

        public override void ReadWrite()
        {
            SCAN_IoRefresh(_handle);
            
            foreach (KeyValuePair<int, NodeInfo> nodeInfo in NodeInfoList)
            {
                _inputData.DataSize = nodeInfo.Value.InputSize;
                SCAN_GetInData(_handle, nodeInfo.Value.MacID, ref _inputData, IntPtr.Zero);

                for (int i = 0; i < nodeInfo.Value.InputSize; i++)                
                    nodeInfo.Value.InputIO[i] = _inputData.Data[i];                                    

                _outputData.DataSize = nodeInfo.Value.OutputSize;
                for (int i = 0; i < nodeInfo.Value.OutputSize; i++)
                    _outputData.Data[i] = nodeInfo.Value.OutputIO[i];

                SCAN_SetOutData(_handle, nodeInfo.Value.MacID, ref _outputData, IntPtr.Zero);                
            }            
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

        private int RegisterSlaveDevice(uint deviceHandle)
        {
            foreach (KeyValuePair<int, NodeInfo> item in _nodeInfoList)
            {
                int result = SCAN_RegisterSlaveDevice(deviceHandle,
                    item.Value.MacID,
                    item.Value.OutputSize,
                    item.Value.InputSize);

                if (result == 0) return 0;
            }

            return 1;
        }
    }
}