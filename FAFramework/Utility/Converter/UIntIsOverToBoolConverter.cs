using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace FAFramework.Utility.Converter
{
    public class UIntIsOverToBoolConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                uint v = (uint)System.Convert.ChangeType(values[0], typeof(uint));
                uint p = (uint)System.Convert.ChangeType(values[1], typeof(uint));

                if (p > v) return true;
                else return false;
            }
            catch
            {
                return false;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
