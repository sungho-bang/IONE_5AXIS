using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace FAFramework.Utility.Converter
{
    public class OnOffPartAvailableStatusConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                dynamic part = values[0];
                dynamic partStatus = values[1];

                var properties = part.StatusList.GetType().GetProperties();
                List<dynamic> statusObjectList = new List<dynamic>();
                int i = 0;
                int matchedIndex = -1;
                foreach (var item in properties)
                {
                    var propertyValue = item.GetValue(part.StatusList, null);
                    if (partStatus == propertyValue)
                        matchedIndex = i;
                    statusObjectList.Add(propertyValue);
                    i++;
                }

                if (matchedIndex == 0)
                    return statusObjectList[1].DisplayName;
                else if (matchedIndex == 1)
                    return statusObjectList[0].DisplayName;
                else
                    return "Unknown";

            }
            catch
            {
                return "Error";
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new InvalidOperationException("OnOffPartAvailableStatusConverter can only be used OneWay.");
        }
    }
}
