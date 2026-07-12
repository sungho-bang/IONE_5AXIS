using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FAFramework.Manager
{
    public class MessageWindowNameManager
    {
        private static volatile MessageWindowNameManager _instance = null;
        private static object syncRoot = new Object();
        private static object threadRoot = new Object();
        private Dictionary<object, Dictionary<string, string>> _objectAndNameDictionary = new Dictionary<object, Dictionary<string, string>>();
        private MessageWindowNameManager()
        {
        }

        public static MessageWindowNameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                            _instance = new MessageWindowNameManager();
                    }
                }

                return _instance;
            }
        }

        public void SetWindowName(object source, string key, string name)
        {
            if (source == null || string.IsNullOrEmpty(key)) return;

            if (_objectAndNameDictionary.ContainsKey(source) == false)
                _objectAndNameDictionary.Add(source, new Dictionary<string, string>());

            if (_objectAndNameDictionary[source].ContainsKey(key))
                _objectAndNameDictionary[source][key] = name;
            else
                _objectAndNameDictionary[source].Add(key, name);
        }

        public string GetWindowName(object source, string key)
        {
            if (source == null || string.IsNullOrEmpty(key)) return string.Empty;

            if (_objectAndNameDictionary.ContainsKey(source) == false) return string.Empty;

            if (_objectAndNameDictionary[source].ContainsKey(key) == false) return string.Empty;

            return _objectAndNameDictionary[source][key];
        }
    }
}
