using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace FALibrary.Device.RSAutomation
{
    public class FANMCIODevice : MemoryBaseDevice.FAMemoryBaseDevice
    {
        protected struct NodeInfo
        {
            public ushort NodeID { get; set; }
            public ushort InputSize { get; set; }
            public ushort OutputSize { get; set; }
            public byte[] InputIO { get; set; }
            public byte[] OutputIO { get; set; }
        };

        public ushort BoardID { get; set; }

        private byte[] _inputArray = null;
        private byte[] _outputArray = null;
        private byte _inputByte;
        private byte _outputByte;

        private Dictionary<int, NodeInfo> _nodeInfoList = new Dictionary<int, NodeInfo>();
        protected Dictionary<int, NodeInfo> NodeInfoList { get { return _nodeInfoList; } }

        public override void Open()
        {
            FANMCCommon.Initialize(this, BoardID);
        }

        public override void Close()
        {
            FANMCCommon.Stop(this, BoardID);
        }

        public override void LoadParameters(System.Xml.Linq.XElement xml)
        {
            base.LoadParameters(xml);

            LoadNodeInfo(xml.Element("NodeInfoList"));

            var inputSize = GetInputSize();
            var outputSize = GetOutputSize();

            _inputArray = new byte[inputSize];
            _outputArray = new byte[outputSize];
        }

        public override bool GetInputIOValue(int index)
        {
            int nodeID = index / 10000;
            int byteIndex = (index - nodeID * 10000) / 10;
            int bitIndex = index % 10;
            return Utility.FAUtility.CheckBit(NodeInfoList[nodeID].InputIO[byteIndex], bitIndex);
        }

        public override void SetInputIOValue(int index, bool value) //Simulation에서만 사용
        {
            int nodeID = (ushort)(index / 10000);
            int byteIndex = (index - nodeID * 10000) / 10;
            int bitIndex = index % 10;
            NodeInfoList[nodeID].InputIO[byteIndex] =
                Utility.FAUtility.SetBit(NodeInfoList[nodeID].InputIO[byteIndex], bitIndex, value);
        }

        public override bool GetOutputIOValue(int index)
        {
            int nodeID = index / 10000;
            int byteIndex = (index - nodeID * 10000) / 10;
            int bitIndex = index % 10;
            if (NodeInfoList.ContainsKey(nodeID) == false) return false;
            if (NodeInfoList[nodeID].OutputIO.Length <= byteIndex) return false;

            return Utility.FAUtility.CheckBit(NodeInfoList[nodeID].OutputIO[byteIndex], bitIndex);
        }

        public override void SetOutputIOValue(int index, bool value)
        {
            int nodeID = (ushort)(index / 10000);
            int byteIndex = (index - nodeID * 10000) / 10;
            int bitIndex = index % 10;
            NodeInfoList[nodeID].OutputIO[byteIndex] =
                Utility.FAUtility.SetBit(NodeInfoList[nodeID].OutputIO[byteIndex], bitIndex, value);
        }

        public override void GetInputIOBytes(int index, byte[] bytes)
        {
            int nodeID = index / 10000;
            int byteIndex = (index - nodeID * 10000) / 10;
            if (NodeInfoList.ContainsKey(nodeID) == false) return;
            if (NodeInfoList[nodeID].InputIO.Length <= byteIndex) return;

            for (int i = 0; i < bytes.Length; i++)
            {
                int currentIndex = byteIndex + i;
                if (NodeInfoList[nodeID].InputIO.Length <= currentIndex) break;
                bytes[i] = NodeInfoList[nodeID].InputIO[currentIndex];
            }
        }

        public override void SetOutputIOBytes(int index, byte[] bytes)
        {
            try
            {
                int nodeID = (ushort)(index / 10000);
                int currentIndex = (index - nodeID * 10000) / 10;

                for (int i = 0; i < bytes.Length; i++)
                {
                    if (NodeInfoList[nodeID].OutputIO.Length <= currentIndex) break;
                    NodeInfoList[nodeID].OutputIO[currentIndex] = bytes[i];
                }
            }
            catch
            {
            }
        }

        public override void ReadWrite()
        {
            ReadInputIO();
            WriteOutputIO();
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

        private void LoadNodeInfo(XElement xml)
        {
            foreach (XElement item in xml.Elements())
            {
                NodeInfo nodeInfo = new NodeInfo();
                nodeInfo.NodeID = ushort.Parse(item.Element("NodeID").Value.Trim());
                nodeInfo.InputSize = ushort.Parse(item.Element("InputSize").Value.Trim());
                nodeInfo.OutputSize = ushort.Parse(item.Element("OutputSize").Value.Trim());
                nodeInfo.InputIO = new byte[nodeInfo.InputSize];
                nodeInfo.OutputIO = new byte[nodeInfo.OutputSize];
                NodeInfoList.Add(nodeInfo.NodeID, nodeInfo);
            }
        }

        private void ReadInputIO()
        {
            //NMCSDK.NMCSDKLib.MC_IO_RAW_READ(BoardID,
            //    (ushort)NMCSDK.NMCSDKLib.IOBufMode.BUF_IN,
            //    0,//0x400, //1024 offset
            //    (uint)_inputArray.Length,
            //    _inputArray);
            //int index = 0;
            //foreach (var node in NodeInfoList)
            //{
            //    for (int i = 0; i < node.Value.InputSize; i++)
            //    {
            //        //node.Value.InputIO[i] = (byte)(~_inputArray[index]);
            //        node.Value.InputIO[i] = (byte)(_inputArray[index]);
            //        index++;
            //    }
            //}

            //----------------------------------------------
            foreach (var node in NodeInfoList)
            {
                NMCSDK.NMCSDKLib.MC_IO_READ(BoardID,
                    (ushort)(node.Value.NodeID + 100),
                    (ushort)NMCSDK.NMCSDKLib.IOBufMode.BUF_IN,
                    0, //offset
                    node.Value.InputSize,
                    node.Value.InputIO);

                //for (int i = 0; i < node.Value.InputSize; i++)
                //{
                //    NMCSDK.NMCSDKLib.MC_IO_READ_BYTE(BoardID,
                //        (ushort)(node.Value.NodeID + 100),
                //        (ushort)NMCSDK.NMCSDKLib.IOBufMode.BUF_IN,
                //        (uint)i, //offset
                //        ref _inputByte);
                //    node.Value.InputIO[i] = _inputByte;
                //}
            }
            //----------------------------------------------
        }

        private void WriteOutputIO()
        {
            //int index = 0;            
            //foreach (var node in NodeInfoList)
            //{
            //    foreach (var ioByte in node.Value.OutputIO)
            //    {
            //        _outputArray[index] = ioByte;
            //        index++;
            //    }
            //}            
            //NMCSDK.NMCSDKLib.MC_IO_RAW_WRITE(BoardID,
            //    0,//0x200, //512 offset
            //    (uint)_outputArray.Length,
            //    _outputArray);

            //----------------------------------------------
            foreach (var node in NodeInfoList)
            {
                NMCSDK.NMCSDKLib.MC_IO_WRITE(BoardID,
                    (ushort)(node.Value.NodeID + 100),
                    0, //offset
                    node.Value.OutputSize,
                    node.Value.OutputIO);
            }
            //----------------------------------------------
        }
    }
}
