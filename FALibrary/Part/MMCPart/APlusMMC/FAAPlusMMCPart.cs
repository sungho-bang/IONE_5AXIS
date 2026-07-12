using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Device.MMCDevice.APlusMMC;
using FALibrary.Sequence;
using FALibrary.Utility;
using System.Xml.Linq;
using System.Reflection;
using System.Xml;
using System.Diagnostics;

namespace FALibrary.Part.MMCPart.APlusMMC
{   
    public class FAAPlusMMCPart : FAPart
    {
        protected class PositionUtility
        {
            public FAAPlusMMCPart Part { get; set; }
            public FAAPlusMMCPosition Position { get; set; }
            public FAPartAction PartAction { get; set; }
            public FASequence Sequence { get; set; }
            public FATime Timeout { get; set; }
            public string AlarmPropertyName { get; set; }

            public void Initialize()
            {
                PartAction.SetActionMethod(DoMove);

                Sequence.Steps.Add("StartMove", new StepInfo());
                Sequence.Steps["StartMove"].StepIndex = Sequence.AddItem(PartAction.ExecuteForSequence);
                Sequence.AddItem(ConfirmMovingCompleted);
            }

            private void DoMove(object sender)
            {
                Part.MovePosition(Position);
            }

            private void ConfirmMovingCompleted(FASequence actor, TimeSpan time)
            {
                if (Part.IsMotionDone() && Part.IsInPosition(Position))
                    actor.NextStep();
                else if (Timeout.Time < time)
                {
                    Part.DoStop(actor);

                    int alarm = 0;
                    Type type = Part.GetType();
                    PropertyInfo info = type.GetProperty(AlarmPropertyName);
                    if (info != null)                    
                        alarm = (int)info.GetValue(Part, null);                    

                    FALibrary.Alarm.FAAlarmManager.Instance.RaiseAlarm(actor, alarm, Position.ToString());
                    actor.NextStep("StartMove");
                }
            }
        }

        #region Field
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

        private FAAPlusMMCPosition _targetPosition = new FAAPlusMMCPosition();
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



        private FAPartAction _moveHome = new FAPartAction();
        private FAPartAction _moveToPos = new FAPartAction();
        private FAPartAction _moveVelocity = new FAPartAction();
        private FAPartAction _stop = new FAPartAction();

        private FAAPlusMMCDevice _device = null;
        #endregion

