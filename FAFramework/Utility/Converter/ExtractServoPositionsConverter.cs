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
using FALibrary.Part.MMCPart;

namespace FAFramework.Utility.Converter
{
    public class ExtractServoPositionsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is FAMMCPart))
                return null;

            var part = value as FAPart;
            return UtilityClass.GetAllPropertiesValue<FAMMCPosition>(part);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("NullToBooleanConverter can only be used OneWay.");
        }
    }
}
