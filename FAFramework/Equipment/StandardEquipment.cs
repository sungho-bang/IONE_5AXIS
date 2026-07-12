using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary;
using FALibrary.Part.MemoryBasePart;
using FALibrary.Sequence;
using FAFramework.Utility;

namespace FAFramework.Equipment
{
    public class StandardEquipment : EquipmentBase
    {
        #region State Classes
        public class PreRunState : EquipmentState
        {
            public PreRunState(EquipmentBase equipment)
                : base(equipment)
            {
                Name = "PreRunState";
            }

            public override void Start(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StateRun;
            }

            public override void RequestStop(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreStop;
            }

            public override void RequestEmergency(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreEmergency;
            }

            public override void RaiseAlarm(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreAlarm;
            }

            public override void RaiseWarning(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreWarning;
            }
        }

        public class RunState : EquipmentState
        {
            public RunState(EquipmentBase equipment)
                : base(equipment)
            {
                Name = "RunState";
            }

            public override void RequestStop(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreStop;
            }

            public override void SetRundown(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StateRundown;
            }

            public override void RequestEmergency(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreEmergency;
            }

            public override void RaiseAlarm(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreAlarm;
            }

            public override void SetSuspend(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreSuspend;
            }

            public override void RaiseWarning(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreWarning;
            }
        }

        public class PreStopState : EquipmentState
        {
            public PreStopState(EquipmentBase equipment)
                : base(equipment)
            {
                Name = "PreStopState";
            }

            public override void Stop(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StateStop;
            }

            public override void RequestEmergency(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreEmergency;
            }

            public override void RaiseAlarm(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreAlarm;
            }

            public override void RaiseWarning(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreWarning;
            }
        }

        public class StopState : EquipmentState
        {
            public StopState(EquipmentBase equipment)
                : base(equipment)
            {
                Name = "StopState";
            }

            public override void RequestStart(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreRun;
            }

            public override void RequestInitialize(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreInitialize;
            }

            public override void RequestEmergency(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreEmergency;
            }
        }

        public class PreInitializeState : EquipmentState
        {
            public PreInitializeState(EquipmentBase equipment)
                : base(equipment)
            {
                Name = "PreInitializeState";
            }

            public override void RequestStop(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreStop;
            }

            public override void Initialize(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StateInitialize;
            }

            public override void RequestEmergency(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreEmergency;
            }

            public override void RaiseAlarm(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreAlarm;
            }

            public override void RaiseWarning(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreWarning;
            }
        }

        public class InitializeState : EquipmentState
        {
            public InitializeState(EquipmentBase equipment)
                : base(equipment)
            {
                Name = "InitializeState";
            }

            public override void RequestStop(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreStop;
            }

            public override void RequestEmergency(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreEmergency;
            }

            public override void RaiseAlarm(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreAlarm;
            }

            public override void RaiseWarning(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreWarning;
            }
        }

        public class RundownState : EquipmentState
        {
            public RundownState(EquipmentBase equipment)
                : base(equipment)
            {
                Name = "RundownState";
            }

            public override void RequestStart(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StateRun;
            }

            public override void RequestStop(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreStop;
            }

            public override void RequestEmergency(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreEmergency;
            }

            public override void RaiseAlarm(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreAlarm;
            }

            public override void SetSuspend(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreSuspend;
            }

            public override void RaiseWarning(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreWarning;
            }
        }

        public class PreEmergencyState : EquipmentState
        {
            public PreEmergencyState(EquipmentBase equipment)
                : base(equipment)
            {
                Name = "PreEmergencyState";
            }

            public override void SetEmergency(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StateEmergency;
            }
        }

        public class EmergencyState : EquipmentState
        {
            public EmergencyState(EquipmentBase equipment)
                : base(equipment)
            {
                Name = "EmergencyState";
            }

            public override void ReleaseEmergency(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StateEmergencyReset;
            }
        }

        public class EmergencyResetState : EquipmentState
        {
            public EmergencyResetState(EquipmentBase equipment)
                : base(equipment)
            {
                Name = "EmergencyResetState";
            }
        }

        public class PreAlarmState : EquipmentState
        {
            public PreAlarmState(EquipmentBase equipment)
                : base(equipment)
            {
                Name = "PreAlarmState";
            }

            public override void RequestEmergency(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreEmergency;
            }

            public override void SetAlarm(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StateAlarm;
            }
        }

        public class AlarmState : EquipmentState
        {
            public AlarmState(EquipmentBase equipment)
                : base(equipment)
            {
                Name = "AlarmState";
            }

            public override void RequestEmergency(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreEmergency;
            }

