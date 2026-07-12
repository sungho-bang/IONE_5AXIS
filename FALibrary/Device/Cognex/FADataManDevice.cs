using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Xml.Linq;
using Cognex.DataMan.SDK;
using System.Threading;
using Cognex.DataMan.Discovery;
using Cognex.DataMan.Utils;
using System.Xml;
using System.IO;

namespace FALibrary.Device.Cognex
{
    public class FADataManDevice : FADevice
    {
        public class LoadConfigEventArgs : EventArgs
        {
            public string CameraName { get; set; }
            public string FileName { get; set; }

            public LoadConfigEventArgs()
            {
            }

            public LoadConfigEventArgs(string cameraName, string fileName)
            {
                CameraName = cameraName;
                FileName = fileName;
            }
        }

        private SynchronizationContext _syncContext = null;
        private EthSystemDiscoverer _ethSystemDiscoverer = null;
        private SerSystemDiscoverer _serSystemDiscoverer = null;
        private Dictionary<string, DataManSystem> _systemList = new Dictionary<string, DataManSystem>();
        private Dictionary<string, Action<string>> _readMethodList = new Dictionary<string, Action<string>>();
        private Dictionary<string, Action<System.Drawing.Image>> _arrivedImageMethodList = new Dictionary<string, Action<System.Drawing.Image>>();

        private int _discoveredCount = 0;
        private ManualResetEvent _allDone = new ManualResetEvent(false);        
        public bool Simulation { get; set; }
        public event EventHandler OnConneded;
        public event EventHandler OnDisconnected;
        public event EventHandler<LoadConfigEventArgs> OnLoadConfig;

        public FADataManDevice()
        {
            _syncContext = System.Threading.SynchronizationContext.Current;
        }

        public override void LoadParameters(XElement xml)
        {
            base.LoadParameters(xml);

            if (xml.Element("CameraList") != null)
                LoadCameraList(xml.Element("CameraList"));
        }

        private void LoadCameraList(XElement xml)
        {
            foreach (XElement item in xml.Elements())
            {
                string name = item.Element("Name").Value.Trim();
                if (name != "")
                {
                    _systemList.Add(name, null);                    
                }
            }
        }

        public void AddReadMethod(string camaraName, Action<string> method)
        {
            if (_readMethodList.ContainsKey(camaraName) == false)
                _readMethodList.Add(camaraName, null);

             _readMethodList[camaraName] = method;
           
        }

        public void AddArrivedImageMethod(string camaraName, Action<System.Drawing.Image> method)
        {
            if (_arrivedImageMethodList.ContainsKey(camaraName) == false)
                _arrivedImageMethodList.Add(camaraName, null);

            _arrivedImageMethodList[camaraName] = method;

        }

