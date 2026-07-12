using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using FALibrary.Sequence;
using FALibrary.Device;
using FALibrary.Utility;
using System.Xml;

namespace FALibrary.Part
{
    public delegate bool FAInterlock(ref string msg);
   
    public class FAPartAction : FAObject
    {
        private string _name;        
        private string _description;
        private string _actionName = "";
        private Dictionary<string, dynamic> _parameters = new Dictionary<string, dynamic>();
        private List<FAInterlock> _interlockList = new List<FAInterlock>();
        private FASequence _sequence = null;
        private FAPart _part = null;        

        public string ActionName
        {
            get
            {
                return _actionName;
            }

            set
            {
                _actionName = value;
                Sequence.Name = value;
            }
        }
        public TimeSpan Timeout { get; set; }
        public FAPart Part
        {
            get { return _part; }
            set { _part = value; }
        }

        protected Action<object> ActionMethod { get; set; }

        public bool IgnoreInterlock { get; set; }

        public Dictionary<string, dynamic> Parameters
        {
            get { return _parameters; }
        }        

        [FAAttribute("", true)]
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                if (_name == value) return;

                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        [FAAttribute("", true)]
        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                if (_description == value) return;

                _description = value;
                NotifyPropertyChanged("Description");
            }
        }

        public List<FAInterlock> InterlockList
        {
            get { return _interlockList; }
        }

        [FAAttribute("")]
        public FASequence Sequence
        {
            get { return _sequence; }            
        }

        [FAAttribute("Operation")]
        public virtual void Execute(object sender)
        {
            if (IsInterlock() == true)
                return;

            ActionMethod(sender);
        }

        public void CreateSequence(FASequenceManager aSequenceManager)
        {
            Object thisLock = new Object();
            lock (thisLock)
            {
                if (_sequence == null)
                {
                    _sequence = new FASequence(aSequenceManager);
                }
            }
        }

        public bool IsInterlock()
        {
            string msg = "";

            if (IgnoreInterlock == true)
                return false;

            if (InterlockManager.Instance.IsSystemInterlock() == true)
            {                
                return true;
            }

            if (InterlockManager.Instance.IgnoreInterlock == true)
                return false;

            foreach (FAInterlock item in _interlockList)
            {
                bool result;
                try
                {
                    result = item(ref msg);
                }
                catch (Exception e)
                {
                    result = true;
                    msg = e.Message;
                }

                if (result == true)
                {
                    msg = "DO NOT " + ActionName + " " + msg;                    
                    return true;
                }                
            }
            
            return false;
        }

        public void ExecuteForSequence(FASequence actor, TimeSpan time)
        {
            if (IsInterlock() == true)                            
                return;            

            ActionMethod(actor);
            actor.NextStep();
        }

        public void SetActionMethod(Action<object> method)
        {
            ActionMethod = method;
        }

        private void IsTimeout(FASequence actor, TimeSpan time)
        {
            if (time > Timeout)
            {
                actor.SetAlarm();
            }
        }
    }

    public class FAAutoInterruptPartAction : FAPartAction
    {
        private Action<object> InterruptMethod;

        [FAAttributePushButtonMethod("PushButtonOperation", "DoStop")]
        public override void Execute(object sender)
        {
            base.Execute(sender);
        }

        public void SetInterruptMethod(Action<object> method)
        {
            InterruptMethod = method;
        }

        public void DoStop(object sender)
        {
            if (InterruptMethod == null) return;

            InterruptMethod(sender);
        }
    }

    public abstract class FAPart : FAMachine
    {
        private bool _simulationMode;

        public string DeviceName { get; set; }
        public event EventHandler<FAGenericEventArgs<bool>> SimulationModeChanged = delegate { };

        [FAAttribute("")]
        public bool SimulationMode
        {
            get { return _simulationMode; }
            set
            {
                if (_simulationMode == value) return;

                _simulationMode = value;
                NotifyPropertyChanged("SimulationMode");
                if (SimulationModeChanged != null)
                    SimulationModeChanged(this, new FAGenericEventArgs<bool>(value));
            }
        }

        private bool _ignoreInterlock;
        [FAAttribute("")]
        [FAPropertyAttribute]
        public virtual bool IgnoreInterlock
        {
            get { return _ignoreInterlock; }
            set
            {
                if (_ignoreInterlock == value) return;

                _ignoreInterlock = value;

                var properties = this.GetType().GetProperties();
                foreach (var item in properties)
                {
                    object propValue = item.GetValue(this, null);
                    if (propValue != null && propValue is FAPartAction)
                    {
                        ((FAPartAction)propValue).IgnoreInterlock = value;
                    }
                }

                NotifyPropertyChanged("IgnoreInterlock");
            }
        }

        private bool _loadedParameters = false;

        public override void LoadParameters(XElement xml)
        {
            base.LoadParameters(xml);
            try
            {
                if (_loadedParameters == false)
                {
                    if (xml.Element("Actions") != null)
                        LoadActions(xml.Element("Actions"));

                    _loadedParameters = true;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Name : " + Name + ", " + e.Message);
            }
        }

        private void LoadActions(XElement xml)
        {
            foreach (XElement item in xml.Elements())
            {
                string actionName = item.Element("Name").Value.Trim();
                string displayName = item.Element("DisplayName").Value.Trim();

                FAPartAction actionObject = FAReflection.GetPropertyValue(this, actionName) as FAPartAction;
                FAReflection.SetPropertyValue(actionObject, "ActionName", displayName);
            }
        }

        public abstract void SetDevice(FADevice aDevice);

        public virtual void Validate()
        {
            //NotifyPropertyChanged(string.Empty);
        }        
    }
}
