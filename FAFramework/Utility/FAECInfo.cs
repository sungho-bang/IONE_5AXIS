using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary;
using System.Text.RegularExpressions;

namespace FAFramework.Utility
{
    public enum FAECResult
    {
        OK,
        FAIL
    }

    public enum FAECYesNo
    {
        NO, YES
    }

    public enum FAAlarmHistoryState
    {
        DISABLE,
        ENABLE
    }

    public class ECProperty : FAAttribute
    {
        public string ECKeyName { get; private set; }

        public ECProperty(string groupName, string ecKeyName) : base(groupName)
        {
            ECKeyName = ecKeyName;
        }

        public ECProperty(string ecKeyName) : base("")
        {
            ECKeyName = ecKeyName;
        }

        public ECProperty() : base("")
        {
        }
    }

    public class ECNotNecessary : Attribute
    {

    }

    public class ECIgnoreParsing : Attribute
    {

    }

    public class ECParsingInfo
    {
        public string PropertyName { get; set; }
        public Type PropertyType { get; set; }
        public string ECKeyName { get; set; }
        public bool NotNecessary { get; set; }
        public bool IgnoreParsing { get; set; }
    }

    public interface ECCommand
    {
        string ToCommand();
    }

    public class ECReply : FAObject
    {
        private static Dictionary<Type, List<ECParsingInfo>> ECParsingInfos { get; } = new Dictionary<Type, List<ECParsingInfo>>();

        [FAAttribute("")]
        public FAECResult RESULT { get; set; }

        public ECReply()
        {
            AddECParsingInfos();
        }

        public void Clear()
        {

        }

        public string ToCommand()
        {
            return ToString(";");
        }

        public override string ToString()
        {
            return ToString(",");
        }

        public bool Parse(string str, out List<string> errorMsg)
        {
            errorMsg = new List<string>();

            var keyValues = FAECInfo.SplitECData(str);

            if (keyValues.ContainsKey("RESULT"))
            {
                if (keyValues["RESULT"] == "OK")
                    RESULT = FAECResult.OK;
                else
                    RESULT = FAECResult.FAIL;
            }
            else
            {
                errorMsg.Add("RESULT is not exist");
                return false;
            }

            var type = this.GetType();
            foreach (var item in ECParsingInfos[type])
            {
                Parse(keyValues, item, ref errorMsg);
            }

            if (errorMsg.Count > 0)
                return false;
            else
                return true;
        }

        protected void Parse(Dictionary<string, string> dic,
            ECParsingInfo ecParsingInfo,
            ref List<string> errorMessage)
        {
            if (ecParsingInfo.IgnoreParsing) return;
            if (dic.ContainsKey(ecParsingInfo.ECKeyName))
            {
                try
                {
                    var value = Convert.ChangeType(dic[ecParsingInfo.ECKeyName], ecParsingInfo.PropertyType);
                    var type = this.GetType();
                    type.GetProperty(ecParsingInfo.PropertyName).SetValue(this, value);
                }
                catch (Exception e)
                {
                    if (!ecParsingInfo.NotNecessary)
                        errorMessage.Add($"{ecParsingInfo.ECKeyName}'s value convert error.\n{e.ToString()}");
                }
            }
            else
            {
                if (!ecParsingInfo.NotNecessary)
                    errorMessage.Add($"{ecParsingInfo.ECKeyName} is not exist");
            }
        }

        protected static void Copy<T>(T source, T dest) where T : ECReply
        {
            var type = typeof(T);

            ECParsingInfos[type].ForEach(
                x =>
                {
                    var sourceValue = type.GetProperty(x.PropertyName).GetValue(source);
                    type.GetProperty(x.PropertyName).SetValue(dest, sourceValue);
                });
        }

        private string ToString(string delimiter)
        {
            var type = this.GetType();
            return string.Join(delimiter,
                ECParsingInfos[type].Select(x => $"{x.ECKeyName}={type.GetProperty(x.PropertyName).GetValue(this)}"));
        }

