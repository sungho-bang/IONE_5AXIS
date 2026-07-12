using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using System.Net;
using System.Net.Sockets;

namespace FALibrary.Device.MemoryBaseDevice
{
    public class FARemoteIOServer : FAMemoryBaseDevice
    {
        protected struct NodeInfo
        {
            public ushort NodeID { get; set; }
            public ushort InputSize { get; set; }
            public ushort OutputSize { get; set; }
            public byte[] InputIO { get; set; }
            public byte[] OutputIO { get; set; }
        };

        private class TCPRemoteIOServer
        {
            private static object _inputSyncRoot = new Object();
            private static object _outputSyncRoot = new Object();

            public int HostPort { get; set; }            
            public string HostIPAddress { get; set; }
            public int InputSize { get; set; }
            public int OutputSize { get; set; }
            IPEndPoint _ipEndPoint;            

            bool _threadStop = false;            

            byte[] _inputBytes;
            byte[] _outputBytes;

            public void Initialize()
            {
                _inputBytes = new byte[InputSize];
                _outputBytes = new byte[OutputSize];
                _ipEndPoint = new System.Net.IPEndPoint(IPAddress.Parse(HostIPAddress), HostPort);
            }

            public void Start()
            {
                byte[] oldOutputs = new byte[OutputSize];

                System.Threading.Thread tcpThread = new System.Threading.Thread(
                    delegate(object obj)
                    {
                        while (_threadStop == false)
                        {
                            System.Threading.Thread.Sleep(500);

                            TcpClient client = new TcpClient();
                            client.ReceiveTimeout = 1000;
                            client.SendTimeout = 1000;

                            try
                            {
                                client.Connect(_ipEndPoint);
                            }
                            catch (Exception e)
                            {
                                Utility.Trace.WriteLine(this, "Device", e.ToString());
                                continue;
                            }

                            ReadWrite(client);
                            try
                            {
                                if (client != null)
                                    client.Close();
                            }
                            catch
                            {
                            }
                        }
                    });

                tcpThread.Start();
            }

            public void Stop()
            {
                _threadStop = true;
            }

            public void ReadWrite(out byte[] inputBytes, byte[] outputBytes)
            {
                lock (_inputSyncRoot)
                {
                    inputBytes = new byte[InputSize];
                    Array.Copy(_inputBytes, inputBytes, InputSize);
                }

                if (outputBytes != null)
                {
                    lock (_outputSyncRoot)
                    {
                        int len = outputBytes.Length;
                        if (len > _outputBytes.Length)
                            len = _outputBytes.Length;
                        Array.Copy(outputBytes, 0, _outputBytes, 0, len);
                    }
                }
            }

            private void ReadWrite(TcpClient client)
            {
                while (!_threadStop)
                {
                    try
                    {
                        lock (_outputSyncRoot)
                        {
                            client.GetStream().Write(_outputBytes, 0, _outputBytes.Length);
                        }

                        byte[] buffer = new byte[1024];
                        client.GetStream().Read(buffer, 0, buffer.Length);

                        lock (_inputSyncRoot)
                        {
                            int len = buffer.Length;
                            if (buffer.Length > _inputBytes.Length)
                                len = _inputBytes.Length;
                            Array.Copy(buffer, 0, _inputBytes, 0, len);
                        }

                        System.Threading.Thread.Sleep(1);
                    }
                    catch
                    {
                        return;
                    }
                }
            }
        }
        
        private byte[] _inputArray = null;
        private byte[] _outputArray = null;
        private Dictionary<int, NodeInfo> _nodeInfoList = new Dictionary<int, NodeInfo>();
        protected Dictionary<int, NodeInfo> NodeInfoList { get { return _nodeInfoList; } }
        TCPRemoteIOServer _remoteIOServer;
        public int HostPort { get; set; }
        public string HostIPAddress { get; set; }

        public override void Open()
        {
            base.Open();
            _remoteIOServer.Start();
        }

        public override void Close()
        {
            _remoteIOServer.Stop();
            base.Close();            
        }

        public override void LoadParameters(System.Xml.Linq.XElement xml)
        {
            base.LoadParameters(xml);

            LoadNodeInfo(xml.Element("NodeInfoList"));

            var inputSize = GetInputSize();
            var outputSize = GetOutputSize();

            _inputArray = new byte[inputSize];
            _outputArray = new byte[outputSize];

            _remoteIOServer = new TCPRemoteIOServer { InputSize = inputSize, OutputSize = outputSize, HostIPAddress = HostIPAddress, HostPort = HostPort };
            _remoteIOServer.Initialize();
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
            WriteOutputIO();
            _remoteIOServer.ReadWrite(out _inputArray, _outputArray);
            ReadInputIO();
        }

        private void ReadInputIO()
        {
            int index = 0;

            foreach (var node in NodeInfoList)
            {
                for (int i = 0; i < node.Value.InputSize; i++)
                {
                    node.Value.InputIO[i] = (byte)(_inputArray[index]);
                    index++;
                }
            }
        }

        private void WriteOutputIO()
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
    }
}
