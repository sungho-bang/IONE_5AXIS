using System;
using System.Collections.Generic;
using FAFramework.Utility;
using FALibrary.Alarm;
using FALibrary.Sequence;
using FALibrary;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Linq;
using System.Diagnostics;

namespace FAFramework.Module
{
    public abstract class FAModule : FALibrary.FAMachine
    {
        private List<PreStartInterlock> _preInterlockList = new List<PreStartInterlock>();
        private List<FASequence> _sequenceList = new List<FASequence>();

        private bool _parametersLoaded = false;

        public delegate bool PreStartInterlock(out int alarm);

        [FAAttribute("Status")]
        public FAProductInfo ProductInfo { get; set; }

        public Equipment.EquipmentBase Equipment { get; set; }
        public FASequenceManager SequenceManager { get; set; }

        private Dictionary<object, QueryMessageResultState> _questionResultMap = new Dictionary<object, QueryMessageResultState>();

        public FAModule()
        {
            ProductInfo = new FAProductInfo();

            Manager.MachineManager.Instance.AddModule(this);

            PropertyChanged +=
                delegate (object sender, PropertyChangedEventArgs e)
                {
                    if (_parametersLoaded == false) return;

                    var property = this.GetType().GetProperty(e.PropertyName);
                    if (property == null) return;

                    Attribute[] attr = Attribute.GetCustomAttributes(property);
                    bool autoSave = false;
                    bool saveLog = false;

                    foreach (Attribute at in attr)
                    {
                        if (at is FAAttribute)
                        {
                            if ((at as FAAttribute).GroupName == "Option")
                                saveLog = true;

                            if (at is EnablePropertyChangeLog)
                                saveLog = true;

                            if (at is ExtensionSaveProperty)
                            {
                                var esp = at as ExtensionSaveProperty;
                                if (esp.UseAutoSave)
                                    autoSave = true;

                                if (esp.UseWriteChangeLog)
                                    saveLog = true;
                            }
                        }
                    }

                    if (saveLog == false) return;

                    object obj = this.GetType().GetProperty(e.PropertyName).GetValue(this, null);

                    if (obj is bool)
                    {
                        if ((bool)obj == true)
                            WriteTraceLog(string.Format("ENABLE {0} of {1}", e.PropertyName, this.FullName));
                        else
                            WriteTraceLog(string.Format("DISABLE {0} of {1}", e.PropertyName, this.FullName));
                    }
                    else
                    {
                        Manager.LogManager.Instance.WriteTraceLog(Equipment,
                            string.Format("CHANGED {0} : {1} of {2}", e.PropertyName, obj.ToString(), this.FullName));
                    }

                    if (autoSave)
                        Equipment.Save();
                };
        }

        public override void LoadParameters(System.Xml.Linq.XElement xml)
        {
            base.LoadParameters(xml);
            _parametersLoaded = true;
            SetAlarmID();
        }

        public void WriteDebugLog(string msg)
        {
            Manager.LogManager.Instance.WriteDebugLog(Equipment, string.Format("[{0}]\t{1}", this.Name, msg));
        }

        public void WriteTraceLog(string msg)
        {
            if (string.IsNullOrEmpty(ProductInfo.UniqueID))
                Manager.LogManager.Instance.WriteTraceLog(Equipment, $"[{this.Name}]\t{msg}");
            else
                Manager.LogManager.Instance.WriteTraceLog(Equipment, $"[{this.Name}]\t{ProductInfo.UniqueID}\t{msg}");
        }

        public void RaiseAlarm(object sender, int alarmID, string message, object tag)
        {
            FAAlarmManager.Instance.RaiseAlarm(sender, alarmID, $"[{this.Name}]\t{message}", tag);
        }

        public void RaiseAlarm(object sender, int alarmID)
        {
            FAAlarmManager.Instance.RaiseAlarm(sender, alarmID, this.Name);
        }

        public void RaiseAlarm(object sender, int alarmID, params string[] message)
        {
            RaiseAlarm(sender, ", ", alarmID, message);
        }

        public void RaiseAlarm(object sender, string seperator, int alarmID, params string[] message)
        {
            string msg = string.Empty;
            try
            {
                msg = string.Join(seperator, message);
            }
            catch (Exception e)
            {
                WriteDebugLog(e.ToString());
            }

            FAAlarmManager.Instance.RaiseAlarm(sender, alarmID, msg);
        }

