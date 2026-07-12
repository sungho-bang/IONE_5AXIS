using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Device.MMCDevice;
using FALibrary.Sequence;
using FALibrary.Utility;
using System.Xml.Linq;
using System.Reflection;
using System.Xml;
using System.Diagnostics;
using System.IO;

namespace FALibrary.Part.MMCPart
{
    public class FAMMCPart : FAPart
    {
        public class CanIMoveInterlockEventArgs : EventArgs
        {
            public double TargetPosition { get; set; }

            public CanIMoveInterlockEventArgs()
            {
            }

            public CanIMoveInterlockEventArgs(double targetPosition)
            {
                TargetPosition = targetPosition;
            }
        }

        public class LimitInterlockEventArgs : EventArgs
        {
            public string Message { get; set; }

            public LimitInterlockEventArgs()
            {
            }

            public LimitInterlockEventArgs(string msg)
            {
                Message = msg;
            }
        }

        public enum Direction
        {
            Positive,
            Negative
        }

        protected class PositionUtility
        {
            public FAMMCPart Part { get; set; }
            public FAMMCPosition Position { get; set; }
            public FAPartAction PartAction { get; set; }
            public FATime Timeout { get; set; }
            public string AlarmPropertyName { get; set; }
            public FARetryInfo RetryInfoMoveRetry { get; set; }
            public bool Suspended { get; private set; }

            /// <summary>
            /// 이동을 정지하고 시퀀스를 끝낸다.
            /// </summary>
            public Func<bool> CustomMotionCompleteCondition { get; set; }

            public void Initialize()
            {
                PartAction.SetActionMethod(DoMove);
                
                PartAction.InterlockList.Add(
                    delegate(ref string msg)
                    {
                        if (Part.CanIMove(Position.Position) == false)
                            return true;
                        else if (Position.Position < Part.ActualPos)
                            return Part.CheckDynamicLimitInterlock(Direction.Negative, ref msg);
                        else
                            return Part.CheckDynamicLimitInterlock(Direction.Positive, ref msg);
                    });

                FASequence seq = PartAction.Sequence;
                seq.OnStart += EventHandlerOnStart;
                seq.OnSuspended +=
                    delegate
                    {
                        Suspended = true;
                        Part.Stop.Execute(this);
                    };

                seq.Steps.Add("StartMove", new StepInfo());
                seq.Steps["StartMove"].StepIndex = seq.AddItem(PartAction.ExecuteForSequence);
                seq.AddItem(ConfirmMovingCompleted);
            }

            private void EventHandlerOnStart(object sender, EventArgs e)
            {
                if (RetryInfoMoveRetry != null)
                    RetryInfoMoveRetry.ClearCount();

                Suspended = false;
            }

            private void DoMove(object sender)
            {
                if (Part.LinkMode)
                    Part.SetLinkMode(Part.SlaveAxis, Part.LinkRatio);

                Part.MovePosition(Position);

//jbpark_2019.05.07
                //if (Part.isStepMot)
                //{
                //    Part.MoveIncPosition(Position);
                //}
                //else
                //{
                //    Part.MovePosition(Position);
                //}
            }

            private void ConfirmMovingCompleted(FASequence actor, TimeSpan time)
            {
                //if (Part.ServoAlarm)
                //{
                //    FALibrary.Alarm.FAAlarmManager.Instance.RaiseAlarm(actor, Part.ServoAlarmFlagOn, Position.ToString());
                //    actor.NextStep("StartMove");
                //}
                //else 
                if (Part.AxisNo == 8 || Part.AxisNo == 9 || Part.AxisNo == 10 || Part.AxisNo == 11)
                {

                    if (Suspended && Part.RunFlag == false)
                    {
                        Suspended = false;
                        actor.NextStep("StartMove");
                    }
                    else if (Part.IsMotionDone())//&& Part.IsInPosition(Position))
                    {
                        if (Part.LinkMode)
                            Part.ResetLinkeMode();
                        actor.NextStep();
                    }
                    else if (CustomMotionCompleteCondition != null &&
                        CustomMotionCompleteCondition())
                    {
                        Part.DoStop(actor);
                        actor.NextStep();
                    }
                    else if (Timeout.Time < time)
                    {
                        Part.DoStop(actor);



                        int alarm = 0;
                        Type type = Part.GetType();
                        PropertyInfo info = type.GetProperty(AlarmPropertyName);
                        if (info != null)
                            alarm = (int)info.GetValue(Part, null);

                        if (RetryInfoMoveRetry != null)
                        {
                            if (RetryInfoMoveRetry.IncreaseCount() == false)
                            {
                                if (Part.LinkMode)
                                    Part.Device.Stop(Part.AxisNo);
                                FALibrary.Alarm.FAAlarmManager.Instance.RaiseAlarm(actor, alarm, Position.ToString());
                            }
                        }
                        else
                        {
                            if (Part.LinkMode)
                                Part.Device.Stop(Part.AxisNo);

                            if (Part.AxisNo == 10)
                            {
                                Part.DoStop(actor);
                                actor.NextStep();
                            }
                            else
                            {
                                FALibrary.Alarm.FAAlarmManager.Instance.RaiseAlarm(actor, alarm, Position.ToString());
                            }
                        }

                        actor.NextStep("StartMove");
                    }
                }
                else
                {

                    if (Suspended && Part.RunFlag == false)
                    {
                        Suspended = false;
                        actor.NextStep("StartMove");
                    }
                    else if (Part.IsMotionDone() && Part.IsInPosition(Position))
                    {
                        if (Part.LinkMode)
                            Part.ResetLinkeMode();
                        actor.NextStep();
                    }
                    else if (CustomMotionCompleteCondition != null &&
                        CustomMotionCompleteCondition())
                    {
                        Part.DoStop(actor);
                        actor.NextStep();
                    }
                    else if (Timeout.Time < time)
                    {
                        Part.DoStop(actor);



                        int alarm = 0;
                        Type type = Part.GetType();
                        PropertyInfo info = type.GetProperty(AlarmPropertyName);
                        if (info != null)
                            alarm = (int)info.GetValue(Part, null);

                        if (RetryInfoMoveRetry != null)
                        {
                            if (RetryInfoMoveRetry.IncreaseCount() == false)
                            {
                                if (Part.LinkMode)
                                    Part.Device.Stop(Part.AxisNo);
                                FALibrary.Alarm.FAAlarmManager.Instance.RaiseAlarm(actor, alarm, Position.ToString());
                            }
                        }
                        else
                        {
                            if (Part.LinkMode)
                                Part.Device.Stop(Part.AxisNo);

                            if (Part.AxisNo == 10)
                            {
                                Part.DoStop(actor);
                                actor.NextStep();
                            }
                            else
                            {
                                FALibrary.Alarm.FAAlarmManager.Instance.RaiseAlarm(actor, alarm, Position.ToString());
                            }
                        }

                        actor.NextStep("StartMove");
                    }
                }

            }
        }
        /// <summary>
        /// 두 포지션을 비교하여 true, 혹은 false를 반환한다.
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <returns>targetPosition으로 이동 가능하면 true, 불가능하면 false를 리턴</returns>
        public delegate bool DelegateComparePosition(double pos1, double pos2);
        public delegate bool DynamicLimitCheckDelegate(ref string msg);
        #region Field     
        private bool _isSuspended;
        private double _oldPosition;
        private double _scale = 1;
        private double _tolerance = 10;
        private bool _motionDone = false;
        private bool _servoOn = false;
        private bool _origin = false;
        private bool _encoderZ = false;
        private bool _emergency = false;
        private bool _inPosition = false;
        private bool _servoAlarm = false;
        private bool _positiveLimit = false;
        private bool _negativeLimit = false;
        private bool _runFlag = false;
        private bool _errorFlag = false;
        private bool _homeFlag = false;
        private double _commandPos = 0;
        private double _actualPos = 0;
        private double _servoRatio = 0;



