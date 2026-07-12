using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace FAFramework.Utility.Converter
{
    public class CamelCaseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            try
            {
                var seperator = (string)parameter;
                string source = (string)value;
                string lowerCase = source.ToLower();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < source.Length; i++)
                {
                    if (i != 0 && source[i] != lowerCase[i])
                        sb.Append(seperator);
                    sb.Append(source[i]);
                }

                return sb.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
