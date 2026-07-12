using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Sequence;
using FALibrary.Alarm;
using FALibrary.Utility;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;

namespace FALibrary.Part.MemoryBasePart
{
    public class FAPartMemoryBaseIOList : FAMemoryBasePart
    {
        public new string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                base.Name = value;
            }
        }

        public new string Description
        {
            get
            {
                return base.Description;
            }
            set
            {
                base.Description = value;
            }
        }
    }

    public class FAPartOnOffSensor : FAMemoryBasePart
    {
        private bool _inputInverse = false;
        private bool _outputInverse = false;
        private bool _isORSensor = false;
        private StatusDefine _statusList = new StatusDefine();

        private FAStatus _oldStatus;
        private bool _oldIsOn;
        private bool _oldIsOff;

        public class StatusDefine
        {
            private FAStatus _on = new FAStatus("On");
            private FAStatus _off = new FAStatus("Off");
            private FAStatus _unknown = new FAStatus("Unknown");

            public FAStatus On { get { return _on; } }
            public FAStatus Off { get { return _off; } }
            public FAStatus Unknown { get { return _unknown; } }
        }

        public StatusDefine StatusList { get { return _statusList; } }

        [FAAttribute("")]
        public FAStatus Status
        {
            get
            {
                if (IsTurnOn() == true && IsTurnOff() == false)
                    return StatusList.On;
                else if (IsTurnOn() == false && IsTurnOff() == true)
                    return StatusList.Off;
                else
                    return StatusList.Unknown;
            }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool InputInverse
        {
            get { return _inputInverse; }
            set
            {
                if (_inputInverse == value) return;

                _inputInverse = value;
                NotifyPropertyChanged("InputInverse");
            }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool OutputInverse
        {
            get { return _outputInverse; }
            set
            {
                if (_outputInverse == value) return;

                _outputInverse = value;
                NotifyPropertyChanged("OutputInverse");
            }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool IsORSensor
        {
            get { return _isORSensor; }
            set
            {
                if (_isORSensor == value) return;

                _isORSensor = value;
                NotifyPropertyChanged("IsORSensor");
            }
        }

        private bool _isAndSensor;
        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool IsAndSensor
        {
            get { return _isAndSensor; }
            set
            {
                if (_isAndSensor == value) return;

                _isAndSensor = value;
                NotifyPropertyChanged("IsAndSensor");
            }
        }        

        private bool IsOnORSensor()
        {
            foreach (FAPartInputIOInfo item in InputIO)
            {
                if (InputInverse)
                {
                    if (item.CorrectionValue == false)
                        return true;
                }
                else
                {
                    if (item.CorrectionValue == true)
                        return true;
                }
            }

            return false;
        }

        private bool IsOnAndSensor()
        {
            foreach (FAPartInputIOInfo item in InputIO)
            {
                if (InputInverse)
                {
                    if (item.CorrectionValue == true)
                        return false;
                }
                else
                {
                    if (item.CorrectionValue == false)
                        return false;
                }
            }

            return true;
        }

        private bool IsOnNormalSensor()
        {
            bool value = true;
            if (InputInverse)
                value = false;

            switch (InputIO.Count)
            {
                case 1:                   
                    if (InputIO[0].CorrectionValue == value)
                        return true;
                    else
                        return false;

                case 2:
                    if (InputIO[0].CorrectionValue == value &&
                        InputIO[1].CorrectionValue == !value)
                        return true;
                    else
                        return false;

                case 4:
                    if (InputIO[0].CorrectionValue == value &&
                        InputIO[1].CorrectionValue == !value &&
                        InputIO[2].CorrectionValue == value &&
                        InputIO[3].CorrectionValue == !value)
                        return true;
                    else
                        return false;

                case 6:
                    if (InputIO[0].CorrectionValue == value &&
                        InputIO[1].CorrectionValue == !value &&
                        InputIO[2].CorrectionValue == value &&
                        InputIO[3].CorrectionValue == !value &&
                        InputIO[4].CorrectionValue == value &&
                        InputIO[5].CorrectionValue == !value)
                        return true;
                    else
                        return false;

                case 8:
                    if (InputIO[0].CorrectionValue == value &&
                        InputIO[1].CorrectionValue == !value &&
                        InputIO[2].CorrectionValue == value &&
                        InputIO[3].CorrectionValue == !value &&
                        InputIO[4].CorrectionValue == value &&
                        InputIO[5].CorrectionValue == !value &&
                        InputIO[6].CorrectionValue == value &&
                        InputIO[7].CorrectionValue == !value)
                        return true;
                    else
                        return false;
            }

            return false;
        }

        protected bool IsTurnOn()
        {
            if (IsORSensor)
                return IsOnORSensor();
            else if (IsAndSensor)
                return IsOnAndSensor();
            else
                return IsOnNormalSensor();            
        }

        private bool IsOffORSensor()
        {
            foreach (FAPartInputIOInfo item in InputIO)
            {
                if (InputInverse)
                {
                    if (item.CorrectionValue == false)
                        return false;
                }
                else
                {
                    if (item.CorrectionValue == true)
                        return false;
                }
            }

            return true;
        }

        private bool IsOffAndSensor()
        {
            foreach (FAPartInputIOInfo item in InputIO)
            {
                if (InputInverse)
                {
                    if (item.CorrectionValue == true)
                        return true;
                }
                else
                {
                    if (item.CorrectionValue == false)
                        return true;
                }
            }

            return false;
        }

        private bool IsOffNormalSensor()
        {
            bool value = false;
            if (InputInverse)
                value = true;

            switch (InputIO.Count)
            {
                case 1:
                    if (InputIO[0].CorrectionValue == value)
                        return true;
                    else
                        return false;

                case 2:
                    if (InputIO[0].CorrectionValue == value &&
                        InputIO[1].CorrectionValue == !value)
                        return true;
                    else
                        return false;

                case 4:
                    if (InputIO[0].CorrectionValue == value &&
                        InputIO[1].CorrectionValue == !value &&
                        InputIO[2].CorrectionValue == value &&
                        InputIO[3].CorrectionValue == !value)
                        return true;
                    else
                        return false;

                case 6:
                    if (InputIO[0].CorrectionValue == value &&
                        InputIO[1].CorrectionValue == !value &&
                        InputIO[2].CorrectionValue == value &&
                        InputIO[3].CorrectionValue == !value &&
                        InputIO[4].CorrectionValue == value &&
                        InputIO[5].CorrectionValue == !value)
                        return true;
                    else
                        return false;

                case 8:
                    if (InputIO[0].CorrectionValue == value &&
                        InputIO[1].CorrectionValue == !value &&
                        InputIO[2].CorrectionValue == value &&
                        InputIO[3].CorrectionValue == !value &&
                        InputIO[4].CorrectionValue == value &&
                        InputIO[5].CorrectionValue == !value &&
                        InputIO[6].CorrectionValue == value &&
                        InputIO[7].CorrectionValue == !value)
                        return true;
                    else
                        return false;
            }

            return false;
        }

        protected bool IsTurnOff()
        {
            if (IsORSensor)
                return IsOffORSensor();
            else if (IsAndSensor)
                return IsOffAndSensor();
            else
                return IsOffNormalSensor();             
        }        

        public bool IsOn
        {
            get { return IsTurnOn(); }
        }

        public bool IsOff
        {
            get { return IsTurnOff(); }
        }

        public override void Validate()
        {
            base.Validate();

            if (_oldStatus != Status)
            {
                _oldStatus = Status;
                NotifyPropertyChanged("Status");
            }

            if (_oldIsOn != IsOn)
            {
                _oldIsOn = IsOn;
                NotifyPropertyChanged("IsOn");
            }

            if (_oldIsOff != IsOff)
            {
                _oldIsOff = IsOff;
                NotifyPropertyChanged("IsOff");
            }
        }
    }    

    public class FAPartMemoryBaseGeneric : FAMemoryBasePart
    {
        enum EActionType
        {
            On, Off
        }

        class ActionInfo
        {
            public EActionType ActionType { get; set; }
            public DateTime StartTime  { get; set; }
            public FATime LifeTime { get; set; }
        }

        private FAPartAction _turnOnAction = new FAPartAction();
        private FAPartAction _turnOffAction = new FAPartAction();

        private bool _multicast = false;
        private bool _undoActionOnTurnOnFailAlarm;
        private bool _undoActionOnTurnOffFailAlarm;
        private bool _inputInverse = false;
        private bool _outputInverse = false;
        private bool _outputOffOnActionComplete = false;
        private bool _haveOnewaySensor = false;
        private string _onStatusDisplayName = string.Empty;
        private string _offStatusDisplayName = string.Empty;
        private Queue<ActionInfo> _actionInfos = new Queue<ActionInfo>();

        public FAPartAction TurnOnAction
        {
            get { return _turnOnAction; }
        }

        public FAPartAction TurnOffAction
        {
            get { return _turnOffAction; }
        }

        public FAPartAction PulseOnAction { get; private set; }
        public FAPartAction PulseOffAction { get; private set; }

        protected FATime TurnOnTimeout { get; set; }        
        protected FATime TurnOffTimeout { get; set; }
        protected FARetryInfo RetryInfoTurnOnRetry { get; set; }
        protected FARetryInfo RetryInfoTurnOffRetry { get; set; }
        protected int TurnOnTimeoutAlarm { get; set; }
        protected int TurnOffTimeoutAlarm { get; set; }
        
        protected bool IgnoreOnActionFail { get; set; }        
        protected bool IgnoreOffActionFail { get; set; }

        private bool _enableRetryOnAction = true;
        protected bool EnableRetryOnAction
        {
            get { return _enableRetryOnAction; }
            set { _enableRetryOnAction = value; }
        }

        private bool _enableRetryOffAction = true;
        protected bool EnableRetryOffAction
        {
            get { return _enableRetryOffAction; }
            set { _enableRetryOffAction = value; }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool UndoActionOnTurnOnFailAlarm
        {
            get { return _undoActionOnTurnOnFailAlarm; }
            set
            {
                if (_undoActionOnTurnOnFailAlarm == value) return;

                _undoActionOnTurnOnFailAlarm = value;
                NotifyPropertyChanged("UndoActionOnTurnOnFailAlarm");
            }
        }
        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool UndoActionOnTurnOffFailAlarm
        {
            get { return _undoActionOnTurnOffFailAlarm; }
            set
            {
                if (_undoActionOnTurnOffFailAlarm == value) return;

                _undoActionOnTurnOffFailAlarm = value;
                NotifyPropertyChanged("UndoActionOnTurnOffFailAlarm");
            }
        }
        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool Multicast
        {
            get { return _multicast; }
            set
            {
                if (_multicast == value) return;

                _multicast = value;
                NotifyPropertyChanged("Multicast");
            }
        }
        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool InputInverse
        {
            get { return _inputInverse; }
            set
            {
                if (_inputInverse == value) return;

                _inputInverse = value;
                NotifyPropertyChanged("InputInverse");
            }
        }
        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool OutputInverse
        {
            get { return _outputInverse; }
            set
            {
                if (_outputInverse == value) return;

                _outputInverse = value;
                NotifyPropertyChanged("OutputInverse");
            }
        }
        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool OutputOffOnActionComplete
        {
            get { return _outputOffOnActionComplete; }
            set
            {
                if (_outputOffOnActionComplete == value) return;

                _outputOffOnActionComplete = value;
                NotifyPropertyChanged("OutputOffOnActionComplete");
            }
        }
        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool HaveOnewaySensor
        {
            get { return _haveOnewaySensor; }
            set
            {
                if (_haveOnewaySensor == value) return;

                _haveOnewaySensor = value;
                NotifyPropertyChanged("HaveOnewaySensor");
            }
        }
        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public string OnStatusDisplayName
        {
            get { return _onStatusDisplayName; }
            set
            {
                if (_onStatusDisplayName == value) return;

                _onStatusDisplayName = value;
                NotifyPropertyChanged("OnStatusDisplayName");
            }
        }
        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public string OffStatusDisplayName
        {
            get { return _offStatusDisplayName; }
            set
            {
                if (_offStatusDisplayName == value) return;

                _offStatusDisplayName = value;
                NotifyPropertyChanged("OffStatusDisplayName");
            }
        }

        private bool _useActionLifeManagement;
        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool UseActionLifeManagement
        {
            get { return _useActionLifeManagement; }
            set
            {
                if (_useActionLifeManagement == value) return;

                _useActionLifeManagement = value;
                NotifyPropertyChanged("UseActionLifeManagement");
            }
        }        

        [FAAttribute("Time")]
        public FATime PulseTime { get; set; }
        [FAAttribute("Time")]
        public FATime OnLifeTime { get; set; }
        [FAAttribute("Time")]
        public FATime OffLifeTime { get; set; }

        [FAAttribute("Time")]
        public FATime DelayAfterAction { get; set; }

        public FAPartMemoryBaseGeneric(FASequenceManager aSequenceManager)
        {
            TurnOnTimeout = new FATime(FATimeType.second, 5);
            TurnOffTimeout = new FATime(FATimeType.second, 5);
            DelayAfterAction = new FATime(FATimeType.millisecond, 0);
            PulseOnAction = new FAPartAction();
            PulseOffAction = new FAPartAction();

            _turnOnAction.SetActionMethod(DoTurnOn);
            _turnOffAction.SetActionMethod(DoTurnOff);
            PulseOnAction.SetActionMethod(
                (o) =>
                {
                    Task.Factory.StartNew(
                        () =>
                        {
                            DoTurnOn(o);
                            Thread.Sleep(PulseTime.Time);
                            DoTurnOff(o);
                        });
                });

            PulseOffAction.SetActionMethod(
                (o) =>
                {
                    Task.Factory.StartNew(
                        () =>
                        {
                            DoTurnOff(o);
                            Thread.Sleep(PulseTime.Time);
                            DoTurnOn(o);
                        });
                });

            _turnOnAction.CreateSequence(aSequenceManager);
            _turnOffAction.CreateSequence(aSequenceManager);
            PulseOnAction.CreateSequence(aSequenceManager);
            PulseOffAction.CreateSequence(aSequenceManager);

            MakeTurnOnAction(_turnOnAction.Sequence);
            MakeTurnOffAction(_turnOffAction.Sequence);

            PulseOnAction.Sequence.AddItem(_turnOnAction.Execute);
            PulseOnAction.Sequence.AddItem(PulseTime);
            PulseOnAction.Sequence.AddItem(_turnOffAction.Execute);
            PulseOnAction.Sequence.AddTerminate();

            PulseOffAction.Sequence.AddItem(_turnOffAction.Execute);
            PulseOffAction.Sequence.AddItem(PulseTime);
            PulseOffAction.Sequence.AddItem(_turnOnAction.Execute);
            PulseOffAction.Sequence.AddTerminate();
            
        }

        public override void Validate()
        {
            base.Validate();
            ManageActionLife();
        }

        private void MakeTurnOnAction(FASequence seq)
        {
            seq.OnStart += OnStartOnAction;
            seq.Steps.Add("Start", new StepInfo());
            seq.Steps.Add("ConfirmTurnOn", new StepInfo());
            seq.Steps.Add("Terminate", new StepInfo());
            seq.Steps.Add("Retry", new StepInfo());

            seq.Steps["Start"].StepIndex =
                seq.AddItem(_turnOnAction.ExecuteForSequence);
            seq.Steps["ConfirmTurnOn"].StepIndex =
                seq.AddItem(ConfirmTurnOn);
            seq.AddItem(
                (actor, time) =>
                {
                    if (DelayAfterAction.TypeValue == 0)
                        actor.NextStep("Terminate");
                    else
                        actor.NextStep();
                });
            seq.AddItem(DelayAfterAction);
            seq.Steps["Terminate"].StepIndex = seq.AddItem(TerminateSequence);

            seq.Steps["Retry"].StepIndex =
                seq.AddItem(_turnOffAction.Execute);
            seq.AddItem(new FATime(FATimeType.second, 2));
            seq.AddItem("Start");
        }

        private void MakeTurnOffAction(FASequence seq)
        {
            seq.OnStart += OnStartOffAction;
            seq.Steps.Add("Start", new StepInfo());
            seq.Steps.Add("ConfirmTurnOff", new StepInfo());
            seq.Steps.Add("Terminate", new StepInfo());
            seq.Steps.Add("Retry", new StepInfo());

            seq.Steps["Start"].StepIndex =
                seq.AddItem(_turnOffAction.ExecuteForSequence);
            seq.Steps["ConfirmTurnOff"].StepIndex =
                seq.AddItem(ConfirmTurnOff);
            seq.AddItem(DelayAfterAction);
            seq.AddItem(
                (actor, time) =>
                {
                    if (DelayAfterAction.TypeValue == 0)
                        actor.NextStep("Terminate");
                    else
                        actor.NextStep();
                });
            seq.Steps["Terminate"].StepIndex = seq.AddItem(TerminateSequence);

            seq.Steps["Retry"].StepIndex =
                seq.AddItem(_turnOnAction.Execute);
            seq.AddItem(new FATime(FATimeType.second, 2));
            seq.AddItem("Start");
        }

        private void DoTurnOnMulticast(object sender)
        {
            foreach (FAPartOutputIOInfo item in OutputIO)
            {
                if (OutputInverse)
                    item.CorrectionValue = false;
                else
                    item.CorrectionValue = true;
            }
        }

        private void DoTurnOnNormal(object sender)
        {
            bool value = true;
            if (OutputInverse)
                value = false;

            switch (OutputIO.Count)
            {
                case 1:
                    OutputIO[0].CorrectionValue = value;
                    break;

                case 2:
                    OutputIO[0].CorrectionValue = value;
                    OutputIO[1].CorrectionValue = !value;
                    break;

                case 4:
                    OutputIO[0].CorrectionValue = value;
                    OutputIO[1].CorrectionValue = !value;
                    OutputIO[2].CorrectionValue = value;
                    OutputIO[3].CorrectionValue = !value;
                    break;

                case 8:
                    OutputIO[0].CorrectionValue = value;
                    OutputIO[1].CorrectionValue = !value;
                    OutputIO[2].CorrectionValue = value;
                    OutputIO[3].CorrectionValue = !value;
                    OutputIO[4].CorrectionValue = value;
                    OutputIO[5].CorrectionValue = !value;
                    OutputIO[6].CorrectionValue = value;
                    OutputIO[7].CorrectionValue = !value;
                    break;
            }
        }

        public void DoTurnOn(object sender)
        {
            if (Multicast)
                DoTurnOnMulticast(sender);
            else
                DoTurnOnNormal(sender);

            if (SimulationMode)
                TurnOnInputForSimulation();

            if (UseActionLifeManagement)
                AddActionLifeItem(new ActionInfo { ActionType = EActionType.On, StartTime = DateTime.Now, LifeTime = OnLifeTime });
        }

        private void DoTurnOffMulticast(object sender)
        {
            foreach (FAPartOutputIOInfo item in OutputIO)
            {
                if (OutputInverse)
                    item.CorrectionValue = true;
                else
                    item.CorrectionValue = false;
            }
        }

        private void DoTurnOffNormal(object sender)
        {
            bool value = false;
            if (OutputInverse)
                value = true;

            switch (OutputIO.Count)
            {
                case 1:
                    OutputIO[0].CorrectionValue = value;
                    break;

                case 2:
                    OutputIO[0].CorrectionValue = value;
                    OutputIO[1].CorrectionValue = !value;
                    break;

                case 4:
                    OutputIO[0].CorrectionValue = value;
                    OutputIO[1].CorrectionValue = !value;
                    OutputIO[2].CorrectionValue = value;
                    OutputIO[3].CorrectionValue = !value;
                    break;

                case 8:
                    OutputIO[0].CorrectionValue = value;
                    OutputIO[1].CorrectionValue = !value;
                    OutputIO[2].CorrectionValue = value;
                    OutputIO[3].CorrectionValue = !value;
                    OutputIO[4].CorrectionValue = value;
                    OutputIO[5].CorrectionValue = !value;
                    OutputIO[6].CorrectionValue = value;
                    OutputIO[7].CorrectionValue = !value;
                    break;
            }
        }

        public void DoTurnOff(object sender)
        {
            if (Multicast)
                DoTurnOffMulticast(sender);
            else
                DoTurnOffNormal(sender);

            if (SimulationMode)
                TurnOffInputForSimulation();

            if (UseActionLifeManagement)
                AddActionLifeItem(new ActionInfo { ActionType = EActionType.Off, StartTime = DateTime.Now, LifeTime = OffLifeTime });
        }

        protected void TerminateSequence(FASequence actor, TimeSpan time)
        {
            actor.NextTerminate();
        }

        protected void OnStartOnAction(object sender, EventArgs e)
        {
            RetryInfoTurnOnRetry.ClearCount();
        }

        protected void OnStartOffAction(object sender, EventArgs e)
        {
            RetryInfoTurnOffRetry.ClearCount();
        }

        protected void ConfirmTurnOn(FASequence actor, TimeSpan time)
        {
            if (SimulationMode &&
                InputIO.Count == 0 &&
                OutputIO.Count == 0)
            {
                actor.NextStep();
            }
            else if (IsTurnOn() == true)
            {
                if (OutputOffOnActionComplete == true)
                    TurnOffAllOutput();

                actor.NextStep();
            }
            else if (time > TurnOnTimeout.Time)
            {
                if (EnableRetryOnAction == false)
                {
                    FAAlarmManager.Instance.RaiseAlarm(actor, TurnOnTimeoutAlarm,
                            "FAIL ACTION Part " + FullName + " " + TurnOnAction.ActionName + '\n');

                    actor.NextStep("Start");
                }
                else if (RetryInfoTurnOnRetry.IncreaseCount() == false)
                {
                    if (IgnoreOnActionFail == false)
                    {
                        if (UndoActionOnTurnOnFailAlarm)
                            DoTurnOff(actor);

                        FAAlarmManager.Instance.RaiseAlarm(actor, TurnOnTimeoutAlarm,
                            "FAIL ACTION Part " + FullName + " " + TurnOnAction.ActionName + '\n');

                        actor.NextStep("Retry");
                    }
                    else
                        actor.NextStep();
                }
                else
                    actor.NextStep("Retry");
            }
        }

        protected void ConfirmTurnOff(FASequence actor, TimeSpan time)
        {
            if (SimulationMode &&
                InputIO.Count == 0 &&
                OutputIO.Count == 0)
            {
                actor.NextStep();
            }
            else if (IsTurnOff() == true)
            {
                if (OutputOffOnActionComplete == true)
                    TurnOffAllOutput();

                actor.NextStep();
            }
            else if (time > TurnOffTimeout.Time)
            {
                if (EnableRetryOffAction == false)
                {
                    FAAlarmManager.Instance.RaiseAlarm(actor, TurnOffTimeoutAlarm,
                            "FAIL ACTION Part " + FullName + " " + TurnOffAction.ActionName + '\n');

                    actor.NextStep("Start");
                }
                else if (RetryInfoTurnOffRetry.IncreaseCount() == false)
                {
                    if (IgnoreOffActionFail == false)
                    {
                        if (UndoActionOnTurnOffFailAlarm)
                            DoTurnOn(actor);

                        FAAlarmManager.Instance.RaiseAlarm(actor, TurnOffTimeoutAlarm,
                            "FAIL ACTION Part " + FullName + " " + TurnOffAction.ActionName + '\n');

                        actor.NextStep("Retry");
                    }
                    else
                        actor.NextStep();
                }
                else
                    actor.NextStep("Retry");
            }
        }

        public bool IsTurnOn()
        {
            bool value = true;
            if (InputInverse)
                value = false;

            if (InputIO.Count == 0)
            {
                return IsTurnOnWhenInputCountZero();
            }
            else
            {
                if (HaveOnewaySensor)
                    return IsTurnOnWhenHaveOnewaySensor(value);
                else
                    return IsTurnOnWhenNormalCase(value);
            }            
        }

        public bool IsTurnOff()
        {
            bool value = false;
            if (InputInverse)
                value = true;

            if (InputIO.Count == 0)
            {
                return IsTurnOffWhenInputCountZero();
            }
            else
            {
                if (HaveOnewaySensor)
                    return IsTurnOffWhenHaveOnewaySensor(value);
                else
                    return IsTurnOffWhenNormalCase(value);
            }
        }

        protected void TurnOnInputForSimulation()
        {
            bool value = true;
            if (InputInverse)
                value = false;

            if (InputIO.Count == 0)
            {
                if (Multicast)
                {
                    if (InputInverse)
                    {
                        foreach (FAPartOutputIOInfo io in OutputIO)
                            io.CorrectionValue = false;
                    }
                    else
                    {
                        foreach (FAPartOutputIOInfo io in OutputIO)                        
                            io.CorrectionValue = true;
                    }
                }
                else
                {                    
                    switch (OutputIO.Count)
                    {
                        case 1:
                            if (OutputInverse == false)
                            {
                                OutputIO[0].CorrectionValue = true;
                            }
                            else
                            {
                                OutputIO[0].CorrectionValue = false;
                            }

                            break;
                    }                    
                }
            }

            switch (InputIO.Count)
            {
                case 1:
                    InputIO[0].CorrectionValue = value;
                    break;

                case 2:
                    InputIO[0].CorrectionValue = value;
                    InputIO[1].CorrectionValue = !value;
                    break;

                case 4:
                    InputIO[0].CorrectionValue = value;
                    InputIO[1].CorrectionValue = !value;
                    InputIO[2].CorrectionValue = value;
                    InputIO[3].CorrectionValue = !value;
                    break;

                case 6:
                    InputIO[0].CorrectionValue = value;
                    InputIO[1].CorrectionValue = !value;
                    InputIO[2].CorrectionValue = value;
                    InputIO[3].CorrectionValue = !value;
                    InputIO[4].CorrectionValue = value;
                    InputIO[5].CorrectionValue = !value;
                    break;

                case 8:
                    InputIO[0].CorrectionValue = value;
                    InputIO[1].CorrectionValue = !value;
                    InputIO[2].CorrectionValue = value;
                    InputIO[3].CorrectionValue = !value;
                    InputIO[4].CorrectionValue = value;
                    InputIO[5].CorrectionValue = !value;
                    InputIO[6].CorrectionValue = value;
                    InputIO[7].CorrectionValue = !value;
                    break;
            }
        }

        protected void TurnOffInputForSimulation()
        {
            bool value = false;
            if (InputInverse)
                value = true;

            if (InputIO.Count == 0)
            {
                if (Multicast)
                {
                    if (InputInverse)
                    {
                        foreach (FAPartOutputIOInfo io in OutputIO)                        
                            io.CorrectionValue = true;
                    }
                    else
                    {
                        foreach (FAPartOutputIOInfo io in OutputIO)                        
                            io.CorrectionValue = false;
                    }
                }
                else
                {                    
                    switch (OutputIO.Count)
                    {
                        case 1:
                            if (OutputInverse == false)                            
                                OutputIO[0].CorrectionValue = false;
                            else
                                OutputIO[0].CorrectionValue = true;
                            break;
                    }
                }
            }

            switch (InputIO.Count)
            {
                case 1:
                    InputIO[0].CorrectionValue = value;
                    break;

                case 2:
                    InputIO[0].CorrectionValue = value;
                    InputIO[1].CorrectionValue = !value;
                    break;

                case 4:
                    InputIO[0].CorrectionValue = value;
                    InputIO[1].CorrectionValue = !value;
                    InputIO[2].CorrectionValue = value;
                    InputIO[3].CorrectionValue = !value;
                    break;    

                case 6:
                    InputIO[0].CorrectionValue = value;
                    InputIO[1].CorrectionValue = !value;
                    InputIO[2].CorrectionValue = value;
                    InputIO[3].CorrectionValue = !value;
                    InputIO[4].CorrectionValue = value;
                    InputIO[5].CorrectionValue = !value;
                    break;

                case 8:
                    InputIO[0].CorrectionValue = value;
                    InputIO[1].CorrectionValue = !value;
                    InputIO[2].CorrectionValue = value;
                    InputIO[3].CorrectionValue = !value;
                    InputIO[4].CorrectionValue = value;
                    InputIO[5].CorrectionValue = !value;
                    InputIO[6].CorrectionValue = value;
                    InputIO[7].CorrectionValue = !value;
                    break;
            }            
        }

        protected void TurnOffAllOutput()
        {
            foreach (FAPartOutputIOInfo item in OutputIO)
            {
                item.CorrectionValue = false;
            }
        }

        private bool IsTurnOnWhenInputCountZero()
        {
            if (Multicast)
            {
                if (InputInverse)
                {
                    foreach (FAPartOutputIOInfo io in OutputIO)
                    {
                        if (io.CorrectionValue == false)
                        {
                            return true;
                        }
                    }

                    return false;
                }
                else
                {
                    foreach (FAPartOutputIOInfo io in OutputIO)
                    {
                        if (io.CorrectionValue == true)
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }
            else
            {
                bool result = false;

                switch (OutputIO.Count)
                {
                    case 1:
                        if (OutputInverse == false)
                        {
                            if (OutputIO[0].CorrectionValue == true)
                                result = true;
                            else
                                result = false;
                        }
                        else
                        {
                            if (OutputIO[0].CorrectionValue == false)
                                result = true;
                            else
                                result = false;
                        }

                        break;
                }

                return result;
            }
        }

        private bool IsTurnOnWhenHaveOnewaySensor(bool value)
        {
            foreach (var item in InputIO)
            {
                if (item.Value != value)
                    return false;
            }

            return true;
        }

        private bool IsTurnOnWhenNormalCase(bool value)
        {
            switch (InputIO.Count)
            {
                case 1:
                    if (InputIO[0].CorrectionValue == value)
                        return true;
                    else
                        return false;

                case 2:
                    if (InputIO[0].CorrectionValue == value &&
                        InputIO[1].CorrectionValue == !value)
                        return true;
                    else
                        return false;

                case 4:
                    if (InputIO[0].CorrectionValue == value &&
                        InputIO[1].CorrectionValue == !value &&
                        InputIO[2].CorrectionValue == value &&
                        InputIO[3].CorrectionValue == !value)
                        return true;
                    else
                        return false;

                case 6:
                    if (InputIO[0].CorrectionValue == value &&
                        InputIO[1].CorrectionValue == !value &&
                        InputIO[2].CorrectionValue == value &&
                        InputIO[3].CorrectionValue == !value &&
                        InputIO[4].CorrectionValue == value &&
                        InputIO[5].CorrectionValue == !value)
                        return true;
                    else
                        return false;

                case 8:
                    if (InputIO[0].CorrectionValue == value &&
                        InputIO[1].CorrectionValue == !value &&
                        InputIO[2].CorrectionValue == value &&
                        InputIO[3].CorrectionValue == !value &&
                        InputIO[4].CorrectionValue == value &&
                        InputIO[5].CorrectionValue == !value &&
                        InputIO[6].CorrectionValue == value &&
                        InputIO[7].CorrectionValue == !value)
                        return true;
                    else
                        return false;
            }

            return false;
        }

        private bool IsTurnOffWhenInputCountZero()
        {
            if (Multicast)
            {
                if (InputInverse)
                {
                    foreach (FAPartOutputIOInfo io in OutputIO)
                    {
                        if (io.CorrectionValue == false)
                        {
                            return false;
                        }
                    }

                    return true;
                }
                else
                {
                    foreach (FAPartOutputIOInfo io in OutputIO)
                    {
                        if (io.CorrectionValue == true)
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }
            else
            {
                bool result = false;
                switch (OutputIO.Count)
                {
                    case 1:
                        if (OutputInverse == false)
                        {
                            if (OutputIO[0].CorrectionValue == false)
                                result = true;
                            else
                                result = false;
                        }
                        else
                        {
                            if (OutputIO[0].CorrectionValue == true)
                                result = true;
                            else
                                result = false;
                        }

                        break;
                }

                return result;
            }
        }

        private bool IsTurnOffWhenHaveOnewaySensor(bool value)
        {
            foreach (var item in InputIO)
            {
                if (item.Value != value)
                    return false;
            }

            return true;
        }

        private bool IsTurnOffWhenNormalCase(bool value)
        {
            switch (InputIO.Count)
            {
                case 1:
                    if (InputIO[0].CorrectionValue == value)
                        return true;
                    else
                        return false;

                case 2:
                    if (InputIO[0].CorrectionValue == value &&
                        InputIO[1].CorrectionValue == !value)
                        return true;
                    else
                        return false;

                case 4:
                    if (InputIO[0].CorrectionValue == value &&
                        InputIO[1].CorrectionValue == !value &&
                        InputIO[2].CorrectionValue == value &&
                        InputIO[3].CorrectionValue == !value)
                        return true;
                    else
                        return false;

                case 6:
                    if (InputIO[0].CorrectionValue == value &&
                        InputIO[1].CorrectionValue == !value &&
                        InputIO[2].CorrectionValue == value &&
                        InputIO[3].CorrectionValue == !value &&
                        InputIO[4].CorrectionValue == value &&
                        InputIO[5].CorrectionValue == !value)
                        return true;
                    else
                        return false;

                case 8:
                    if (InputIO[0].CorrectionValue == value &&
                        InputIO[1].CorrectionValue == !value &&
                        InputIO[2].CorrectionValue == value &&
                        InputIO[3].CorrectionValue == !value &&
                        InputIO[4].CorrectionValue == value &&
                        InputIO[5].CorrectionValue == !value &&
                        InputIO[6].CorrectionValue == value &&
                        InputIO[7].CorrectionValue == !value)
                        return true;
                    else
                        return false;
            }

            return false;
        }

        private void ManageActionLife()
        {
            if (UseActionLifeManagement == false)
            {
                if (_actionInfos.Count > 0) _actionInfos.Clear();
                return;
            }

            if (_actionInfos.Count == 0) return;

            var actionInfo = _actionInfos.First();
            var elapsed = DateTime.Now - actionInfo.StartTime;
            if (elapsed > actionInfo.LifeTime.Time)
            {
                _actionInfos.Dequeue();

                // 현재 액션 다음에 들어온 액션이 있는 경우.
                // 다음 액션에서 동작이 방해받지 않도록 다음에 들어온 액션이 없는 경우만 동작을 수행한다.
                if (_actionInfos.Count == 0)
                {
                    if (actionInfo.ActionType == EActionType.On)
                        AllOffOutput(this);
                    else if (actionInfo.ActionType == EActionType.Off)
                        AllOffOutput(this);
                }
            }
        }

        private void AddActionLifeItem(ActionInfo actionLifeItem)
        {
            var sameItems = _actionInfos.Where(x => x.ActionType == actionLifeItem.ActionType);
            if (sameItems != null && sameItems.Count() > 0)
                return;
           
            _actionInfos.Enqueue(actionLifeItem);
        }
    }    

    public class FAPartOnOff : FAPartMemoryBaseGeneric
    {        
        private StatusDefine _statusList = new StatusDefine();

        public class StatusDefine
        {
            private FAStatus _on = new FAStatus("On");
            private FAStatus _off = new FAStatus("Off");
            private FAStatus _unknown = new FAStatus("Unknown");

            public FAStatus On { get { return _on;} }
            public FAStatus Off { get { return _off; } }
            public FAStatus Unknown { get { return _unknown; } }
        }

        public StatusDefine StatusList { get { return _statusList; } }

        public FAPartOnOff(FASequenceManager aSequenceManager) : base(aSequenceManager)
        {            
            On.ActionName = "ON";            
            Off.ActionName = "OFF";
            OnStatusDisplayName = StatusList.On.DisplayName;
            OffStatusDisplayName = StatusList.Off.DisplayName;
        }

        private bool _loadedParameters = false;

        public override void LoadParameters(System.Xml.Linq.XElement xml)
        {
            base.LoadParameters(xml);

            if (_loadedParameters == false)
            {
                if (string.IsNullOrEmpty(OnStatusDisplayName) == false)
                    StatusList.On.DisplayName = OnStatusDisplayName;

                if (string.IsNullOrEmpty(OffStatusDisplayName) == false)
                    StatusList.Off.DisplayName = OffStatusDisplayName;

                _loadedParameters = true;
            }
        }

        [FAAttribute("Action")]
        public FAPartAction On
        {
            get { return base.TurnOnAction; }
        }

        [FAAttribute("Action")]
        public FAPartAction Off
        {
            get { return base.TurnOffAction; }
        }

        [FAAttribute("Status")]
        public FAStatus Status
        {
            get
            {                
                if (IsTurnOn() == true && IsTurnOff() == false)
                    return StatusList.On;
                else if (IsTurnOn() == false && IsTurnOff() == true)
                    return StatusList.Off;
                else
                    return StatusList.Unknown;
            }            
        }

        public bool IsOn
        {
            get { return IsTurnOn(); }
        }

        public bool IsOff
        {
            get { return IsTurnOff(); }
        }        

        [FAAttribute("Time")]
        public FATime OnTimeout
        {
            get { return TurnOnTimeout; }
            set
            {
                if (TurnOnTimeout == value) return;

                TurnOnTimeout = value;
                NotifyPropertyChanged("OnTimeout");
            }
        }

        [FAAttribute("Time")]
        public FATime OffTimeout
        {
            get { return TurnOffTimeout; }
            set
            {
                if (TurnOffTimeout == value) return;

                TurnOffTimeout = value;
                NotifyPropertyChanged("OffTimeout");
            }
        }

        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int OnTimeoutAlarm
        {
            get { return TurnOnTimeoutAlarm; }
            set
            {
                if (TurnOnTimeoutAlarm == value) return;

                TurnOnTimeoutAlarm = value;
                NotifyPropertyChanged("OnTimeoutAlarm");
            }
        }

        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int OffTimeoutAlarm
        {
            get { return TurnOffTimeoutAlarm; }
            set
            {
                if (TurnOffTimeoutAlarm == value) return;

                TurnOffTimeoutAlarm = value;
                NotifyPropertyChanged("OffTimeoutAlarm");
            }
        }

        [FAAttribute("RetryInfo")]
        public FARetryInfo RetryInfoOnRetry
        {
            get { return RetryInfoTurnOnRetry; }
            set
            {
                if (RetryInfoTurnOnRetry == value) return;

                RetryInfoTurnOnRetry = value;
                NotifyPropertyChanged("RetryInfoOnRetry");
            }
        }

        [FAAttribute("RetryInfo")]
        public FARetryInfo RetryInfoOffRetry
        {
            get { return RetryInfoTurnOffRetry; }
            set
            {
                if (RetryInfoTurnOffRetry == value) return;

                RetryInfoTurnOffRetry = value;
                NotifyPropertyChanged("RetryInfoOffRetry");
            }
        }
        
        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool IgnoreOnFail
        {
            get { return IgnoreOnActionFail; }
            set
            {
                if (IgnoreOnActionFail == value) return;

                IgnoreOnActionFail = value;
                NotifyPropertyChanged("IgnoreOnFail");
            }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool IgnoreOffFail
        {
            get { return IgnoreOffActionFail; }
            set
            {
                if (IgnoreOffActionFail == value) return;

                IgnoreOffActionFail = value;
                NotifyPropertyChanged("IgnoreOffFail");
            }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool EnableRetryOn
        {
            get { return EnableRetryOnAction; }
            set
            {
                if (EnableRetryOnAction == value) return;
                EnableRetryOnAction = value;
                NotifyPropertyChanged("EnableRetryOn");
            }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool EnableRetryOff
        {
            get { return EnableRetryOffAction; }
            set
            {
                if (EnableRetryOffAction == value) return;
                EnableRetryOffAction = value;
                NotifyPropertyChanged("EnableRetryOff");
            }
        }

        private bool _oldIsOn;
        private bool _oldIsOff;
        private FAStatus _oldStatus;

        public override void Validate()
        {
            base.Validate();

            if (_oldIsOn != IsOn)
            {
                _oldIsOn = IsOn;
                NotifyPropertyChanged("IsOn");
            }

            if (_oldIsOff != IsOff)
            {
                _oldIsOff = IsOff;
                NotifyPropertyChanged("IsOff");
            }

            if (_oldStatus != Status)
            {
                _oldStatus = Status;
                NotifyPropertyChanged("Status");
            }
        }
    }

    public class FAPartOpenClose : FAPartMemoryBaseGeneric
    {
        private StatusDefine _statusList = new StatusDefine();

        public class StatusDefine
        {
            private FAStatus _open = new FAStatus("Open");
            private FAStatus _close = new FAStatus("Close");
            private FAStatus _unknown = new FAStatus("Unknown");

            public FAStatus Open { get { return _open; } }
            public FAStatus Close { get { return _close; } }
            public FAStatus Unknown { get { return _unknown; } }
        }

        public StatusDefine StatusList { get { return _statusList; } }

        [FAAttribute("Status")]
        public FAStatus Status
        {
            get
            {
                if (IsTurnOn() == true && IsTurnOff() == false)
                    return StatusList.Open;
                else if (IsTurnOn() == false && IsTurnOff() == true)
                    return StatusList.Close;
                else
                    return StatusList.Unknown;
            }
        }

        public FAPartOpenClose(FASequenceManager aSequenceManager)
            : base(aSequenceManager)
        {
            Open.ActionName = "OPEN";
            Close.ActionName = "CLOSE";
            OnStatusDisplayName = StatusList.Open.DisplayName;
            OffStatusDisplayName = StatusList.Close.DisplayName;
        }

        public override void LoadParameters(System.Xml.Linq.XElement xml)
        {
            base.LoadParameters(xml);

            if (string.IsNullOrEmpty(OnStatusDisplayName) == false)
                StatusList.Open.DisplayName = OnStatusDisplayName;

            if (string.IsNullOrEmpty(OffStatusDisplayName) == false)
                StatusList.Close.DisplayName = OffStatusDisplayName;
        }

        [FAAttribute("Action")]
        public FAPartAction Open
        {
            get { return base.TurnOnAction; }
        }

        [FAAttribute("Action")]
        public FAPartAction Close
        {
            get { return base.TurnOffAction; }
        }

        [FAAttribute("Time")]        
        public FATime OpenTimeout
        {
            get { return TurnOnTimeout; }
            set
            {
                if (TurnOnTimeout == value) return;

                TurnOnTimeout = value;
                NotifyPropertyChanged("OpenTimeout");
            }
        }

        [FAAttribute("Time")]
        public FATime CloseTimeout
        {
            get { return TurnOffTimeout; }
            set
            {
                if (TurnOffTimeout == value) return;

                TurnOffTimeout = value;
                NotifyPropertyChanged("CloseTimeout");
            }
        }

        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int OpenTimeoutAlarm
        {
            get { return TurnOnTimeoutAlarm; }
            set
            {
                if (TurnOnTimeoutAlarm == value) return;

                TurnOnTimeoutAlarm = value;
                NotifyPropertyChanged("OpenTimeoutAlarm");
            }
        }

        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int CloseTimeoutAlarm
        {
            get { return TurnOffTimeoutAlarm; }
            set
            {
                if (TurnOffTimeoutAlarm == value) return;

                TurnOffTimeoutAlarm = value;
                NotifyPropertyChanged("CloseTimeoutAlarm");
            }
        }

        [FAAttribute("RetryInfo")]
        public FARetryInfo RetryInfoOpenRetry
        {
            get { return RetryInfoTurnOnRetry; }
            set
            {
                if (RetryInfoTurnOnRetry == value) return;

                RetryInfoTurnOnRetry = value;
                NotifyPropertyChanged("RetryInfoOpenRetry");
            }
        }

        [FAAttribute("RetryInfo")]
        public FARetryInfo RetryInfoCloseRetry
        {
            get { return RetryInfoTurnOffRetry; }
            set
            {
                if (RetryInfoTurnOffRetry == value) return;

                RetryInfoTurnOffRetry = value;
                NotifyPropertyChanged("RetryInfoCloseRetry");
            }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool IgnoreOpenFail
        {
            get { return IgnoreOnActionFail; }
            set
            {
                if (IgnoreOnActionFail == value) return;

                IgnoreOnActionFail = value;
                NotifyPropertyChanged("IgnoreOpenFail");
            }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool IgnoreCloseFail
        {
            get { return IgnoreOffActionFail; }
            set
            {
                if (IgnoreOffActionFail == value) return;

                IgnoreOffActionFail = value;
                NotifyPropertyChanged("IgnoreCloseFail");
            }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool EnableRetryOpen
        {
            get { return EnableRetryOnAction; }
            set
            {
                if (EnableRetryOnAction == value) return;
                EnableRetryOnAction = value;
                NotifyPropertyChanged("EnableRetryOpen");
            }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool EnableRetryClose
        {
            get { return EnableRetryOffAction; }
            set
            {
                if (EnableRetryOffAction == value) return;
                EnableRetryOffAction = value;
                NotifyPropertyChanged("EnableRetryClose");
            }
        }

        private FAStatus _oldStatus;

        public override void Validate()
        {
            base.Validate();

            if (_oldStatus != Status)
            {
                _oldStatus = Status;
                NotifyPropertyChanged("Status");
            }
        }

        public bool IsOpened()
        {
            return Status == StatusList.Open;
        }

        public bool IsClosed()
        {
            return Status == StatusList.Close;
        }
    }

    public class FAPartUpDown : FAPartMemoryBaseGeneric
    {
        private StatusDefine _statusList = new StatusDefine();

        public class StatusDefine
        {
            private FAStatus _up = new FAStatus("Up");
            private FAStatus _down = new FAStatus("Down");
            private FAStatus _unknown = new FAStatus("Unknown");

            public FAStatus Up { get { return _up; } }
            public FAStatus Down { get { return _down; } }
            public FAStatus Unknown { get { return _unknown; } }
        }

        public StatusDefine StatusList { get { return _statusList; } }

        [FAAttribute("Status")]
        public FAStatus Status
        {
            get
            {
                if (IsTurnOn() == true && IsTurnOff() == false)
                    return StatusList.Up;
                else if (IsTurnOn() == false && IsTurnOff() == true)
                    return StatusList.Down;
                else
                    return StatusList.Unknown;
            }
        }

        public FAPartUpDown(FASequenceManager aSequenceManager) : base(aSequenceManager)
        {
            Up.ActionName = "UP";
            Down.ActionName = "DOWN";
            OnStatusDisplayName = StatusList.Up.DisplayName;
            OffStatusDisplayName = StatusList.Down.DisplayName;
        }

        public override void LoadParameters(System.Xml.Linq.XElement xml)
        {
            base.LoadParameters(xml);

            if (string.IsNullOrEmpty(OnStatusDisplayName) == false)
                StatusList.Up.DisplayName = OnStatusDisplayName;

            if (string.IsNullOrEmpty(OffStatusDisplayName) == false)
                StatusList.Down.DisplayName = OffStatusDisplayName;
        }

        [FAAttribute("Action")]
        public FAPartAction Up
        {
            get { return base.TurnOnAction; }
        }

        [FAAttribute("Action")]
        public FAPartAction Down
        {
            get { return base.TurnOffAction; }
        }

        [FAAttribute("Time")]
        public FATime UpTimeout
        {
            get { return TurnOnTimeout; }
            set
            {
                if (TurnOnTimeout == value) return;

                TurnOnTimeout = value;
                NotifyPropertyChanged("UpTimeout");
            }
        }

        [FAAttribute("Time")]
        public FATime DownTimeout
        {
            get { return TurnOffTimeout; }
            set
            {
                if (TurnOffTimeout == value) return;

                TurnOffTimeout = value;
                NotifyPropertyChanged("DownTimeout");
            }
        }

        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int UpTimeoutAlarm
        {
            get { return TurnOnTimeoutAlarm; }
            set
            {
                if (TurnOnTimeoutAlarm == value) return;

                TurnOnTimeoutAlarm = value;
                NotifyPropertyChanged("UpTimeoutAlarm");
            }
        }

        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int DownTimeoutAlarm
        {
            get { return TurnOffTimeoutAlarm; }
            set
            {
                if (TurnOffTimeoutAlarm == value) return;

                TurnOffTimeoutAlarm = value;
                NotifyPropertyChanged("DownTimeoutAlarm");
            }
        }

        [FAAttribute("RetryInfo")]
        public FARetryInfo RetryInfoUpRetry
        {
            get { return RetryInfoTurnOnRetry; }
            set
            {
                if (RetryInfoTurnOnRetry == value) return;

                RetryInfoTurnOnRetry = value;
                NotifyPropertyChanged("RetryInfoUpRetry");
            }
        }

        [FAAttribute("RetryInfo")]
        public FARetryInfo RetryInfoDownRetry
        {
            get { return RetryInfoTurnOffRetry; }
            set
            {
                if (RetryInfoTurnOffRetry == value) return;

                RetryInfoTurnOffRetry = value;
                NotifyPropertyChanged("RetryInfoDownRetry");
            }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool IgnoreUpFail
        {
            get { return IgnoreOnActionFail; }
            set
            {
                if (IgnoreOnActionFail == value) return;

                IgnoreOnActionFail = value;
                NotifyPropertyChanged("IgnoreUpFail");
            }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool IgnoreDownFail
        {
            get { return IgnoreOffActionFail; }
            set
            {
                if (IgnoreOffActionFail == value) return;

                IgnoreOffActionFail = value;
                NotifyPropertyChanged("IgnoreDownFail");
            }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool EnableRetryUp
        {
            get { return EnableRetryOnAction; }
            set
            {
                if (EnableRetryOnAction == value) return;
                EnableRetryOnAction = value;
                NotifyPropertyChanged("EnableRetryUp");
            }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool EnableRetryDown
        {
            get { return EnableRetryOffAction; }
            set
            {
                if (EnableRetryOffAction == value) return;
                EnableRetryOffAction = value;
                NotifyPropertyChanged("EnableRetryDown");
            }
        }

        private FAStatus _oldStatus;

        public override void Validate()
        {
            base.Validate();

            if (_oldStatus != Status)
            {
                _oldStatus = Status;
                NotifyPropertyChanged("Status");
            }
        }
    }

    public class FAPartExtendRetract : FAPartMemoryBaseGeneric
    {
        private StatusDefine _statusList = new StatusDefine();

        public class StatusDefine
        {
            private FAStatus _extend = new FAStatus("Extend");
            private FAStatus _retract = new FAStatus("Retract");
            private FAStatus _unknown = new FAStatus("Unknown");

            public FAStatus Extend { get { return _extend; } }
            public FAStatus Retract { get { return _retract; } }
            public FAStatus Unknown { get { return _unknown; } }
        }

        public StatusDefine StatusList { get { return _statusList; } }

        [FAAttribute("Status")]
        public FAStatus Status
        {
            get
            {
                if (IsTurnOn() == true && IsTurnOff() == false)
                    return StatusList.Extend;
                else if (IsTurnOn() == false && IsTurnOff() == true)
                    return StatusList.Retract;
                else
                    return StatusList.Unknown;
            }
        }

        public FAPartExtendRetract(FASequenceManager aSequenceManager) : base(aSequenceManager)
        {
            Extend.ActionName = "Extend";
            Retract.ActionName = "Retract";
            OnStatusDisplayName = StatusList.Extend.DisplayName;
            OffStatusDisplayName = StatusList.Retract.DisplayName;
        }

        public override void LoadParameters(System.Xml.Linq.XElement xml)
        {
            base.LoadParameters(xml);

            if (string.IsNullOrEmpty(OnStatusDisplayName) == false)
                StatusList.Extend.DisplayName = OnStatusDisplayName;

            if (string.IsNullOrEmpty(OffStatusDisplayName) == false)
                StatusList.Retract.DisplayName = OffStatusDisplayName;

        }

        [FAAttribute("Action")]
        public FAPartAction Extend
        {
            get { return base.TurnOnAction; }
        }

        [FAAttribute("Action")]
        public FAPartAction Retract
        {
            get { return base.TurnOffAction; }
        }

        [FAAttribute("Time")]        
        public FATime ExtendTimeout
        {
            get { return TurnOnTimeout; }
            set
            {
                if (TurnOnTimeout == value) return;

                TurnOnTimeout = value;
                NotifyPropertyChanged("ExtendTimeout");
            }
        }

        [FAAttribute("Time")]
        public FATime RetractTimeout
        {
            get { return TurnOffTimeout; }
            set
            {
                if (TurnOffTimeout == value) return;

                TurnOffTimeout = value;
                NotifyPropertyChanged("RetractTimeout");
            }
        }

        [FAAttribute("Alarm")]
		[FAPropertyAttribute]
        public int ExtendTimeoutAlarm
        {
            get { return TurnOnTimeoutAlarm; }
            set
            {
                if (TurnOnTimeoutAlarm == value) return;

                TurnOnTimeoutAlarm = value;
                NotifyPropertyChanged("ExtendTimeoutAlarm");
            }
        }

        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int RetractTimeoutAlarm
        {
            get { return TurnOffTimeoutAlarm; }
            set
            {
                if (TurnOffTimeoutAlarm == value) return;

                TurnOffTimeoutAlarm = value;
                NotifyPropertyChanged("RetractTimeoutAlarm");
            }
        }

        [FAAttribute("RetryInfo")]
        public FARetryInfo RetryInfoExtendRetry
        {
            get { return RetryInfoTurnOnRetry; }
            set
            {
                if (RetryInfoTurnOnRetry == value) return;

                RetryInfoTurnOnRetry = value;
                NotifyPropertyChanged("RetryInfoExtendRetry");
            }
        }

        [FAAttribute("RetryInfo")]
        public FARetryInfo RetryInfoRetractRetry
        {
            get { return RetryInfoTurnOffRetry; }
            set
            {
                if (RetryInfoTurnOffRetry == value) return;

                RetryInfoTurnOffRetry = value;
                NotifyPropertyChanged("RetryInfoRetractRetry");
            }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool IgnoreExtendFail
        {
            get { return IgnoreOnActionFail; }
            set
            {
                if (IgnoreOnActionFail == value) return;

                IgnoreOnActionFail = value;
                NotifyPropertyChanged("IgnoreExtendFail");
            }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool IgnoreRetractFail
        {
            get { return IgnoreOffActionFail; }
            set
            {
                if (IgnoreOffActionFail == value) return;

                IgnoreOffActionFail = value;
                NotifyPropertyChanged("IgnoreRetractFail");
            }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool EnableRetryExtend
        {
            get { return EnableRetryOnAction; }
            set
            {
                if (EnableRetryOnAction == value) return;
                EnableRetryOnAction = value;
                NotifyPropertyChanged("EnableRetryExtend");
            }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool EnableRetryRetract
        {
            get { return EnableRetryOffAction; }
            set
            {
                if (EnableRetryOffAction == value) return;
                EnableRetryOffAction = value;
                NotifyPropertyChanged("EnableRetryRetract");
            }
        }

        private FAStatus _oldStatus;

        public override void Validate()
        {
            base.Validate();

            if (_oldStatus != Status)
            {
                _oldStatus = Status;
                NotifyPropertyChanged("Status");
            }
        }
    }

    public class FAPartTurnHome : FAPartMemoryBaseGeneric
    {
        private StatusDefine _statusList = new StatusDefine();

        public class StatusDefine
        {
            private FAStatus _turn = new FAStatus("Turn");
            private FAStatus _home = new FAStatus("Home");
            private FAStatus _unknown = new FAStatus("Unknown");

            public FAStatus Turn { get { return _turn; } }
            public FAStatus Home { get { return _home; } }
            public FAStatus Unknown { get { return _unknown; } }
        }

        public StatusDefine StatusList { get { return _statusList; } }

        [FAAttribute("Status")]
        public FAStatus Status
        {
            get
            {
                if (IsTurnOn() == true && IsTurnOff() == false)
                    return StatusList.Turn;
                else if (IsTurnOn() == false && IsTurnOff() == true)
                    return StatusList.Home;
                else
                    return StatusList.Unknown;
            }
        }

        public FAPartTurnHome(FASequenceManager aSequenceManager)
            : base(aSequenceManager)
        {
            Turn.ActionName = "Turn";
            Home.ActionName = "Home";
            OnStatusDisplayName = StatusList.Turn.DisplayName;
            OffStatusDisplayName = StatusList.Home.DisplayName;
        }

        public override void LoadParameters(System.Xml.Linq.XElement xml)
        {
            base.LoadParameters(xml);

            if (string.IsNullOrEmpty(OnStatusDisplayName) == false)
                StatusList.Turn.DisplayName = OnStatusDisplayName;

            if (string.IsNullOrEmpty(OffStatusDisplayName) == false)
                StatusList.Home.DisplayName = OffStatusDisplayName;
        }

        [FAAttribute("Action")]
        public FAPartAction Turn
        {
            get { return base.TurnOnAction; }
        }

        [FAAttribute("Action")]
        public FAPartAction Home
        {
            get { return base.TurnOffAction; }
        }

        [FAAttribute("Time")]
        public FATime TurnTimeout
        {
            get { return TurnOnTimeout; }
            set
            {
                if (TurnOnTimeout == value) return;

                TurnOnTimeout = value;
                NotifyPropertyChanged("TurnTimeout");
            }
        }

        [FAAttribute("Time")]
        public FATime HomeTimeout
        {
            get { return TurnOffTimeout; }
            set
            {
                if (TurnOffTimeout == value) return;

                TurnOffTimeout = value;
                NotifyPropertyChanged("HomeTimeout");
            }
        }

        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int TurnTimeoutAlarm
        {
            get { return TurnOnTimeoutAlarm; }
            set
            {
                if (TurnOnTimeoutAlarm == value) return;

                TurnOnTimeoutAlarm = value;
                NotifyPropertyChanged("TurnTimeoutAlarm");
            }
        }

        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int HomeTimeoutAlarm
        {
            get { return TurnOffTimeoutAlarm; }
            set
            {
                if (TurnOffTimeoutAlarm == value) return;

                TurnOffTimeoutAlarm = value;
                NotifyPropertyChanged("HomeTimeoutAlarm");
            }
        }

        [FAAttribute("RetryInfo")]
        public FARetryInfo RetryInfoTurnRetry
        {
            get { return RetryInfoTurnOnRetry; }
            set
            {
                if (RetryInfoTurnOnRetry == value) return;

                RetryInfoTurnOnRetry = value;
                NotifyPropertyChanged("RetryInfoTurnRetry");
            }
        }

        [FAAttribute("RetryInfo")]
        public FARetryInfo RetryInfoHomeRetry
        {
            get { return RetryInfoTurnOffRetry; }
            set
            {
                if (RetryInfoTurnOffRetry == value) return;

                RetryInfoTurnOffRetry = value;
                NotifyPropertyChanged("RetryInfoHomeRetry");
            }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool IgnoreTurnFail
        {
            get { return IgnoreOnActionFail; }
            set
            {
                if (IgnoreOnActionFail == value) return;

                IgnoreOnActionFail = value;
                NotifyPropertyChanged("IgnoreTurnFail");
            }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool IgnoreHomeFail
        {
            get { return IgnoreOffActionFail; }
            set
            {
                if (IgnoreOffActionFail == value) return;

                IgnoreOffActionFail = value;
                NotifyPropertyChanged("IgnoreHomeFail");
            }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool EnableRetryTurn
        {
            get { return EnableRetryOnAction; }
            set
            {
                if (EnableRetryOnAction == value) return;
                EnableRetryOnAction = value;
                NotifyPropertyChanged("EnableRetryTurn");
            }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool EnableRetryHome
        {
            get { return EnableRetryOffAction; }
            set
            {
                if (EnableRetryOffAction == value) return;
                EnableRetryOffAction = value;
                NotifyPropertyChanged("EnableRetryHome");
            }
        }

        private FAStatus _oldStatus;

        public override void Validate()
        {
            base.Validate();

            if (_oldStatus != Status)
            {
                _oldStatus = Status;
                NotifyPropertyChanged("Status");
            }
        }
    }

    public class FAPartLockRelease : FAPartMemoryBaseGeneric
    {
        private StatusDefine _statusList = new StatusDefine();

        public class StatusDefine
        {
            private FAStatus _lock = new FAStatus("Lock");
            private FAStatus _release = new FAStatus("Release");
            private FAStatus _unknown = new FAStatus("Unknown");

            public FAStatus Lock { get { return _lock; } }
            public FAStatus Release { get { return _release; } }
            public FAStatus Unknown { get { return _unknown; } }
        }

        public StatusDefine StatusList { get { return _statusList; } }

        [FAAttribute("Status")]
        public FAStatus Status
        {
            get
            {
                if (IsTurnOn() == true && IsTurnOff() == false)
                    return StatusList.Lock;
                else if (IsTurnOn() == false && IsTurnOff() == true)
                    return StatusList.Release;
                else
                    return StatusList.Unknown;
            }
        }

        public FAPartLockRelease(FASequenceManager aSequenceManager)
            : base(aSequenceManager)
        {
            Lock.ActionName = "LOCK";
            Release.ActionName = "RELEASE";
            OnStatusDisplayName = StatusList.Lock.DisplayName;
            OffStatusDisplayName = StatusList.Release.DisplayName;
        }

        public override void LoadParameters(System.Xml.Linq.XElement xml)
        {
            base.LoadParameters(xml);

            if (string.IsNullOrEmpty(OnStatusDisplayName) == false)
                StatusList.Lock.DisplayName = OnStatusDisplayName;

            if (string.IsNullOrEmpty(OffStatusDisplayName) == false)
                StatusList.Release.DisplayName = OffStatusDisplayName;
        }

        [FAAttribute("Action")]
        public FAPartAction Lock
        {
            get { return base.TurnOnAction; }
        }

        [FAAttribute("Action")]
        public FAPartAction Release
        {
            get { return base.TurnOffAction; }
        }

        [FAAttribute("Time")]
        public FATime LockTimeout
        {
            get { return TurnOnTimeout; }
            set
            {
                if (TurnOnTimeout == value) return;

                TurnOnTimeout = value;
                NotifyPropertyChanged("LockTimeout");
            }
        }

        [FAAttribute("Time")]
        public FATime ReleaseTimeout
        {
            get { return TurnOffTimeout; }
            set
            {
                if (TurnOffTimeout == value) return;

                TurnOffTimeout = value;
                NotifyPropertyChanged("ReleaseTimeout");
            }
        }

        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int LockTimeoutAlarm
        {
            get { return TurnOnTimeoutAlarm; }
            set
            {
                if (TurnOnTimeoutAlarm == value) return;

                TurnOnTimeoutAlarm = value;
                NotifyPropertyChanged("LockTimeoutAlarm");
            }
        }

        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int ReleaseTimeoutAlarm
        {
            get { return TurnOffTimeoutAlarm; }
            set
            {
                if (TurnOffTimeoutAlarm == value) return;

                TurnOffTimeoutAlarm = value;
                NotifyPropertyChanged("ReleaseTimeoutAlarm");
            }
        }

        [FAAttribute("RetryInfo")]
        public FARetryInfo RetryInfoLockRetry
        {
            get { return RetryInfoTurnOnRetry; }
            set
            {
                if (RetryInfoTurnOnRetry == value) return;

                RetryInfoTurnOnRetry = value;
                NotifyPropertyChanged("RetryInfoLockRetry");
            }
        }

        [FAAttribute("RetryInfo")]
        public FARetryInfo RetryInfoReleaseRetry
        {
            get { return RetryInfoTurnOffRetry; }
            set
            {
                if (RetryInfoTurnOffRetry == value) return;

                RetryInfoTurnOffRetry = value;
                NotifyPropertyChanged("RetryInfoReleaseRetry");
            }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool IgnoreLockFail
        {
            get { return IgnoreOnActionFail; }
            set
            {
                if (IgnoreOnActionFail == value) return;

                IgnoreOnActionFail = value;
                NotifyPropertyChanged("IgnoreLockFail");
            }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool IgnoreReleaseFail
        {
            get { return IgnoreOffActionFail; }
            set
            {
                if (IgnoreOffActionFail == value) return;

                IgnoreOffActionFail = value;
                NotifyPropertyChanged("IgnoreReleaseFail");
            }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool EnableRetryLock
        {
            get { return EnableRetryOnAction; }
            set
            {
                if (EnableRetryOnAction == value) return;
                EnableRetryOnAction = value;
                NotifyPropertyChanged("EnableRetryLock");
            }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool EnableRetryRelease
        {
            get { return EnableRetryOffAction; }
            set
            {
                if (EnableRetryOffAction == value) return;
                EnableRetryOffAction = value;
                NotifyPropertyChanged("EnableRetryRelease");
            }
        }

        private FAStatus _oldStatus;

        public override void Validate()
        {
            base.Validate();

            if (_oldStatus != Status)
            {
                _oldStatus = Status;
                NotifyPropertyChanged("Status");
            }
        }
    }

    public class FAPartGripRelease : FAPartMemoryBaseGeneric
    {
        private StatusDefine _statusList = new StatusDefine();

        public class StatusDefine
        {
            private FAStatus _grip = new FAStatus("Grip");
            private FAStatus _release = new FAStatus("Release");
            private FAStatus _unknown = new FAStatus("Unknown");

            public FAStatus Grip { get { return _grip; } }
            public FAStatus Release { get { return _release; } }
            public FAStatus Unknown { get { return _unknown; } }
        }

        public StatusDefine StatusList { get { return _statusList; } }

        [FAAttribute("Status")]
        public FAStatus Status
        {
            get
            {
                if (IsTurnOn() == true && IsTurnOff() == false)
                    return StatusList.Grip;
                else if (IsTurnOn() == false && IsTurnOff() == true)
                    return StatusList.Release;
                else
                    return StatusList.Unknown;
            }
        }

        public FAPartGripRelease(FASequenceManager aSequenceManager)
            : base(aSequenceManager)
        {
            Grip.ActionName = "GRIP";
            Release.ActionName = "RELEASE";
            OnStatusDisplayName = StatusList.Grip.DisplayName;
            OffStatusDisplayName = StatusList.Release.DisplayName;
        }

        public override void LoadParameters(System.Xml.Linq.XElement xml)
        {
            base.LoadParameters(xml);

            if (string.IsNullOrEmpty(OnStatusDisplayName) == false)
                StatusList.Grip.DisplayName = OnStatusDisplayName;

            if (string.IsNullOrEmpty(OffStatusDisplayName) == false)
                StatusList.Release.DisplayName = OffStatusDisplayName;
        }

        [FAAttribute("Action")]
        public FAPartAction Grip
        {
            get { return base.TurnOnAction; }
        }

        [FAAttribute("Action")]
        public FAPartAction Release
        {
            get { return base.TurnOffAction; }
        }

        [FAAttribute("Time")]
        public FATime GripTimeout
        {
            get { return TurnOnTimeout; }
            set
            {
                if (TurnOnTimeout == value) return;

                TurnOnTimeout = value;
                NotifyPropertyChanged("GripTimeout");
            }
        }

        [FAAttribute("Time")]
        public FATime ReleaseTimeout
        {
            get { return TurnOffTimeout; }
            set
            {
                if (TurnOffTimeout == value) return;

                TurnOffTimeout = value;
                NotifyPropertyChanged("ReleaseTimeout");
            }
        }

        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int GripTimeoutAlarm
        {
            get { return TurnOnTimeoutAlarm; }
            set
            {
                if (TurnOnTimeoutAlarm == value) return;

                TurnOnTimeoutAlarm = value;
                NotifyPropertyChanged("GripTimeoutAlarm");
            }
        }

        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int ReleaseTimeoutAlarm
        {
            get { return TurnOffTimeoutAlarm; }
            set
            {
                if (TurnOffTimeoutAlarm == value) return;

                TurnOffTimeoutAlarm = value;
                NotifyPropertyChanged("ReleaseTimeoutAlarm");
            }
        }

        [FAAttribute("RetryInfo")]
        public FARetryInfo RetryInfoGripRetry
        {
            get { return RetryInfoTurnOnRetry; }
            set
            {
                if (RetryInfoTurnOnRetry == value) return;

                RetryInfoTurnOnRetry = value;
                NotifyPropertyChanged("RetryInfoGripRetry");
            }
        }

        [FAAttribute("RetryInfo")]
        public FARetryInfo RetryInfoReleaseRetry
        {
            get { return RetryInfoTurnOffRetry; }
            set
            {
                if (RetryInfoTurnOffRetry == value) return;

                RetryInfoTurnOffRetry = value;
                NotifyPropertyChanged("RetryInfoReleaseRetry");
            }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool IgnoreGripFail
        {
            get { return IgnoreOnActionFail; }
            set
            {
                if (IgnoreOnActionFail == value) return;

                IgnoreOnActionFail = value;
                NotifyPropertyChanged("IgnoreGripFail");
            }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool IgnoreReleaseFail
        {
            get { return IgnoreOffActionFail; }
            set
            {
                if (IgnoreOffActionFail == value) return;

                IgnoreOffActionFail = value;
                NotifyPropertyChanged("IgnoreReleaseFail");
            }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool EnableRetryGrip
        {
            get { return EnableRetryOnAction; }
            set
            {
                if (EnableRetryOnAction == value) return;
                EnableRetryOnAction = value;
                NotifyPropertyChanged("EnableRetryGrip");
            }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool EnableRetryRelease
        {
            get { return EnableRetryOffAction; }
            set
            {
                if (EnableRetryOffAction == value) return;
                EnableRetryOffAction = value;
                NotifyPropertyChanged("EnableRetryRelease");
            }
        }

        private FAStatus _oldStatus;

        public override void Validate()
        {
            base.Validate();

            if (_oldStatus != Status)
            {
                _oldStatus = Status;
                NotifyPropertyChanged("Status");
            }
        }
    }

    public class FAPartPushHome : FAPartMemoryBaseGeneric
    {
        private StatusDefine _statusList = new StatusDefine();

        public class StatusDefine
        {
            private FAStatus _push = new FAStatus("Push");
            private FAStatus _home = new FAStatus("Home");
            private FAStatus _unknown = new FAStatus("Unknown");

            public FAStatus Push { get { return _push; } }
            public FAStatus Home { get { return _home; } }
            public FAStatus Unknown { get { return _unknown; } }
        }

        public StatusDefine StatusList { get { return _statusList; } }

        [FAAttribute("Status")]
        public FAStatus Status
        {
            get
            {
                if (IsTurnOn() == true && IsTurnOff() == false)
                    return StatusList.Push;
                else if (IsTurnOn() == false && IsTurnOff() == true)
                    return StatusList.Home;
                else
                    return StatusList.Unknown;
            }
        }

        public FAPartPushHome(FASequenceManager aSequenceManager)
            : base(aSequenceManager)
        {
            Push.ActionName = "PUSH";
            Home.ActionName = "HOME";
            OnStatusDisplayName = StatusList.Push.DisplayName;
            OffStatusDisplayName = StatusList.Home.DisplayName;
        }

        public override void LoadParameters(System.Xml.Linq.XElement xml)
        {
            base.LoadParameters(xml);

            if (string.IsNullOrEmpty(OnStatusDisplayName) == false)
                StatusList.Push.DisplayName = OnStatusDisplayName;

            if (string.IsNullOrEmpty(OffStatusDisplayName) == false)
                StatusList.Home.DisplayName = OffStatusDisplayName;
        }

        [FAAttribute("Action")]
        public FAPartAction Push
        {
            get { return base.TurnOnAction; }
        }

        [FAAttribute("Action")]
        public FAPartAction Home
        {
            get { return base.TurnOffAction; }
        }

        [FAAttribute("Time")]
        public FATime PushTimeout
        {
            get { return TurnOnTimeout; }
            set
            {
                if (TurnOnTimeout == value) return;

                TurnOnTimeout = value;
                NotifyPropertyChanged("PushTimeout");
            }
        }

        [FAAttribute("Time")]
        public FATime HomeTimeout
        {
            get { return TurnOffTimeout; }
            set
            {
                if (TurnOffTimeout == value) return;

                TurnOffTimeout = value;
                NotifyPropertyChanged("HomeTimeout");
            }
        }

        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int PushTimeoutAlarm
        {
            get { return TurnOnTimeoutAlarm; }
            set
            {
                if (TurnOnTimeoutAlarm == value) return;

                TurnOnTimeoutAlarm = value;
                NotifyPropertyChanged("PushTimeoutAlarm");
            }
        }

        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int HomeTimeoutAlarm
        {
            get { return TurnOffTimeoutAlarm; }
            set
            {
                if (TurnOffTimeoutAlarm == value) return;

                TurnOffTimeoutAlarm = value;
                NotifyPropertyChanged("HomeTimeoutAlarm");
            }
        }

        [FAAttribute("RetryInfo")]
        public FARetryInfo RetryInfoPushRetry
        {
            get { return RetryInfoTurnOnRetry; }
            set
            {
                if (RetryInfoTurnOnRetry == value) return;

                RetryInfoTurnOnRetry = value;
                NotifyPropertyChanged("RetryInfoPushRetry");
            }
        }

        [FAAttribute("RetryInfo")]
        public FARetryInfo RetryInfoHomeRetry
        {
            get { return RetryInfoTurnOffRetry; }
            set
            {
                if (RetryInfoTurnOffRetry == value) return;

                RetryInfoTurnOffRetry = value;
                NotifyPropertyChanged("RetryInfoHomeRetry");
            }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool IgnorePushFail
        {
            get { return IgnoreOnActionFail; }
            set
            {
                if (IgnoreOnActionFail == value) return;

                IgnoreOnActionFail = value;
                NotifyPropertyChanged("IgnorePushFail");
            }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool IgnoreHomeFail
        {
            get { return IgnoreOffActionFail; }
            set
            {
                if (IgnoreOffActionFail == value) return;

                IgnoreOffActionFail = value;
                NotifyPropertyChanged("IgnoreHomeFail");
            }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool EnableRetryPush
        {
            get { return EnableRetryOnAction; }
            set
            {
                if (EnableRetryOnAction == value) return;
                EnableRetryOnAction = value;
                NotifyPropertyChanged("EnableRetryPush");
            }
        }

        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool EnableRetryHome
        {
            get { return EnableRetryOffAction; }
            set
            {
                if (EnableRetryOffAction == value) return;
                EnableRetryOffAction = value;
                NotifyPropertyChanged("EnableRetryHome");
            }
        }

        private FAStatus _oldStatus;

        public override void Validate()
        {
            base.Validate();

            if (_oldStatus != Status)
            {
                _oldStatus = Status;
                NotifyPropertyChanged("Status");
            }
        }
    }

    public class FAPartOneWayACMotor : FAMemoryBasePart
    {
        private StatusDefine _statusList = new StatusDefine();
        private FAPartAction _runAction = new FAPartAction();
        private FAPartAction _stopAction = new FAPartAction();

        [FAAttribute("Action")]
        public FAPartAction Run
        {
            get { return _runAction; }
        }

        [FAAttribute("Action")]
        public FAPartAction Stop
        {
            get { return _stopAction; }
        }

        public class StatusDefine
        {
            private FAStatus _running = new FAStatus("Running");
            private FAStatus _stop = new FAStatus("Stop");
            private FAStatus _unknown = new FAStatus("Unknown");

            public FAStatus Running { get { return _running; } }
            public FAStatus Stop { get { return _stop; } }
            public FAStatus Unknown { get { return _unknown; } }
        }

        public StatusDefine StatusList { get { return _statusList; } }

        [FAAttribute("")]
        public FAStatus Status
        {
            get
            {
                if (IsRunning() == true || IsStop() == false)
                    return StatusList.Running;
                else if (IsStop() == true && IsRunning() == false)
                    return StatusList.Stop;
                else
                    return StatusList.Unknown;
            }
        }

        public FAPartOneWayACMotor(FASequenceManager aSequenceManager)
        {
            Run.SetActionMethod(DoRun);
            Stop.SetActionMethod(DoStop);

            Run.CreateSequence(aSequenceManager);
            Stop.CreateSequence(aSequenceManager);

            Run.Sequence.AddItem(Run.ExecuteForSequence);

            Stop.Sequence.AddItem(Stop.ExecuteForSequence);

            Run.ActionName = "Run";
            Stop.ActionName = "Stop";
        }

        public bool IsRunning()
        {
            if (InputIO.Count > 0)            
                return InputIO[0].CorrectionValue;

            if (OutputIO.Count > 0)
                return OutputIO[0].CorrectionValue;

            return false;
        }

        public bool IsStop()
        {
            if (InputIO.Count > 0)
            {
                if (InputIO[0].CorrectionValue == false)
                    return true;
                else
                    return false;
            }

            if (OutputIO.Count > 0)
                return !OutputIO[0].CorrectionValue;

            return false;
        }

        protected void DoRun(object sender)
        {
            if (OutputIO.Count > 0)
                OutputIO[0].CorrectionValue = true;

            if (OutputIO.Count > 1)
                OutputIO[1].CorrectionValue = false;

            //if (SimulationMode)
            //{
            //    if (InputIO.Count > 0)
            //        InputIO[0].CorrectionValue = true;
            //}
        }

        protected void DoStop(object sender)
        {
            if (OutputIO.Count > 0)
                OutputIO[0].CorrectionValue = false;

            if (OutputIO.Count > 1)
                OutputIO[1].CorrectionValue = true;

            //if (SimulationMode)
            //{
            //    if (InputIO.Count > 0)
            //        InputIO[0].CorrectionValue = false;
            //}
        }

        private FAStatus _oldStatus;

        public override void Validate()
        {
            base.Validate();

            if (_oldStatus != Status)
            {
                _oldStatus = Status;
                NotifyPropertyChanged("Status");
            }
        }

        public bool IsRunOutputOn()
        {
            if (OutputIO.Count > 0)
                return OutputIO[0].CorrectionValue;

            return false;
        }
    }

    public class FAPartTwoWayACMotor : FAMemoryBasePart
    {
        private StatusDefine _statusList = new StatusDefine();
        private FAPartAction _runAction = new FAPartAction();
        private FAPartAction _reverseRunAction = new FAPartAction();
        private FAPartAction _stopAction = new FAPartAction();

        [FAAttribute("Action")]
        public FAPartAction RunAction
        {
            get { return _runAction; }
        }

        [FAAttribute("Action")]
        public FAPartAction ReverseRunAction
        {
            get { return _reverseRunAction; }
        }

        [FAAttribute("Action")]
        public FAPartAction StopAction
        {
            get { return _stopAction; }
        }

        private bool _manualJogMode;
        [FAAttribute("Parameter")]
        [FAProperty]
        public bool ManualJogMode
        {
            get { return _manualJogMode; }
            set
            {
                if (_manualJogMode == value) return;
                _manualJogMode = value;
                NotifyPropertyChanged("ManualJogMode");
            }
        }

        public class StatusDefine
        {
            private FAStatus _running = new FAStatus("Running");
            private FAStatus _reverseRunn = new FAStatus("ReverseRun");
            private FAStatus _stop = new FAStatus("Stop");
            private FAStatus _unknown = new FAStatus("Unknown");

            public FAStatus Running { get { return _running; } }
            public FAStatus ReverseRun { get { return _reverseRunn; } }
            public FAStatus Stop { get { return _stop; } }
            public FAStatus Unknown { get { return _unknown; } }
        }

        public StatusDefine StatusList { get { return _statusList; } }

        [FAAttribute("Status")]
        public FAStatus Status
        {
            get
            {
                if (IsRunOutputOn() == true)
                    return StatusList.Running;
                else if (IsReverseRunOutputOn() == true)
                    return StatusList.ReverseRun;
                else if (IsStop() == true && IsRunOutputOn() == false && IsReverseRunOutputOn() == false)
                    return StatusList.Stop;
                else
                    return StatusList.Unknown;
            }
        }

        public FAPartTwoWayACMotor(FASequenceManager aSequenceManager)
        {
            RunAction.SetActionMethod(DoRun);
            ReverseRunAction.SetActionMethod(DoReverseRun);
            StopAction.SetActionMethod(DoStop);            

            RunAction.CreateSequence(aSequenceManager);
            ReverseRunAction.CreateSequence(aSequenceManager);
            StopAction.CreateSequence(aSequenceManager);

            RunAction.Sequence.AddItem(RunAction.ExecuteForSequence);
            ReverseRunAction.Sequence.AddItem(ReverseRunAction.ExecuteForSequence);
            StopAction.Sequence.AddItem(StopAction.ExecuteForSequence);

            RunAction.ActionName = "Run";
            ReverseRunAction.ActionName = "ReverseRun";
            StopAction.ActionName = "Stop";
        }

        //public bool IsRunning()
        //{
        //    if (InputIO.Count > 0)
        //        return InputIO[0].CorrectionValue;

        //    return false;
        //}

        public bool IsStop()
        {
            if (OutputIO.Count == 2)
            {
                if (OutputIO[0].CorrectionValue == false &&
                    OutputIO[1].CorrectionValue == false)
                    return true;
                else
                    return false;
            }
            if (OutputIO.Count == 3)
            {
                if (OutputIO[0].CorrectionValue == false &&
                    OutputIO[1].CorrectionValue == false &&
                    OutputIO[2].CorrectionValue == false)
                    return true;
                else
                    return false;
            }
            return false;
        }

        protected void DoRun(object sender)
        {
            if (OutputIO.Count == 2)
            {
                OutputIO[0].CorrectionValue = true;
                OutputIO[1].CorrectionValue = false;
            }
            else if (OutputIO.Count == 3)
            {
                OutputIO[0].CorrectionValue = true;
                OutputIO[1].CorrectionValue = true;
                OutputIO[2].CorrectionValue = false;
            }

            //if (SimulationMode)
            //{
            //    if (InputIO.Count > 0)
            //        InputIO[0].CorrectionValue = true;
            //}
        }

        protected void DoReverseRun(object sender)
        {
            if (OutputIO.Count == 2)
            {
                OutputIO[0].CorrectionValue = false;
                OutputIO[1].CorrectionValue = true;
            }
            else if (OutputIO.Count == 3)
            {
                OutputIO[0].CorrectionValue = true;
                OutputIO[1].CorrectionValue = false;
                OutputIO[2].CorrectionValue = true;
            }

            //if (SimulationMode)
            //{
            //    if (InputIO.Count > 0)
            //        InputIO[0].CorrectionValue = true;
            //}
        }

        protected void DoStop(object sender)
        {
            AllOffOutput(sender);

            //if (SimulationMode)
            //{
            //    if (InputIO.Count > 0)
            //        InputIO[0].CorrectionValue = false;
            //}
        }

        private FAStatus _oldStatus;

        public override void Validate()
        {
            base.Validate();

            if (_oldStatus != Status)
            {
                _oldStatus = Status;
                NotifyPropertyChanged("Status");
            }
        }

        public bool IsRunOutputOn()
        {
            if (OutputIO.Count == 2)
            {
                return OutputIO[0].CorrectionValue && !OutputIO[1].CorrectionValue;
            }
            else if (OutputIO.Count == 3)
            {
                return OutputIO[0].CorrectionValue &&
                    OutputIO[1].CorrectionValue &&
                    !OutputIO[2].CorrectionValue;
            }

            return false;
        }

        public bool IsReverseRunOutputOn()
        {
            if (OutputIO.Count == 2)
            {
                return !OutputIO[0].CorrectionValue && OutputIO[1].CorrectionValue;
            }
            else if (OutputIO.Count == 3)
            {
                return OutputIO[0].CorrectionValue &&
                    !OutputIO[1].CorrectionValue &&
                    OutputIO[2].CorrectionValue;
            }

            return false;
        }
    }

    public class FAPartLoadUnloadACMotor : FAPartTwoWayACMotor
    {
        [FAAttribute("Action")]
        public FAPartAction LoadingAction
        {
            get { return RunAction; }
        }

        [FAAttribute("Action")]
        public FAPartAction UnloadingAction
        {
            get { return ReverseRunAction; }
        }

        public FAPartLoadUnloadACMotor(FASequenceManager aSequenceManager) : base(aSequenceManager)
        {
            LoadingAction.ActionName = "Load";
            UnloadingAction.ActionName = "Unload";
            StopAction.ActionName = "Stop";
        }

        public bool IsLoadingOutputOn()
        {
            return IsRunOutputOn();
        }

        public bool IsUnloadingOutputOn()
        {
            return IsReverseRunOutputOn();
        }
    }

    public class FAPartUpDownACMotor : FAPartTwoWayACMotor
    {
        [FAAttribute("Action")]
        public FAPartAction UpAction
        {
            get { return RunAction; }
        }

        [FAAttribute("Action")]
        public FAPartAction DownAction
        {
            get { return ReverseRunAction; }
        }

        public FAPartUpDownACMotor(FASequenceManager aSequenceManager)
            : base(aSequenceManager)
        {
            UpAction.ActionName = "Up";
            DownAction.ActionName = "Down";
            StopAction.ActionName = "Stop";
        }

        public bool IsUpOutputOn()
        {
            return IsRunOutputOn();
        }

        public bool IsDownOutputOn()
        {
            return IsReverseRunOutputOn();
        }
    }
}
