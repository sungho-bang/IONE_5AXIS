using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary;
using FALibrary.Sequence;
using FALibrary.Utility;
using FALibrary.Part.MemoryBasePart;
using FAFramework.Utility;
using FAFramework.GUI;
using System.Diagnostics;

namespace FAFramework.Module
{
    public class FAKukaRobotModule : FAModule
    {
        public class SendDataInfo : FAObject
        {
            private byte _actionCode;
            [FAAttribute("")]
            public byte ActionCode
            {
                get { return _actionCode; }
                set
                {
                    if (_actionCode == value) return;
                    _actionCode = value;
                    NotifyPropertyChanged("ActionCode");
                }
            }

            // parameter 가 이면 kuka robot 이 동작하지 않는다.
            // 기본적으로 parameter가 1로 동작한다.
            private byte _parameter = 1;
            [FAAttribute("")]
            public byte Parameter
            {
                get { return _parameter; }
                set
                {
                    if (_parameter == value) return;
                    _parameter = value;
                    NotifyPropertyChanged("Parameter");
                }
            }

            public void Clear()
            {
                ActionCode = 0;
                Parameter = 0;
            }
        }

        public class ReceiveDataInfo : FAObject
        {
            private byte _commandCode;
            [FAAttribute("")]
            public byte CommandCode
            {
                get { return _commandCode; }
                set
                {
                    if (_commandCode == value) return;
                    _commandCode = value;
                    NotifyPropertyChanged("CommandCode");
                }
            }

            private byte _actionCode;
            [FAAttribute("")]
            public byte ActionCode
            {
                get { return _actionCode; }
                set
                {
                    if (_actionCode == value) return;
                    _actionCode = value;
                    NotifyPropertyChanged("ActionCode");
                }
            }

            private byte _parameter;
            [FAAttribute("")]
            public byte Parameter
            {
                get { return _parameter; }
                set
                {
                    if (_parameter == value) return;
                    _parameter = value;
                    NotifyPropertyChanged("Parameter");
                }
            }

            private byte _resultParameter;
            [FAAttribute("")]
            public byte ResultParameter
            {
                get { return _resultParameter; }
                set
                {
                    if (_resultParameter == value) return;
                    _resultParameter = value;
                    NotifyPropertyChanged("ResultParameter");
                }
            }

            private bool _actionSuccess;
            [FAAttribute("")]
            public bool ActionSuccess
            {
                get { return _actionSuccess; }
                set
                {
                    if (_actionSuccess == value) return;
                    _actionSuccess = value;
                    NotifyPropertyChanged("ActionSuccess");
                }
            }

            private bool _actionFail;
            [FAAttribute("")]
            public bool ActionFail
            {
                get { return _actionFail; }
                set
                {
                    if (_actionFail == value) return;
                    _actionFail = value;
                    NotifyPropertyChanged("ActionFail");
                }
            }

            public void Clear()
            {
                CommandCode = 0;
                ActionCode = 0;
                Parameter = 0;
                ResultParameter = 0;
                ActionSuccess = false;
                ActionFail = false;
            }
        }

        public enum SubActionResultType
        {
            None, NextStep, Terminate
        }

        public List<Func<TimeSpan, SubActionResultType>> SubActionList { get; set; }
        private Stopwatch _subActionStopWatch = new Stopwatch();

        #region Sequences
        [FAAttribute("Sequences")]
        public FASequence ExecuteAction { get; set; }
        #endregion

        #region Time
        [FAAttribute("Time")]
        public FATime TimeRobotActionTimeout { get; set; }
        [FAAttribute("Time")]
        public FATime TimeRobotActionPulseTime { get; set; }
        [FAAttribute("Time")]
        public FATime TimeRobotStatusCheckTime { get; set; }
        #endregion

