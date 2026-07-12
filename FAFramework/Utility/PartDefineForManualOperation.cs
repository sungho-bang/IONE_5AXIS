using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using FAFramework.Utility;

namespace FAFramework.Utility
{
    public class PartDefineForManualOperation : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private List<object> _statusObjectList;
        public List<object> StatusObjectList
        {
            get { return _statusObjectList; }
            set
            {
                _statusObjectList = value;
                NotifyPropertyChanged("StatusObjectList");
            }
        }

        private List<AliasPartAction> _actions;
        public List<AliasPartAction> Actions
        {
            get { return _actions; }
            private set
            {
                _actions = value;
                NotifyPropertyChanged("Actions");
            }
        }

        private FAFramework.Utility.ThreadSafeObservableCollection<AliasPartAction> _repeatActionList;
        public FAFramework.Utility.ThreadSafeObservableCollection<AliasPartAction> RepeatActionList
        {
            get { return _repeatActionList; }
            private set
            {
                _repeatActionList = value;
                NotifyPropertyChanged("RepeatActionList");
            }
        }

        private bool _isStopedRepeatAction = true;
        public bool IsStopedRepeatAction
        {
            get { return _isStopedRepeatAction; }
            set
            {
                _isStopedRepeatAction = value;
                NotifyPropertyChanged("IsStopedRepeatAction");
            }
        }

        private int _currentActionIndex;
        public int CurrentActionIndex
        {
            get { return _currentActionIndex; }
            set
            {
                _currentActionIndex = value;
                NotifyPropertyChanged("CurrentActionIndex");
            }
        }

        private TimeSpan _repeatActionTimeout;
        public TimeSpan RepeatActionTimeout
        {
            get { return _repeatActionTimeout; }
            set
            {
                _repeatActionTimeout = value;
                NotifyPropertyChanged("RepeatActionTimeout");
            }
        }

        public DateTime _actionStartedTime;
        public DateTime ActionStartedTime
        {
            get { return _actionStartedTime; }
            set
            {
                _actionStartedTime = value;
                NotifyPropertyChanged("ActionStartedTime");
            }
        }

        private List<Func<bool>> _repeatSequence = new List<Func<bool>>();

        private int _repeatSequenceCurrentIndex = 0;

        private DispatcherTimer _timer = new DispatcherTimer();

        public PartDefineForManualOperation()
        {
            Actions = new List<AliasPartAction>();
            RepeatActionList = new FAFramework.Utility.ThreadSafeObservableCollection<AliasPartAction>();
            StatusObjectList = new List<object>();

            _repeatSequence.Add(ExecuteCurrentAction);
            _repeatSequence.Add(IsCurrentActionTerminated);

            _timer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            _timer.Tick += new EventHandler(
                delegate (object sender, EventArgs e)
                {
                    ExecuteRepeatAction();
                });
        }

        public void AddAction(AliasPartAction aliasPartAction)
        {
            Actions.Add(aliasPartAction);
            AddRepeatAction(aliasPartAction);
        }

        public void AddAction(string alias, Action<object> method, Func<bool> isStateOk)
        {
            var partAction = new AliasPartAction(alias, method, isStateOk);
            Actions.Add(partAction);
            AddRepeatAction(partAction);
        }

        public void AddAction(string alias, Action method, Func<bool> isStateOk)
        {
            var partAction = new AliasPartAction(alias, method, isStateOk);
            Actions.Add(partAction);
            AddRepeatAction(partAction);
        }

        public void AddRepeatAction(AliasPartAction partAction)
        {
            if (RepeatActionList.Contains(partAction) == false)
                RepeatActionList.Add(partAction);
        }

        public void RemoveRepeatAction(AliasPartAction partAction)
        {
            if (RepeatActionList.Contains(partAction) == true)
                RepeatActionList.Remove(partAction);
        }

        public void StartRepeatAction()
        {
            CurrentActionIndex = 0;
            IsStopedRepeatAction = false;
            _timer.Start();
        }

        public void StopRepeatAction()
        {
            _timer.Stop();
            CurrentActionIndex = 0;
            IsStopedRepeatAction = true;
        }

        private void ExecuteRepeatAction()
        {
            if (_repeatSequence[_repeatSequenceCurrentIndex]() == true)
            {
                _repeatSequenceCurrentIndex++;
                if (_repeatSequenceCurrentIndex >= _repeatSequence.Count)
                    _repeatSequenceCurrentIndex = 0;
            }
        }

        private bool ExecuteCurrentAction()
        {
            ActionStartedTime = DateTime.Now;
            RepeatActionList[CurrentActionIndex].ActionMethod(this);
            return true;
        }

        private bool IsCurrentActionTerminated()
        {
            if (RepeatActionList[CurrentActionIndex].IsStateOk() == true)
            {
                CurrentActionIndex++;
                if (CurrentActionIndex >= RepeatActionList.Count)
                    CurrentActionIndex = 0;

                return true;
            }
            else if (DateTime.Now - ActionStartedTime > RepeatActionTimeout)
            {
                CurrentActionIndex++;
                if (CurrentActionIndex >= RepeatActionList.Count)
                    CurrentActionIndex = 0;

                return true;
            }
            else
                return false;
        }
    }

    public class AliasPartAction
    {
        public string Alias { get; set; }
        public Action<object> ActionMethod { get; set; }
        public Func<bool> IsStateOk { get; set; }

        public AliasPartAction(string alias, Action<object> method, Func<bool> isStateOk)
        {
            Alias = alias;
            ActionMethod = method;
            IsStateOk = isStateOk;
        }

        public AliasPartAction(string alias, Action method, Func<bool> isStateOk)
        {
            Alias = alias;
            ActionMethod = new Action<object>(
                delegate (object obj)
                {
                    method();
                });

            IsStateOk = isStateOk;
        }
    }
}