        private FAMMCPosition _targetPosition = new FAMMCPosition();
        private bool _isInitialized;
        private ushort _speedMode ;
        private ushort _homeDirection;
        private ushort _homeMode; 
        private uint _homeStartSpeed; 
        private uint _homeMoveSpeed; 
        private uint _homeAccelTime; 
        private int _homeOffset; 
        private uint _jogStartSpeed;
        private uint _jogMoveSpeed;
        private uint _jogAccelTime;



        private double _torpMax;
        private double _torpMin;
        private double _getTorpMax;
        private double _getTorpMin;



        private FAPartAction _settorquelimitparams = new FAPartAction(); //->edge-bang 2024-06-05
        private FAPartAction _gettorquelimitparams = new FAPartAction(); //->edge-bang 2024-06-05
        private FAPartAction _torquemoveto = new FAPartAction(); //->edge-bang 2024-06-05
        private FAPartAction _torqueStop = new FAPartAction(); //->edge-Lee 2024-06-05
        private FAPartAction _servoOnAction = new FAPartAction();
        private FAPartAction _servoOffAction = new FAPartAction();
        private FAPartAction _alarmResetAction = new FAPartAction();
        private FAPartAction _moveHome = new FAPartAction();
        private FAPartAction _moveToPos = new FAPartAction();
        private FAPartAction _moveVelocity = new FAPartAction();
        private FAPartAction _moveSpeedOverrideVelocity = new FAPartAction();
        private FAPartAction _stop = new FAPartAction();
        private FAAutoInterruptPartAction _moveJogPositive = new FAAutoInterruptPartAction();
        private FAAutoInterruptPartAction _moveJogNegative = new FAAutoInterruptPartAction();
        private FAAutoInterruptPartAction _MoveVelSequence = new FAAutoInterruptPartAction();


        private FAMMCDevice _device = null;

        private List<Func<bool>> _canIHomingMethods = new List<Func<bool>>();
        private List<DelegateComparePosition> _canIMoveMethods = new List<DelegateComparePosition>();
        #endregion

        #region Property
        public FAMMCDevice Device
        {
            get { return _device; }
            private set { _device = value; }
        }
        public override void SetDevice(Device.FADevice aDevice)
        {
            if (aDevice is FAMMCDevice)
                Device = aDevice as FAMMCDevice;
            else
                throw new Exception("Device Type is not correct." + " Device Name : " + Name + " Device Type : " + aDevice.GetType().ToString());
        }

        [FAAttribute("")]
        public bool isStepMot { get; set; }

        [FAAttribute("")]
        public ushort AxisNo { get; set; }
        #endregion

        #region "Alarm"
        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int HomeFail { get; set; }
        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int MovePosFail { get; set; }
        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int StopFail { get; set; }
        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int ServoAlarmFlagOn { get; set; }
        #endregion

        #region "Time"
        [FAAttribute("Time")]
        public FATime HomeTimeout { get; set; }
        [FAAttribute("Time")]
        public FATime MoveToPosTimeout { get; set; }
        [FAAttribute("Time")]
        public FATime StopTimeout { get; set; }
        #endregion








        #region "Action"





        [FAAttribute("Action")]
        public FAPartAction SetTorqueLimitParams //->edge-bang 2024-06-05
        {
            get { return _settorquelimitparams; }

        }

        [FAAttribute("Action")]
        public FAPartAction GetTorqueLimitParams //->edge-bang 2024-06-05
        {
            get { return _gettorquelimitparams; }

        }




        [FAAttribute("Action")]
        public FAPartAction TorqueMoveTo //->edge-bang 2024-06-05
        {
            get { return _torquemoveto; }
        }
        [FAAttribute("Action")]
        public FAPartAction TorqueStop //->edge-LEE 2024-06-05
        {
            get { return _torqueStop; }
        }






        [FAAttribute("Action")]
        public FAPartAction ServoOnAction
        {
            get { return _servoOnAction; }
        }

        [FAAttribute("Action")]
        public FAPartAction ServoOffAction
        {
            get { return _servoOffAction; }
        }

        [FAAttribute("Action")]
        public FAPartAction AlarmResetAction
        {
            get { return _alarmResetAction; }
        }

        [FAAttribute("Action")]        
        public FAPartAction MoveHome
        {
            get { return _moveHome; }
        }

        [FAAttribute("Action")]        
        public FAPartAction MoveToPos
        {
            get { return _moveToPos; }
        }

        [FAAttribute("Action")]      
        public FAPartAction MoveVelocity
        {
            get { return _moveVelocity; }
        }

        [FAAttribute("Action")]
        public FAPartAction MoveSpeedOverrideVelocity
        {
            get { return _moveSpeedOverrideVelocity; }
        }

        [FAAttribute("Action")]        
        public FAPartAction Stop
        {
            get { return _stop; }
        }

        [FAAttribute("Action")]
        public FAAutoInterruptPartAction MoveJogPositive
        {
            get { return _moveJogPositive; }
        }

        [FAAttribute("Action")]
        public FAAutoInterruptPartAction MoveJogNegative
        {
            get { return _moveJogNegative; }
        }

        //edge-ldg 240809
        [FAAttribute("Action")]
        public FAAutoInterruptPartAction MoveVelSequence
        {
            get { return _MoveVelSequence; }
        }






        #endregion

        #region "Status"
        [FAAttribute("Status")]
        public bool IsInitialized
        {
            get { return _isInitialized; }
            set
            {
                if (_isInitialized == value) return;

                _isInitialized = value;
                NotifyPropertyChanged("IsInitialized");
            }
        }

        [FAAttribute("Status")]
        public bool MotionDone
        {
            get { return _motionDone; }
            private set
            {
                if (_motionDone == value) return;

                _motionDone = value;
                NotifyPropertyChanged("MotionDone");
            }
        }

        [FAAttribute("Status")]
        public bool ServoOn
        {
            get { return _servoOn; }
            private set
            {
                if (_servoOn == value) return;

                _servoOn = value;
                NotifyPropertyChanged("ServoOn");
            }
        }

        [FAAttribute("Status")]
        public bool Origin
        {
            get { return _origin; }
            private set
            {
                if (_origin == value) return;

                _origin = value;
                NotifyPropertyChanged("Origin");
            }
        }

        [FAAttribute("Status")]
        public bool EncoderZ
        {
            get { return _encoderZ; }
            private set
            {
                if (_encoderZ == value) return;

                _encoderZ = value;
                NotifyPropertyChanged("EncoderZ");
            }
        }

        [FAAttribute("Status")]
        public bool Emergency
        {
            get { return _emergency; }
            private set
            {
                if (_emergency == value) return;

                _emergency = value;
                NotifyPropertyChanged("Emergency");
            }
        }

        [FAAttribute("Status")]
        public bool InPosition
        {
            get { return _inPosition; }
            private set
            {
                if (_inPosition == value) return;

                _inPosition = value;
                NotifyPropertyChanged("InPosition");
            }
        }

        [FAAttribute("Status")]
        public bool ServoAlarm
        {
            get { return _servoAlarm; }
            private set
            {
                if (_servoAlarm == value) return;

                _servoAlarm = value;
                NotifyPropertyChanged("ServoAlarm");
            }
        }

