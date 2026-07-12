using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace FAFramework.Utility.GEM
{
    public class GEMManager
    {
        [Serializable]
        public class GemConfig
        {
            public string LogPath { get; set; }
            public short LogRetention { get; set; }
            public string FormatFilePath { get; set; }
            public short FormatCheck { get; set; }

            [Serializable]
            public class HSMSConfig
            {
                public short Port { get; set; }
                public short DeviceID { get; set; }
                public short LinkTestInterval { get; set; }
                public short Retry { get; set; }
                public short T3 { get; set; }
                public short T5 { get; set; }
                public short T6 { get; set; }
                public short T7 { get; set; }
                public short T8 { get; set; }
                public string IP { get; set; }
                public bool HostMode { get; set; }
                public short CTTime { get; set; }
            }

            [Serializable]
            public class GEMConfig
            {
                public int DefaultCommState { get; set; }
                public string DefaultControlState { get; set; }
                public int CommRequestTimeout { get; set; }
                public string MDLN { get; set; }
                public string SoftReversion { get; set; }
                public int SpoolingMode { get; set; }
                public int MaxSpoolCount { get; set; }
                public int MaxSpoolTransmit { get; set; }
                public int SpoolOverwrite { get; set; }
                public int TimeFormat { get; set; }
                public int JobInfoOperatorConfirm { get; set; }
                public int IdleTime { get; set; }
            }

            public HSMSConfig HSMS { get; set; }
            public GEMConfig GEM { get; set; }
        }

        private static volatile GEMManager _instance = null;
        private static object syncRoot = new Object();

        Window _win;
        System.Windows.Forms.Integration.WindowsFormsHost _host;
        AxEZGEMLib.AxEZGEM _gem;

        public AxEZGEMLib.AxEZGEM GEM { get { return _gem; } }
        public bool IsLoaded { get; private set; }

        public GemConfig Config { get; set; }

        private GEMManager()
        {
        }

        public static GEMManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                            _instance = new GEMManager();
                    }
                }

                return _instance;
            }
        }

        public void Initialize(List<CEID> ceidList, List<SVID> svidList)
        {
            CreateGEM();
            LoadConfig();
            SetConfig();
            SetCEIDList(ceidList);
            SetSVIDList(svidList);
        }

        public void Start()
        {
            GEM.Start();
            GEM.GoOnlineRemote();
        }

        private void CreateGEM()
        {
            App.Current.Dispatcher.Invoke(
                new Action(delegate
                {
                    _win = new Window();

                    _win.Initialized +=
                        delegate
                        {
                            _host = new System.Windows.Forms.Integration.WindowsFormsHost();
                            _win.Content = _host;
                            _gem = new AxEZGEMLib.AxEZGEM();
                            _host.Child = GEM;
                            IsLoaded = true;
                        };

                    _win.Loaded +=
                        delegate
                        {
                        };

                    _win.WindowStyle = WindowStyle.ToolWindow;
                    _win.WindowState = WindowState.Minimized;
                    _win.Show();
                }), null);
        }
        
        private void LoadConfig()
        {
            var filename = Path.Combine(ConfigClasses.GlobalConst.CONFIG_PATH, "gemconfig.xml");

            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                using (XmlReader sr = XmlReader.Create(fs))
                {
                    XmlSerializer xs = FALibrary.Utility.FAUtility.GetXmlSerializer(typeof(GemConfig));
                    Config = (GemConfig)xs.Deserialize(sr);
                }
            }
        }

        private void SetConfig()
        {
            string logFilePath = CombinePath(AppDomain.CurrentDomain.BaseDirectory, Config.LogPath);
            string formatFilePath = CombinePath(AppDomain.CurrentDomain.BaseDirectory, Config.FormatFilePath);

            _gem.SetLogFile(logFilePath, 1);
            _gem.SetLogRetention(Config.LogRetention);
            _gem.SetFormatFile(formatFilePath);
            _gem.SetFormatCheck(Config.FormatCheck);

            _gem.DeviceID = Config.HSMS.DeviceID;
            _gem.IP = Config.HSMS.IP;
            _gem.Port = Config.HSMS.Port;
            _gem.Retry = Config.HSMS.Retry;
            _gem.LinkTestInterval = Config.HSMS.LinkTestInterval;
            _gem.T3 = Config.HSMS.T3;
            _gem.T5 = Config.HSMS.T5;
            _gem.T6 = Config.HSMS.T6;
            _gem.T7 = Config.HSMS.T7;
            _gem.T8 = Config.HSMS.T8;
            _gem.HostMode = Config.HSMS.HostMode;
            _gem.SetModelName(Config.GEM.MDLN);
            _gem.SetSoftwareRev(Config.GEM.SoftReversion);

            _gem.SetFormatCodeALID(54);
            _gem.SetFormatCodeCEID(54);
            _gem.SetFormatCodeDATAID(54);
            _gem.SetFormatCodeECID(54);
            _gem.SetFormatCodeRPTID(54);
            _gem.SetFormatCodeSVID(54);
            _gem.SetFormatCodeTRACEID(54);
        }

        private void SetCEIDList(List<CEID> idList)
        {
            foreach (var item in idList)
                AddCEID(item);
        }

        private void SetSVIDList(List<SVID> idList)
        {
            foreach (var item in idList)
                AddSVID(item);
        }

        private void AddCEID(CEID ceid)
        {
            _gem.AddCEID(ceid.ID, ceid.Name, ceid.Description);
        }

        private void AddSVID(SVID svid)
        {
            _gem.AddSVID(svid.ID, svid.Name, svid.DataFormat, svid.Unit);
        }

        private string CombinePath(string path1, string path2)
        {
            if (string.IsNullOrEmpty(path2))
                return path1;
            else
            {
                if (string.IsNullOrEmpty(path1)) return path2;

                if (path2[0] == Path.DirectorySeparatorChar)
                    path2 = path2.Remove(0, 1);

                return Path.Combine(path1, path2);
            }
        }
    }
}
