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
using FALibrary.Sequence;

namespace FAFramework.Utility.Converter
{
    public class SequenceStartableToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var state = (SequenceState)value;
                var isStartable = state == SequenceState.Available || state == SequenceState.Terminated;
                if (parameter is string &&
                    !string.IsNullOrEmpty(parameter as string))
                {
                    if ((parameter as string).Trim() == "inverse")
                    {
                        isStartable = !isStartable;
                    }
                }

                if (isStartable)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;

            }
            catch
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("SequenceStartableConverter can only be used OneWay.");
        }
    }
}