        private void AddECParsingInfos()
        {
            var type = this.GetType();
            if (ECParsingInfos.ContainsKey(type)) return;

            ECParsingInfos.Add(type, new List<ECParsingInfo>());
            var list = ECParsingInfos[type];
            foreach (var prop in type.GetProperties())
            {
                var ecParsingInfo = new ECParsingInfo();
                ecParsingInfo.PropertyName = prop.Name;
                ecParsingInfo.PropertyType = prop.PropertyType;

                var ecPropertys = Attribute.GetCustomAttributes(prop, typeof(ECProperty));
                if (ecPropertys.Length <= 0) continue;
                var ecProperty = (ecPropertys.First() as ECProperty);

                if (string.IsNullOrEmpty(ecProperty.ECKeyName))
                    ecParsingInfo.ECKeyName = prop.Name;
                else
                    ecParsingInfo.ECKeyName = (ecPropertys.First() as ECProperty).ECKeyName;

                ecParsingInfo.NotNecessary = Attribute.GetCustomAttributes(prop, typeof(ECNotNecessary)).Length != 0;
                ecParsingInfo.IgnoreParsing = Attribute.GetCustomAttributes(prop, typeof(ECIgnoreParsing)).Length != 0;

                list.Add(ecParsingInfo);
            }
        }
    }

    public class LABEL_PRINT_INFO_REQ : FAObject, ECCommand
    {
        [FAAttribute("")]
        public string CMD { get; } = "LABEL_PRINT_INFO";

        [FAAttribute("")]
        public string EQPID { get; set; }

        [FAAttribute("")]
        public string OPERID { get; set; }

        [FAAttribute("")]
        public string MODE { get; } = "ONLINE";

        [FAAttribute("")]
        public string BAR_LOT { get; set; }

        public void Clear()
        {
            EQPID = string.Empty;
            OPERID = string.Empty;
            BAR_LOT = string.Empty;
        }

        public void CopyTo(LABEL_PRINT_INFO_REQ obj)
        {
            obj.EQPID = EQPID;
            obj.OPERID = OPERID;
            obj.BAR_LOT = BAR_LOT;
        }

        public string ToCommand()
        {
            return ToString(";");
        }

        public override string ToString()
        {
            return ToString(",");
        }

        public string ToString(string seperator)
        {
            List<string> list = new List<string>();

            AppendKeyValue(list, "CMD", CMD);
            AppendKeyValue(list, "EQPID", EQPID);
            AppendKeyValue(list, "OPERID", OPERID);
            AppendKeyValue(list, "MODE", MODE);
            AppendKeyValue(list, "BAR_LOT", BAR_LOT);

            return string.Join(seperator, list);
        }

        private void AppendKeyValue(List<string> list, string key, object value)
        {
            list.Add($"{key}={value}");
        }
    }

    public class LABEL_PRINT_INFO_RPY : ECReply
    {
        public static readonly string CMD_NAME = "LABEL_PRINT_INFO_REP";

        #region Properties
        [ECProperty("")]
        public string CMD { get; set; }

        [ECProperty("")]
        public string TKIN { get; set; }

        [ECProperty("")]
        public string LOTCLOSECHK { get; set; }

        [ECProperty("")]
        public string STORE { get; set; }

        [ECProperty("")]
        public string LOTID { get; set; }

        [ECProperty("")]
        public int SEQ { get; set; }

        [ECProperty("")]
        public string PART_NO { get; set; }

        [ECProperty("")]
        public int LOT_QTY { get; set; }

        [ECProperty("")]
        public string LOT_STEP { get; set; }

        [ECProperty("")]
        public string PKG_TYPE { get; set; }

        [ECProperty("")]
        public string TRAY_CODE { get; set; }

        [ECProperty("")]
        public string TRAY_MARKING { get; set; }

        [ECProperty("")]
        public double TR_ARRAY_X { get; set; }

        [ECProperty("")]
        public double TR_ARRAY_Y { get; set; }

        [ECProperty("")]
        public double TRAY_THICK { get; set; }

        [ECProperty("")]
        public double POCKET_PITCH_X { get; set; }

        [ECProperty("")]
        public double POCKET_PITCH_Y { get; set; }

        [ECProperty("")]
        public string CTAPE_CODE { get; set; }

        [ECProperty("")]
        public string REEL_CODE { get; set; }

        [ECProperty("")]
        public string REEL_MODEL { get; set; }

        [ECProperty("")]
        public string DESIPAK_CODE { get; set; }

        [ECProperty("")]
        public string DESIPAK_MODEL { get; set; }