        #region Alarm
        [DefaultAlarmInfo(1, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        [AlarmDescription(KnownCulture.EnglishUS, "Robot emergency", "Check robot and robot controller")]
        [AlarmDescription(KnownCulture.Korean, "로봇 비상 정지 상태.", "로봇을 확인하세요.")]
        public int AlarmRobotEmergency { get; set; }

        [DefaultAlarmInfo(2, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        [AlarmDescription(KnownCulture.EnglishUS, "Robot not ready", "Check robot and robot controller")]
        [AlarmDescription(KnownCulture.Korean, "로봇이 준비 상태가 아닙니다.", "로봇을 확인하세요.")]
        public int AlarmRobotNotReady { get; set; }

        [DefaultAlarmInfo(3, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        [AlarmDescription(KnownCulture.EnglishUS, "Robot running", "Check robot and robot controller")]
        [AlarmDescription(KnownCulture.Korean, "로봇이 동작 중입니다.", "로봇을 확인하세요.")]
        public int AlarmRobotRunning { get; set; }

        [DefaultAlarmInfo(4, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        [AlarmDescription(KnownCulture.EnglishUS, "Robot action timeout", "Check robot and robot controller")]
        [AlarmDescription(KnownCulture.Korean, "로봇 동작 시간 초과.", "로봇을 확인하세요.")]
        public int AlarmRobotActionTimeOut { get; set; }

        [DefaultAlarmInfo(5, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        [AlarmDescription(KnownCulture.EnglishUS, "Robot action fail", "Check robot and robot controller")]
        [AlarmDescription(KnownCulture.Korean, "로봇 동작 실패.", "로봇을 확인하세요.")]
        public int AlarmRobotActionFail { get; set; }

        [DefaultAlarmInfo(6, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        [AlarmDescription(KnownCulture.EnglishUS, "Robot alarm state", "Check robot and robot controller")]
        [AlarmDescription(KnownCulture.Korean, "로봇 알람 상태.", "로봇을 확인하세요.")]
        public int AlarmRobotAlarm { get; set; }

        [DefaultAlarmInfo(7, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        [AlarmDescription(KnownCulture.EnglishUS, "Robot controller is not ready", "Check robot and robot controller")]
        [AlarmDescription(KnownCulture.Korean, "로봇 컨트롤러가 준비 상태 아닙니다.", "로봇을 확인하세요.")]
        public int AlarmRobotControllerIsNotReady { get; set; }

        [DefaultAlarmInfo(8, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        [AlarmDescription(KnownCulture.EnglishUS, "Robot motor drive power off", "Check robot and robot controller")]
        [AlarmDescription(KnownCulture.Korean, "로봇 모터 드라이브 전원 미인가.", "로봇을 확인하세요.")]
        public int AlarmMotorDriverPowerOff { get; set; }

        [DefaultAlarmInfo(9, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        [AlarmDescription(KnownCulture.EnglishUS, "Robot moving", "Check robot and robot controller")]
        [AlarmDescription(KnownCulture.Korean, "로봇 동작 중.", "로봇을 확인하세요.")]
        public int AlarmRobotMoving { get; set; }

        [DefaultAlarmInfo(10, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        [AlarmDescription(KnownCulture.EnglishUS, "Robot is not safety position", "Check robot and robot controller")]
        [AlarmDescription(KnownCulture.Korean, "로봇이 안전위치에 있지 않음.", "로봇을 확인하세요.")]
        public int AlarmRobotIsNotSafetyPosition { get; set; }

        [DefaultAlarmInfo(11, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        [AlarmDescription(KnownCulture.EnglishUS, "Robot is not auto mode", "Check robot and robot controller")]
        [AlarmDescription(KnownCulture.Korean, "로봇이 외부 자동모드가 아닙니다.", "로봇을 확인하세요.")]
        public int AlarmRobotIsNotAutoMode { get; set; }

        [DefaultAlarmInfo(12, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        [AlarmDescription(KnownCulture.EnglishUS, "Robot can not off the job terminate signal", "Check robot and robot controller")]
        [AlarmDescription(KnownCulture.Korean, "로봇 잡 완료 신호가 꺼지지 않았습니다.", "로봇을 확인하세요.")]
        public int AlarmRobotCanNotOffJobTerminateSignal { get; set; }
        #endregion

        #region RetryInfo
        [FAAttribute("RetryInfo")]
        public FARetryInfo RetryInfoConfirmTerminate { get; set; }
        #endregion

        #region Status
        private SendDataInfo _sendData = new SendDataInfo();
        [FAAttribute("Status")]
        public SendDataInfo SendData
        {
            get { return _sendData; }
            set
            {
                if (_sendData == value) return;
                _sendData = value;
                NotifyPropertyChanged("SendData");
            }
        }

        private ReceiveDataInfo _receiveData = new ReceiveDataInfo();
        [FAAttribute("Status")]
        public ReceiveDataInfo ReceiveData
        {
            get { return _receiveData; }
            set
            {
                if (_receiveData == value) return;
                _receiveData = value;
                NotifyPropertyChanged("ReceiveData");
            }
        }

        private bool _subActionTerminated;
        [FAAttribute("Status")]
        public bool SubActionTerminated
        {
            get { return _subActionTerminated; }
            set
            {
                if (_subActionTerminated == value) return;
                _subActionTerminated = value;
                NotifyPropertyChanged("SubActionTerminated");
            }
        }

        private int _currentSubActionIndex;
        [FAAttribute("Status")]
        public int CurrentSubActionIndex
        {
            get { return _currentSubActionIndex; }
            set
            {
                if (_currentSubActionIndex == value) return;
                _currentSubActionIndex = value;
                NotifyPropertyChanged("CurrentSubActionIndex");
            }
        }

        private bool _actionResult;
        [FAAttribute("Status")]
        public bool ActionResult
        {
            get { return _actionResult; }
            set
            {
                if (_actionResult == value) return;
                _actionResult = value;
                NotifyPropertyChanged("ActionResult");
            }
        }

        private int _alarmNo;
        [FAAttribute("Status")]
        public int AlarmNo
        {
            get { return _alarmNo; }
            set
            {
                if (_alarmNo == value) return;
                _alarmNo = value;
                NotifyPropertyChanged("AlarmNo");
            }
        }

        private string _moreAlarmMessage;
        [FAAttribute("Status")]
        public string MoreAlarmMessage
        {
            get { return _moreAlarmMessage; }
            set
            {
                if (_moreAlarmMessage == value) return;
                _moreAlarmMessage = value;
                NotifyPropertyChanged("MoreAlarmMessage");
            }
        }

        public int RobotAlarmCode { get; private set; }
        #endregion

        #region Parts
        public FAPartOnOff ActionRequest { get; set; }
        public FAPartOnOff ActionTerminatedConfirm { get; set; }
        public FAPartOnOffSensor RobotStatusEmergency { get; set; }
        public FAPartOnOffSensor RobotStatusRunning { get; set; }
        public FAPartOnOffSensor RobotStatusReady { get; set; }
        public FAPartOnOffSensor RobotStatusActionFail { get; set; }
        public FAPartOnOffSensor RobotStatusActionSuccess { get; set; }
        public FAPartByte OutputCommandAction { get; set; }
        public FAPartByte OutputCommandParameter { get; set; }
        public FAPartByte InputCommandAction { get; set; }
        public FAPartByte InputCommandParameter { get; set; }
        public FAPartUInt16 InputAlarmCode { get; set; }
        public FAPartOnOffSensor ControllerReady { get; set; }
        public FAPartOnOffSensor DoorOpenedWhenAutoModeChanging { get; set; }
        public FAPartOnOffSensor MotorDriverPowerOn { get; set; }
        public FAPartOnOffSensor CommonSystemMessageOccurred { get; set; }
        public FAPartOnOffSensor RobotReady { get; set; }
        public FAPartOnOffSensor RobotMoving { get; set; }
        public FAPartOnOffSensor RobotHomePosition { get; set; }
        public FAPartOnOffSensor RobotSafetyPosition { get; set; }
        public FAPartOnOffSensor RobotProgramedPosition { get; set; }
        public FAPartOnOffSensor LowerSpeedManualMode { get; set; }
        public FAPartOnOffSensor HighSpeedManualMode { get; set; }
        public FAPartOnOffSensor InternalAutoMode { get; set; }
        public FAPartOnOffSensor ExternalAutoMode { get; set; }
        #endregion

        public string GetLastAlarmMessage()
        {
            try
            {
                var alarm = Utility.AlarmUtility.GetAlarm(AlarmNo, "Can not found alarm");
                return $"AlarmNo={AlarmNo}, AlarmName={alarm.AlarmName}, MoreAlarmMessage={MoreAlarmMessage}";
            }
            catch (Exception e)
            {
                return $"Raise Exception On GetLastAlarmMessage : {e.Message}";
            }
        }

        public override void InitializeSequence()
        {
            MakeExecuteAction();
        }

        public override void ClearProductInfo()
        {
            base.ClearProductInfo();

            SendData.Clear();
            ReceiveData.Clear();
            CurrentSubActionIndex = 0;
        }

        private void MakeExecuteAction()
        {
            var seq = ExecuteAction;

            #region Events
            seq.OnStart +=
                delegate
                {
                    OutputCommandAction.OutputData = 0;
                    OutputCommandParameter.OutputData = 0;
                    RetryInfoConfirmTerminate.ClearCount();
                    SubActionTerminated = false;
                    _subActionStopWatch.Reset();
                    ActionResult = false;
                    ReceiveData.Clear();
                    CurrentSubActionIndex = 0;
                    WriteTraceLog($"START ROBOT ACTION. ACTION CODE={SendData.ActionCode}, ACTION PARAM={SendData.Parameter}");
                };

            seq.OnStop += delegate { SubActionList = null; };
            seq.OnTerminate +=
                delegate
                {
                    SubActionList = null;
                    SendData.Clear();
                    _subActionStopWatch.Reset();
                };
            #endregion

            seq.AddStep("Start").StepIndex = seq.AddItem(ActionRequest.Off.Execute);
            seq.AddItem(
                (actor, time) =>
                {
                    var actionNo = InputCommandAction.InputData;
                    var parameter = InputCommandParameter.InputData;

                    if (actionNo == 0 &&
                        parameter == 0)
                    {
                        actor.NextStep();
                    }
                    else if (TimeRobotStatusCheckTime.Time < time)
                    {
                        AlarmNo = AlarmRobotActionFail;
                        MoreAlarmMessage = $"Action Reflection and Parameter Clear Fail. " +
                            $"Action Reflection={actionNo}, Parameter={parameter}";
                        actor.NextStep("Terminate");
                    }
                });
            seq.AddStep("ConfirmActionResultSignalOnBeforeSetActionCode").StepIndex = seq.AddItem(ConfirmActionResultSignalOnBeforeSetActionCode);
            seq.AddItem(ActionTerminatedConfirm.On.Execute);
            seq.AddStep("PulseOnActionBeforeSetActionCode").StepIndex = seq.AddItem(ConfirmPulseTimeOver);
            seq.AddItem(ActionTerminatedConfirm.Off.Execute);
            seq.AddStep("SetActionCode").StepIndex = seq.AddItem(SetActionCode);
            seq.AddItem(
                delegate (FASequence actor, TimeSpan time)
                {
                    if (OutputCommandAction.OutputData == InputCommandAction.InputData ||
                        OutputCommandParameter.OutputData == InputCommandParameter.InputData)
                    {
                        actor.NextStep();
                    }
                    else if (TimeRobotActionTimeout.Time < time)
                    {
                        AlarmNo = AlarmRobotActionFail;
                        MoreAlarmMessage = string.Format("Not response from kuka robot. OutputCommand={0}, OutputParameter={1}, InputCommand{2}, InputParameter={3}", OutputCommandAction.OutputData,
                            OutputCommandParameter.OutputData,
                            InputCommandAction.InputData,
                            InputCommandParameter.InputData);
                        actor.NextStep("Terminate");
                    }
                });
            seq.AddItem(ActionRequest.On.Execute);
            seq.AddStep("PulseOnAction").StepIndex = seq.AddItem(ConfirmPulseTimeOver);
            seq.AddItem(ActionRequest.Off.Execute);
            seq.AddStep("ConfirmActionTerminate").StepIndex = seq.AddItem(ConfirmActionTerminate);
            seq.AddStep("ActionBeforeTerminate").StepIndex = seq.AddItem(
                delegate (object obj)
                {
                    ActionRequest.Off.Execute(obj);
                    SubActionList = null;
                });
            seq.AddStep("TurnOnActionTerminatedConfirm").StepIndex = seq.AddItem(ActionTerminatedConfirm.On.Execute);
            seq.AddItem(
                delegate (FASequence actor, TimeSpan time)
                {
                    if (RetryInfoConfirmTerminate.IncreaseCount() == false)
                        RaiseAlarm(actor, AlarmRobotCanNotOffJobTerminateSignal);

                    if (RobotStatusActionSuccess.IsOff && RobotStatusActionFail.IsOff)
                        actor.NextStep("Terminate");
                    else
                        actor.NextStep();
                });
            seq.AddStep("PulseOffAction").StepIndex = seq.AddItem(ConfirmPulseTimeOver);
            seq.AddItem(ActionTerminatedConfirm.Off.Execute);
            seq.AddItem(
                delegate (FASequence actor, TimeSpan time)
                {
                    if (RobotStatusActionSuccess.IsOff && RobotStatusActionFail.IsOff)
                        actor.NextStep("Terminate");
                    else
                        actor.NextStep("TurnOnActionTerminatedConfirm");
                });
            seq.AddStep("Terminate").StepIndex = seq.AddItem(ActionTerminatedConfirm.Off.Execute);
            seq.AddTerminate();
        }

        private void ConfirmActionResultSignalOnBeforeSetActionCode(FASequence actor, TimeSpan time)
        {
            if (RobotStatusActionFail.IsOn || RobotStatusActionSuccess.IsOn)
            {
                actor.NextStep();
            }
            else
                actor.NextStep("SetActionCode");
        }

        private void ConfirmPulseTimeOver(FASequence actor, TimeSpan time)
        {
            if (TimeRobotActionPulseTime.Time < time)
                actor.NextStep();
        }

        private void SetActionCode(FASequence actor, TimeSpan time)
        {
            int alarm;

            if (IsReady(out alarm) == false)
            {
                AlarmNo = alarm;
                MoreAlarmMessage = string.Empty;
                actor.NextStep("Terminate");
            }
            else if (RobotStatusReady.IsOn && RobotStatusActionSuccess.IsOff && RobotStatusActionFail.IsOff)
            {
                OutputCommandParameter.OutputData = SendData.Parameter;
                OutputCommandAction.OutputData = SendData.ActionCode;
                actor.NextStep();
            }
            else
                actor.NextStep("Start");
        }

        private void ConfirmActionTerminate(FASequence actor, TimeSpan time)
        {
            if (SubActionList != null && SubActionTerminated == false)
                ExecuteSubAction();

            if (RobotStatusEmergency.IsOn)
            {
                WriteTraceLog("ROBOT EMERGENCY");
                AlarmNo = AlarmRobotEmergency;
                actor.NextStep("ActionBeforeTerminate");
            }
            else if (IsActionSuccess() == true)
            {
                WriteTraceLog($"ROBOT ACTION SUCCESS. ACTION CODE={SendData.ActionCode}, ACTION PARAM={SendData.Parameter}, SUCCESS={RobotStatusActionSuccess.IsOn}, FAIL={RobotStatusActionFail.IsOn}");
                ActionResult = true;
                actor.NextStep("ActionBeforeTerminate");
            }
            else if (IsActionFail() == true)
            {
                WriteTraceLog($"ROBOT ACTION FAIL. ACTION CODE={SendData.ActionCode}, ACTION PARAM={SendData.Parameter}, SUCCESS={RobotStatusActionSuccess.IsOn}, FAIL={RobotStatusActionFail.IsOn}");
                AlarmNo = AlarmRobotActionFail;
                MoreAlarmMessage = "KUKA ROBOT ACTION ERROR NO : " + InputAlarmCode.InputData.ToString();
                RobotAlarmCode = InputAlarmCode.InputData;
                actor.NextStep("ActionBeforeTerminate");
            }
            else if (TimeRobotActionTimeout.Time < time)
            {
                var timeStatus = $"{TimeRobotActionTimeout.Time.ToString(@"hh\:mm\:ss\.fff")}/{time.ToString(@"hh\:mm\:ss\.fff")}";
                WriteTraceLog($"ROBOT ACTION TIEMOUT. {timeStatus}");
                AlarmNo = AlarmRobotActionTimeOut;
                actor.NextStep("ActionBeforeTerminate");
            }
        }

        public bool IsReady(out int alarm)
        {
            alarm = -1;

            if (ControllerReady.IsOn == false) alarm = AlarmRobotControllerIsNotReady;
            else if (MotorDriverPowerOn.IsOn == false) alarm = AlarmMotorDriverPowerOff;
            else if (RobotStatusEmergency.IsOn) alarm = AlarmRobotEmergency;
            else if (RobotReady.IsOn == false) alarm = AlarmRobotNotReady;
            else if (RobotMoving.IsOff == false) alarm = AlarmRobotMoving;
            else if (RobotSafetyPosition.IsOn == false) alarm = AlarmRobotIsNotSafetyPosition;
            else if ((InternalAutoMode.IsOn || ExternalAutoMode.IsOn) == false) alarm = AlarmRobotIsNotAutoMode;

            if (alarm >= 0) return false;
            else return true;
        }

        private bool IsActionSuccess()
        {
            if (RobotStatusActionSuccess.IsOn &&
                RobotStatusActionFail.IsOff)
            {
                ReceiveData.ActionSuccess = true;
                ReceiveData.ActionFail = false;
                return true;
            }

            return false;
        }

        private bool IsActionFail()
        {
            if (RobotStatusActionFail.IsOn)
            {
                ReceiveData.ActionSuccess = false;
                ReceiveData.ActionFail = true;
                return true;
            }

            return false;
        }

        private void ExecuteSubAction()
        {
            if (SubActionList == null) return;

            if (CurrentSubActionIndex >= SubActionList.Count) return;
            if (CurrentSubActionIndex < 0) return;

            var method = SubActionList[CurrentSubActionIndex];

            if (_subActionStopWatch.IsRunning == false)
                _subActionStopWatch.Start();

            var methodResult = method(_subActionStopWatch.Elapsed);

            if (methodResult == SubActionResultType.NextStep)
            {
                WriteTraceLog($"ACTION={SendData.ActionCode}. SUB ACTION NEXT STEP. CURRENT STEP IS {CurrentSubActionIndex}");

                _subActionStopWatch.Reset();

                if (CurrentSubActionIndex == SubActionList.Count - 1)
                    SubActionTerminated = true;
                else
                {
                    CurrentSubActionIndex++;

                    if (CurrentSubActionIndex >= SubActionList.Count)
                    {
                        SubActionTerminated = true;
                        _subActionStopWatch.Reset();
                    }
                }

                WriteTraceLog("ACTION={SendData.ActionCode}. ACTION TERMINATED BY AUTOMATIC");
            }
            else if (methodResult == SubActionResultType.Terminate)
            {
                WriteTraceLog("ACTION={SendData.ActionCode}. ACTION TERMINATED BY MANUALLY");
                SubActionTerminated = true;
                _subActionStopWatch.Reset();
            }
        }
    }
}
