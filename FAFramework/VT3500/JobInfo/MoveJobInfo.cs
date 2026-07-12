using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FAFramework.Utility;
using System.Collections.ObjectModel;
using FALibrary.Utility;
using FALibrary;

namespace FAFramework.VT3500.JobInfo
{
    public class MoveJobInfo : FAJobInfo
    {
        //원단피딩속도
        private double _feedingSpeed;
        [FA("Job")]
        public double FeedingSpeed
        {
            get { return _feedingSpeed; }
            set
            {
                if (_feedingSpeed == value) return;
                _feedingSpeed = value;
                NotifyPropertyChanged("FeedingSpeed");
            }
        }

        //이송피치
        private double _feedingPitch;
        [FA("Job")]
        public double FeedingPitch
        {
            get { return _feedingPitch; }
            set
            {
                if (_feedingPitch == value) return;
                _feedingPitch = value;
                NotifyPropertyChanged("FeedingPitch");
            }
        }
        //성형시간
        private FATime _moldingTime;
        [FA("Job")]
        public FATime MoldingTime
        {
            get { return _moldingTime; }
            set
            {
                if (_moldingTime == value) return;
                _moldingTime = value;
                NotifyPropertyChanged("MoldingTime");
            }
        }

        //포장피딩속도
        private double _packingFeedSpeed;
        [FA("Job")]
        public double PackingFeedSpeed
        {
            get { return _packingFeedSpeed; }
            set
            {
                if (_packingFeedSpeed == value) return;
                _packingFeedSpeed = value;
                NotifyPropertyChanged("PackingFeedSpeed");
            }
        }
        //포장피치
        private double _packingFeedPitch;
        [FA("Job")]
        public double PackingFeedPitch
        {
            get { return _packingFeedPitch; }
            set
            {
                if (_packingFeedPitch == value) return;
                _packingFeedPitch = value;
                NotifyPropertyChanged("PackingFeedPitch");
            }
        }
        //실링시간
        private FATime _sealingTime;
        [FA("Job")]
        public FATime SealingTime
        {
            get { return _sealingTime; }
            set
            {
                if (_sealingTime == value) return;
                _sealingTime = value;
                NotifyPropertyChanged("SealingTime");
            }
        }
        //제품수량
        private int _packageCount;
        [FA("Job")]
        public int PackageCount
        {
            get { return _packageCount; }
            set
            {
                if (_packageCount == value) return;
                _packageCount = value;
                NotifyPropertyChanged("PackageCount");
            }
        }
        //컷팅횟수
        private int _cuttingCount;
        [FA("Job")]
        public int CuttingCount
        {
            get { return _cuttingCount; }
            set
            {
                if (_cuttingCount == value) return;
                _cuttingCount = value;
                NotifyPropertyChanged("CuttingCount");
            }
        }
        private bool _useFirstPress;
        [FA("Job")]
        public bool UseFirstPress
        {
            get { return _useFirstPress; }
            set
            {
                if (_useFirstPress == value) return;
                _useFirstPress = value;
                NotifyPropertyChanged("UseFirstPress");
            }
        }
        private bool _useSecondPress;
        [FA("Job")]
        public bool UseSecondPress
        {
            get { return _useSecondPress; }
            set
            {
                if (_useSecondPress == value) return;
                _useSecondPress = value;
                NotifyPropertyChanged("UseSecondPress");
            }
        }
        private bool _useOptionPress;
        [FA("Job")]
        public bool UseOptionPress
        {
            get { return _useOptionPress; }
            set
            {
                if (_useOptionPress == value) return;
                _useOptionPress = value;
                NotifyPropertyChanged("UseOptionPress");
            }
        }
        private bool _useThirdPress;
        [FA("Job")]
        public bool UseThirdPress
        {
            get { return _useThirdPress; }
            set
            {
                if (_useThirdPress == value) return;
                _useThirdPress = value;
                NotifyPropertyChanged("UseThirdPress");
            }
        }
        private bool _useFourthPress;
        [FA("Job")]
        public bool UseFourthPress
        {
            get { return _useFourthPress; }
            set
            {
                if (_useFourthPress == value) return;
                _useFourthPress = value;
                NotifyPropertyChanged("UseFourthPress");
            }
        }
        private bool _useTopPeeling;
        [FA("Job")]
        public bool UseTopPeeling
        {
            get { return _useTopPeeling; }
            set
            {
                if (_useTopPeeling == value) return;
                _useTopPeeling = value;
                NotifyPropertyChanged("UseTopPeeling");
            }
        }
        private bool _useBottomPeeling;
        [FA("Job")]
        public bool UseBottomPeeling
        {
            get { return _useBottomPeeling; }
            set
            {
                if (_useBottomPeeling == value) return;
                _useBottomPeeling = value;
                NotifyPropertyChanged("UseBottomPeeling");
            }
        }
        //private bool _useTopPacking;
        //[FALibrary.FAAttribute("")]
        //public bool UseTopPacking
        //{
        //    get { return _useTopPacking; }
        //    set
        //    {
        //        if (_useTopPacking == value) return;
        //        _useTopPacking = value;
        //        NotifyPropertyChanged("UseTopPacking");
        //    }
        //}
        //private bool _useBottomPacking;
        //[FALibrary.FAAttribute("")]
        //public bool UseBottomPacking
        //{
        //    get { return _useBottomPacking; }
        //    set
        //    {
        //        if (_useBottomPacking == value) return;
        //        _useBottomPacking = value;
        //        NotifyPropertyChanged("UseBottomPacking");
        //    }
        //}
        private bool _usePackingScrap;
        [FA("Job")]
        public bool UsePackingScrap
        {
            get { return _usePackingScrap; }
            set
            {
                if (_usePackingScrap == value) return;
                _usePackingScrap = value;
                NotifyPropertyChanged("UsePackingScrap");
            }
        }