        [FAAttribute("Status")]
        public bool PositiveLimit
        {
            get { return _positiveLimit; }
            private set
            {
                if (_positiveLimit == value) return;

                _positiveLimit = value;
                NotifyPropertyChanged("PositiveLimit");
            }
        }

        [FAAttribute("Status")]
        public bool NegativeLimit
        {
            get { return _negativeLimit; }
            private set
            {
                if (_negativeLimit == value) return;

                _negativeLimit = value;
                NotifyPropertyChanged("NegativeLimit");
            }
        }

        [FAAttribute("Status")]
        public bool RunFlag
        {
            get { return _runFlag; }
            private set
            {
                if (_runFlag == value) return;

                _runFlag = value;
                NotifyPropertyChanged("RunFlag");
            }
        }

        [FAAttribute("Status")]
        public bool ErrorFlag
        {
            get { return _errorFlag; }
            private set
            {
                if (_errorFlag == value) return;

                _errorFlag = value;
                NotifyPropertyChanged("ErrorFlag");
            }
        }

        [FAAttribute("Status")]
        public bool HomeFlag
        {
            get { return _homeFlag; }
            private set
            {
                if (_homeFlag == value) return;

                _homeFlag = value;
                NotifyPropertyChanged("HomeFlag");
            }
        }



        [FAAttribute("Status")]
        public double ReadServoRatio
        {
            get { return _servoRatio; }
            private set
            {
                if (_servoRatio == value) return;

                _servoRatio = value;
                NotifyPropertyChanged("ReadServoRatio");
            }
        }







        [FAAttribute("Status")]
        public double ActualPos
        {
            get { return _actualPos; }
            private set
            {
                if (_actualPos == value) return;

                _actualPos = value;
                NotifyPropertyChanged("ActualPos");
            }
        }

        [FAAttribute("Status")]
        public double CommandPos
        {
            get { return _commandPos; }
            private set
            {
                if (_commandPos == value) return;

                _commandPos = value;
                NotifyPropertyChanged("CommandPos");
            }
        }
        #endregion        

        #region "Parameter"
        private float _speedRate = 100;
        [FAAttribute("")]
        [FAPropertyAttribute]
        public float SpeedRate
        {
            get { return _speedRate; }
            set
            {
                if (_speedRate == value) return;

                if (value > 100)
                    _speedRate = 100;
                else
                    _speedRate = value;

                NotifyPropertyChanged("SpeedRate");
            }
        }

        private bool _linkMode;
        [FAAttribute("")]
        [FAPropertyAttribute]
        public bool LinkMode
        {
            get
            {
                return _linkMode;
            }

            set
            {
                if (_linkMode == value) return;

                _linkMode = value;
                NotifyPropertyChanged("LinkMode");
            }
        }

        private ushort _slaveAxis;
        [FAAttribute("")]
        [FAPropertyAttribute]
        public ushort SlaveAxis
        {
            get
            {
                return _slaveAxis;
            }

            set
            {
                if (_slaveAxis == value) return;

                _slaveAxis = value;
                NotifyPropertyChanged("SlaveAxis");
            }
        }

        private double _linkRatio;
        [FAAttribute("")]
        [FAPropertyAttribute]
        public double LinkRatio
        {
            get
            {
                return _linkRatio;
            }

            set
            {
                if (_linkRatio == value) return;

                _linkRatio = value;
                NotifyPropertyChanged("LinkRatio");
            }
        }

        [FAAttribute("")]
        [FAPropertyAttribute]
        public double Scale
        {
            get
            {                
                return _scale;
            }

            set
            {
                if (_scale == value) return;

                _scale = value;
                NotifyPropertyChanged("Scale");
            }
        }

        [FAAttribute("")]
        [FAPropertyAttribute]
        public double Tolerance
        {
            get
            {
                return _tolerance;
            }

            set
            {
                if (_tolerance == value) return;

                _tolerance = value;
                NotifyPropertyChanged("Tolerance");
            }
        }




        [FAAttribute("GetTorqMaxParameter")]
        [FAPropertyAttribute]
        public double GetTorqMaxParamter
        {
            get { return _getTorpMax; }
            set
            {
                if (_getTorpMax == value) return;

                _getTorpMax = value;
                NotifyPropertyChanged("GetTorqMax");
            }
        }

        [FAAttribute("GetTorqMinParameter")]
        [FAPropertyAttribute]
        public double GetTorqMinParamter
        {
            get { return _getTorpMin; }
            set
            {
                if (_getTorpMin == value) return;

                _getTorpMin = value;
                NotifyPropertyChanged("GetTorqMin");
            }
        }

        [FAAttribute("TorqMaxParameter")]
        [FAPropertyAttribute]
        public double TorqMaxParamter
        {
            get { return _torpMax; }
            set
            {
                if (_torpMax == value) return;

                _torpMax = value;
                NotifyPropertyChanged("TorqMax");
            }
        }


        [FAAttribute("TorqMaxParameter")]
        [FAPropertyAttribute]
        public double TorqMinParamter
        {
            get { return _torpMin; }
            set
            {
                if (_torpMin == value) return;

                _torpMin = value;
                NotifyPropertyChanged("TorqMain");
            }
        }




















        [FAAttribute("MoveParameter")]
        public FAMMCPosition TargetPosition { get; set; }

        [FAAttribute("MoveParameter")]
        [FAPropertyAttribute]
        public ushort SpeedMode
        {
            get { return _speedMode; }
            set
            {
                if (_speedMode == value) return;

                _speedMode = value;
                NotifyPropertyChanged("SpeedMode");
            }
        }

        [FAAttribute("HomeParameter")]
        [FAPropertyAttribute]
        public ushort HomeDirection
        {
            get { return _homeDirection; }
            set
            {
                if (_homeDirection == value) return;

                _homeDirection = value;
                NotifyPropertyChanged("HomeDirection");
            }
        }

        [FAAttribute("HomeParameter")]
        [FAPropertyAttribute]
        public ushort HomeMode
        {
            get { return _homeMode; }
            set
            {
                if (_homeMode == value) return;

                _homeMode = value;
                NotifyPropertyChanged("HomeMode");
            }
        }

        [FAAttribute("HomeParameter")]
        [FAPropertyAttribute]
        public uint HomeStartSpeed
        {
            get { return _homeStartSpeed; }
            set
            {
                if (_homeStartSpeed == value) return;

                _homeStartSpeed = value;
                NotifyPropertyChanged("HomeStartSpeed");
            }
        }

        [FAAttribute("HomeParameter")]
        [FAPropertyAttribute]
        public uint HomeMoveSpeed
        {
            get { return _homeMoveSpeed; }
            set
            {
                if (_homeMoveSpeed == value) return;

                _homeMoveSpeed = value;
                NotifyPropertyChanged("HomeMoveSpeed");
            }
        }

        [FAAttribute("HomeParameter")]
        [FAPropertyAttribute]
        public uint HomeAccelTime
        {
            get { return _homeAccelTime; }
            set
            {
                if (_homeAccelTime == value) return;

                _homeAccelTime = value;
                NotifyPropertyChanged("HomeAccelTime");
            }
        }

        [FAAttribute("HomeParameter")]
        [FAPropertyAttribute]
        public int HomeOffset
        {
            get { return _homeOffset; }
            set
            {
                if (_homeOffset == value) return;

                _homeOffset = value;
                NotifyPropertyChanged("HomeOffset");
            }
        }

