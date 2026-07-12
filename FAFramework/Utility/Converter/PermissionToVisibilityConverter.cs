using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace FAFramework.Utility.Converter
{
    public class PermissionToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if ((value is Equipment.UserPermissionTypes) == false) return System.Windows.Visibility.Visible;
            if ((parameter is string) == false) return System.Windows.Visibility.Visible;

            var paramArray = (parameter as string).Split(',');
            var userPermission = (Equipment.UserPermissionTypes)value;
            foreach (var item in paramArray)
            {
                Equipment.UserPermissionTypes parseResult;
                if (Enum.TryParse(item, out parseResult) == true)
                {
                    if (userPermission == parseResult) return System.Windows.Visibility.Visible;
                }
            }

            return System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return Equipment.UserPermissionTypes.OPERATOR;
        }
    }
}
