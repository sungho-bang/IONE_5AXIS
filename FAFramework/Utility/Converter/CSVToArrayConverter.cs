using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System.Text.RegularExpressions;

namespace FAFramework.Utility.Converter
{
    public class CSVToArrayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            string[] defaultResult = new string[] { };
            try
            {
                if ((value is string) == false) return defaultResult;
                if ((parameter is string) == false) return defaultResult;

                string source = value as string;
                string seperator = parameter as string;

                var arr = Regex.Split(source, seperator);
                return arr;
            }
            catch
            {
                return defaultResult;
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            try
            {
                return string.Join(parameter as string, value);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
