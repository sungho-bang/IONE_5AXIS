using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace FAFramework.Utility.Converter
{
    public class AlarmStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if ((value is int) == false) return "NONE";

            int type = (int)value;

            if (type == ConfigClasses.GlobalConst.ALARM)
                return "ALARM";
            else if (type == ConfigClasses.GlobalConst.WARNING)
                return "WARNING";
            else
                return "NONE";
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if ((value is string) == false) return -1;

            string typeName = ((string)value).ToUpper();

            if (typeName == "ALARM")
                return ConfigClasses.GlobalConst.ALARM;
            else if (typeName == "WARNING")
                return ConfigClasses.GlobalConst.WARNING;
            else
                return -1;
        }
    }
}