            public override void ClearAlarm(EquipmentState oldStatus)
            {
                base.ClearAlarm(oldStatus);
                Equipment.State = Equipment.StateStop;
            }
        }

        public class PreWarningState : EquipmentState
        {
            public PreWarningState(EquipmentBase equipment)
                : base(equipment)
            {
                Name = "PreWarningState";
            }

            public override void RequestEmergency(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreEmergency;
            }

            public override void SetWarning(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StateWarning;
            }
        }

        public class WarningState : EquipmentState
        {
            public WarningState(EquipmentBase equipment)
                : base(equipment)
            {
                Name = "WarningState";
            }

            public override void RequestEmergency(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreEmergency;
            }

            public override void ClearAlarm(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StateStop;
            }
        }

        public class PreSuspendState : EquipmentState
        {
            public PreSuspendState(EquipmentBase equipment)
                : base(equipment)
            {
                Name = "PreSuspendState";
            }

            public override void RequestEmergency(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreEmergency;
            }

            public override void RaiseAlarm(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreAlarm;
            }

            public override void RaiseWarning(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreWarning;
            }

            public override void Suspend(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StateSuspend;
            }
        }

        public class SuspendState : EquipmentState
        {
            public SuspendState(EquipmentBase equipment)
                : base(equipment)
            {
                Name = "SuspendState";
            }

            public override void RequestStop(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreStop;
            }

            public override void RequestEmergency(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreEmergency;
            }

            public override void RaiseAlarm(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreAlarm;
            }

            public override void RaiseWarning(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreWarning;
            }

            public override void ReleaseSuspend(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StateRun;
            }
        }

        public class EmergencyAlarmStopState : EquipmentState
        {
            public EmergencyAlarmStopState(EquipmentBase equipment)
                : base(equipment)
            {
                Name = "EmergencyAlarmStopState";
            }

            public override void RequestEmergency(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreEmergency;
            }

            public override void RaiseAlarm(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreAlarm;
            }
        }
        #endregion

        #region Status        
        private bool _pressedStartButton;
        public bool PressedStartButton
        {
            get { return _pressedStartButton; }
            set
            {
                if (_pressedStartButton == value) return;
                _pressedStartButton = value;
                NotifyPropertyChanged("PressedStartButton");
                if (value == true)
                {
                    StartButtonPressed?.Invoke(this, EventArgs.Empty);
                    RequestStart();
                }
            }
        }

        private bool _pressedStopButton;
        public bool PressedStopButton
        {
            get { return _pressedStopButton; }
            set
            {
                if (_pressedStopButton == value) return;
                _pressedStopButton = value;
                NotifyPropertyChanged("PressedStopButton");
                if (value == true)
                {
                    StopButtonPressed?.Invoke(this, EventArgs.Empty);
                    RequestStop();
                }
            }
        }

        private bool _pressedInitialButton;
        public bool PressedInitialButton
        {
            get { return _pressedInitialButton; }
            set
            {
                if (_pressedInitialButton == value) return;
                _pressedInitialButton = value;
                NotifyPropertyChanged("PressedInitialButton");
                if (value == true)
                    RequestInitialize();
            }
        }

        private bool _pressedAlarmClearButton;
        public bool PressedAlarmClearButton
        {
            get { return _pressedAlarmClearButton; }
            set
            {
                if (_pressedAlarmClearButton == value) return;
                _pressedAlarmClearButton = value;
                NotifyPropertyChanged("PressedAlarmClearButton");
                if (value == true)
                {
                    if (AlarmClearable())
                        AlarmRaisingStatusManager.ClearCurrentAlarm();
                }
            }
        }

        private bool _pressedSoundClearButton;
        public bool PressedSoundClearButton
        {
            get { return _pressedSoundClearButton; }
            set
            {
                if (_pressedSoundClearButton == value) return;
                _pressedSoundClearButton = value;
                NotifyPropertyChanged("PressedSoundClearButton");

                if (value == true)
                {
                    TurnOffSound();
                }
            }
        }

        private bool _pressedEmergencyButton;
        public bool PressedEmergencyButton
        {
            get { return _pressedEmergencyButton; }
            set
            {
                if (_pressedEmergencyButton == value) return;
                _pressedEmergencyButton = value;
                NotifyPropertyChanged("PressedEmergencyButton");

                if (value == true)
                {
                    //CommonUnit.EmergencyReset.DoTurnOff(this);
                }
            }
        }
        #endregion