        [FAAttribute("JogParameter")]
        [FAPropertyAttribute]
        public uint JogStartSpeed
        {
            get { return _jogStartSpeed; }
            set
            {
                if (_jogStartSpeed == value) return;

                _jogStartSpeed = value;
                NotifyPropertyChanged("JogStartSpeed");
            }
        }

        [FAAttribute("JogParameter")]
        [FAPropertyAttribute]
        public uint JogMoveSpeed
        {
            get { return _jogMoveSpeed; }
            set
            {
                if (_jogMoveSpeed == value) return;

                _jogMoveSpeed = value;
                NotifyPropertyChanged("JogMoveSpeed");
            }
        }

        [FAAttribute("JogParameter")]
        [FAPropertyAttribute]
        public uint JogAccelTime
        {
            get { return _jogAccelTime; }
            set
            {
                if (_jogAccelTime == value) return;

                _jogAccelTime = value;
                NotifyPropertyChanged("JogAccelTime");
            }
        }

        private double _velocity;
        [FAAttribute("VelocityMoveParameter")]
        [FAPropertyAttribute]
        public double Velocity
        {
            get { return _velocity; }
            set
            {
                if (_velocity == value) return;

                _velocity = value;
                NotifyPropertyChanged("Velocity");
            }
        }

        private double _overrideVelocity;
        [FAAttribute("VelocityMoveParameter")]
        [FAPropertyAttribute]
        public double OverrideVelocity
        {
            get { return _overrideVelocity; }
            set
            {
                if (_overrideVelocity == value) return;

                _overrideVelocity = value;
                NotifyPropertyChanged("OverrideVelocity");
            }
        }
        
        public double _velocityAccelTime;
        [FAAttribute("VelocityMoveParameter")]
        [FAPropertyAttribute]
        public double VelocityAccelTime
        {
            get { return _velocityAccelTime; }
            set
            {
                if (_velocityAccelTime == value) return;

                _velocityAccelTime = value;
                NotifyPropertyChanged("VelocityAccelTime");
            }
        }
        #endregion

        /// <summary>
        /// Home 동작을 할 수 있으면 true, 할 수 없으면 false를 리턴한다.
        /// </summary>
        public Func<bool> CanIHommingDelegate { get; set; }

        /// <summary>
        /// param1은 currentPosition, param2는 targetPosition.
        /// targetPosition으로 이동가능하면 true를, 불가능하면 false를 리턴한다.
        /// </summary>
        public DelegateComparePosition CanIMoveDelegate { get; set; }

        /// <summary>
        /// 유동적으로 -Limit을 체크한다.
        /// </summary>
        public DynamicLimitCheckDelegate CheckDynamicNegativeLimitDelegate { get; set; }

        /// <summary>
        /// 유동적으로 +Limit을 체크한다.
        /// </summary>
        public DynamicLimitCheckDelegate CheckDynamicPositiveLimitDelegate { get; set; }

        public EventHandler OnCanIHommingInterlock = delegate { };
        public EventHandler<CanIMoveInterlockEventArgs> OnCanIMoveInterlock = delegate { };
        public EventHandler<LimitInterlockEventArgs> OnDynamicNegativeLimitInterlock = delegate { };
        public EventHandler<LimitInterlockEventArgs> OnDynamicPositiveLimitInterlock = delegate { };

        public void AddCanIHomingMethod(Func<bool> method)
        {
            _canIHomingMethods.Add(method);
        }

        public void AddCanIMoveMethod(DelegateComparePosition method)
        {
            _canIMoveMethods.Add(method);
        }

        public bool CanIHomming()
        {
            foreach (var item in _canIHomingMethods)
            {
                if (item() == false)
                    return false;
            }

            if (CanIHommingDelegate == null) return true;

            if (CanIHommingDelegate() == false)
            {
                OnCanIHommingInterlock(this, EventArgs.Empty);
                return false;
            }
            else
                return true;
        }

        public bool CanIMove(double targetPosition)
        {
            foreach (var item in _canIMoveMethods)
            {
                if (item(ActualPos, targetPosition) == false)
                    return false;
            }

            if (CanIMoveDelegate == null) return true;

            if (CanIMoveDelegate(ActualPos, targetPosition) == false)
            {
                OnCanIMoveInterlock(this, new CanIMoveInterlockEventArgs(targetPosition));
                return false;
            }
            else
                return true;
        }
        
        private bool CheckDynamicLimitInterlock(Direction direction, ref string msg)
        {
            if (direction == Direction.Positive)
            {
                if (CheckDynamicPositiveLimitDelegate != null)
                {
                    if (CheckDynamicPositiveLimitDelegate(ref msg))
                    {
                        OnDynamicPositiveLimitInterlock(this, new LimitInterlockEventArgs(msg));
                        return true;
                    }
                }
            }
            else if (direction == Direction.Negative)
            {
                if (CheckDynamicNegativeLimitDelegate != null)
                {
                    if (CheckDynamicNegativeLimitDelegate(ref msg))
                    {
                        OnDynamicNegativeLimitInterlock(this, new LimitInterlockEventArgs(msg));
                        return true;
                    }
                }
            }

            return false;
        }

