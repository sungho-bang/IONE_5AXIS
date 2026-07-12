using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace FAFramework.Manager
{
    public class AlarmResourceManager
    {
        public readonly string ALARM_LIST_FOLDER_PATH = ConfigClasses.GlobalConst.CONFIG_PATH + "AlarmList";
        public readonly string ALARM_LIST_FILENAME_PREFIX = "AlarmList";
        public readonly string DEFAULT_CULTURE = "en-US";

        public event EventHandler OnLoadAlarmList;

        public void Load()
        {
            string culture = "";

            try
            {
                culture = Manager.StringResourceManager.Instance.CurrentCultureInstance.Name;
            }
            catch
            {
                culture = DEFAULT_CULTURE;
            }

            string filename = string.Format("{0}_{1}.xml", ALARM_LIST_FILENAME_PREFIX, culture);
            string filepath = System.IO.Path.Combine(ALARM_LIST_FOLDER_PATH, filename);

            if (System.IO.File.Exists(filepath) == false)
            {
                string logMsg = string.Format("Can't read a alarmlist file. Not exists ", filepath);
                Manager.LogManager.Instance.WriteSystemLog(logMsg);

                filename = string.Format("{0}_{1}.xml", ALARM_LIST_FILENAME_PREFIX, DEFAULT_CULTURE);
                filepath = System.IO.Path.Combine(ALARM_LIST_FOLDER_PATH, filename);

                if (System.IO.File.Exists(filepath) == false)
                {
                    logMsg = string.Format("Can't read a alarmlist file. Not exists ", filepath);
                    Manager.LogManager.Instance.WriteSystemLog(logMsg);
                    return;
                }
            }

            var alarmList = FALibrary.Alarm.FAAlarmManager.Instance.Items;
            XElement xml = XElement.Load(filepath);
            foreach (var item in xml.Elements())
            {
                var alarm = ParsingAlarmFromXml(item);

                if (alarmList.ContainsKey(alarm.AlarmNo) == true)
                {
                    alarm.CopyTo(alarmList[alarm.AlarmNo]);
                }
                else
                    alarmList.Add(alarm.AlarmNo, alarm);
            }

            Save();

            if (OnLoadAlarmList != null)
                OnLoadAlarmList(this, EventArgs.Empty);
        }

        public void Save()
        {
            string culture = "";

            try
            {
                culture = Manager.StringResourceManager.Instance.CurrentCultureInstance.Name;
            }
            catch
            {
                culture = DEFAULT_CULTURE;
            }

            string filename = string.Format("{0}_{1}.xml", ALARM_LIST_FILENAME_PREFIX, culture);
            string filepath = System.IO.Path.Combine(ALARM_LIST_FOLDER_PATH, filename);

            XElement xml = null;
            try
            {
                xml = new XElement("AlarmList",
                    from keyValue in FALibrary.Alarm.FAAlarmManager.Instance.Items
                    select new XElement("Item",
                        new XElement("AlarmNo", keyValue.Value.AlarmNo),
                        new XElement("AlarmName", keyValue.Value.AlarmName),
                        new XElement("Description", keyValue.Value.Description),
                        new XElement("Solution", keyValue.Value.Solution),
                        new XElement("ImagePath", keyValue.Value.ImagePath),
                        new XElement("Status", keyValue.Value.Status),
                        new XElement("Type", keyValue.Value.Type)
                        ));
            }
            catch
            {
                return;
            }

            System.Xml.Linq.XDocument doc = new XDocument();
            doc.Add(xml);

            if (System.IO.Directory.Exists(ALARM_LIST_FOLDER_PATH) == false)
                System.IO.Directory.CreateDirectory(ALARM_LIST_FOLDER_PATH);

            doc.Save(filepath);
        }

        private FALibrary.Alarm.FAAlarm ParsingAlarmFromXml(XElement xml)
        {
            FALibrary.Alarm.FAAlarm alarm = new FALibrary.Alarm.FAAlarm();

            try
            {
                if (xml.Element("AlarmNo") != null)
                    alarm.AlarmNo = int.Parse(xml.Element("AlarmNo").Value);

                alarm.AlarmName = xml.Element("AlarmName").Value;
                alarm.Description = xml.Element("Description").Value;
                alarm.Solution = xml.Element("Solution").Value;
                alarm.ImagePath = xml.Element("ImagePath").Value;
                if (xml.Element("Status") != null)
                    alarm.Status = int.Parse(xml.Element("Status").Value);
                if (xml.Element("Type") != null)
                    alarm.Type = int.Parse(xml.Element("Type").Value);

                return alarm;
            }
            catch
            {
                return null;
            }
        }
    }
}