        #region SubUnits
        [FAAttribute("SubUnits")]
        [Utility.SubSequenceManagerName("MainSequenceManager")]
        public CommonSubUnit CommonUnit { get; set; }
        [FAAttribute("SubUnits")]
        [Utility.SubSequenceManagerName("MainSequenceManager")]
        public OperationPanelSubUnit OperationUnit { get; set; }
        #endregion

        [FAIncludeSequenceAttribute("Modules", "SystemBackgroundManager")]
        public FASystemModule SystemModule { get; set; }

        public event EventHandler StartButtonPressed;
        public event EventHandler StopButtonPressed;

        public StandardEquipment()
        {
            OnRaiseAlarm +=
                delegate (object sender, FALibrary.Alarm.FAAlarmEventArgs e)
                {
                    if (State != null && State.ProcessAlarm != null)
                        State.ProcessAlarm(e);
                };
        }

        public override void AssignModule()
        {
            base.AssignModule();
            SystemModule.OperationUnit = OperationUnit;
        }

        protected override void CreateStates()
        {
            StatePreRun = new PreRunState(this);
            StateRun = new RunState(this);
            StatePreStop = new PreStopState(this);
            StateStop = new StopState(this);
            StatePreInitialize = new PreInitializeState(this);
            StateInitialize = new InitializeState(this);
            StateRundown = new RundownState(this);
            StatePreEmergency = new PreEmergencyState(this);
            StateEmergency = new EmergencyState(this);
            StateEmergencyReset = new EmergencyResetState(this);
            StatePreAlarm = new PreAlarmState(this);
            StateAlarm = new AlarmState(this);
            StatePreWarning = new PreWarningState(this);
            StateWarning = new WarningState(this);
            StatePreSuspend = new PreSuspendState(this);
            StateSuspend = new SuspendState(this);
        }

        protected override void SetStatesCustomActions()
        {
        }

        protected override void SetStateChangedEventHandler()
        {
            StateEmergency.OnChangedState +=
                EventHandlerOnStateChangeForEmergency;
        }

        protected override void ActionAfterInitialize()
        {
            base.ActionAfterInitialize();
            SystemModule.ForceStopSequence.Start();
        }

        protected override void ProcInput()
        {
            if (CommonUnit.EmergencyReset.IsOn)
                ReleaseEmergency();
            if (CommonUnit.EmergencyOff.IsOn)
            {
                CommonUnit.EmergencyReset.DoTurnOff(this);
                RequestEmergency();
            }

            PressedStartButton = OperationUnit.StartButtonSwitch.IsOn;
            PressedStopButton = OperationUnit.StopButtonSwitch.IsOn;
            PressedInitialButton = OperationUnit.InitializeButtonSwitch.IsOn;
            PressedAlarmClearButton = OperationUnit.AlarmClearButtonSwitch.IsOn;
            PressedSoundClearButton = OperationUnit.SoundClearButtonSwitch.IsOn;
        }

        protected override void SetDefaultState()
        {
            State = StateStop;
        }

        private void EventHandlerOnStateChangeForEmergency(object sender, EventArgs e)
        {
            IsInitializedOk = false;
            MainSequenceManager.AllClearState();
            ClearSubSequencesState();
        }

        public override bool AlarmClearable()
        {
            if (State == StateAlarm ||
                State == StateWarning ||
                State == StateStop)
                return true;
            else
                return false;
        }
    }

    public partial class CommonSubUnit : SubUnitBase
    {
        [FAAttribute("")]
        public FAPartMemoryBaseIOList IOList { get; set; }
        [FAAttribute("")]
        public FAPartOnOffSensor EmergencyOff { get; set; }
        [FAAttribute("")]
        public FAPartOnOffSensor MainAirPressureOn { get; set; }
        [FAAttribute("")]
        public FAPartOnOffSensor ControlPowerOn { get; set; }
        [FAAttribute("")]
        public FAPartOnOff EmergencyReset { get; set; }
        [FAAttribute("")]
        public FAPartOnOff SignalTowerRed { get; set; }
        [FAAttribute("")]
        public FAPartOnOff SignalTowerYellow { get; set; }
        [FAAttribute("")]
        public FAPartOnOff SignalTowerGreen { get; set; }
        [FAAttribute("")]
        public FAPartOnOff SignalTowerBuzzer { get; set; }
        [FAAttribute("")]
        public FAPartOnOff SignalPhoneMelodie1 { get; set; }
        [FAAttribute("")]
        public FAPartOnOff SignalPhoneMelodie2 { get; set; }
        [FAAttribute("")]
        public FAPartOnOff SignalPhoneMelodie3 { get; set; }
        [FAAttribute("")]
        public FAPartOnOff SignalPhoneMelodie4 { get; set; }
    }

