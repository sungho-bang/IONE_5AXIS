using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FAFramework.GUI;

namespace FAFramework.Manager
{
    public class QueryMessageBoxManager
    {
        private class QueryWindowInfo
        {
            public QuestionMessageBoxWindow WindowInstance { get; set; }

            public QueryWindowInfo(QuestionMessageBoxWindow windowInstance)
            {
                WindowInstance = windowInstance;
            }
        }

        private static volatile QueryMessageBoxManager _instance = null;
        private static object syncRoot = new Object();
        private Dictionary<object, QueryWindowInfo> _windowObjects = new Dictionary<object, QueryWindowInfo>();

        public static QueryMessageBoxManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                            _instance = new QueryMessageBoxManager();
                    }
                }

                return _instance;
            }
        }

        public QuestionMessageBoxWindow.QuestionResult Show(object owner, string message, Equipment.EquipmentBase equipment, bool cancelAble, bool useSound)
        {
            QuestionMessageBoxWindow.QuestionResult result = QuestionMessageBoxWindow.QuestionResult.None;

            App.Current.Dispatcher.Invoke(
                new Action(
                    delegate ()
                    {
                        if (_windowObjects.ContainsKey(owner) == false)
                            _windowObjects.Add(owner, new QueryWindowInfo(null));

                        var windowInfo = _windowObjects[owner];
                        var window = _windowObjects[owner].WindowInstance;

                        if (window == null ||
                            window.IsLoaded == false)
                        {
                            window = new QuestionMessageBoxWindow();
                            windowInfo.WindowInstance = window;
                            window.EquipmentInstance = equipment;
                            window.Message = message;
                            window.Cancelable = cancelAble;
                            window.UseSound = useSound;
                            window.Caption = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            window.Show();
                        }
                        else
                        {
                            window.Message = message;
                            window.UseSound = useSound;
                        }

                        result = window.Result;
                    }));

            return result;
        }
    }
}
