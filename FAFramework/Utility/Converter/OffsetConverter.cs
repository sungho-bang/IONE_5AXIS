using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;

namespace FAFramework.Utility.Converter
{
    public class OffsetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            double v = (double)System.Convert.ChangeType(value, typeof(double));
            string param = (string)System.Convert.ChangeType(parameter, typeof(string));
            var args = param.Split(';');
            double offset = double.Parse(args[0]);
            double rate = 1.0;
            if (args.Length > 1)
                rate = double.Parse(args[1]);

            return v * rate + offset;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