        #region Property
        public FAAPlusMMCDevice Device
        {
            get { return _device; }
            private set { _device = value; }
        }
        public override void SetDevice(Device.FADevice aDevice)
        {
            if (aDevice is FAAPlusMMCDevice)
                Device = aDevice as FAAPlusMMCDevice;
            else
                throw new Exception("Device Type is not correct." + " Device Name : " + Name + " Device Type : " + aDevice.GetType().ToString());
        }

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
        public FAPartAction Stop
        {
            get { return _stop; }
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





        [FAAttribute("MoveParameter")]
        public FAAPlusMMCPosition TargetPosition { get; set; }



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
        #endregion

        public FAAPlusMMCPart(FASequenceManager aSequenceManager)
        {
            CreatePositionDefine();

            MoveHome.SetActionMethod(DoHome);
            MoveToPos.SetActionMethod(DoMoveToPos);
            MoveVelocity.SetActionMethod(DoMoveVelocity);
            Stop.SetActionMethod(DoStop);

            MoveHome.CreateSequence(aSequenceManager);
            MoveToPos.CreateSequence(aSequenceManager);
            MoveVelocity.CreateSequence(aSequenceManager);
            Stop.CreateSequence(aSequenceManager);

            MoveHome.Sequence.Steps.Add("StartMove", new StepInfo());

            MoveHome.Sequence.Steps["StartMove"].StepIndex = MoveHome.Sequence.AddItem(MoveHome.ExecuteForSequence);
            MoveHome.Sequence.AddItem(ConfirmFirstMoveHomeDone);
            MoveHome.Sequence.AddItem(new FATime(FATimeType.second, 2));
            MoveHome.Sequence.AddItem(ConfirmMoveHomeDone);
            MoveHome.Sequence.AddItem(SetHomeMarking);
            MoveHome.Sequence.AddItem(SetInitializeOk);

            MoveToPos.Sequence.Steps.Add("StartMove", new StepInfo());
            MoveToPos.Sequence.Steps["StartMove"].StepIndex = MoveToPos.Sequence.AddItem(MoveToPos.ExecuteForSequence);
            MoveToPos.Sequence.AddItem(ConfirmMoveDone);
            
            MoveVelocity.Sequence.AddItem(MoveVelocity.ExecuteForSequence);

            Stop.Sequence.Steps.Add("Stop", new StepInfo());
            Stop.Sequence.AddItem(Stop.ExecuteForSequence);
            Stop.Sequence.Steps["Stop"].StepIndex = Stop.Sequence.AddItem(ConfirmStopOk);
        }

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

//jbpark_수정2
                int cmdpos = (int)(((int)Device.GetCommandPos(AxisNo)) * Scale * 100);
                int actpos = (int)(((int)Device.GetActualPos(AxisNo)) * Scale * 100);
                CommandPos = cmdpos * 0.01;
                ActualPos = actpos * 0.01;
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
        public void DoServoOn(object sender)
        {
            if (SimulationMode)
                ServoOn = true;
            else
                Device.ServoOn(AxisNo);
        }

        [FAAttribute("Operation")]
        public void DoServoOff(object sender)
        {
            if (SimulationMode)
                ServoOn = false;
            else
                Device.ServoOff(AxisNo);
        }

        [FAAttribute("Operation")]
        public void DoAlarmReset(object sender)
        {
            if (SimulationMode == false)
                Device.ResetAlarm(AxisNo);
        }

        [FAAttribute("Operation")]
        public void DoHome(object sender)
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
                IsInitialized = false;
                Device.MoveHome(AxisNo,
                    HomeDirection,
                    HomeMode,
                    (uint)(HomeStartSpeed / Scale),
                    (uint)(HomeMoveSpeed / Scale),
                    HomeAccelTime,
                    HomeOffset);
            }
        }

