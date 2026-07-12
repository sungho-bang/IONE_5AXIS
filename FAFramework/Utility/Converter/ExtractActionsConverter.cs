using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using FALibrary.Part;
using FAFramework.Utility;

namespace FAFramework.Utility.Converter
{
    public class ExtractActionsConverter : IValueConverter
    {
        static char[] seperator = new char[] { ';' };
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is FAPart))
                return null;
            List<string> exceptActions = new List<string>();
            if (parameter is string && !string.IsNullOrEmpty(parameter as string))
            {
                exceptActions.AddRange((parameter as string).Split(seperator));
            }

            var part = value as FAPart;
            var result = part.GetAllPartAction().
                Where(x => !exceptActions.Contains(x.ActionName)).ToArray();
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("ExtractActionsConverter can only be used OneWay.");
        }
    }
}
