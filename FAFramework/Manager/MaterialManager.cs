using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using FALibrary;

namespace FAFramework.Manager
{
    public class MaterialInfoBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string _materialName;
        [FAAttribute("")]
        public string MaterialName
        {
            get { return _materialName; }
            set
            {
                if (_materialName == value) return;
                _materialName = value;
                NotifyPropertyChanged("MaterialName");
            }
        }
    }

    public class CountableMaterialInfo : MaterialInfoBase
    {
        private double _max;
        [FAAttribute("")]
        public double Max
        {
            get { return _max; }
            set
            {
                if (_max == value) return;
                _max = value;
                NotifyPropertyChanged("Max");
            }
        }

        private double _min;
        [FAAttribute("")]
        public double Min
        {
            get { return _min; }
            set
            {
                if (_min == value) return;
                _min = value;
                NotifyPropertyChanged("Min");
            }
        }

        private double _remainQuantity;
        [FAAttribute("")]
        public double RemainQuantity
        {
            get { return _remainQuantity; }
            set
            {
                if (_remainQuantity == value) return;
                _remainQuantity = value;
                NotifyPropertyChanged("RemainQuantity");
            }
        }

        private double _useQuantity;
        [FAAttribute("")]
        public double UseQuantity
        {
            get { return _useQuantity; }
            set
            {
                if (_useQuantity == value) return;
                _useQuantity = value;
                NotifyPropertyChanged("UseQuantity");
            }
        }

        public void Reset()
        {
            RemainQuantity = Max;
            UseQuantity = 0;
        }

        public bool Use()
        {
            return Use(1);
        }

        public bool Use(double count)
        {
            UseQuantity += count;
            RemainQuantity = Max - UseQuantity;

            if (RemainQuantity < Min)
                return false;
            else
                return true;
        }
    }

    public class UncountableMaterialInfo : MaterialInfoBase
    {
        private string _state = string.Empty;
        [FAAttribute("")]
        public string State
        {
            get { return _state; }
            set
            {
                if (_state == value) return;
                _state = value;
                NotifyPropertyChanged("State");
            }
        }
    }

    public abstract class MaterialManagerBase
    {
        private List<MaterialInfoBase> _materialInfoList = new List<MaterialInfoBase>();
        protected List<MaterialInfoBase> MaterialInfoList
        {
            get { return _materialInfoList; }
            private set
            {
                _materialInfoList = value;
            }
        }

        public event EventHandler<FAGenericEventArgs<string[]>> OnUpdate = delegate { };

        public MaterialManagerBase()
        {
            Load();
        }

        public void AddItem(MaterialInfoBase item)
        {
            _materialInfoList.Add(item);
        }

        public void Update()
        {
            Update(DateTime.Now);
        }

        public virtual void Update(DateTime date)
        {
            try
            {
                var list = _materialInfoList.Select(x => string.Format("{0}, {1}", date.ToString("yyy/MM/dd HH:mm:ss.fff"), MaterialToString(x)));

                OnUpdate(this, new FAGenericEventArgs<string[]>(list.ToArray<string>()));
                //Save();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.Write(e.ToString());
            }
        }

        protected virtual string MaterialToString(MaterialInfoBase obj)
        {
            if (obj is CountableMaterialInfo)
                return CounticMaterialInfoToString(obj as CountableMaterialInfo);
            else if (obj is UncountableMaterialInfo)
                return StaticMaterialInfoToString(obj as UncountableMaterialInfo);
            else
                return string.Empty;
        }

        protected virtual string CounticMaterialInfoToString(CountableMaterialInfo obj)
        {
            if (obj == null) return string.Empty;

            return string.Format("{0}, {1}, {2}", obj.MaterialName, obj.UseQuantity, obj.RemainQuantity);
        }

        protected virtual string StaticMaterialInfoToString(UncountableMaterialInfo obj)
        {
            if (obj == null) return string.Empty;

            return string.Format("{0}, {1}", obj.MaterialName, obj.State);
        }

        public abstract void Load();

        public abstract void Save();
    }
}