        [FAAttribute("Operation")]
        public void DoMoveToPos(object sender)
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
                if (MotionDone && IsInPosition(TargetPosition))
                    return;
                else
                {
                    Device.MovePos(AxisNo,
                        (double)(TargetPosition.Position / Scale),
                        SpeedMode,
                        (uint)(TargetPosition.StartSpeed / Scale),
                        (uint)(TargetPosition.DriveSpeed / Scale),
                        TargetPosition.AccelTime,
                        TargetPosition.DeaccelTime);
                }
            }
        }

        public void DoMoveVelocity(object sender)
        {
        }

        public void MovePosition(FAAPlusMMCPosition positionDefine)
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
                if (MotionDone && IsInPosition(positionDefine))
                    return;
                else
                {
                    positionDefine.CopyTo(TargetPosition);

                    Device.MovePos(AxisNo,
                        (double)(positionDefine.Position / Scale),
                        SpeedMode,
                        (uint)(positionDefine.StartSpeed / Scale),
                        (uint)(positionDefine.DriveSpeed / Scale),
                        positionDefine.AccelTime,
                        positionDefine.DeaccelTime);
                }
            }
        }

        public void MoveIncPosition(FAAPlusMMCPosition positionDefine)
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
                if (MotionDone && IsInPosition(positionDefine))
                    return;
                else
                {
                    Device.MoveIncPos(AxisNo,
                        (int)(positionDefine.Position / Scale),
                        SpeedMode,
                        (uint)(positionDefine.StartSpeed / Scale),
                        (uint)(positionDefine.DriveSpeed / Scale),
                        positionDefine.AccelTime,
                        positionDefine.DeaccelTime);
                }
            }
        }

        [FAAttribute("Operation")]
        public void DoStop(object sender)
        {
            if (SimulationMode == false)
                Device.Stop(AxisNo);
        }

        [FAAttributePushButtonMethod("Operation", "DoStop")]
        public void JogPositive(object sender)
        {
            if (SimulationMode)
            {
                CommandPos += JogMoveSpeed;
                ActualPos = CommandPos;
            }
            else
            {
                Device.JogPositive(AxisNo,
                    (uint)(JogStartSpeed / Scale),
                    (uint)(JogMoveSpeed / Scale),
                    JogAccelTime);
            }
        }

        [FAAttributePushButtonMethod("Operation", "DoStop")]
        public void JogNegative(object sender)
        {
            if (SimulationMode)
            {
                CommandPos -= JogMoveSpeed;
                ActualPos = CommandPos;
            }
            else
            {
                Device.JogNegative(AxisNo,
                    (uint)(JogStartSpeed / Scale),
                    (uint)(JogMoveSpeed / Scale),
                    JogAccelTime);
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

        public bool IsInPosition(FAAPlusMMCPosition position)
        {
            if (Math.Abs(ActualPos - position.Position) <= Tolerance &&
                Math.Abs(CommandPos - position.Position) <= Tolerance)
                return true;
            else
                return false;
        }

        public bool IsInPosition(FAAPlusMMCPosition position, double tolerance)
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
                return MotionDone;
            }
            else
            {
                return Device.IsMotionDone(AxisNo);
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
                    actor.NextStep();
                else if (MoveToPosTimeout.Time < time)
                {
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
                if (IsMotionDone() == true)
                    actor.NextStep();
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
                if (IsMotionDone() == true)
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
                uint deaccelTime = 0;

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

                if (item.Element("DeaccelTime") != null)
                    if (uint.TryParse(item.Element("DeaccelTime").Value.Trim(), out deaccelTime) == false)
                        throw new Exception("DeaccelTime Value is not Unsigned Integer.\n" +
                            "Part Name : " + Name + ", " +
                            "DeaccelTime : " + item.Element("DeaccelTime").Value);

                if (reload)
                {
                    var positionDefine = FAReflection.GetPropertyValue(this, name) as FAAPlusMMCPosition;
                    positionDefine.Name = name;
                    positionDefine.Position = position;
                    positionDefine.StartSpeed = startSpeed;
                    positionDefine.DriveSpeed = driveSpeed;
                    positionDefine.AccelTime = accelTime;
                    positionDefine.DeaccelTime = deaccelTime;
                }
                else
                {
                    FAAPlusMMCPosition positionDefine = new FAAPlusMMCPosition();
                    positionDefine.Name = name;
                    positionDefine.Position = position;
                    positionDefine.StartSpeed = startSpeed;
                    positionDefine.DriveSpeed = driveSpeed;
                    positionDefine.AccelTime = accelTime;
                    positionDefine.DeaccelTime = deaccelTime;
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
                if (info.PropertyType == typeof(FAAPlusMMCPosition))
                {
                    string name = info.Name;
                    FAAPlusMMCPosition value = (FAAPlusMMCPosition)info.GetValue(this, null);
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

                    if (item.Element("DeaccelTime") != null)
                        item.Element("DeaccelTime").SetValue(value.DeaccelTime);
                    else
                        item.Add(new XElement("DeaccelTime", value.DeaccelTime));    
                }
            }         
        }

        private void CreatePositionDefine()
        {
            PropertyInfo[] propList;
            propList = this.GetType().GetProperties();

            foreach (PropertyInfo info in propList)
            {
                if (info.PropertyType == typeof(FAAPlusMMCPosition))
                {
                    FAAPlusMMCPosition pos = new FAAPlusMMCPosition();
                    info.SetValue(this, pos, null);
                }
            }
        }

        private void SetInitializeOk(object sender)
        {
            IsInitialized = true;
        }
    }
}
