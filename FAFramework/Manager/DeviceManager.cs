using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection;
using FALibrary.Device;
using FAFramework.Utility;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace FAFramework.Manager
{
    public class OpenFailDeviceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if ((value is string) == false) return null;

            try
            {
                var parseResult = OpenFailDeviceInfo.Parse(value as string);
                return parseResult;
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("OpenFailDeviceConverter can only be used OneWay.");
        }
    }

    public class OpenFailDeviceInfoItem
    {
        public string DeviceName { get; set; }
        public string ExceptionMessage { get; set; }

        public OpenFailDeviceInfoItem()
        {
        }

        public OpenFailDeviceInfoItem(string deviceName, string exceptionMessage)
        {
            DeviceName = deviceName;
            ExceptionMessage = exceptionMessage;
        }
    }

    public class OpenFailDeviceInfo
    {
        public List<OpenFailDeviceInfoItem> Items { get; private set; }

        public OpenFailDeviceInfo()
        {
            Items = new List<OpenFailDeviceInfoItem>();
        }

        public void AddItem(OpenFailDeviceInfoItem obj)
        {
            Items.Add(obj);
        }

        public int Count
        {
            get
            {
                return Items.Count;
            }
        }

        public override string ToString()
        {
            var xml = this.ToXElement();
            return xml.ToString();
        }

        public static OpenFailDeviceInfo Parse(string str)
        {
            XElement xml = XElement.Parse(str);
            return xml.ToObject<OpenFailDeviceInfo>();
        }
    }

    public class DeviceManager
    {
        private Dictionary<string, FADevice> _items = new Dictionary<string, FADevice>();

        public Dictionary<string, FADevice> Items
        {
            get { return _items; }
        }

        public void Load(string filename)
        {
            XElement xml = null;
            xml = XElement.Load(filename);
            Load(xml);
        }

        public void Load(XElement xml)
        {
            Assembly assembly = Assembly.Load("FALibrary");

            foreach (XElement item in xml.Elements())
            {
                string name = item.Element("Name").Value.Trim();
                string deviceType = item.Element("DeviceType").Value.Trim();
                string description = item.Element("Description").Value.Trim();

                Type type = System.Type.GetType("FALibrary.Device." + deviceType);
                object obj = assembly.CreateInstance("FALibrary.Device." + deviceType);
                FADevice device = obj as FADevice;
                device.Name = name;
                device.Description = description;

                try
                {
                    device.LoadParameters(item.Element("Parameters"));
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }

                Items.Add(name, device);
            }
        }

        public bool Open(List<string> openDeviceList, out OpenFailDeviceInfo openFailDeviceInfo)
        {
            openFailDeviceInfo = new OpenFailDeviceInfo();

            Dictionary<string, FADevice> openDeviceDictionary = new Dictionary<string, FADevice>();
            if (openDeviceList == null)
            {
                foreach (var device in Items)
                    openDeviceDictionary.Add(device.Key, device.Value);
            }
            else
            {
                foreach (var item in openDeviceList)
                {
                    if (Items.ContainsKey(item) == true)
                        openDeviceDictionary.Add(item, Items[item]);
                }
            }

            foreach (KeyValuePair<string, FADevice> device in openDeviceDictionary)
            {
                try
                {
                    device.Value.Open();
                }
                catch (Exception e)
                {
                    openFailDeviceInfo.AddItem(
                        new OpenFailDeviceInfoItem(device.Key, e.Message));
                }
            }

            if (openFailDeviceInfo.Count > 0)
                return false;

            return true;
        }

        public void Close()
        {
            foreach (KeyValuePair<string, FADevice> device in _items)
            {
                device.Value.Close();
            }
        }

        public void ReadWrite()
        {
            foreach (KeyValuePair<string, FADevice> device in _items)
            {
                device.Value.ReadWrite();
            }
        }
    }
}