        public void ShowMessage(string name, int alarmNo, string defaultAlarmName)
        {
            string windowName;
            var alarm = Utility.AlarmUtility.GetAlarm(alarmNo, defaultAlarmName);
            Manager.MessageWindowManager.Instance.Show(Equipment, name, out windowName, alarm, string.Empty);
            Manager.MessageWindowNameManager.Instance.SetWindowName(this, name, windowName);
        }

        public void ShowMessage(string name, int alarmNo, string defaultAlarmName, string moreMessage)
        {
            string windowName;
            var alarm = Utility.AlarmUtility.GetAlarm(alarmNo, defaultAlarmName);
            Manager.MessageWindowManager.Instance.Show(Equipment, name, out windowName, alarm, moreMessage);
            Manager.MessageWindowNameManager.Instance.SetWindowName(this, name, windowName);
        }

        public void ShowMessage(string name, int alarmNo,
            string defaultAlarmName, string moreMessage, Utility.Alarm.AlarmMoreInfo tag)
        {
            string windowName;
            var alarm = Utility.AlarmUtility.GetAlarm(alarmNo, defaultAlarmName);
            if (tag != null) 
            {
                var imagePath = System.IO.Path.Combine(
                                    ConfigClasses.GlobalConst.ROOT_PATH,
                                    "Image",
                                    tag.AutoImageName + ".png");
                alarm.ImagePath = tag.AutoImageName;
            }

            Manager.MessageWindowManager.Instance.Show(Equipment, name, out windowName, alarm, moreMessage);
            Manager.MessageWindowNameManager.Instance.SetWindowName(this, name, windowName);
        }

        public void CloseMessage(string name, bool searchWindowFromMessageWindowNameManager = true)
        {
            string windowName = name;

            if (searchWindowFromMessageWindowNameManager)
                windowName = Manager.MessageWindowNameManager.Instance.GetWindowName(this, name);

            Manager.MessageWindowManager.Instance.CloseWindow(windowName);
        }

        public bool IsMessageClosed(string name, bool searchWindowFromMessageWindowNameManager = true)
        {
            string windowName = name;

            if (searchWindowFromMessageWindowNameManager)
                windowName = Manager.MessageWindowNameManager.Instance.GetWindowName(this, name);

            return Manager.MessageWindowManager.Instance.IsClosed(windowName);
        }

        public FAFramework.GUI.QuestionMessageBoxWindow.QuestionResult ShowQueryMessage(object owner, int alarmNo, string defaultAlarmName, bool cancelAble, bool useSound, string addingMessage)
        {
            var alarm = Utility.AlarmUtility.GetAlarm(alarmNo, defaultAlarmName);

            if (owner is FASequence)
            {
                if (_questionResultMap.ContainsKey(owner) == false)
                    _questionResultMap.Add(owner, new QueryMessageResultState());

                if (!_questionResultMap[owner].Showed)
                {
                    var action = new Func<bool>(
                        () =>
                        {
                            var queryResult = Manager.QueryMessageBoxManager.Instance.Show(owner,
                                        alarm.AlarmName + "\n" + addingMessage,
                                        Equipment,
                                        cancelAble,
                                        useSound);
                            if (queryResult != GUI.QuestionMessageBoxWindow.QuestionResult.None)
                            {
                                if (_questionResultMap[owner] != null)
                                    _questionResultMap[owner].Result = queryResult;
                                return true;
                            }
                            else
                                return false;
                        });
                    _questionResultMap[owner].Action = action;
                    _questionResultMap[owner].Show();
                }

                var result = _questionResultMap[owner].Result;
                if (_questionResultMap[owner].Result != GUI.QuestionMessageBoxWindow.QuestionResult.None)
                    _questionResultMap[owner].Clear();

                return result;
            }
            else
            {
                return Manager.QueryMessageBoxManager.Instance.Show(owner,
                        alarm.AlarmName + "\n" + addingMessage,
                        Equipment,
                        cancelAble,
                        useSound);
            }
        }

