using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FAFramework.Utility.SamsungTPLog
{
    public class LogBase
    {
        public DateTime Date { get; set; }

        private List<string> _elements = new List<string>();
        private Dictionary<string, object> _data = new Dictionary<string, object>();

        public LogBase()
        {
            Date = DateTime.Now;
        }

        public void AddData(string key, object value)
        {
            _data.Add(key, value);
        }

        protected string ToLog()
        {
            _elements.Add(DataToString());
            return Date.ToString("yyyy/MM/dd") + " " +
                Date.ToString("HH:mm:ss.fff") + " " +
                string.Join(" ", _elements);
        }

        protected void AppendElement(string str)
        {
            if (string.IsNullOrEmpty(str))
                str = "$";

            _elements.Add("'" + str + "'");
        }

        private string DataToString()
        {
            List<string> list = new List<string>();

            foreach (var item in _data)
            {
                string valueString = null;

                if (item.Value == null)
                {
                    valueString = "$";
                }
                else
                {
                    Type valueType = valueType = item.Value.GetType();

                    if (item.Value is Boolean == false && valueType.IsPrimitive)
                        valueString = item.Value.ToString();
                    else if (item.Value is Array)
                        valueString = ArrayToString(item.Value as Array);
                    else
                        valueString = string.Format("'{0}'", item.Value.ToString());
                }

                list.Add(string.Format("('{0}', {1})", item.Key, valueString));
            }

            return string.Join(" ", list);
        }

        private string ArrayToString(Array arr)
        {
            List<string> list = new List<string>();

            foreach (var item in arr)
            {
                if (item == null)
                {
                    list.Add("$");
                }
                else
                {
                    Type valueType = item.GetType();

                    if (item is Boolean == false && valueType.IsPrimitive)
                        list.Add(string.Format("{0}", item.ToString()));
                    else
                        list.Add(string.Format("'{0}'", item.ToString()));
                }
            }

            return string.Format("[{0}]", string.Join(", ", list));
        }
    }
}