        private bool _useIMark;
        [FA("Job")]
        public bool UseIMark
        {
            get { return _useIMark; }
            set
            {
                if (_useIMark == value) return;
                _useIMark = value;
                NotifyPropertyChanged("UseIMark");
            }
        }
        //210705
        private double _tapeLoadingPos;
        [FA("Job")]
        public double TapeLoadingPos
        {
            get { return _tapeLoadingPos; }
            set
            {
                if (_tapeLoadingPos == value) return;
                _tapeLoadingPos = value;
                NotifyPropertyChanged("TapeLoadingPos");
            }
        }
        private double _tapeLoadingSlowPos;
        [FA("Job")]
        public double TapeLoadingSlowPos
        {
            get { return _tapeLoadingSlowPos; }
            set
            {
                if (_tapeLoadingSlowPos == value) return;
                _tapeLoadingSlowPos = value;
                NotifyPropertyChanged("TapeLoadingSlowPos");
            }
        }
        private double _tapeUnUseImarkPos;
        [FA("Job")]
        public double TapeUnUseImarkPos
        {
            get { return _tapeUnUseImarkPos; }
            set
            {
                if (_tapeUnUseImarkPos == value) return;
                _tapeUnUseImarkPos = value;
                NotifyPropertyChanged("TapeLoadingSlowPos");
            }
        }

        public override void CopyTo(FAJobInfo obj)
        {
            if ((obj is MoveJobInfo) == false) return;
            var dest = obj as MoveJobInfo;

            string name;
            name = "FeedingSpeed"; SetValue(dest, name, GetValue(name));
            name = "FeedingPitch"; SetValue(dest, name, GetValue(name));
            name = "MoldingTime"; SetValue(dest, name, GetValue(name));
            name = "PackingFeedSpeed"; SetValue(dest, name, GetValue(name));
            name = "PackingFeedPitch"; SetValue(dest, name, GetValue(name));
            name = "SealingTime"; SetValue(dest, name, GetValue(name));
            name = "PackageCount"; SetValue(dest, name, GetValue(name));
            name = "CuttingCount"; SetValue(dest, name, GetValue(name));
            name = "UseFirstPress"; SetValue(dest, name, GetValue(name));
            name = "UseSecondPress"; SetValue(dest, name, GetValue(name));
            name = "UseOptionPress"; SetValue(dest, name, GetValue(name));
            name = "UseThirdPress"; SetValue(dest, name, GetValue(name)); 
            name = "UseFourthPress"; SetValue(dest, name, GetValue(name));
            name = "UseTopPeeling"; SetValue(dest, name, GetValue(name));
            name = "UseBottomPeeling"; SetValue(dest, name, GetValue(name));
            //name = "UseTopPacking"; SetValue(dest, name, GetValue(name));
            //name = "UseBottomPacking"; SetValue(dest, name, GetValue(name));
            name = "UsePackingScrap"; SetValue(dest, name, GetValue(name));
            name = "UseIMark"; SetValue(dest, name, GetValue(name));
            //210705
            name = "TapeLoadingPos"; SetValue(dest, name, GetValue(name));
            name = "TapeLoadingSlowPos"; SetValue(dest, name, GetValue(name));
            name = "TapeUnUseImarkPos"; SetValue(dest, name, GetValue(name));
        }

        public override FAJobInfo Clone()
        {
            var cloneObj = new MoveJobInfo();
            CopyTo(cloneObj);
            return cloneObj;
        }