        [ECProperty("")]
        public string INDICATOR_CODE { get; set; }

        [ECProperty("")]
        public string INDICATOR_MODEL { get; set; }

        [ECProperty("")]
        public string BODY_SIZE { get; set; }

        [ECProperty("")]
        public int REEL_MOQ { get; set; }

        [ECProperty("")]
        public string REEL_CNT { get; set; }

        [ECProperty("")]
        public string CUSTOMER { get; set; }

        [ECProperty("")]
        public string LARGE_BOX { get; set; }

        [ECProperty("")]
        public string LBOX_VID { get; set; }

        [ECProperty("")]
        public int BOX_CNT { get; set; }

        [ECProperty("")]
        public int LABEL_CNT { get; set; }

        [ECProperty("")]
        public int MSG_LENGTH { get; set; }

        [ECProperty("")]
        public string LABEL_TYPE_1 { get; set; }

        [ECProperty("")]
        public string LABEL_FORMAT_1 { get; set; }

        [ECProperty("")]
        public string LABEL_TYPE_2 { get; set; }

        [ECProperty("")]
        public string LABEL_FORMAT_2 { get; set; }
        #endregion        

        public void CopyTo(LABEL_PRINT_INFO_RPY obj)
        {
            Copy<LABEL_PRINT_INFO_RPY>(this, obj);
        }
    }
    
    public class EQPALARMHISTORY_REQ : FAObject, ECCommand
    {
        [FAAttribute("")]
        public string CMD { get; } = "EQPALARMHISTORY";

        [FAAttribute("")]
        public string EQPID { get; set; }

        [FAAttribute("")]
        public string LOTID { get; set; }

        [FAAttribute("")]
        public string PARTID { get; set; }

        [FAAttribute("")]
        public FAAlarmHistoryState STATE { get; set; }

        [FAAttribute("")]
        public int ALARM_CODE { get; set; }

        [FAAttribute("")]
        public string ALARM_SCRIPT { get; set; }

        public void Clear()
        {
            EQPID = string.Empty;
            LOTID = string.Empty;
            PARTID = string.Empty;
            STATE = FAAlarmHistoryState.DISABLE;
            ALARM_CODE = 0;
            ALARM_SCRIPT = String.Empty;
        }

        public void CopyTo(EQPALARMHISTORY_REQ obj)
        {
            obj.EQPID = EQPID;
            obj.LOTID = LOTID;
            obj.PARTID = PARTID;
            obj.STATE = STATE;
            obj.ALARM_CODE = ALARM_CODE;
            obj.ALARM_SCRIPT = ALARM_SCRIPT;
        }

        public string ToCommand()
        {
            return ToString(";");
        }

        public override string ToString()
        {
            return ToString(",");
        }

        public string ToString(string seperator)
        {
            List<string> list = new List<string>();

            AppendKeyValue(list, "CMD", CMD);
            AppendKeyValue(list, "EQPID", EQPID);
            AppendKeyValue(list, "LOTID", LOTID);
            AppendKeyValue(list, "PARTID", PARTID);
            AppendKeyValue(list, "STATE", STATE);
            AppendKeyValue(list, "ALARM_CODE", ALARM_CODE);
            AppendKeyValue(list, "ALARM_SCRIPT", ALARM_SCRIPT);

            return string.Join(seperator, list);
        }

        private void AppendKeyValue(List<string> list, string key, object value)
        {
            list.Add($"{key}={value}");
        }
    }

    public class EQPALARMHISTORY_RPY : ECReply
    {
        public static readonly string CMD_NAME = "EQPALARMHISTORY_REP";

        #region Properties
        [ECProperty("")]
        public string CMD { get; set; }
        #endregion

        public void CopyTo(EQPALARMHISTORY_RPY obj)
        {
            Copy(this, obj);
        }
    }

    public class EQPSTATUSCHANGE_REQ : FAObject, ECCommand
    {
        [FAAttribute("")]
        public string CMD { get; } = "EQPALARMHISTORY";

        [FAAttribute("")]
        public int CODE { get; set; } = 0;

        [FAAttribute("")]
        public string CONNECT_DB { get; set; } = "COMSIMAX";

        [FAAttribute("")]
        public string EQPID { get; set; }

        [FAAttribute("")]
        public string OPERID { get; set; } = "AUTO";

