using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace FAFramework.Utility.Converter
{
    public class IsExistConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null) return false;
            if (values.Length < 2) return false;

            var array = values[0] as System.Collections.IEnumerable;
            object first = values[1];
            if (array == null) return false;

            foreach (var item in array)
            {
                if (first.Equals(item)) return true;
            }

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("No two way conversion, one way binding only.");
        }
    }
}