    public partial class OperationPanelSubUnit : SubUnitBase
    {
        [FAAttribute("")]
        public FAPartOnOffSensor StartButtonSwitch { get; set; }
        [FAAttribute("")]
        public FAPartOnOffSensor StopButtonSwitch { get; set; }
        [FAAttribute("")]
        public FAPartOnOffSensor InitializeButtonSwitch { get; set; }
        [FAAttribute("")]
        public FAPartOnOffSensor AlarmClearButtonSwitch { get; set; }
        [FAAttribute("")]
        public FAPartOnOffSensor SoundClearButtonSwitch { get; set; }
        [FAAttribute("")]
        public FAPartOnOff StartButtonLamp { get; set; }
        [FAAttribute("")]
        public FAPartOnOff StopButtonLamp { get; set; }
        [FAAttribute("")]
        public FAPartOnOff InitializeButtonLamp { get; set; }
        [FAAttribute("")]
        public FAPartOnOff AlarmClearButtonLamp { get; set; }
        [FAAttribute("")]
        public FAPartOnOff SoundClearButtonLamp { get; set; }
        [FAAttribute("")]
        public FAPartOnOffSensor FrontMonitorActiveSwitch { get; set; }
        [FAAttribute("")]
        public FAPartOnOffSensor RearMonitorActiveSwitch { get; set; }
        [FAAttribute("")]
        [FAFramework.Utility.HideOnScreenProperty]
        public FAPartOnOff FrontMonitorActiveSwitchLamp { get; set; }
        [FAAttribute("")]
        [FAFramework.Utility.HideOnScreenProperty]
        public FAPartOnOff RearMonitorActiveSwitchLamp { get; set; }
        [FAAttribute("")]
        [FAFramework.Utility.HideOnScreenProperty]
        public FAPartOnOff MonitorSelectSignal { get; set; }

        [FAAttribute("")]
        [FAFramework.Utility.HideOnScreenProperty]
        public FAPartOnOffSensor PendantEmergencySwitchCheck { get; set; }

        [FAAttribute("")]
        [FAFramework.Utility.HideOnScreenProperty]
        public FAPartOnOffSensor PendantTouchActivationSwitchCheck { get; set; }

        [FAAttribute("")]
        [FAFramework.Utility.HideOnScreenProperty]
        public FAPartOnOffSensor PendantEnablingSwitchCheck { get; set; }
    }

    public partial class FASystemModule : Module.FAModule
    {
        public OperationPanelSubUnit OperationUnit { get; set; }

        #region Sequence
        [FAAttribute("Sequence")]
        public FASequence ForceStopSequence { get; set; }
        #endregion

        #region Time
        [FAAttribute("Time")]
        public FALibrary.Utility.FATime ForceStopTime { get; set; }
        #endregion

        public override void InitializeSequence()
        {
            FAFramework.GUI.QuestionMessageBoxWindow questionWindow = null;

            var seq = ForceStopSequence;

            seq.AddStep("LoopHead").StepIndex = seq.AddItem(
                delegate (FASequence actor, TimeSpan time)
                {
                    if (Equipment.State != Equipment.StatePreStop &&
                        Equipment.State != Equipment.StatePreWarning &&
                        Equipment.State != Equipment.StatePreAlarm &&
                        Equipment.State != Equipment.StatePreSuspend)
                    {
                        actor.NextStep("LoopHead");
                    }
                    else if (OperationUnit.StopButtonSwitch.IsOn)
                    {
                        if (time > ForceStopTime.Time)
                        {
                            App.Current.Dispatcher.Invoke(
                                new Action(
                                    delegate
                                    {
                                        questionWindow = new FAFramework.GUI.QuestionMessageBoxWindow();
                                        questionWindow.Message = "Are you sure you want to force stop?";
                                        questionWindow.EquipmentInstance = Equipment;
                                        questionWindow.Show();
                                    }), null);
                            actor.NextStep();
                        }
                    }
                    else
                        actor.NextStep("LoopHead");
                });
            seq.AddItem(
                delegate (FASequence actor, TimeSpan time)
                {
                    if (questionWindow.Result == FAFramework.GUI.QuestionMessageBoxWindow.QuestionResult.Yes)
                    {
                        Equipment.ForceSuspendSubSequence();
                        Equipment.RequestStop();
                        actor.NextStep();
                    }
                    else if (questionWindow.Result == FAFramework.GUI.QuestionMessageBoxWindow.QuestionResult.No)
                    {
                        actor.NextStep();
                    }
                });
            seq.AddItem("LoopHead");
        }
    }
}