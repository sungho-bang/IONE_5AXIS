using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using FALibrary.Alarm;
using FAFramework.Utility;

namespace FAFramework.Manager
{
    public class AlarmRaisingStatusManager : INotifyPropertyChanged
    {
        public class AlarmInfo : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged = delegate { };

            private void NotifyPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

            private FAAlarmEventArgs _alarmEventArgs;
            public FAAlarmEventArgs AlarmEventArgs
            {
                get { return _alarmEventArgs; }
                set
                {
                    _alarmEventArgs = value;
                    NotifyPropertyChanged("AlarmEventArgs");
                }
            }

            private CommandHandler _clearAlarm;
            public CommandHandler ClearAlarm
            {
                get { return _clearAlarm; }
                set
                {
                    if (_clearAlarm == value) return;
                    _clearAlarm = value;
                    NotifyPropertyChanged("ClearAlarm");
                }
            }

            private DateTime _raisedTime;
            public DateTime RaisedTime
            {
                get { return _raisedTime; }
                set
                {
                    _raisedTime = value;
                    NotifyPropertyChanged("RaisedTime");
                }
            }

            private DateTime _clearedTime;
            public DateTime ClearedTime
            {
                get { return _clearedTime; }
                set
                {
                    _clearedTime = value;
                    NotifyPropertyChanged("ClearedTime");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private FAFramework.Utility.ThreadSafeObservableCollection<AlarmInfo> _raisingAlarmList;
        public FAFramework.Utility.ThreadSafeObservableCollection<AlarmInfo> RaisingAlarmList
        {
            get { return _raisingAlarmList; }
            private set
            {
                _raisingAlarmList = value;
                NotifyPropertyChanged("RaisingAlarmList");
            }
        }

        private AlarmInfo _currentAlarmInfo;
        [FALibrary.FAAttribute("")]
        public AlarmInfo CurrentAlarmInfo
        {
            get { return _currentAlarmInfo; }
            set
            {
                if (_currentAlarmInfo == value) return;
                _currentAlarmInfo = value;
                NotifyPropertyChanged("CurrentAlarmInfo");
            }
        }

        private int _alarmCount;
        [FALibrary.FAAttribute("")]
        public int AlarmCount
        {
            get { return _alarmCount; }
            set
            {
                if (_alarmCount == value) return;
                _alarmCount = value;
                NotifyPropertyChanged("AlarmCount");
            }
        }

        public CommandHandler TurnOffCommand { get; private set; }

        public event EventHandler<FAAlarmEventArgs> OnRaiseAlarm;
        public event EventHandler<FAAlarmEventArgs> OnClearAlarm;

        private Equipment.EquipmentBase Equipment { get; set; }

        public AlarmRaisingStatusManager(Equipment.EquipmentBase equipment)
        {
            Equipment = equipment;
            RaisingAlarmList = new FAFramework.Utility.ThreadSafeObservableCollection<AlarmInfo>();

            TurnOffCommand = new CommandHandler(
                delegate
                {
                    if (Equipment != null)
                        Equipment.TurnOffSound();
                }, true);

            _raisingAlarmList.CollectionChanged +=
                delegate
                {
                    AlarmCount = _raisingAlarmList.Count;
                };
        }

        public void RaiseAlarm(object sender, FALibrary.Alarm.FAAlarmEventArgs e)
        {
            OnRaiseAlarm?.Invoke(sender, e);
            OnRaiseAlarm?.GetType();
            AlarmInfo alarmInfo = new AlarmInfo();

            if (e.Alarm.ContainsMetaProperty("Tag"))
            {
                var tag = e.Alarm.GetMetaPropertyValue("Tag");
                if (tag != null)
                {
                    if (tag is Utility.Alarm.AlarmMoreInfo)
                    {
                        var obj = tag as Utility.Alarm.AlarmMoreInfo;
                        if (!string.IsNullOrEmpty(obj.AutoImageName))
                        {
                            var imagePath = System.IO.Path.Combine(
                                ConfigClasses.GlobalConst.ROOT_PATH,
                                "Image",
                                obj.AutoImageName + ".png");

                            if (System.IO.File.Exists(imagePath))
                                e.Alarm.ImagePath = imagePath;
                        }
                    }
                }
            }

            alarmInfo.RaisedTime = DateTime.Now;
            alarmInfo.AlarmEventArgs = e;
            if (sender is FALibrary.Sequence.FASequence)
            {
                FALibrary.Sequence.FASequence sequence = sender as FALibrary.Sequence.FASequence;
                SetSequenceRetryCommand(alarmInfo, sequence);
            }
            else
            {
                alarmInfo.ClearAlarm =
                    new CommandHandler(
                        obj =>
                        {
                            ClearAlarm(alarmInfo);
                        }, true);
            }

            App.Current.Dispatcher.Invoke(
                new Action(
                    delegate
                    {
                        RaisingAlarmList.Add(alarmInfo);
                        CurrentAlarmInfo = alarmInfo;
                    }));
        }

        public void AllClear()
        {
            if (Equipment.AlarmClearable() == true)
            {
                while (RaisingAlarmList.Count > 0)
                {
                    ClearAlarm(RaisingAlarmList.First());
                }
            }
        }

        public void ClearCurrentAlarm()
        {
            if (Equipment.AlarmClearable() == true)
            {
                if (CurrentAlarmInfo == null) return;

                ClearAlarm(CurrentAlarmInfo);
            }
        }

        private void SetSequenceRetryCommand(AlarmInfo alarmInfo, FALibrary.Sequence.FASequence sequence)
        {
            Action<object, PropertyChangedEventArgs> confirmStatus = null;

            PropertyChangedEventHandler changedStatus =
                    delegate (object sender, PropertyChangedEventArgs e)
                    {
                        if (confirmStatus != null)
                            confirmStatus(sender, e);
                    };

            alarmInfo.ClearAlarm =
                new CommandHandler(
                    obj =>
                    {
                        if (Equipment.AlarmClearable() == false) return;

                        Equipment.TurnOffSound();
                        ClearAlarm(alarmInfo);
                        sequence.PropertyChanged -= changedStatus;
                        if (Equipment.State == Equipment.StateRun ||
                            Equipment.State == Equipment.StateRundown)
                        {
                            sequence.Resume();
                        }
                    }, false);

            if (sequence.State == FALibrary.Sequence.SequenceState.Suspended)
                alarmInfo.ClearAlarm.SetCanExecute(true);
            else
                sequence.OnSuspended +=
                    delegate (object sender, EventArgs e)
                    {
                        try
                        {
                            alarmInfo.ClearAlarm.SetCanExecute(true);
                        }
                        catch (Exception exception)
                        {
                            Manager.LogManager.Instance.WriteSystemLog(exception.ToString());
                        }
                    };
        }

        private void ClearAlarm(object obj)
        {
            if (Equipment.AlarmClearable() == false) return;

            App.Current.Dispatcher.Invoke(
                new Action(
                    delegate
                    {
                        try
                        {
                            OnClearAlarm?.Invoke(this, (obj as AlarmInfo).AlarmEventArgs);

                            RaisingAlarmList.Remove(obj as AlarmInfo);

                            if (RaisingAlarmList.Count == 0)
                            {
                            }
                            else
                            {
                                CurrentAlarmInfo = RaisingAlarmList.Last();
                            }
                        }
                        catch
                        {
                        }
                    }));

            Equipment.ClearAlarm();
        }
    }
}