        public void RegisterSequence()
        {
            PropertyInfo[] propList = this.GetType().GetProperties();
            foreach (PropertyInfo info in propList)
            {
                if (info.PropertyType != typeof(FASequence)) continue;

                FASequence sequence = new FASequence(SequenceManager);
                _sequenceList.Add(sequence);

                sequence.Name = info.Name;
                info.SetValue(this, sequence, null);

                sequence.OnStart +=
                    delegate
                    {
                        foreach (var key in _questionResultMap.Keys.ToArray())
                        {
                            _questionResultMap[key].Clear();
                        }
                    };

                sequence.OnChangeStep +=
                    delegate
                    {
                        foreach (var key in _questionResultMap.Keys.ToArray())
                        {
                            _questionResultMap[key].Clear();
                        }
                    };
            }
        }

        public abstract void InitializeSequence();

        public virtual void ClearProductInfo()
        {
            ProductInfo.Clear();
        }

        public virtual void SetInterlock()
        {
        }

        public void ClearRetryInfo()
        {
            foreach (KeyValuePair<string, FARetryInfo> retryInfo in RetryInfoList)
                retryInfo.Value.ClearCount();
        }

        public bool IsInterlockPreStart(out int alarm)
        {
            alarm = 0;
            foreach (var method in _preInterlockList)
            {
                if (method == null) continue;
                try
                {
                    if (method(out alarm) == true)
                        return true;
                }
                catch
                {
                }
            }

            return false;
        }

        public IEnumerable<FASequence> GetAllSequences()
        {
            return _sequenceList;
        }

        protected void AddPreStartInterlock(PreStartInterlock method)
        {
            _preInterlockList.Add(method);
        }

        private void WriteInterlockResource(string filepath,
            string defaultMessage)
        {
            var defaultMessageXml = new System.Xml.Linq.XElement("DefaultMessage");
            defaultMessageXml.Add(defaultMessage);

            var message = new System.Xml.Linq.XElement("Message");
            message.Add(defaultMessage);

            var imagePath = new System.Xml.Linq.XElement("ImagePath");

            var xml = new System.Xml.Linq.XElement("Interlock");
            xml.Add(defaultMessageXml);
            xml.Add(message);
            xml.Add(imagePath);
            xml.Save(filepath);
        }

        /// <summary>
        /// PartAction에 인터락을 추가한다.
        /// </summary>
        /// <param name="partAction">인터락을 추가할 PartAction</param>
        /// <param name="interlockOn">인터락 조건 함수. 반환값이 true이면 인터락 경고가 발생한다.</param>
        /// <param name="defaultMessage">인터락 리소스를 찾지 못하거나 예외가 발생한 경우 보여줄 기본 메시지</param>
        protected void AddPartInterlock(FALibrary.Part.FAPartAction partAction,
            Func<bool> interlockOn,
            string defaultMessage)
        {
            if (string.IsNullOrEmpty(partAction.Part.Name))
                return;

            if (!partAction.ContainsMetaProperty("InterlockNo"))
                partAction.SetMetaPropertyValue("InterlockNo", 0);
            int interlockNo = (int)partAction.GetMetaPropertyValue("InterlockNo") + 1;
            partAction.SetMetaPropertyValue("InterlockNo", interlockNo);

            var dir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "InterlockResource",
                this.Name,
                partAction.Part.Name);
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            var filepath = System.IO.Path.Combine(dir, $"{partAction.ActionName}_{interlockNo}.xml");

            if (System.IO.File.Exists(filepath))
            {
                var xml = System.Xml.Linq.XElement.Load(filepath);
                if (xml.Element("DefaultMessage") != null &&
                    xml.Element("DefaultMessage").Value != defaultMessage)
                {
                    WriteInterlockResource(filepath, defaultMessage);
                }
            }
            else
                WriteInterlockResource(filepath, defaultMessage);