        public FAMMCPart(FASequenceManager aSequenceManager)
        {
            CreatePositionDefine();

            MoveHome.InterlockList.Add(
                delegate(ref string msg)
                {
                    if (CanIHomming() == false)
                        return true;
                    else
                        return false;
                });

            MoveToPos.InterlockList.Add(
                delegate(ref string msg)
                {
                    if (CanIMove(TargetPosition.Position) == false)
                        return true;
                    else if (TargetPosition.Position < ActualPos)
                        return CheckDynamicLimitInterlock(Direction.Negative, ref msg);
                    else
                        return CheckDynamicLimitInterlock(Direction.Positive, ref msg);
                });

            MoveVelocity.InterlockList.Add(
                delegate(ref string msg)
                {
                    double velocity = Velocity;
                    if (velocity == 0)
                        velocity = 1;

                    if (CanIMove(ActualPos + velocity * 10) == false)
                        return true;
                    else if (Velocity < 0)
                        return CheckDynamicLimitInterlock(Direction.Negative, ref msg);
                    else
                        return CheckDynamicLimitInterlock(Direction.Positive, ref msg);
                });

            MoveJogPositive.InterlockList.Add(
                delegate(ref string msg)
                {
                    double driveSpeed = ((double)JogMoveSpeed * (double)SpeedRate) / 100.0;

                    if (CanIMove(ActualPos + driveSpeed) == false)
                        return true;
                    else if (CheckDynamicLimitInterlock(Direction.Positive, ref msg))
                        return true;
                    else
                        return false;
                });

            MoveJogNegative.InterlockList.Add(
                delegate(ref string msg)
                {
                    double driveSpeed = ((double)JogMoveSpeed * (double)SpeedRate) / 100.0;

                    if (CanIMove(ActualPos - driveSpeed) == false)
                        return true;
                    else if (CheckDynamicLimitInterlock(Direction.Negative, ref msg))
                        return true;
                    else
                        return false;
                });

            ServoOnAction.SetActionMethod(DoServoOn);
            ServoOffAction.SetActionMethod(DoServoOff);
            AlarmResetAction.SetActionMethod(DoAlarmReset);
            MoveHome.SetActionMethod(DoHome);
            MoveToPos.SetActionMethod(DoMoveToPos);


            //     //edge-lee 2024-06-04
            SetTorqueLimitParams.SetActionMethod(DoSetTorqueLimitParams);
            GetTorqueLimitParams.SetActionMethod(DoGetTorqueLimitParams);





            MoveVelocity.SetActionMethod(DoMoveVelocity);
            MoveSpeedOverrideVelocity.SetActionMethod(DoMoveSpeedOverrideVelocity);
            Stop.SetActionMethod(DoStop);


            //edge-ldg 240809
            MoveVelSequence.SetActionMethod(VelMoveSequence);
            MoveVelSequence.SetInterruptMethod(DoStop);






            MoveJogPositive.SetActionMethod(JogPositive);
            MoveJogPositive.SetInterruptMethod(DoStop);
            MoveJogNegative.SetActionMethod(JogNegative);
            MoveJogNegative.SetInterruptMethod(DoStop);

            MoveHome.CreateSequence(aSequenceManager);
            MoveToPos.CreateSequence(aSequenceManager);
            MoveVelocity.CreateSequence(aSequenceManager);
            Stop.CreateSequence(aSequenceManager);
            MoveSpeedOverrideVelocity.CreateSequence(aSequenceManager);
            MoveHome.Sequence.Steps.Add("StartMove", new StepInfo());

            MoveHome.Sequence.Steps["StartMove"].StepIndex =
                MoveHome.Sequence.AddItem(
                    (o) =>
                    {
                        if (Device.IsServoAlarm(AxisNo))
                            DoAlarmReset(o);
                    });            
            MoveHome.Sequence.AddItem(new FATime(FATimeType.second, 1));
            MoveHome.Sequence.AddItem(ServoOnAction.ExecuteForSequence);
            MoveHome.Sequence.AddItem(SetOldPosition);
            MoveHome.Sequence.AddItem(new FATime(FATimeType.millisecond, 100));
            MoveHome.Sequence.AddItem(MoveHome.ExecuteForSequence);
            MoveHome.Sequence.AddItem(new FATime(FATimeType.second, 1));
            MoveHome.Sequence.AddItem(ConfirmFirstMoveHomeDone);
            MoveHome.Sequence.AddItem(new FATime(FATimeType.second, 1));
            MoveHome.Sequence.AddItem(ConfirmMoveHomeDone);
            MoveHome.Sequence.AddItem(SetHomeMarking);
            MoveHome.Sequence.AddItem(SetInitializeOk);

            MoveToPos.Sequence.OnStart +=
                    delegate
                    {
                        _isSuspended = false;
                    };

            MoveToPos.Sequence.OnSuspended +=
                    delegate
                    {
                        _isSuspended = true;
                    };

            MoveToPos.Sequence.Steps.Add("StartMove", new StepInfo());
            MoveToPos.Sequence.Steps["StartMove"].StepIndex = MoveToPos.Sequence.AddItem(MoveToPos.ExecuteForSequence);
            MoveToPos.Sequence.AddItem(ConfirmMoveDone);
            
            MoveVelocity.Sequence.AddItem(MoveVelocity.ExecuteForSequence);

            Stop.Sequence.Steps.Add("Stop", new StepInfo());
            Stop.Sequence.AddItem(Stop.ExecuteForSequence);
            Stop.Sequence.Steps["Stop"].StepIndex = Stop.Sequence.AddItem(ConfirmStopOk);
        }

        private void SetOldPosition(object sender)
        {           
            _oldPosition = ActualPos;
        }

        private bool IsHomePositionMoved()
        {
            if (_oldPosition == 0)
                return true;
            else if (Math.Abs(_oldPosition - ActualPos) > Tolerance)
                return true;
            else
                return false;
        }

        double readRatio = 0;
        int currentTorque = 0;

        public override void Validate()
        {
            if (SimulationMode == false)
            {
                MotionDone = IsMotionDone();
                ServoOn = Device.IsServoOn(AxisNo);
                Origin = Device.IsOrigin(AxisNo);
                EncoderZ = Device.IsEncoderZ(AxisNo);
                Emergency = Device.IsEmergency(AxisNo);
                InPosition = Device.IsInposition(AxisNo);
                ServoAlarm = Device.IsServoAlarm(AxisNo);
                PositiveLimit = Device.IsPositiveLimit(AxisNo);
                NegativeLimit = Device.IsNegativeLimit(AxisNo);
                RunFlag = Device.IsRunFlag(AxisNo);
                ErrorFlag = Device.IsErrorFlag(AxisNo);
                HomeFlag = Device.IsHomeFlag(AxisNo);


                //edge-lee 240704
                if (AxisNo == 8 || AxisNo == 9 || AxisNo == 10 || AxisNo == 11 || AxisNo == 12)
                {

                    Device.StatusReadServoLoadRatio(AxisNo, ref readRatio);
                    currentTorque = Convert.ToInt32(readRatio / 2.94);
                    ReadServoRatio = currentTorque;
                }



                //jbpark_수정2
                int cmdpos = (int)(((int)Device.GetCommandPos(AxisNo)) * Scale * 100);
                int actpos = (int)(((int)Device.GetActualPos(AxisNo)) * Scale * 100);

                CommandPos = cmdpos * 0.01;
				ActualPos  = actpos * 0.01;

                //if (isStepMot)
                //    ActualPos = cmdpos * 0.01;
                //else
                //    ActualPos = actpos * 0.01;
            }            
        }

        private bool _loadedParameters = false;

        public override void LoadParameters(System.Xml.Linq.XElement xml)
        {
            base.LoadParameters(xml);

            if (xml.Element("PositionDefineList") != null)
            {
                if (_loadedParameters)
                {
                    LoadPositionDefine(xml.Element("PositionDefineList"), true);
                }
                else
                {
                    LoadPositionDefine(xml.Element("PositionDefineList"), false);
                    _loadedParameters = true;
                }
            }
        }

        public override void SaveParameters(XElement xml)
        {
            base.SaveParameters(xml);

            SavePositionDefine(xml);
        }

        [FAAttribute("Operation")]
        public void SetLinkMode(ushort slaveAxis, double ratio)
        {
            SlaveAxis = slaveAxis;
            Device.SetLinkMode(AxisNo, slaveAxis, ratio);
        }

        [FAAttribute("Operation")]
        public void ResetLinkeMode()
        {
            Device.ResetLinkMode(AxisNo);
        }

        private void DoServoOn(object sender)
        {
            if (SimulationMode)
                ServoOn = true;
            else
                Device.ServoOn(AxisNo);
        }

        private void DoServoOff(object sender)
        {
            if (SimulationMode)
                ServoOn = false;
            else
                Device.ServoOff(AxisNo);
        }

        private void DoAlarmReset(object sender)
        {
            if (SimulationMode == false)
                Device.ResetAlarm(AxisNo);
        }
        
        private void DoHome(object sender)
        {
            if (SimulationMode)
            {
                IsInitialized = true;
                CommandPos = 0;
                ActualPos = 0;
                NegativeLimit = false;
                PositiveLimit = false;
                MotionDone = true;
            }
            else
            {
                if (LinkMode)
                    ResetLinkeMode();

                double startSpeed = ((double)HomeStartSpeed * (double)SpeedRate) / 100.0;
                double driveSpeed = ((double)HomeMoveSpeed * (double)SpeedRate) / 100.0;

                IsInitialized = false;
                Device.MoveHome(AxisNo,
                    HomeDirection,
                    HomeMode,
                    (startSpeed / Scale),
                    (driveSpeed / Scale),
                    (HomeAccelTime),
                    (int)(HomeOffset / Scale));
            }
        }



        private void _SetTorqueLimitParams(FAMMCPosition positionDefine) // edge-lee 2024-06-05
        {

            if (SimulationMode == false)
            {
                Device.MotSetTorqueLimit(AxisNo, positionDefine.Position, positionDefine.DriveSpeed);
            }
        }

