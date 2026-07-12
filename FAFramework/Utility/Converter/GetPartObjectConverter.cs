using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System.Collections;

namespace FAFramework.Utility.Converter
{
    public class GetPartObjectConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[0] == null)
                return null;
            if (values[1] == null)
                return null;

            var source = values[1];
            var partPaths = (values[0] as string).Split(' ').Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim());
            var parts = partPaths.Select(x => UtilityClass.GetPropertyValue(source, x.Split('.')));

            return parts;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("No two way conversion, one way binding only.");
        }
    }
}
