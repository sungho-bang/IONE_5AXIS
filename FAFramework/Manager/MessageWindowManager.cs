using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FAFramework.GUI;
using FALibrary;

namespace FAFramework.Manager
{
    public class MessageWindowManager
    {
        public class MessageWindowInfo
        {
            public MessageWindow WindowInstance { get; set; }
            public Module.StateSignalModule StateSignalModule { get; set; }

            public MessageWindowInfo(MessageWindow windowInstance, Module.StateSignalModule stateSignalModule)
            {
                WindowInstance = windowInstance;
                StateSignalModule = stateSignalModule;
            }
        }

        private static volatile MessageWindowManager _instance = null;
        private static object syncRoot = new Object();
        private Dictionary<string, MessageWindowInfo> _messageWindowList = new Dictionary<string, MessageWindowInfo>();

        private MessageWindowManager()
        {
        }

        public static MessageWindowManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                            _instance = new MessageWindowManager();
                    }
                }

                return _instance;
            }
        }

        public bool Show(string name, string message)
        {
            System.Windows.Window owner = null;

            System.Windows.Application.Current.Dispatcher.Invoke(
                new Action(
                    delegate
                    {
                        owner = System.Windows.Application.Current.Windows.OfType<System.Windows.Window>().SingleOrDefault(x => x.IsActive);
                    }));

            return Show(owner, name, message);
        }

        public bool Show(System.Windows.Window owner, string name, string message)
        {
            bool result = false;

            App.Current.Dispatcher.Invoke(
                new Action(
                    delegate ()
                    {
                        if (_messageWindowList.ContainsKey(name) == false)
                        {
                            _messageWindowList.Add(name, new MessageWindowInfo(null, null));
                        }

                        var windowInfo = _messageWindowList[name];
                        var window = _messageWindowList[name].WindowInstance;

                        if (window == null ||
                            window.IsLoaded == false)
                        {
                            var messageWindow = new MessageWindow();
                            windowInfo.WindowInstance = messageWindow;

                            messageWindow.EquipmentInstance = null;
                            messageWindow.RaisedTime = DateTime.Now;
                            messageWindow.Message = message;
                            messageWindow.Caption = name;
                            messageWindow.UseSound = false;
                            messageWindow.Owner = owner;
                            messageWindow.Show();

                            result = true;
                        }
                        else if (window.Message != message)
                        {
                            var messageWindow = _messageWindowList[name].WindowInstance;
                            messageWindow.RaisedTime = DateTime.Now;
                            messageWindow.Message = message;
                            messageWindow.Caption = name;
                            messageWindow.UseSound = false;

                            result = true;
                        }
                        else
                            result = false;
                    }));


            return result;
        }

        public bool Show(Equipment.EquipmentBase equipment, string name, string message, System.Windows.Media.ImageSource image, bool useSound)
        {
            Module.StateSignalModule stateSignalModule = null;
            ConfigClasses.EquipmentStateConfig stateConfig = null;

            if (equipment != null)
            {
                try
                {
                    stateSignalModule = equipment.StateSignalModuleReferance;

                    if (stateSignalModule != null && stateSignalModule.EquipmentStateConfigs != null)
                        stateConfig = stateSignalModule.EquipmentStateConfigs.NotifyMessageConfig;
                }
                catch
                {
                }
            }

            return Show(equipment, name, message, image, useSound, stateSignalModule, stateConfig);
        }

        public bool Show(Equipment.EquipmentBase equipment, string name, string message, System.Windows.Media.ImageSource image, bool useSound, Module.StateSignalModule stateSignalModule)
        {
            ConfigClasses.EquipmentStateConfig stateConfig = null;

            if (equipment != null)
            {
                try
                {
                    stateSignalModule = equipment.StateSignalModuleReferance;

                    if (stateSignalModule != null && stateSignalModule.EquipmentStateConfigs != null)
                        stateConfig = stateSignalModule.EquipmentStateConfigs.NotifyMessageConfig;
                }
                catch
                {
                }
            }

            return Show(equipment, name, message, image, useSound, stateSignalModule, stateConfig);
        }

        public bool Show(Equipment.EquipmentBase equipment, string name, string message, System.Windows.Media.ImageSource image, bool useSound, Module.StateSignalModule stateSignalModule, ConfigClasses.EquipmentStateConfig stateConfig)
        {
            bool result = false;

            App.Current.Dispatcher.Invoke(
                new Action(
                    delegate ()
                    {
                        if (_messageWindowList.ContainsKey(name) == false)
                        {
                            _messageWindowList.Add(name, new MessageWindowInfo(null, stateSignalModule));
                        }

                        _messageWindowList[name].StateSignalModule = stateSignalModule;

                        bool customSoundMode = false;
                        if (stateSignalModule != null && stateConfig != null)
                        {
                            try
                            {
                                customSoundMode = true;
                                stateConfig.CopyTo(stateSignalModule.CustomState);
                            }
                            catch
                            {
                            }
                        }

                        var windowInfo = _messageWindowList[name];
                        var window = _messageWindowList[name].WindowInstance;

                        if (window == null ||
                            window.IsLoaded == false)
                        {
                            if (equipment != null && equipment.StateSignalModuleReferance != null)
                                equipment.StateSignalModuleReferance.OffSound = false;

                            var messageWindow = new MessageWindow();
                            windowInfo.WindowInstance = messageWindow;

                            if (equipment != null)
                                messageWindow.EquipmentInstance = equipment;
                            messageWindow.RaisedTime = DateTime.Now;
                            messageWindow.Message = message;
                            messageWindow.Caption = name;
                            messageWindow.ImageSource = image;

                            messageWindow.UseSound = useSound;
                            messageWindow.UseCustomSound = customSoundMode;

                            if (equipment != null)
                                messageWindow.Owner = equipment.Window;
                            messageWindow.OnCloseWindow +=
                                delegate
                                {
                                    if (stateSignalModule != null)
                                    {
                                        stateSignalModule.CustomMode = false;
                                    }
                                };
                            messageWindow.Show();

                            result = true;
                        }
                        else if (window.Message != message)
                        {
                            var messageWindow = windowInfo.WindowInstance;
                            messageWindow.RaisedTime = DateTime.Now;
                            messageWindow.Message = message;
                            messageWindow.Caption = name;
                            messageWindow.ImageSource = image;
                            if (equipment != null)
                                messageWindow.Owner = equipment.Window;
                            messageWindow.UseSound = useSound;

                            result = true;
                        }
                        else
                            result = false;

                        if (stateSignalModule != null)
                        {
                            stateSignalModule.CustomMode = true;
                        }

                        if (equipment != null && result == true)
                            LogManager.Instance.WriteTraceLog(equipment, string.Format("MESSAGE : CAPTION = {0}, MESSAGE = {1}", name, message));
                    }));


            return result;
        }

        public bool Show(Equipment.EquipmentBase equipment, string name, string message, bool useSound)
        {
            Module.StateSignalModule stateSignalModule = null;
            ConfigClasses.EquipmentStateConfig stateConfig = null;

            if (equipment != null)
            {
                try
                {
                    stateSignalModule = equipment.StateSignalModuleReferance;

                    if (stateSignalModule != null && stateSignalModule.EquipmentStateConfigs != null)
                        stateConfig = stateSignalModule.EquipmentStateConfigs.NotifyMessageConfig;
                }
                catch
                {
                }
            }

            return Show(equipment, name, message, null, useSound, stateSignalModule, stateConfig);
        }

        public bool Show(Equipment.EquipmentBase equipment, string name, string message)
        {
            Module.StateSignalModule stateSignalModule = null;
            ConfigClasses.EquipmentStateConfig stateConfig = null;

            if (equipment != null)
            {
                try
                {
                    stateSignalModule = equipment.StateSignalModuleReferance;

                    if (stateSignalModule != null && stateSignalModule.EquipmentStateConfigs != null)
                        stateConfig = stateSignalModule.EquipmentStateConfigs.NotifyMessageConfig;
                }
                catch
                {
                }
            }

            return Show(equipment, name, message, null, true, stateSignalModule, stateConfig);
        }

        public bool Show(Equipment.EquipmentBase equipment, string defaultName, out string name, FALibrary.Alarm.FAAlarm alarm, string moreMsg)
        {
            Module.StateSignalModule stateSignalModule = null;
            ConfigClasses.EquipmentStateConfig stateConfig = null;

            if (equipment != null)
            {
                try
                {
                    stateSignalModule = equipment.StateSignalModuleReferance;

                    if (stateSignalModule != null && stateSignalModule.EquipmentStateConfigs != null)
                        stateConfig = stateSignalModule.EquipmentStateConfigs.NotifyMessageConfig;
                }
                catch
                {
                }
            }

            return Show(equipment, defaultName, out name, alarm, moreMsg, true, stateSignalModule, stateConfig);
        }

        public bool Show(Equipment.EquipmentBase equipment, string defaultName, out string name, FALibrary.Alarm.FAAlarm alarm, string moreMsg, bool useSound)
        {
            Module.StateSignalModule stateSignalModule = null;
            ConfigClasses.EquipmentStateConfig stateConfig = null;

            if (equipment != null)
            {
                try
                {
                    stateSignalModule = equipment.StateSignalModuleReferance;

                    if (stateSignalModule != null && stateSignalModule.EquipmentStateConfigs != null)
                        stateConfig = stateSignalModule.EquipmentStateConfigs.NotifyMessageConfig;
                }
                catch
                {
                }
            }

            return Show(equipment, defaultName, out name, alarm, moreMsg, useSound, stateSignalModule, stateConfig);
        }

        public bool Show(Equipment.EquipmentBase equipment, string defaultName, out string name, FALibrary.Alarm.FAAlarm alarm, string moreMsg, bool useSound, Module.StateSignalModule stateSignalModule)
        {
            ConfigClasses.EquipmentStateConfig stateConfig = null;

            if (equipment != null)
            {
                try
                {
                    stateSignalModule = equipment.StateSignalModuleReferance;

                    if (stateSignalModule != null && stateSignalModule.EquipmentStateConfigs != null)
                        stateConfig = stateSignalModule.EquipmentStateConfigs.NotifyMessageConfig;
                }
                catch
                {
                }
            }

            return Show(equipment, defaultName, out name, alarm, moreMsg, useSound, stateSignalModule);
        }

        public bool Show(Equipment.EquipmentBase equipment, string defaultName, out string name, FALibrary.Alarm.FAAlarm alarm, string moreMsg, bool useSound, Module.StateSignalModule stateSignalModule, ConfigClasses.EquipmentStateConfig stateConfig)
        {
            if (alarm != null)
            {
                System.Windows.Media.Imaging.BitmapImage bitmap = null;

                if (System.IO.File.Exists(alarm.ImagePath))
                {
                    try
                    {
                        App.Current.Dispatcher.Invoke(
                            new Action(
                                delegate
                                {
                                    bitmap = new System.Windows.Media.Imaging.BitmapImage(new Uri((string)alarm.ImagePath));
                                }));
                    }
                    catch
                    {
                    }
                }

                string caption = string.Format("{0}{1}{2}{3}", "[", alarm.AlarmNo, "] ", alarm.AlarmName);
                string message = string.Empty;
                if (string.IsNullOrEmpty(moreMsg))
                    message = alarm.Solution;
                else
                    message = string.Format("{0}\n{1}", moreMsg, alarm.Solution);

                name = caption;
                return Show(equipment, caption, message, bitmap, useSound, stateSignalModule, stateConfig);
            }
            else
            {
                name = defaultName;
                return Show(equipment, defaultName, defaultName, null, true, stateSignalModule, stateConfig);
            }
        }

        public void CloseWindow(string name)
        {
            try
            {
                System.Windows.Application.Current.Dispatcher.Invoke(
                    new Action(
                        delegate ()
                        {
                            try
                            {
                                if (string.IsNullOrEmpty(name) == true) return;

                                if (_messageWindowList.ContainsKey(name))
                                {
                                    var windowInfo = _messageWindowList[name];
                                    var window = windowInfo.WindowInstance;

                                    if (window != null &&
                                        window.IsLoaded)
                                    {
                                        if (windowInfo.StateSignalModule != null)
                                            windowInfo.StateSignalModule.CustomMode = false;
                                        window.Close();
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }));
            }
            catch
            {
            }
        }

        public bool IsClosed(string name)
        {
            bool result = false;

            App.Current.Dispatcher.Invoke(
                () =>
                {
                    if (_messageWindowList.ContainsKey(name) == false)
                    {
                        _messageWindowList.Add(name, new MessageWindowInfo(null, null));
                    }

                    if (_messageWindowList[name].WindowInstance == null ||
                        _messageWindowList[name].WindowInstance.IsLoaded == false)
                    {
                        result = true;
                    }

                    result = false;
                });

            return result;
        }
    }
}