        private void DoGetTorqueLimitParams(object sender) // edge-lee 2024-06-05
        {


            if (SimulationMode == false)
            {
                double dbpPluseDirTorqueLimit = 0;
                double dbpMinusDirTorqueLimit = 0;
                double test = 0;

                Device.MotGetTorqueLimit(AxisNo, ref dbpPluseDirTorqueLimit, ref dbpMinusDirTorqueLimit);
                Device.StatusReadServoLoadRatio(AxisNo, ref test);

                GetTorqMaxParamter = dbpPluseDirTorqueLimit;
                GetTorqMinParamter = dbpMinusDirTorqueLimit;
                ReadServoRatio = test;

            }


        }


        private void DoSetTorqueLimitParams(object sender) // edge-lee 2024-06-05
        {

            if (SimulationMode == false)
            {
                Device.MotSetTorqueLimit(AxisNo, TorqMaxParamter, TorqMinParamter);

            }
            //if (SimulationMode == false)
            //{
            //    //double startSpeed = ((double)JogStartSpeed * (double)SpeedRate) / 100.0;
            //    //double driveSpeed = ((double)JogMoveSpeed * (double)SpeedRate) / 100.0;
            //    double startSpeed = ((double)TargetPosition.StartSpeed * (double)SpeedRate) / 100.0;
            //    double driveSpeed = ((double)TargetPosition.DriveSpeed * (double)SpeedRate) / 100.0;

            //    Device.JogPositive(AxisNo,(startSpeed / Scale),(driveSpeed / Scale),JogAccelTime);
            //    double testtorque = 0;
            //    bool test = true;
            //    while (test)
            //    {

            //        //Device.StatusReadServoLoadRatio(AxisNo, ref testtorque);
            //        Console.WriteLine(currentTorque);

            //        if (currentTorque >= (TorqMaxParamter / 2.94))
            //        {
            //            test = false;

            //        }

            //    }
            //    Device.Stop(AxisNo);
            //}


        }

        private void DoTorqueMoveTo(object sender) // edge-lee 2024-06-05
        {
       
        }
        private void MoveTorqueStop(object sender) // edge-lee 2024-06-05
        {


         
        }











        private void DoMoveToPos(object sender)
        {
            if (SimulationMode)
            {
                ServoOn = true;
                CommandPos = TargetPosition.Position;
                ActualPos = CommandPos;
                MotionDone = true;
            }
            else
            {
                if (IsMotionDone() && IsInPosition(TargetPosition))
                    return;
                else
                {
                    if (LinkMode)
                        SetLinkMode(SlaveAxis, LinkRatio);

                    double startSpeed = ((double)TargetPosition.StartSpeed * (double)SpeedRate) / 100.0;
                    double driveSpeed = ((double)TargetPosition.DriveSpeed * (double)SpeedRate) / 100.0;

                    int iResult = Device.MovePos(AxisNo,
                        (double)(TargetPosition.Position / Scale),
                        SpeedMode,
                        (startSpeed / Scale),
                        (driveSpeed / Scale),
                        (TargetPosition.AccelTime),
                        (TargetPosition.DecelTime));

                    if (iResult != 0)
                    {
                        //------------------------------------------------------------------------------------------------------
                        string szMsg = "1.Motion MovePos Error! Result=" + iResult.ToString("X");
                        //System.Diagnostics.Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + szMsg);
                        PartLogWrite(szMsg);
                        //------------------------------------------------------------------------------------------------------
                    }
                }
				
	            //if (isStepMot)
	            //    MoveIncPosition(TargetPosition);
	            //else
	            //    MovePosition(TargetPosition);
            }
        }

        private void DoMoveVelocity(object sender)
        {
            double driveSpeed = (double)Velocity * (double)SpeedRate / 100.0;

            if (SimulationMode)
            {
                CommandPos += driveSpeed;
                ActualPos += driveSpeed;
            }
            else
            {
                Device.MoveVelocity(AxisNo,
                    (driveSpeed / Scale),
                    (driveSpeed / Scale),
                    VelocityAccelTime,
                    VelocityAccelTime);
            }
        }

        private void DoMoveSpeedOverrideVelocity(object sender)
        {
            double driveSpeed = (double)OverrideVelocity * (double)SpeedRate / 100.0;

            if (SimulationMode)
            {
                CommandPos += driveSpeed;
                ActualPos += driveSpeed;
            }
            else
            {
                Device.MoveSpeedOverrideVelocity(AxisNo, (driveSpeed / Scale));
            }
        }

        public void MovePositionNoCopyTargetPosition(FAMMCPosition positionDefine)
        {
            if (SimulationMode)
            {
                ServoOn = true;
                CommandPos = positionDefine.Position;
                ActualPos = CommandPos;
                MotionDone = true;
            }
            else
            {
                if (IsMotionDone() && IsInPosition(positionDefine))
                    return;
                else
                {
                    if (LinkMode)
                        SetLinkMode(SlaveAxis, LinkRatio);

                    double startSpeed = ((double)positionDefine.StartSpeed * (double)SpeedRate) / 100.0;
                    double driveSpeed = ((double)positionDefine.DriveSpeed * (double)SpeedRate) / 100.0;

                    int iResult = Device.MovePos(AxisNo,
                        (double)(positionDefine.Position / Scale),
                        SpeedMode,
                        (startSpeed / Scale),
                        (driveSpeed / Scale),
                        (positionDefine.AccelTime),
                        (positionDefine.DecelTime));

                    if (iResult != 0)
                    {
                        //------------------------------------------------------------------------------------------------------
                        string szMsg = "2.Motion MovePos Error! Result=" + iResult.ToString("X");
                        //System.Diagnostics.Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + szMsg);
                        PartLogWrite(szMsg);
                        //------------------------------------------------------------------------------------------------------
                    }
                }
            }
        }

        private void MovePosition(FAMMCPosition positionDefine)
        {
            if (SimulationMode)
            {
                ServoOn = true;
                CommandPos = positionDefine.Position;
                ActualPos = CommandPos;
                MotionDone = true;
            }
            else
            {
                if (IsMotionDone() && IsInPosition(positionDefine))
                    return;
                else
                {
                    if (LinkMode)
                        SetLinkMode(SlaveAxis, LinkRatio);

                    positionDefine.CopyTo(TargetPosition);

                    double startSpeed = ((double)positionDefine.StartSpeed * (double)SpeedRate) / 100.0;
                    double driveSpeed = ((double)positionDefine.DriveSpeed * (double)SpeedRate) / 100.0;

                    int iResult = Device.MovePos(AxisNo,
                        (double)(positionDefine.Position / Scale),
                        SpeedMode,
                        (startSpeed / Scale),
                        (driveSpeed / Scale),
                        (positionDefine.AccelTime),
                        (positionDefine.DecelTime));

                    if (iResult != 0)
                    {
                        //------------------------------------------------------------------------------------------------------
                        string szMsg = "3.Motion MovePos Error! Result=" + iResult.ToString("X");
                        //System.Diagnostics.Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + szMsg);
                        PartLogWrite(szMsg);
                        //------------------------------------------------------------------------------------------------------
                    }
                }
            }
        }

