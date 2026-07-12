using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace FAFramework.Utility.Converter
{
    public class IsEqualMultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null) return false;
            if (values.Length < 2) return false;

            object first = values[0];
            if (first == null)
                return false;

            foreach (var item in values)
            {
                if (first.Equals(item) == false) return false;
            }

            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("No two way conversion, one way binding only.");
        }
    }
}
