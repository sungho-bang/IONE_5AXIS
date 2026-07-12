using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace FAFramework.Utility.Converter
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            try
            {
                if ((value is bool) == false) return Visibility.Visible;

                var visibility = (bool)value;
                var useHidden = false;
                if (parameter is string)
                {
                    var param = parameter as string;
                    if (string.IsNullOrEmpty(param) == false)
                    {
                        if (param.ToLower() == "inverse")
                            visibility = !visibility;
                        else if (param.ToLower() == "use_hidden")
                            useHidden = true;
                    }
                }

                if (visibility == true)
                    return Visibility.Visible;
                else if (useHidden)
                    return Visibility.Hidden;
                else
                    return Visibility.Collapsed;
            }
            catch
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if ((value is Visibility) == false) return false;

            Visibility temp = (Visibility)value;

            if (temp == Visibility.Visible)
                return true;
            else
                return false;
        }
    }
}
