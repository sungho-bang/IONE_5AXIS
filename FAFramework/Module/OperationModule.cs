using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary;
using FALibrary.Sequence;
using FALibrary.Utility;
using FALibrary.Alarm;
using FAFramework.GUI;
using System.Text.RegularExpressions;
using FAFramework.Utility;
using System.ComponentModel;



namespace FAFramework.Module
{
    public class OperationModule : FAModule
    {
        #region Sequences
        [FAAttribute("Sequences")]
        public FASequence PreStart { get; set; }
        [FAAttribute("Sequences")]
        public FASequence PreStop { get; set; }
        [FAAttribute("Sequences")]
        public FASequence PreInitialize { get; set; }
        [FAAttribute("Sequences")]
        public FASequence PreEmergency { get; set; }
        [FAAttribute("Sequences")]
        public FASequence PreAlarm { get; set; }
        [FAAttribute("Sequences")]
        public FASequence PreWarning { get; set; }
        [FAAttribute("Sequences")]
        public FASequence PreSuspend { get; set; }
        #endregion

        #region Status
        private FAAlarm _lastAlarm;
        [FAAttribute("Status")]
        public FAAlarm LastAlarm
        {
            get { return _lastAlarm; }
            set
            {
                if (_lastAlarm == value) return;
                _lastAlarm = value;
                NotifyPropertyChanged("LastAlarm");
            }
        }

        private DateTime _startedTime;
        [FAAttribute("Status")]
        public DateTime StartedTime
        {
            get { return _startedTime; }
            set
            {
                if (_startedTime == value) return;
                _startedTime = value;
                NotifyPropertyChanged("StartedTime");
            }
        }

        private bool _lastAlarmIsRankingAlarm;
        [FAAttribute("Status")]
        public bool LastAlarmIsRankingAlarm
        {
            get { return _lastAlarmIsRankingAlarm; }
            set
            {
                if (_lastAlarmIsRankingAlarm == value) return;
                _lastAlarmIsRankingAlarm = value;
                NotifyPropertyChanged("LastAlarmIsRankingAlarm");
            }
        }
        #endregion

        #region Time
        [FAAttribute("Time")]
        public FATime TimeStopTimeout { get; set; }
        [FAAttribute("Time")]
        public FATime TimePreStopTimeout { get; set; }
        #endregion

