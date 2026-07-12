using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;
using System.Windows;


namespace FAFramework.Utility.Converter
{
    public class AlarmTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if ((value is int) == false) return "NONE";

            int type = (int)value;

            if (type == ConfigClasses.GlobalConst.ALARM_TYPE_HUMAN)
                return "HUMAN";
            else if (type == ConfigClasses.GlobalConst.ALARM_TYPE_MACHINE)
                return "MACHINE";
            else if (type == ConfigClasses.GlobalConst.ALARM_TYPE_MATERIAL)
                return "MATERIAL";
            else if (type == ConfigClasses.GlobalConst.ALARM_TYPE_METHOD)
                return "METHOD";
            else
                return "NONE";
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if ((value is string) == false) return -1;

            string typeName = ((string)value).ToUpper();

            if (typeName == "HUMAN")
                return ConfigClasses.GlobalConst.ALARM_TYPE_HUMAN;
            else if (typeName == "MACHINE")
                return ConfigClasses.GlobalConst.ALARM_TYPE_MACHINE;
            else if (typeName == "MATERIAL")
                return ConfigClasses.GlobalConst.ALARM_TYPE_MATERIAL;
            else if (typeName == "METHOD")
                return ConfigClasses.GlobalConst.ALARM_TYPE_METHOD;
            else
                return -1;
        }
    }
}
