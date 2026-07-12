using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

namespace FAFramework.VT3500.JobInfo
{
    public class JobManager : INotifyPropertyChanged
    {
        public class LotJobList : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private void NotifyPropertyChanged(string propertyName = "")
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            private string[] _jobNames;
            [XmlIgnore]
            public string[] JobNames
            {
                get { return _jobNames; }
                private set
                {
                    if (_jobNames == value) return;
                    _jobNames = value;
                    NotifyPropertyChanged("JobNames");
                }
            }

            private FAFramework.Utility.ThreadSafeObservableCollection<FALotJobInfo> _lotJobInfoList = new FAFramework.Utility.ThreadSafeObservableCollection<FALotJobInfo>();
            public FAFramework.Utility.ThreadSafeObservableCollection<FALotJobInfo> LotJobInfoList
            {
                get { return _lotJobInfoList; }
                set
                {
                    if (_lotJobInfoList == value) return;
                    _lotJobInfoList = value;
                    NotifyPropertyChanged("LotJobInfoList");
                }
            }

            public LotJobList()
            {
                LotJobInfoList.CollectionChanged +=
                    delegate
                    {
                        JobNames = LotJobInfoList.Select(x => x.Name).ToArray<string>();
                    };
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private LotJobList _lotJobInstance = new LotJobList();
        public LotJobList LotJobInstance
        {
            get { return _lotJobInstance; }
            private set
            {
                if (_lotJobInstance == value) return;
                _lotJobInstance = value;
                NotifyPropertyChanged("LotJobInstance");
            }
        }

        public string JobFolderPath { get; set; }
        public readonly string FILENAME = "jobfile.xml";

        public void Load()
        {
            try
            {
                string filename = System.IO.Path.Combine(JobFolderPath, FILENAME);
                if (System.IO.File.Exists(filename) == false) return;

                using (FileStream fs = new FileStream(filename, FileMode.Open))
                {
                    XmlReader sr = XmlReader.Create(fs);
                    XmlSerializer xs = new XmlSerializer(LotJobInstance.GetType());
                    LotJobInstance = (LotJobList)xs.Deserialize(sr);
                }
            }
            catch (Exception e)
            {
                Manager.LogManager.Instance.WriteSystemLog(e.ToString());
            }
        }

        public void Save()
        {
            XmlWriterSettings setting = new XmlWriterSettings();
            setting.Indent = true;
            setting.IndentChars = "  ";
            setting.NewLineOnAttributes = true;
            setting.OmitXmlDeclaration = true;

            try
            {
                string filename = System.IO.Path.Combine(JobFolderPath, FILENAME);

                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    XmlWriter xw = XmlWriter.Create(fs, setting);
                    XmlSerializer xs = new XmlSerializer(LotJobInstance.GetType());
                    xs.Serialize(xw, LotJobInstance);
                }
            }
            catch (Exception e)
            {
                Manager.LogManager.Instance.WriteSystemLog(e.ToString());
            }
        }

        public FALotJobInfo GetJob(string jobname, out string msg)
        {
            msg = "";
            if (LotJobInstance == null)
            {
                msg = "LotJobInstance is null";
                return null;
            }

            var result = LotJobInstance.LotJobInfoList.Where(x => x.Name == jobname);
            return result.First();

        }

    }
}
