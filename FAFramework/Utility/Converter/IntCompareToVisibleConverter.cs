using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace FAFramework.Utility.Converter
{
    public class IntCompareToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            try
            {
                var paramArray = ((string)System.Convert.ChangeType(parameter, typeof(string))).Split(';');
                var compareOperator = (string)System.Convert.ChangeType(paramArray[0], typeof(string));
                var rightValue = (int)System.Convert.ChangeType(paramArray[1], typeof(int));
                int leftValue = (int)System.Convert.ChangeType(value, typeof(int));

                bool compareResult = false;

                if (compareOperator.Trim() == "<")
                    compareResult = leftValue < rightValue;
                else if (compareOperator.Trim() == "=<")
                    compareResult = leftValue <= rightValue;
                else if (compareOperator.Trim() == ">")
                    compareResult = leftValue > rightValue;
                else if (compareOperator.Trim() == ">=")
                    compareResult = leftValue >= rightValue;
                else if (compareOperator.Trim() == "==")
                    compareResult = leftValue == rightValue;

                if (compareResult)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
            catch
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