        [FAAttribute("")]
        public FAAlarmHistoryState MODE { get; set; }

        [FAAttribute("")]
        public FAECYesNo SENDSIMAX { get; set; } = FAECYesNo.NO;

        public void Clear()
        {
            CODE = 0;
            CONNECT_DB = "COMSIMAX";
            EQPID = string.Empty;
            OPERID = "AUTO";
            MODE = FAAlarmHistoryState.DISABLE;
            SENDSIMAX = FAECYesNo.NO;
        }

        public void CopyTo(EQPSTATUSCHANGE_REQ obj)
        {
            obj.CODE = CODE;
            obj.CONNECT_DB = CONNECT_DB;
            obj.EQPID = EQPID;
            obj.OPERID = OPERID;
            obj.MODE = MODE;
            obj.SENDSIMAX = SENDSIMAX;
        }

        public string ToCommand()
        {
            return ToString(";");
        }

        public override string ToString()
        {
            return ToString(",");
        }

        public string ToString(string seperator)
        {
            List<string> list = new List<string>();

            AppendKeyValue(list, "CMD", CMD);
            AppendKeyValue(list, "CODE", CODE);
            AppendKeyValue(list, "CONNECT_DB", CONNECT_DB);
            AppendKeyValue(list, "EQPID", EQPID);
            AppendKeyValue(list, "OPERID", OPERID);
            AppendKeyValue(list, "MODE", MODE);
            AppendKeyValue(list, "SENDSIMAX", SENDSIMAX);

            return string.Join(seperator, list);
        }

        private void AppendKeyValue(List<string> list, string key, object value)
        {
            list.Add($"{key}={value}");
        }
    }

    public class EQPSTATUSCHANGE_RPY : ECReply
    {
        public void CopyTo(EQPSTATUSCHANGE_RPY obj)
        {
            Copy(this, obj);
        }
    }

    public class FAECInfo : FAObject
    {
        public string LotID { get; set; }

        #region Sub Classes

        #endregion

        #region Property
        public LABEL_PRINT_INFO_RPY LABEL_PRINT_INFO { get; } = new LABEL_PRINT_INFO_RPY();
        public EQPALARMHISTORY_RPY EQPALARMHISTORY { get; } = new EQPALARMHISTORY_RPY();
        public EQPSTATUSCHANGE_RPY EQPSTATUSCHANGE { get; } = new EQPSTATUSCHANGE_RPY();
        #endregion

        public void Clear()
        {
            LABEL_PRINT_INFO.Clear();
            EQPALARMHISTORY.Clear();
            EQPSTATUSCHANGE.Clear();
        }

        public void CopyTo(FAECInfo obj)
        {
            LABEL_PRINT_INFO.CopyTo(obj.LABEL_PRINT_INFO);
            EQPALARMHISTORY.CopyTo(obj.EQPALARMHISTORY);
            EQPSTATUSCHANGE.CopyTo(obj.EQPSTATUSCHANGE);
        }

        public static Dictionary<string, string> SplitECData(string str)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            string pattern = ";";
            var matches = Regex.Split(str, pattern);
            foreach (var item in matches)
            {
                if (item == null) continue;
                string[] keyValue = ParsingAttribute(item.ToString());
                if (keyValue == null) continue;

                if (result.ContainsKey(keyValue[0]) == false)
                    result.Add(keyValue[0], keyValue[1]);
            }

            return result;
        }

        public static string[] ParsingAttribute(string str)
        {
            Regex reg = new Regex("=");
            string[] splitResult = reg.Split(str, 2);
            if (splitResult == null || splitResult.Count() < 2)
                return null;

            return splitResult;
        }

        public static bool CheckECDataValid(Dictionary<string, string> dic, out string errorMsg)
        {
            errorMsg = string.Empty;
            return true;
            //errorMsg = string.Empty;
            //bool result = false;
            //List<string> errorList = new List<string>();

            //foreach (var item in dic)
            //{
            //    if (string.IsNullOrEmpty(item.Value) == false)
            //    {
            //        result = false;
            //        errorList.Add(string.Format("Value of {0} is Empty.", item.Key));
            //    }
            //}

            //if (result == false)
            //    errorMsg = string.Join(", ", errorList);

            //return result;
        }
    }
}
