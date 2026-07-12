using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace FAFramework.Utility.Converter
{
    public class IntIsSameToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            try
            {
                int d = (int)System.Convert.ChangeType(value, typeof(int));
                int v = (int)d;
                int p = int.Parse(parameter.ToString());

                if (v == p) return true;
                else return false;
            }
            catch
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
