using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary;
using FALibrary.Part.MemoryBasePart;
using FALibrary.Sequence;

namespace FAFramework.Equipment
{
    public class NoAlarmStateEquipment : EquipmentBase
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

            public override void SetSuspend(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreSuspend;
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

            public override void RaiseWarning(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreWarning;
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

            public override void SetSuspend(EquipmentState oldStatus)
            {
                Equipment.State = Equipment.StatePreSuspend;
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
                    RequestStart();
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
                    RequestStop();
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
                    AlarmRaisingStatusManager.ClearCurrentAlarm();
                }
            }
        }
        #endregion

        #region SubUnits
        [FAAttribute("SubUnits")]
        public CommonSubUnit CommonUnit { get; set; }
        [FAAttribute("SubUnits")]
        public OperationPanelSubUnit OperationUnit { get; set; }
        #endregion        

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

        protected override void ProcInput()
        {
            if (CommonUnit.EmergencyReset.IsOn)
                ReleaseEmergency();
            else if (CommonUnit.EmergencyOff.IsOn)
                RequestEmergency();

            PressedStartButton = OperationUnit.StartButtonSwitch.IsOn;
            PressedStopButton = OperationUnit.StopButtonSwitch.IsOn;
            PressedInitialButton = OperationUnit.InitializeButtonSwitch.IsOn;
            PressedAlarmClearButton = OperationUnit.AlarmClearButtonSwitch.IsOn;
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
    }
}