        #region Alarm
        [DefaultAlarmInfo(1, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        [AlarmDescription(KnownCulture.EnglishUS, "Main Air Pressure Off", "Check Main Air Valve")]
        [AlarmDescription(KnownCulture.Korean, "메인 에어 감지 안 됨.", "메인 에어 벨브를 확인하세요.")]
        public int AlarmMainAirPressureOff { get; set; }
        #endregion

        protected List<FASequence> PreStartSequence { get; set; }
        protected List<FASequence> PreInitializeSequence { get; set; }

        private FALibrary.Alarm.FAAlarmEventArgs LastAlarmEventArgs { get; set; }

        public OperationModule()
        {
            PreStartSequence = new List<FASequence>();
            PreInitializeSequence = new List<FASequence>();
        }

        public override void InitializeSequence()
        {
            AddPreStartSequences();

            MakePreStart();
            MakePreStop();
            MakePreInitialize();
            MakePreEmergency();
            MakePreAlarm();
            MakePreWarning();
            MakePreSuspend();

            SetEventHandler();
            SetProcessAlarmHandler();
        }

        protected virtual void SetEventHandler()
        {
            #region State Execute
            Equipment.StateRun.CustomActions.Add(CheckMainAirPressure);
            Equipment.StatePreRun.CustomActions.Add(CheckMainAirPressure);
            Equipment.StateRundown.CustomActions.Add(CheckMainAirPressure);
            Equipment.StatePreInitialize.CustomActions.Add(CheckMainAirPressure);
            Equipment.StateInitialize.CustomActions.Add(CheckMainAirPressure);
            Equipment.StateStop.CustomActions.Add(CheckMainAirPressure);
            #endregion

            #region State Change
            Equipment.StatePreRun.OnChangedState +=
                delegate (object sender, FALibrary.FAGenericEventArgs<Equipment.EquipmentState> e)
                {
                    PreStart.Start();
                };

            Equipment.StatePreStop.OnChangedState +=
                delegate (object sender, FALibrary.FAGenericEventArgs<Equipment.EquipmentState> e)
                {
                    PreStop.Start();
                };

            Equipment.StatePreInitialize.OnChangedState +=
                delegate (object sender, FALibrary.FAGenericEventArgs<Equipment.EquipmentState> e)
                {
                    PreInitialize.Start();
                };

            Equipment.StatePreEmergency.OnChangedState +=
                delegate (object sender, FALibrary.FAGenericEventArgs<Equipment.EquipmentState> e)
                {
                    PreEmergency.Start();
                };

            Equipment.StatePreAlarm.OnChangedState +=
                delegate (object sender, FALibrary.FAGenericEventArgs<Equipment.EquipmentState> e)
                {
                    PreAlarm.Start();
                };

            Equipment.StatePreWarning.OnChangedState +=
                delegate (object sender, FALibrary.FAGenericEventArgs<Equipment.EquipmentState> e)
                {
                    PreWarning.Start();
                };

            Equipment.StatePreSuspend.OnChangedState +=
                delegate (object sender, FALibrary.FAGenericEventArgs<Equipment.EquipmentState> e)
                {
                    PreSuspend.Start();
                };

            Equipment.StateRun.OnChangedState +=
                delegate (object sender, FALibrary.FAGenericEventArgs<Equipment.EquipmentState> e)
                {
                    if (Equipment.PackingLogManager == null) return;

                    Equipment.PackingLogManager.WriteEventLog(
                        new Utility.PackingLog.EventLog
                        {
                            State = Utility.PackingLog.EventLog.EState.Run,
                            Event = "Packing Start"
                        });
                };

            Equipment.StateRundown.OnChangedState +=
                delegate (object sender, FALibrary.FAGenericEventArgs<Equipment.EquipmentState> e)
                {
                    if (Equipment.PackingLogManager == null) return;

                    Equipment.PackingLogManager.WriteEventLog(
                        new Utility.PackingLog.EventLog
                        {
                            State = Utility.PackingLog.EventLog.EState.Stop,
                            Event = "Run down"
                        });
                };

            Equipment.StatePreStop.OnChangedState +=
                delegate (object sender, FALibrary.FAGenericEventArgs<Equipment.EquipmentState> e)
                {
                    if (Equipment.PackingLogManager == null) return;

                    string eventName = string.Empty;

                    if (e.Value == Equipment.StateAlarm ||
                        e.Value == Equipment.StateWarning ||
                        e.Value == Equipment.StatePreAlarm ||
                        e.Value == Equipment.StatePreWarning)
                        eventName = "Alarm Stop";
                    else if (e.Value == Equipment.StateRun || e.Value == Equipment.StateRundown)
                        eventName = "Manual Stop";

                    if (string.IsNullOrEmpty(eventName) == true) return;

                    Equipment.PackingLogManager.WriteEventLog(
                        new Utility.PackingLog.EventLog
                        {
                            State = Utility.PackingLog.EventLog.EState.Stop,
                            Event = eventName
                        });
                };

            Equipment.StateInitialize.OnChangedState +=
                delegate
                {
                    LastAlarmEventArgs = null;
                };
            #endregion

            #region SAMSUNG TP MARS LOG

            Utility.SamsungTPLog.AlarmLog CreateAlarmLogForStopEvent(string status, string stopType)
            {
                var log = new Utility.SamsungTPLog.AlarmLog();
                log.DeviceID = "EQUIPMENT";
                log.EventID = "EQP_STOP";
                log.AlarmCode = "US_001";
                log.Status = status;
                log.AddData("DESCRIPTION", stopType);
                return log;
            }

            Utility.SamsungTPLog.AlarmLog CreateAlarmLogForAlarmRaise(FAAlarm alarm, string status)
            {
                var log = new Utility.SamsungTPLog.AlarmLog();
                if (alarm.ContainsMetaProperty("MODULE_NAME"))
                    log.DeviceID = alarm.GetMetaPropertyValue("MODULE_NAME").ToString();
                else
                    log.DeviceID = "UNKNOWN";
                log.EventID = AlarmTypeToString(alarm);
                log.AlarmCode = alarm.AlarmNo.ToString();
                log.Status = status;
                log.AddData("DESCRIPTION", alarm.AlarmName);
                return log;
            }

            Equipment.StateStop.OnChangedState +=
                delegate (object sender, FALibrary.FAGenericEventArgs<Equipment.EquipmentState> e)
                {
                    string stopType = "USER_STOP";

                    if (e.Value == Equipment.StateAlarm ||
                        e.Value == Equipment.StateWarning)
                    {
                        stopType = "ERROR_STOP";
                    }

                    var log = CreateAlarmLogForStopEvent("OCCURRED", stopType);
                    Manager.TPLogManager.Instance.WriteLog(log);
                };

            Equipment.StateRun.OnChangedState +=
                delegate
                {
                    string stopType = "USER_STOP";

                    if (LastAlarmEventArgs != null)
                    {
                        stopType = "ERROR_STOP";
                        LastAlarmEventArgs = null;
                    }

                    var log = CreateAlarmLogForStopEvent("RELEASED", stopType);
                    Manager.TPLogManager.Instance.WriteLog(log);
                };

            Equipment.OnRaiseAlarm +=
                (sender, e) =>
                {
                    if (!(Equipment.State == Equipment.StateRun ||
                        Equipment.State == Equipment.StateRundown))
                        return;

                    if (sender is FASequence)
                    {
                        var seq = sender as FASequence;
                        e.Alarm.SetMetaPropertyValue("SEQUENCE", sender);

                        if (seq.ContainsMetaProperty("OwnerModule"))
                            e.Alarm.SetMetaPropertyValue("MODULE", seq.GetMetaPropertyValue("OwnerModule"));
                        else
                            e.Alarm.SetMetaPropertyValue("MODULE", "UNKNOWN");

                        if (seq.ContainsMetaProperty("OwnerModuleName"))
                            e.Alarm.SetMetaPropertyValue("MODULE_NAME", seq.GetMetaPropertyValue("OwnerModuleName"));
                        else
                            e.Alarm.SetMetaPropertyValue("MODULE_NAME", "UNKNOWN");

                        var alarm = e.Alarm;
                        var log = CreateAlarmLogForAlarmRaise(alarm, "OCCURRED");
                        Manager.TPLogManager.Instance.WriteLog(log);

                        LastAlarmEventArgs = null;
                    }
                };

            Equipment.AlarmRaisingStatusManager.OnClearAlarm +=
                (o, e) =>
                {
                    LastAlarmEventArgs = e;

                    var alarm = e.Alarm;
                    var log = CreateAlarmLogForAlarmRaise(alarm, "RELEASED");
                    Manager.TPLogManager.Instance.WriteLog(log);
                };
            #endregion SAMSUNG TP MARS LOG
        }

        protected virtual void SetProcessAlarmHandler()
        {
            Equipment.StatePreRun.ProcessAlarm = ProcessAlarm;
            Equipment.StatePreInitialize.ProcessAlarm = ProcessAlarm;
            Equipment.StatePreSuspend.ProcessAlarm = ProcessAlarm;
            Equipment.StateRun.ProcessAlarm = ProcessAlarm;
            Equipment.StateInitialize.ProcessAlarm = ProcessAlarm;
            Equipment.StateSuspend.ProcessAlarm = ProcessAlarm;
            Equipment.StateRundown.ProcessAlarm = ProcessAlarm;
            Equipment.StateStop.ProcessAlarm = ProcessAlarm;
        }

        protected virtual void AddPreStartSequences()
        {
        }

        protected virtual void MakePreStart()
        {
            var seq = PreStart;

            AddDebugLogToSequence(seq);

            seq.AddItem(
                delegate (FASequence actor, TimeSpan time)
                {
                    if (Equipment.IsInitializedOk)
                    {
                        Manager.MessageWindowManager.Instance.CloseWindow("YouCanStartAfterInitializing");
                        actor.NextStep();
                    }
                    else
                    {
                        Manager.MessageWindowManager.Instance.Show(Equipment, "YouCanStartAfterInitializing",
                            Utility.UtilityClass.GetStringResource(this, "YouCanStartAfterInitializing", "You can start machine after initialinzing."));
                        Equipment.RequestStop();
                        actor.NextStep();
                    }
                });
            //210208
            //seq.AddItem(
            //    delegate (FASequence actor, TimeSpan time)
            //    {
            //        if (Equipment.CurrentUser == null ||
            //            Equipment.CurrentUser.Permission != FAFramework.Equipment.UserPermissionTypes.OPERATOR ||
            //            string.IsNullOrEmpty(Equipment.CurrentUser.Name))
            //        {
            //            Manager.MessageWindowManager.Instance.Show(Equipment, "YouCanStartAfterOperatorLogin",
            //                Utility.UtilityClass.GetStringResource(this, "YouCanStartAfterOperatorLogin", "You can start after operator login."));

            //            Equipment.RequestStop();
            //            actor.NextStep();
            //        }
            //        else
            //        {
            //            Manager.MessageWindowManager.Instance.CloseWindow("YouCanStartAfterOperatorLogin");
            //            actor.NextStep();
            //        }
            //    });
            //seq.AddItem(
            //    delegate (FASequence actor, TimeSpan time)
            //    {
            //        if (Equipment.CurrentUser == null ||
            //            Equipment.CurrentUser.Permission != FAFramework.Equipment.UserPermissionTypes.OPERATOR ||
            //            string.IsNullOrEmpty(Equipment.CurrentUser.Name) ||
            //            Equipment.CurrentUser.Permission == FAFramework.Equipment.UserPermissionTypes.MASTER)
            //        {
            //            actor.NextStep();
            //        }
            //        else
            //        {
            //            Manager.MessageWindowManager.Instance.CloseWindow("YouCanStartAfterOperatorLogin");
            //            actor.NextStep();
            //        }
            //    });
            seq.AddItem(DoStartPreStartSequences);
            seq.AddItem(ConfirmTerminatedPreStartSequences);
            seq.AddItem(DoStart);
        }

        protected virtual void MakePreStop()
        {
            var seq = PreStop;

            AddDebugLogToSequence(seq);

            seq.AddStep("StopSequences").StepIndex = seq.AddItem(StopSequences);

            seq.AddItem(ConfirmStopedSequences);
            seq.AddItem(UnlockDoor);
            seq.AddItem(DoStop);
        }

        protected virtual void MakePreInitialize()
        {
            var seq = PreInitialize;

            AddDebugLogToSequence(seq);

            QuestionMessageBoxWindow questionWindow = null;
            seq.AddItem(
                delegate (object obj)
                {
                    App.Current.Dispatcher.Invoke(
                        new Action(
                            delegate
                            {
                                questionWindow = new QuestionMessageBoxWindow();
                                questionWindow.Message = Utility.UtilityClass.GetStringResource(this, "AreYouSureYouWantToInitializing", "Are you sure you want to initializing?");
                                questionWindow.EquipmentInstance = Equipment;
                                questionWindow.Show();
                            }), null);
                });
            seq.AddItem(
                delegate (FASequence actor, TimeSpan time)
                {
                    if (questionWindow.Result == QuestionMessageBoxWindow.QuestionResult.Yes)
                    {
                        FAFramework.Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.MainLoopModule.InitializeSelect = true;
                        actor.NextStep();
                    }
                    else if (questionWindow.Result == QuestionMessageBoxWindow.QuestionResult.No)
                    {
                        FAFramework.Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.MainLoopModule.InitializeSelect = false;
                        Equipment.RequestStop();
                        actor.NextTerminate();
                    }
                });

            seq.AddItem(DoStartPreInitializeSequences);
            seq.AddItem(ConfirmTerminatedPreInitializeSequences);
            seq.AddItem(ConfirmDoorClosed);
            seq.AddItem(LockDoor);
            seq.AddItem(new FATime(FATimeType.millisecond, 1000));
            seq.AddItem(ConfirmDoorLockOk);  
            seq.AddItem(DoInitialize);
        }

        protected virtual void MakePreEmergency()
        {
            var seq = PreEmergency;

            AddDebugLogToSequence(seq);

            seq.AddItem(DoChangeStateToEmergency);
        }

        protected virtual void MakePreAlarm()
        {
            var seq = PreAlarm;

            AddDebugLogToSequence(seq);

            seq.AddStep("StopSequences").StepIndex = seq.AddItem(StopSequences);
            seq.AddItem(ConfirmStopedSequences);
            seq.AddItem(UnlockDoor);
            seq.AddItem(DoChangeStateToAlarm);
        }

        protected virtual void MakePreWarning()
        {
            var seq = PreWarning;

            AddDebugLogToSequence(seq);

            seq.AddStep("StopSequences").StepIndex = seq.AddItem(StopSequences);
            seq.AddItem(ConfirmStopedSequences);
            seq.AddItem(UnlockDoor);
            seq.AddItem(DoChangeStateToWarning);
        }

        protected virtual void MakePreSuspend()
        {
            var seq = PreSuspend;

            AddDebugLogToSequence(seq);

            seq.AddStep("StopSequences").StepIndex = seq.AddItem(StopSequences);
            seq.AddItem(ConfirmStopedSequences);
            seq.AddItem(DoChangeStateToSuspend);
        }

        protected virtual void ConfirmDoorClosed(FASequence actor, TimeSpan time)
        {
        }

        protected virtual void LockDoor(object sender)
        {
        }

        protected virtual void UnlockDoor(object sender)
        {
        }

        protected virtual void ConfirmDoorLockOk(FASequence actor, TimeSpan time)
        {
        }

        protected void DoStart(object sender)
        {
            Equipment.Start();
            StartedTime = DateTime.Now;
        }

        protected void DoStop(object sender)
        {
            Equipment.Stop();
        }

        protected void DoInitialize(object sender)
        {
            Equipment.Initialize();
            LastAlarm = null;
        }

        protected void StopSequences(object sender)
        {
            Equipment.SuspendSubSequences();
        }

        protected void ConfirmStopedSequences(FASequence actor, TimeSpan time)
        {
            if (Equipment.IsSuspendedSubSuquences())
            {
                actor.NextStep();
            }
            else if (TimeStopTimeout.Time < time)
            {
                actor.NextStep("StopSequences");
            }
        }

    

        protected void DoStartPreStartSequences(object sender)
        {
            if (PreStartSequence == null) return;

            foreach (var item in PreStartSequence)
            {
                item.ClearState();
                item.Start();
            }
        }

        protected void ConfirmTerminatedPreStartSequences(FASequence actor, TimeSpan time)
        {
            bool isAllTerminated = true;

            if (PreStartSequence != null)
            {
                foreach (var item in PreStartSequence)
                {
                    if (item == null) continue;

                    if ((item.IsStartable() ||
                        item.IsRestartable()) == false) isAllTerminated = false;
                }
            }

            if (isAllTerminated)
                actor.NextStep();
        }

        protected void DoStartPreInitializeSequences(object sender)
        {
            if (PreInitializeSequence == null) return;

            foreach (var item in PreInitializeSequence)
            {
                item.ClearState();
                item.Start();
            }
        }

        protected void ConfirmTerminatedPreInitializeSequences(FASequence actor, TimeSpan time)
        {
            bool isAllTerminated = true;

            if (PreInitializeSequence != null)
            {
                foreach (var item in PreInitializeSequence)
                {
                    if (item == null) continue;

                    if ((item.IsStartable() ||
                        item.IsRestartable()) == false) isAllTerminated = false;
                }
            }

            if (isAllTerminated)
                actor.NextStep();
        }

        protected void DoChangeStateToAlarm(object sender)
        {
            Equipment.SetAlarm();
        }

        protected void DoChangeStateToWarning(object sender)
        {
            Equipment.SetWarning();
        }

        protected void DoChangeStateToEmergency(object sender)
        {
            Equipment.SetEmergency();
        }

        protected void DoChangeStateToSuspend(object sender)
        {
            Equipment.Suspend();
        }

        protected virtual void ProcessAlarm(FALibrary.Alarm.FAAlarmEventArgs e)
        {
            Manager.AlarmLogInfo log = new Manager.AlarmLogInfo();
            log.Alarm = e;

            bool isAutoRunning = Equipment.State == Equipment.StateRun ||
                Equipment.State == Equipment.StateRundown;

            log.AutoRunning = isAutoRunning;

            if (isAutoRunning)
            {
                log.Alarm = e;
                log.RankingData = IsRankingData(e.Alarm);
                LastAlarmIsRankingAlarm = log.RankingData;
                LastAlarm = e.Alarm;
            }

            if (e.Alarm.Status == ConfigClasses.GlobalConst.ALARM)
            {
                if (log.AutoRunning && log.RankingData)
                    Equipment.MTBIManager.AddAlarm(e.Alarm);

                Equipment.RaiseAlarm();
            }
            else if (e.Alarm.Status == ConfigClasses.GlobalConst.WARNING)
                Equipment.RaiseWarning();

            Manager.LogManager.Instance.WriteAlarmLog(Equipment, log);
            Manager.LogManager.Instance.WriteTraceLog(Equipment,
                string.Format("Raise Alarm : {0}, {1}",
                e.Alarm.AlarmNo,
                e.Alarm.AlarmName));

            var regEx = new Regex("\\[[^\\[]*]");
            string unitName = "UNKNOWN";
            try
            {
                var result = regEx.Match(log.Alarm.Alarm.AlarmName);

                if (result != null)
                    unitName = result.Value;
            }
            catch
            {
            };

            Equipment.AlarmRaisingStatusManager.RaiseAlarm(this, e);

            if (Equipment.PackingLogManager != null)
            {
                Equipment.PackingLogManager.WriteAlarmLog(
                    new Utility.PackingLog.AlarmLog
                    {
                        AlarmCode = log.Alarm.Alarm.AlarmNo,
                        AlarmDescription = log.Alarm.Alarm.AlarmName,
                        UnitName = unitName
                    });

                Equipment.PackingLogManager.WriteEventLog(
                    new Utility.PackingLog.EventLog
                    {
                        State = Utility.PackingLog.EventLog.EState.Error,
                        Event = log.Alarm.Alarm.AlarmName
                    });
            }
        }

        protected bool IsRankingData(FAAlarm alarm)
        {
            if (LastAlarm != null)
            {
                if ((DateTime.Now - StartedTime).TotalSeconds > Equipment.Config.JamDelay)
                {
                    return true;
                }
            }
            else
            {
                return true;
            }

            return false;
        }

        protected void AddDebugLogToSequence(FASequence seq)
        {
            seq.OnStart +=
                delegate
                {
                    WriteDebugLog(string.Format("Start {0}.{1}", Name, seq.Name));
                };

            seq.OnStop +=
                delegate
                {
                    WriteDebugLog(string.Format("Stop {0}.{1}", Name, seq.Name));
                };

            seq.OnTerminate +=
                delegate
                {
                    WriteDebugLog(string.Format("Terminate {0}.{1}", Name, seq.Name));

                    seq.Stop();
                    seq.ClearState();
                    seq.Start();
                };
        }

        private string AlarmTypeToString(FAAlarm alarm)
        {
            if (alarm.Status == ConfigClasses.GlobalConst.ALARM)
                return "ALARM";
            else if (alarm.Status == ConfigClasses.GlobalConst.WARNING)
                return "WARNING";
            else
                return string.Empty;
        }

        private void CheckMainAirPressure(Equipment.EquipmentBase equipment, ref Equipment.EquipmentState state)
        {
            if (!IsMainAirOn())
            {
                ShowMessage("Main Air Pressure Off", AlarmMainAirPressureOff, "Main Air Pressure Off");
                equipment.RequestStop();
            }
        }

        protected virtual bool IsMainAirOn()
        {
            return false;
        }
    }
}