        public void MoveIncPosition(FAMMCPosition positionDefine)
        {
            if (SimulationMode)
            {
                ServoOn = true;
                CommandPos += positionDefine.Position;
                ActualPos += positionDefine.Position;
                MotionDone = true;
            }
            else
            {
                //if (IsMotionDone() && IsInPosition(positionDefine))
                //    return;
                //else
                {
                    double startSpeed = ((double)positionDefine.StartSpeed * (double)SpeedRate) / 100.0;
                    double driveSpeed = ((double)positionDefine.DriveSpeed * (double)SpeedRate) / 100.0;

                    Device.MoveIncPos(AxisNo,
                        (int)(positionDefine.Position / Scale),
                        SpeedMode,
                        (startSpeed / Scale),
                        (driveSpeed / Scale),
                        positionDefine.AccelTime,
                        positionDefine.DecelTime);
                }
            }
        }
        
        private void DoStop(object sender)
        {
            if (SimulationMode == false)
                Device.Stop(AxisNo);
        }


        private void VelMoveSequence(object sender)
        {

            double startSpeed = ((double)JogStartSpeed * (double)SpeedRate) / 100.0;
            double driveSpeed = ((double)JogMoveSpeed * (double)SpeedRate) / 100.0;

            if (SimulationMode)
            {
                CommandPos += JogMoveSpeed;
                ActualPos = CommandPos;


            }
            else
            {
                Device.JogPositive(AxisNo, (startSpeed / Scale), (driveSpeed / Scale), JogAccelTime);


            }
        }


        private void JogPositive(object sender)
        {
            double startSpeed = ((double)JogStartSpeed * (double)SpeedRate) / 100.0;
            double driveSpeed = ((double)JogMoveSpeed * (double)SpeedRate) / 100.0;            

            if (SimulationMode)
            {
                CommandPos += JogMoveSpeed;
                ActualPos = CommandPos;
            }
            else
            {
                Device.JogPositive(AxisNo,
                    (startSpeed / Scale),
                    (driveSpeed / Scale),
                    JogAccelTime);
            }
        }

        private void JogNegative(object sender)
        {
            double startSpeed = ((double)JogStartSpeed * (double)SpeedRate) / 100.0;
            double driveSpeed = ((double)JogMoveSpeed * (double)SpeedRate) / 100.0; 

            if (SimulationMode)
            {
                CommandPos -= JogMoveSpeed;
                ActualPos = CommandPos;
            }
            else
            {
                Device.JogNegative(AxisNo,
                    (startSpeed / Scale),
                    (driveSpeed / Scale),
                    JogAccelTime);
            }
        }
        [FAAttribute("Operation")]
        public void SettingHomeMarking(object sender)
        {
            if (SimulationMode)
            {
                CommandPos = 0;
                ActualPos = 0;
            }
            else
            {
                Device.ResetActualPos(AxisNo);
                Device.ResetCommandPos(AxisNo);
            }
        }

        public void SetHomeMarking(object sender)
        {
            if (SimulationMode)
            {
                CommandPos = 0;
                ActualPos = 0;
            }
            else
            {
                Device.ResetActualPos(AxisNo);
                Device.ResetCommandPos(AxisNo);
            }
        }        

        public bool IsInPosition(FAMMCPosition position)
        {
            //if (isStepMot)
            //    return true;

            if (Math.Abs(ActualPos - position.Position) <= Tolerance &&
                Math.Abs(CommandPos - position.Position) <= Tolerance)
                return true;
            else
                return false;
        }

        public bool IsInPosition(FAMMCPosition position, double tolerance)
        {
            if (Math.Abs(ActualPos - position.Position) <= tolerance &&
                Math.Abs(CommandPos - position.Position) <= tolerance)
                return true;
            else
                return false;
        }

        public bool IsMotionDone()
        {
            if (SimulationMode)
            {
                MotionDone = true;
                return MotionDone;
            }
            else
            {
                return Device.IsMotionDone(AxisNo);
            }
        }

        public bool IsRunFlag()
        {
            if (SimulationMode)
            {
                return RunFlag;
            }
            else
            {
                return Device.IsRunFlag(AxisNo);
            }
        }

        protected void ConfirmMoveDone(FASequence actor, TimeSpan time)
        {
            if (SimulationMode)
            {
                if (MotionDone == true)
                    actor.NextStep();
            }
            else
            {
                if (IsMotionDone() == true &&
                    IsInPosition(TargetPosition))
                {
                    if (LinkMode)
                        ResetLinkeMode();
                    actor.NextStep();
                }
                else if (_isSuspended && RunFlag == false)
                {
                    _isSuspended = false;
                    actor.NextStep("StartMove");
                }
                else if (MoveToPosTimeout.Time < time)
                {
                    Device.Stop(AxisNo);

                    if (LinkMode)
                        Device.Stop(AxisNo);

                    Alarm.FAAlarmManager.Instance.RaiseAlarm(actor, MovePosFail,
                        "MOVE FAIL POSNAME : " + TargetPosition.Name +
                        " POS : " + TargetPosition.Position +
                        " Part " + Name + '\n');
                    actor.NextStep("StartMove");
                }
            }
        }

        protected void ConfirmFirstMoveHomeDone(FASequence actor, TimeSpan time)
        {
            if (SimulationMode)
            {
                MotionDone = true;
                actor.NextStep();
            }
            else
            {
                if (IsMotionDone() == true &&
//                    IsHomePositionMoved() == true &&
                    Device.IsHomeOk(AxisNo))
                {
                    actor.NextStep();
                }
                else if (RunFlag == false && HomeFlag == false && ErrorFlag == true && NegativeLimit == true)
                {
                    MoveHome.ExecuteForSequence(actor, time);
                    actor.NextStep();
                }
                else if (HomeTimeout.Time < time)
                {
                    Device.Stop(AxisNo);
                    Alarm.FAAlarmManager.Instance.RaiseAlarm(actor, HomeFail,
                        "HOME FAIL Part " + Name + '\n');
                    actor.NextStep("StartMove");
                }
            }
        }

        protected void ConfirmMoveHomeDone(FASequence actor, TimeSpan time)
        {
            if (SimulationMode)
            {
                MotionDone = true;
                actor.NextStep();
            }
            else
            {
                if (IsMotionDone() == true && Device.IsHomeOk(AxisNo))
                    actor.NextStep();
                else if (HomeTimeout.Time < time)
                {
                    Device.Stop(AxisNo);
                    Alarm.FAAlarmManager.Instance.RaiseAlarm(actor, HomeFail,
                        "HOME FAIL Part " + Name + '\n');
                    actor.NextStep("StartMove");
                }
            }
        }

        protected void ConfirmStopOk(FASequence actor, TimeSpan time)
        {
            if (SimulationMode)
            {
                actor.NextStep();
                MotionDone = true;
            }
            else
            {
                if (IsMotionDone() == true)
                    actor.NextStep();
                else if (StopTimeout.Time < time)
                {
                    Alarm.FAAlarmManager.Instance.RaiseAlarm(actor, StopFail,
                        "STOP FAIL Part " + Name + '\n');
                    actor.NextStep("Stop");
                }
            }
        }        

