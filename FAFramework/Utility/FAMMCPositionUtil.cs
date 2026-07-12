using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FAFramework.Utility
{
    public static class FAMMCPositionUtil
    {
        public static string[] ToKeyValueArray(this FALibrary.Part.MMCPart.FAMMCPosition obj, string prefix)
        {
            string[] arr =
            {
                string.Format("{0}Position={1}", prefix, obj.Position),
                //string.Format("{0}StartSpeed={1}", prefix, obj.StartSpeed),
                //string.Format("{0}DriveSpeed={1}", prefix, obj.DriveSpeed),
                //string.Format("{0}AccelTime={1}", prefix, obj.AccelTime),
                //string.Format("{0}DecelTime={1}", prefix, obj.DecelTime)
            };

            return arr;
        }

        public static void Parsing(this FALibrary.Part.MMCPart.FAMMCPosition obj, System.Xml.Linq.XElement xml)
        {
            try
            {
                if (xml.Element("Position") != null)
                    obj.Position = double.Parse(xml.Element("Position").Value);

                if (xml.Element("StartSpeed") != null)
                    obj.StartSpeed = uint.Parse(xml.Element("StartSpeed").Value);

                if (xml.Element("DriveSpeed") != null)
                    obj.DriveSpeed = uint.Parse(xml.Element("DriveSpeed").Value);

                if (xml.Element("AccelTime") != null)
                    obj.AccelTime = uint.Parse(xml.Element("AccelTime").Value);

                if (xml.Element("DecelTime") != null)
                    obj.DecelTime = uint.Parse(xml.Element("DecelTime").Value);
                
                // ★ 선택: Limit 값도 파싱
                if (xml.Element("LwLimit") != null)
                    obj.LwLimit = double.Parse(xml.Element("LwLimit").Value);

                if (xml.Element("UpLimit") != null)
                    obj.UpLimit = double.Parse(xml.Element("UpLimit").Value);
            }
            catch (Exception e)
            {
                Manager.LogManager.Instance.WriteSystemLog(e.ToString());
            }
        }
    }
}