        public string[] ToKeyValueArray(string prefix)
        {
            List<string> list = new List<string>();

            string name;
            name = "FeedingSpeed"; list.Add(string.Format("{0}{1}={2}", prefix, name, GetValue(name)));
            name = "FeedingPitch"; list.Add(string.Format("{0}{1}={2}", prefix, name, GetValue(name)));
            name = "MoldingTime"; list.Add(string.Format("{0}{1}={2}", prefix, name, GetValue(name)));
            name = "PackingFeedSpeed"; list.Add(string.Format("{0}{1}={2}", prefix, name, GetValue(name)));
            name = "PackingFeedPitch"; list.Add(string.Format("{0}{1}={2}", prefix, name, GetValue(name)));
            name = "SealingTime"; list.Add(string.Format("{0}{1}={2}", prefix, name, GetValue(name)));
            name = "PackageCount"; list.Add(string.Format("{0}{1}={2}", prefix, name, GetValue(name)));
            name = "CuttingCount"; list.Add(string.Format("{0}{1}={2}", prefix, name, GetValue(name)));
            name = "UseFirstPress"; list.Add(string.Format("{0}{1}={2}", prefix, name, GetValue(name)));
            name = "UseSecondPress"; list.Add(string.Format("{0}{1}={2}", prefix, name, GetValue(name)));
            name = "UseOptionPress"; list.Add(string.Format("{0}{1}={2}", prefix, name, GetValue(name)));
            name = "UseThirdPress"; list.Add(string.Format("{0}{1}={2}", prefix, name, GetValue(name)));
            name = "UseFourthPress"; list.Add(string.Format("{0}{1}={2}", prefix, name, GetValue(name)));
            name = "UseTopPeeling"; list.Add(string.Format("{0}{1}={2}", prefix, name, GetValue(name)));
            name = "UseBottomPeeling"; list.Add(string.Format("{0}{1}={2}", prefix, name, GetValue(name)));
            //name = "UseTopPacking"; list.Add(string.Format("{0}{1}={2}", prefix, name, GetValue(name)));
            //name = "UseBottomPacking"; list.Add(string.Format("{0}{1}={2}", prefix, name, GetValue(name)));
            name = "UsePackingScrap"; list.Add(string.Format("{0}{1}={2}", prefix, name, GetValue(name)));
            name = "UseIMark"; list.Add(string.Format("{0}{1}={2}", prefix, name, GetValue(name)));
            //210705
            name = "TapeLoadingPos"; list.Add(string.Format("{0}{1}={2}", prefix, name, GetValue(name)));
            name = "TapeLoadingSlowPos"; list.Add(string.Format("{0}{1}={2}", prefix, name, GetValue(name)));
            name = "TapeUnUseImarkPos"; list.Add(string.Format("{0}{1}={2}", prefix, name, GetValue(name)));
            return list.ToArray();
        }

        public void Parsing(System.Xml.Linq.XElement xml)
        {
            string name;
            name = "FeedingSpeed"; if (xml.Element(name) != null) SetValue(name, xml.Element(name).Value);
            name = "FeedingPitch"; if (xml.Element(name) != null) SetValue(name, xml.Element(name).Value);
            name = "MoldingTime"; if (xml.Element(name) != null) SetValue(name, xml.Element(name).Value);
            name = "PackingFeedSpeed"; if (xml.Element(name) != null) SetValue(name, xml.Element(name).Value);
            name = "PackingFeedPitch"; if (xml.Element(name) != null) SetValue(name, xml.Element(name).Value);
            name = "SealingTime"; if (xml.Element(name) != null) SetValue(name, xml.Element(name).Value);
            name = "PackageCount"; if (xml.Element(name) != null) SetValue(name, xml.Element(name).Value);
            name = "CuttingCount"; if (xml.Element(name) != null) SetValue(name, xml.Element(name).Value);
            name = "UseFirstPress"; if (xml.Element(name) != null) SetValue(name, xml.Element(name).Value);
            name = "UseSecondPress"; if (xml.Element(name) != null) SetValue(name, xml.Element(name).Value);
            name = "UseOptionPress"; if (xml.Element(name) != null) SetValue(name, xml.Element(name).Value);
            name = "UseThirdPress"; if (xml.Element(name) != null) SetValue(name, xml.Element(name).Value);
            name = "UseFourthPress"; if (xml.Element(name) != null) SetValue(name, xml.Element(name).Value);
            name = "UseTopPeeling"; if (xml.Element(name) != null) SetValue(name, xml.Element(name).Value);
            name = "UseBottomPeeling"; if (xml.Element(name) != null) SetValue(name, xml.Element(name).Value);
            //name = "UseTopPacking"; if (xml.Element(name) != null) SetValue(name, xml.Element(name).Value);
            //name = "UseBottomPacking"; if (xml.Element(name) != null) SetValue(name, xml.Element(name).Value);
            name = "UsePackingScrap"; if (xml.Element(name) != null) SetValue(name, xml.Element(name).Value);
            name = "UseIMark"; if (xml.Element(name) != null) SetValue(name, xml.Element(name).Value);
            //210705
            name = "TapeLoadingPos"; if (xml.Element(name) != null) SetValue(name, xml.Element(name).Value);
            name = "TapeLoadingSlowPos"; if (xml.Element(name) != null) SetValue(name, xml.Element(name).Value);
            name = "TapeUnUseImarkPos"; if (xml.Element(name) != null) SetValue(name, xml.Element(name).Value);
        }
        public void SetValue(object obj, string name, object val)
        {
            var propertyInfo = obj.GetType().GetProperty(name);
            propertyInfo.SetValue(obj, val, null);
        }
        public void SetValue(string name, object val)
        {
            var propertyInfo = this.GetType().GetProperty(name);
            propertyInfo.SetValue(this, val, null);
        }

        public object GetValue(string name)
        {
            var propertyInfo = this.GetType().GetProperty(name);
            object val = propertyInfo.GetValue(this, null);
            return val;
        }
    }
}