        private void LoadPositionDefine(XElement xml, bool reload)
        {
            foreach (XElement item in xml.Elements())
            {
                double position = 0;
                uint startSpeed = 0;
                uint driveSpeed = 0;
                uint accelTime = 0;
                uint decelTime = 0;

                // ★ 추가: Limit 값
                double lwLimit = -999.9;
                double upLimit = 999.9;

                string name = item.Element("Name").Value.ToString();

                if (item.Element("Position") != null)
                    if (double.TryParse(item.Element("Position").Value.Trim(), out position) == false)
                        throw new Exception("Position Value is not double.\n" +
                            "Part Name : " + Name + ", " +
                            "Position : " + item.Element("Position").Value);

                if (item.Element("StartSpeed") != null)
                    if (uint.TryParse(item.Element("StartSpeed").Value.Trim(), out startSpeed) == false)
                        throw new Exception("StartSpeed Value is not Unsigned Integer.\n" +
                            "Part Name : " + Name + ", " +
                            "StartSpeed : " + item.Element("StartSpeed").Value);

                if (item.Element("DriveSpeed") != null)
                    if (uint.TryParse(item.Element("DriveSpeed").Value.Trim(), out driveSpeed) == false)
                        throw new Exception("DriveSpeed Value is not Unsigned Integer.\n" +
                            "Part Name : " + Name + ", " +
                            "DriveSpeed : " + item.Element("DriveSpeed").Value);

                if (item.Element("AccelTime") != null)
                    if (uint.TryParse(item.Element("AccelTime").Value.Trim(), out accelTime) == false)
                        throw new Exception("AccelTime Value is not Unsigned Integer.\n" +
                            "Part Name : " + Name + ", " +
                            "AccelTime : " + item.Element("AccelTime").Value);

                if (item.Element("DecelTime") != null)
                    if (uint.TryParse(item.Element("DecelTime").Value.Trim(), out decelTime) == false)
                        throw new Exception("DecelTime Value is not Unsigned Integer.\n" +
                            "Part Name : " + Name + ", " +
                            "DecelTime : " + item.Element("DecelTime").Value);

                // ★ 추가: LwLimit / UpLimit (있으면만 사용, 없으면 0 유지)
                if (item.Element("LwLimit") != null)
                    double.TryParse(item.Element("LwLimit").Value.Trim(), out lwLimit);

                if (item.Element("UpLimit") != null)
                    double.TryParse(item.Element("UpLimit").Value.Trim(), out upLimit);

                if (reload)
                {
                    var positionDefine = FAReflection.GetPropertyValue(this, name) as FAMMCPosition;
                    positionDefine.Name = name;
                    positionDefine.Position = position;
                    positionDefine.StartSpeed = startSpeed;
                    positionDefine.DriveSpeed = driveSpeed;
                    positionDefine.AccelTime = accelTime;
                    positionDefine.DecelTime = decelTime;

                    // ★ 추가
                    positionDefine.LwLimit = lwLimit;
                    positionDefine.UpLimit = upLimit;
                }
                else
                {
                    FAMMCPosition positionDefine = new FAMMCPosition();
                    positionDefine.Name = name;
                    positionDefine.Position = position;
                    positionDefine.StartSpeed = startSpeed;
                    positionDefine.DriveSpeed = driveSpeed;
                    positionDefine.AccelTime = accelTime;
                    positionDefine.DecelTime = decelTime;

                    // ★ 추가
                    positionDefine.LwLimit = lwLimit;
                    positionDefine.UpLimit = upLimit;

                    FAReflection.SetPropertyValue(this, name, positionDefine);
                }
            }
        }

        private void SavePositionDefine(XElement xml)
        {
            if (xml == null) return;
            if (xml.Element("PositionDefineList") == null)
                xml.Add(new XElement("PositionDefineList"));

            XElement positionDefineList = xml.Element("PositionDefineList");

            PropertyInfo[] propList;
            propList = this.GetType().GetProperties();

            foreach (PropertyInfo info in propList)
            {
                if (info.PropertyType == typeof(FAMMCPosition))
                {
                    string name = info.Name;
                    FAMMCPosition value = (FAMMCPosition)info.GetValue(this, null);
                    if (value == null) continue;

                    XElement item = FAUtility.GetElement(positionDefineList, "Name", name);
                    if (item == null)
                    {
                        item = new XElement("Item");
                        positionDefineList.Add(item);
                    }

                    if (item.Element("Name") == null)
                        item.Add(new XElement("Name", name));
                    else
                        item.Element("Name").SetValue(name);

                    if (item.Element("Position") != null)
                        item.Element("Position").SetValue(value.Position);
                    else
                        item.Add(new XElement("Position", value.Position));

                    if (item.Element("StartSpeed") != null)
                        item.Element("StartSpeed").SetValue(value.StartSpeed);
                    else
                        item.Add(new XElement("StartSpeed", value.StartSpeed));

                    if (item.Element("DriveSpeed") != null)
                        item.Element("DriveSpeed").SetValue(value.DriveSpeed);
                    else
                        item.Add(new XElement("DriveSpeed", value.DriveSpeed));

                    if (item.Element("AccelTime") != null)
                        item.Element("AccelTime").SetValue(value.AccelTime);
                    else
                        item.Add(new XElement("AccelTime", value.AccelTime));

                    if (item.Element("DecelTime") != null)
                        item.Element("DecelTime").SetValue(value.DecelTime);
                    else
                        item.Add(new XElement("DecelTime", value.DecelTime));

                    // ★ 추가: LwLimit / UpLimit
                    if (item.Element("LwLimit") != null)
                        item.Element("LwLimit").SetValue(value.LwLimit);
                    else
                        item.Add(new XElement("LwLimit", value.LwLimit));

                    if (item.Element("UpLimit") != null)
                        item.Element("UpLimit").SetValue(value.UpLimit);
                    else
                        item.Add(new XElement("UpLimit", value.UpLimit));
                }
            }         
        }

        private void CreatePositionDefine()
        {
            PropertyInfo[] propList;
            propList = this.GetType().GetProperties();

            foreach (PropertyInfo info in propList)
            {
                if (info.PropertyType == typeof(FAMMCPosition))
                {
                    FAMMCPosition pos = new FAMMCPosition();
                    info.SetValue(this, pos, null);
                }
            }
        }

        private void SetInitializeOk(object sender)
        {
            IsInitialized = true;
        }

        public void MoveMultiAxis(Dictionary<ushort, FAMMCPosition> positions)
        {
            int len = positions.Count;
            FAMMCDevice.FAMultiMoveInfo[] info = new FAMMCDevice.FAMultiMoveInfo[len];

            int i = 0;
            foreach (var item in positions)
            {                
                double driveSpeed = ((double)item.Value.DriveSpeed * (double)SpeedRate) / 100.0;

                info[i] = new FAMMCDevice.FAMultiMoveInfo();
                info[i].AxisNo = item.Key;
                info[i].Positions = item.Value.Position / Scale;
                info[i].MaxVelocity = driveSpeed / Scale;
                info[i].MaxAccel = item.Value.AccelTime;
                info[i].MaxDecel = item.Value.DecelTime;
                i++;
            }

            Device.MoveMultiPos(info);
        }

        public static void PartLogWrite(string message)
        {
            string dir;
            string path;
            string wMessage;

            DateTime dt = DateTime.Now;
            StreamWriter sw;

            dir = string.Format(@"{0}\PART_LOG\{1:0000}\{2:00}\{3:00}\", ".\\LOG", dt.Year, dt.Month, dt.Day); //DefaultInfo.PATH

            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }

            path = string.Format(@"{0}{1}_{2:0000}{3:00}{4:00}{5:00}.log",
                                dir, "PART_LOG", dt.Year, dt.Month, dt.Day, dt.Hour);

            sw = new StreamWriter(path, true, System.Text.Encoding.UTF8);

            wMessage = string.Format("{0:0000}/{1:00}/{2:00} {3:00}:{4:00}:{5:00}.{6:000}\t{7}\r\n",
                                        dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond, message);

            try
            {
                sw.Write(wMessage);
                sw.Flush();
            }
            /*
                        catch (Exception e)
                        {
                            //MessageBox.Show(e.StackTrace);
                        }
            */
            finally
            {
                sw.Close();
            }
        }

    }
}