            partAction.InterlockList.Add(
                delegate (ref string msg)
                {
                    if (interlockOn())
                    {
                        string name = "Interlock";
                        string message = defaultMessage;
                        System.Windows.Media.ImageSource image = null;
                        Equipment.EquipmentBase equipment = Equipment;
                        var module = this;

                        try
                        {
                            name = $"Interlock {module.Name}.{partAction.Part.Name}.{partAction.Name}";
                            image = null;

                            if (System.IO.File.Exists(filepath))
                            {
                                var xml = System.Xml.Linq.XElement.Load(filepath);
                                if (xml.Element("Message") != null)
                                    message = xml.Element("Message").Value;
                                if (xml.Element("ImagePath") != null)
                                {
                                    var imagePath = xml.Element("ImagePath").Value;
                                    if (System.IO.File.Exists(imagePath))
                                    {
                                        image = new System.Windows.Media.Imaging.BitmapImage(new Uri(imagePath));
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            message = $"{message}\n{e.ToString()}";
                        }

                        Manager.MessageWindowManager.Instance.Show(equipment,
                                name,
                                message,
                                image,
                                true);
                        Equipment.RequestStop(); //jbpark_2020.06.17
                        return true;
                    }
                    else
                        return false;
                });
        }

        /// <summary>
        /// FALibrary.Part.MemoryBasePart.FAPartMemoryBaseGeneric을 상속받은 클래스에 대해
        /// 래핑한 메소드.
        /// FALibrary.Part.MemoryBasePart.FAPartMemoryBaseGeneric이 가장많이 쓰이기 때문에 래핑 메소드를 제공.
        /// </summary>
        /// <param name="partAction">인터락을 추가할 PartAction</param>
        /// <param name="statusOwner">Status를 비교할 대상 클래스</param>
        /// <param name="status">비교할 Status. statusOwner.Status != status 인 경우 Interlock On</param>
        protected void AddPartInterlock(FALibrary.Part.FAPartAction partAction,
            dynamic statusOwner,
            FALibrary.FAStatus status)
        {
            AddPartInterlock(partAction,
                () => statusOwner.Status != status,
                $"{statusOwner.Name} Part status is not {status.Name}");
        }

        /// <summary>
        /// FALibrary.Part.MemoryBasePart.FAPartMemoryBaseGeneric을 상속받은 클래스에 대해
        /// 래핑한 메소드.
        /// FALibrary.Part.MemoryBasePart.FAPartMemoryBaseGeneric이 가장많이 쓰이기 때문에 래핑 메소드를 제공.
        /// </summary>
        /// <param name="partAction">인터락을 추가할 PartAction</param>
        /// <param name="statusOwner">Status를 비교할 대상 클래스</param>
        /// <param name="status">비교할 Status. statusOwner.Status != status 인 경우 Interlock On</param>
        protected void AddPartInterlock(FALibrary.Part.FAPartAction partAction,
            params (dynamic statusOwner, FALibrary.FAStatus status)[] statusCompareSets)
        {
            foreach (var item in statusCompareSets)
                AddPartInterlock(partAction, item.statusOwner, item.status);
        }

        /// <summary>
        /// 서보 모터의 이동 포지션에 대해 인터락을 설정하는 메소드
        /// </summary>
        /// <param name="part">서보 모터 파트</param>
        /// <param name="interlockOn">인터락 조건 함수. 반환값이 true이면 인터락 경고가 발생한다.
        /// 1번 파라미터(double) = actual pos, 2번 파라미터(double) = target pos</param>
        /// <param name="defaultMessage">인터락 리소스를 찾지 못하거나 예외가 발생한 경우 보여줄 기본 메시지</param>
        protected void AddServoCanIMoveInterlock(FALibrary.Part.MMCPart.FAMMCPart part,
            Func<double, double, bool> interlockOn,
            string defaultMessage)
        {
            if (string.IsNullOrEmpty(part.Name))
                return;

            if (!part.ContainsMetaProperty("InterlockNo"))
                part.SetMetaPropertyValue("InterlockNo", 0);
            int interlockNo = (int)part.GetMetaPropertyValue("InterlockNo") + 1;
            part.SetMetaPropertyValue("InterlockNo", interlockNo);

            var dir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "InterlockResource",
                this.Name,
                part.Name);
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            var filepath = System.IO.Path.Combine(dir, $"{part.Name}_{interlockNo}.xml");

            if (System.IO.File.Exists(filepath))
            {
                var xml = System.Xml.Linq.XElement.Load(filepath);
                if (xml.Element("DefaultMessage") != null &&
                    xml.Element("DefaultMessage").Value != defaultMessage)
                {
                    WriteInterlockResource(filepath, defaultMessage);
                }
            }
            else
                WriteInterlockResource(filepath, defaultMessage);

            part.AddCanIMoveMethod(
                (actualPos, targetPos) =>
                {
                    if (interlockOn(actualPos, targetPos))
                    {
                        string name = "Interlock";
                        string message = defaultMessage;
                        System.Windows.Media.ImageSource image = null;
                        Equipment.EquipmentBase equipment = Equipment;
                        var module = this;

                        try
                        {
                            name = $"Interlock {module.Name}.{part.Name}";
                            image = null;

                            if (System.IO.File.Exists(filepath))
                            {
                                var xml = System.Xml.Linq.XElement.Load(filepath);
                                if (xml.Element("Message") != null)
                                    message = $"{part.Name} Can not move [{actualPos}] -> pos[{targetPos}]. " + xml.Element("Message").Value;
                                if (xml.Element("ImagePath") != null)
                                {
                                    var imagePath = xml.Element("ImagePath").Value;
                                    if (System.IO.File.Exists(imagePath))
                                    {
                                        image = new System.Windows.Media.Imaging.BitmapImage(new Uri(imagePath));
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            message = $"{message}\n{e.ToString()}";
                        }

                        Manager.MessageWindowManager.Instance.Show(equipment,
                                name,
                                message,
                                image,
                                true);
                        Equipment.RequestStop(); //jbpark_2020.06.17
                        return false;
                    }
                    else
                        return true;
                });
        }

        protected void AddServoCanIHomingInterlock(FALibrary.Part.MMCPart.FAMMCPart part,
            Func<bool> interlockOn,
            string defaultMessage)
        {
            if (string.IsNullOrEmpty(part.Name))
                return;

            if (!part.ContainsMetaProperty("InterlockNo"))
                part.SetMetaPropertyValue("InterlockNo", 0);
            int interlockNo = (int)part.GetMetaPropertyValue("InterlockNo") + 1;
            part.SetMetaPropertyValue("InterlockNo", interlockNo);

            var dir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "InterlockResource",
                this.Name,
                part.Name);
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            var filepath = System.IO.Path.Combine(dir, $"{part.Name}_{interlockNo}.xml");

            if (System.IO.File.Exists(filepath))
            {
                var xml = System.Xml.Linq.XElement.Load(filepath);
                if (xml.Element("DefaultMessage") != null &&
                    xml.Element("DefaultMessage").Value != defaultMessage)
                {
                    WriteInterlockResource(filepath, defaultMessage);
                }
            }
            else
                WriteInterlockResource(filepath, defaultMessage);

            part.AddCanIHomingMethod(
                () =>
                {
                    if (interlockOn())
                    {
                        string name = "Interlock";
                        string message = defaultMessage;
                        System.Windows.Media.ImageSource image = null;
                        Equipment.EquipmentBase equipment = Equipment;
                        var module = this;

                        try
                        {
                            name = $"Interlock {module.Name}.{part.Name}";
                            image = null;

                            if (System.IO.File.Exists(filepath))
                            {
                                var xml = System.Xml.Linq.XElement.Load(filepath);
                                if (xml.Element("Message") != null)
                                    message = $"{part.Name} Can not homing. " + xml.Element("Message").Value;
                                if (xml.Element("ImagePath") != null)
                                {
                                    var imagePath = xml.Element("ImagePath").Value;
                                    if (System.IO.File.Exists(imagePath))
                                    {
                                        image = new System.Windows.Media.Imaging.BitmapImage(new Uri(imagePath));
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            message = $"{message}\n{e.ToString()}";
                        }

                        Manager.MessageWindowManager.Instance.Show(equipment,
                                name,
                                message,
                                image,
                                true);
                        Equipment.RequestStop(); //jbpark_2020.06.17
                        return false;
                    }
                    else
                        return true;
                });
        }

        /// <summary>
        /// 서보 모터의 이동 포지션에 대해 인터락을 설정하는 메소드
        /// </summary>
        /// <param name="part">서보 모터 파트</param>
        /// <param name="compareSets">인터락 조건, 인터락 메시지의 Tuple.
        /// interlockOn --> 반환 값이 true이면 인터락 경고가 발생한다.
        /// 1번 파라미터(double) = actual pos, 2번 파라미터(double) = target pos</param>
        protected void AddServoCanIMoveInterlock(FALibrary.Part.MMCPart.FAMMCPart part,
            params (Func<double, double, bool> interlockOn, string defaultMessage)[] compareSets)
        {
            foreach (var item in compareSets)
                AddServoCanIMoveInterlock(part, item.interlockOn, item.defaultMessage);
        }

        protected void AddServoCanIHomingInterlock(FALibrary.Part.MMCPart.FAMMCPart part,
            params (Func<bool> interlockOn, string defaultMessage)[] compareSets)
        {
            foreach (var item in compareSets)
                AddServoCanIHomingInterlock(part, item.interlockOn, item.defaultMessage);
        }

        protected void AddServoCanIHomingInterlock(FALibrary.Part.MMCPart.FAMMCPart part,
            params (dynamic statusOwner, FALibrary.FAStatus status)[] statusCompareSets)
        {
            foreach (var item in statusCompareSets)
            {
                AddServoCanIHomingInterlock(part,
                    (() => item.statusOwner.Status != item.status,
                    $"{item.statusOwner.Name} Part status is not {item.status.Name}"));
            }
        }

        /// <summary>
        /// 서보 모터의 Positive 이동 (actualPos 보다 targetPos이 클 때)에 대해 인터락을 설정하는 메소드.
        /// </summary>
        /// <param name="part">서보 모터 파트</param>
        /// <param name="compareSets">인터락 조건, 인터락 메시지의 Tuple.
        /// interlockOn --> 반환 값이 true이면 인터락 경고가 발생한다.
        /// 1번 파라미터(double) = actual pos, 2번 파라미터(double) = target pos</param>
        protected void AddServoCanIMoveToPositiveDirInterlock(FALibrary.Part.MMCPart.FAMMCPart part,
            params (Func<bool> interlockOn, string defaultMessage)[] compareSets)
        {
            foreach (var item in compareSets)
            {
                AddServoCanIMoveInterlock(part,
                    (actualPos, targetPos) =>
                    {
                        if (actualPos < targetPos)
                            return item.interlockOn();
                        else
                            return false;
                    },
                    item.defaultMessage);
            }
        }

        protected void AddServoCanIMoveToPositiveDirInterlock(FALibrary.Part.MMCPart.FAMMCPart part,
            params (dynamic statusOwner, FALibrary.FAStatus status)[] statusCompareSets)
        {
            foreach (var item in statusCompareSets)
            {
                AddServoCanIMoveToPositiveDirInterlock(part,
                    (() => item.statusOwner.Status != item.status,
                    $"{item.statusOwner.Name} Part status is not {item.status.Name}"));
            }
        }

        /// <summary>
        /// 서보 모터의 Negative 이동 (actualPos 보다 targetPos이 작을 때)에 대해 인터락을 설정하는 메소드.
        /// </summary>
        /// <param name="part">서보 모터 파트</param>
        /// <param name="compareSets">인터락 조건, 인터락 메시지의 Tuple.
        /// interlockOn --> 반환 값이 true이면 인터락 경고가 발생한다.
        /// 1번 파라미터(double) = actual pos, 2번 파라미터(double) = target pos</param>
        protected void AddServoCanIMoveToNegativeDirInterlock(FALibrary.Part.MMCPart.FAMMCPart part,
            params (Func<bool> interlockOn, string defaultMessage)[] compareSets)
        {
            foreach (var item in compareSets)
            {
                AddServoCanIMoveInterlock(part,
                    (actualPos, targetPos) =>
                    {
                        if (actualPos > targetPos)
                            return item.interlockOn();
                        else
                            return false;
                    },
                    item.defaultMessage);
            }
        }

        protected void AddServoCanIMoveToNegativeDirInterlock(FALibrary.Part.MMCPart.FAMMCPart part,
            params (dynamic statusOwner, FALibrary.FAStatus status)[] statusCompareSets)
        {
            foreach (var item in statusCompareSets)
            {
                AddServoCanIMoveToNegativeDirInterlock(part,
                    (() => item.statusOwner.Status != item.status,
                    $"{item.statusOwner.Name} Part status is not {item.status.Name}"));
            }
        }

        private void SetAlarmID()
        {
            foreach (var propInfo in this.GetType().GetProperties())
            {
                var temp = Attribute.GetCustomAttribute(propInfo, typeof(DefaultAlarmInfo), false);
                if (temp != null)
                {
                    var alarmInfo = temp as DefaultAlarmInfo;
                    propInfo.SetValue(this, 2000000 + MachineID * 1000 + alarmInfo.AlarmNo);
                }

                temp = Attribute.GetCustomAttribute(propInfo, typeof(KukaRobotAlarmInfo), false);
                if (temp != null)
                {
                    var alarmInfo = temp as KukaRobotAlarmInfo;
                    propInfo.SetValue(this, 9000000 + MachineID * 1000 + alarmInfo.AlarmNo);
                }
            }
        }
    }
}