        public override void Open()
        {
            if (Simulation) return;

            try
            {
                _ethSystemDiscoverer = new EthSystemDiscoverer();
                _serSystemDiscoverer = new SerSystemDiscoverer();
                
                _ethSystemDiscoverer.SystemDiscovered += new EthSystemDiscoverer.SystemDiscoveredHandler(OnEthSystemDiscovered);
                _serSystemDiscoverer.SystemDiscovered += new SerSystemDiscoverer.SystemDiscoveredHandler(OnSerSystemDiscovered);

                _allDone.Reset();

                _ethSystemDiscoverer.Discover();
                _serSystemDiscoverer.Discover();

                if (_allDone.WaitOne(10000) == false)
                {
                    if (_systemList.Count(x => x.Value == null) > 0)
                    {
                        string loadingFailCameras = string.Join(", ", _systemList.Where(x => x.Value == null));
                        throw new Exception(Name + " device loading fail. loading fail camera is " + loadingFailCameras);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public override void Close()
        {
            if (Simulation) return;

            try
            {
                foreach (var item in _systemList)
                {
                    item.Value.Disconnect();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool Connect(string cameraName)
        {
            if (Simulation) return true;

            try
            {
                if (_systemList.ContainsKey(cameraName))
                {
                    var system = _systemList[cameraName];
                    if (system.IsConnected == false)
                    {
                        system.Connect();
                        return true;
                    }
                    else
                        return true;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        public bool Disconnect(string cameraName)
        {
            if (Simulation) return true;

            try
            {
                if (_systemList.ContainsKey(cameraName))
                {
                    var system = _systemList[cameraName];
                    if (system.IsConnected == true)
                    {
                        system.Disconnect();
                        return true;
                    }
                    else
                        return true;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        public void LoadConfig(string cameraName, string filename)
        {
            if (Simulation) return;

            if (_systemList.ContainsKey(cameraName) == false) return;
            if (System.IO.File.Exists(filename) == false) return;

            _systemList[cameraName].BeginRestore(filename,
                delegate
                {
                    if (OnLoadConfig != null)
                        OnLoadConfig(this, new LoadConfigEventArgs(cameraName, filename));
                }, null);
        }

        public void WriteCommand(string cameraName, string command)
        {
            if (Simulation) return;

            try
            {
                if (_systemList.ContainsKey(cameraName))
                {
                    var system = _systemList[cameraName];
                    system.SendCommand(command, 50);
                }
            }
            catch
            {
            }
        }

        private void CreateSystem(object system_info)
        {
            bool searchOk = false;

            ISystemConnector connector = null;

            string systemName = "";

            if (system_info is EthSystemDiscoverer.SystemInfo)
            {
                EthSystemDiscoverer.SystemInfo eth_system_info = system_info as EthSystemDiscoverer.SystemInfo;
                if (_systemList.ContainsKey(eth_system_info.Name))
                {
                    EthSystemConnector conn = new EthSystemConnector(eth_system_info.IPAddress);

                    conn.UserName = "blank";
                    conn.Password = "";
                    connector = conn;
                    searchOk = true;
                    systemName = eth_system_info.Name;
                }
            }
            else if (system_info is SerSystemDiscoverer.SystemInfo)
            {
                SerSystemDiscoverer.SystemInfo ser_system_info = system_info as SerSystemDiscoverer.SystemInfo;
                if (_systemList.ContainsKey(ser_system_info.Name))
                {
                    SerSystemConnector conn = new SerSystemConnector(ser_system_info.PortName, ser_system_info.Baudrate);
                    connector = conn;
                    searchOk = true;
                    systemName = ser_system_info.Name;
                }
            }

            if (searchOk == false) return;

            var system = new DataManSystem(connector);

            system.SystemConnected += new SystemConnectedHandler(OnSystemConnected);
            system.SystemDisconnected += new SystemDisconnectedHandler(OnSystemDisconnected);

            system.SystemWentOnline += new SystemWentOnlineHandler(OnSystemWentOnline);
            system.SystemWentOffline += new SystemWentOfflineHandler(OnSystemWentOffline);

            // Subscribe to events that are signalled when the deveice sends auto-responses.            
            system.XmlResultArrived += new XmlResultArrivedHandler(
                delegate(object sender, XmlResultArrivedEventArgs args)
                {
                    XmlDocument doc = new XmlDocument();
                    string read_string = "";

                    doc.LoadXml(args.XmlResult);

                    foreach (XmlNode node2 in doc.DocumentElement.ChildNodes)
                    {
                        if (node2.Name.Equals("general"))
                        {
                            foreach (XmlNode node in node2.ChildNodes)
                            {
                                if (node.Name.Equals("full_string"))
                                {
                                    read_string = node.InnerText;

                                    foreach (XmlAttribute att in node.Attributes)
                                    {
                                        if (att.Name.Equals("encoding") && att.InnerText.Equals("base64") && !String.IsNullOrEmpty(node.InnerText))
                                        {
                                            byte[] code = Convert.FromBase64String(node.InnerText);
                                            read_string = System.Text.Encoding.Default.GetString(code, 0, code.Length);
                                        }
                                    }

                                    break;
                                }
                            }

                            if (_readMethodList.ContainsKey(systemName))
                            {
                                if (_readMethodList[systemName] != null)
                                    _readMethodList[systemName](read_string);
                            }
                        }
                    }
                });

            system.ImageArrived +=
                delegate(object sender, ImageArrivedEventArgs args)
                {
                    try
                    {
                        if (_arrivedImageMethodList.ContainsKey(systemName))
                        {
                            if (_arrivedImageMethodList[systemName] != null)
                                _arrivedImageMethodList[systemName](args.Image);
                        }
                    }
                    catch (Exception imageArriveException)
                    {
                        Utility.Trace.WriteLine(this, "Device", imageArriveException.Message);
                    }
                };


            system.Connect();

            _systemList[systemName] = system;

            _discoveredCount++;

            if (_systemList.Count <= _discoveredCount)
            {
                _allDone.Set();
            }
        }

        private void OnEthSystemDiscovered(EthSystemDiscoverer.SystemInfo systemInfo)
        {
            if (_systemList.Count > _discoveredCount)
                CreateSystem(systemInfo);
            //_syncContext.Post(
            //    new SendOrPostCallback(
            //        delegate
            //        {
            //            if (_systemList.Count > _discoveredCount)
            //                CreateSystem(systemInfo);
            //        }),
            //        null);
        }

        private void OnSerSystemDiscovered(SerSystemDiscoverer.SystemInfo systemInfo)
        {
            if (_systemList.Count > _discoveredCount)
                CreateSystem(systemInfo);
            //_syncContext.Post(
            //    new SendOrPostCallback(
            //        delegate
            //        {
            //            if (_systemList.Count > _discoveredCount)
            //                CreateSystem(systemInfo);
            //        }),
            //        null);
        }

        private void OnSystemConnected(object sender, EventArgs args)
        {
            _syncContext.Post(
                delegate
                {
                    if (OnConneded != null)
                        OnConneded(sender, EventArgs.Empty);
                },
                null);
        }

        private void OnSystemDisconnected(object sender, EventArgs args)
        {
            _syncContext.Post(
                delegate
                {
                    if (OnDisconnected != null)
                        OnDisconnected(sender, EventArgs.Empty);
                },
                null);
        }

        private void OnSystemWentOnline(object sender, EventArgs args)
        {
            _syncContext.Post(
                delegate
                {
                },
                null);
        }

        private void OnSystemWentOffline(object sender, EventArgs args)
        {
            _syncContext.Post(
                delegate
                {
                },
                null);
        }

        private void OnXmlResultArrived(object sender, XmlResultArrivedEventArgs args)
        {
            XmlDocument doc = new XmlDocument();
            string read_string = "";

            doc.LoadXml(args.XmlResult);

            foreach (XmlNode node2 in doc.DocumentElement.ChildNodes)
            {
                if (node2.Name.Equals("general"))
                {
                    foreach (XmlNode node in node2.ChildNodes)
                    {
                        if (node.Name.Equals("full_string"))
                        {
                            read_string = node.InnerText;

                            foreach (XmlAttribute att in node.Attributes)
                            {
                                if (att.Name.Equals("encoding") && att.InnerText.Equals("base64") && !String.IsNullOrEmpty(node.InnerText))
                                {
                                    byte[] code = Convert.FromBase64String(node.InnerText);
                                    read_string = System.Text.Encoding.Default.GetString(code, 0, code.Length);
                                }
                            }

                            break;
                        }
                    }
                }
            }
        }
    }
}
