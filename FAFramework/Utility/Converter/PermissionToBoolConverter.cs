using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace FAFramework.Utility.Converter
{
    public class PermissionToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if ((value is Equipment.UserPermissionTypes) == false) return false;

            if ((parameter is Array) == false) return false;

            var userPermission = (Equipment.UserPermissionTypes)value;
            var array = parameter as Array;
            foreach (var item in array)
            {
                if (item is Equipment.UserPermissionTypes)
                {
                    var targetPermission = (Equipment.UserPermissionTypes)item;
                    if (targetPermission == userPermission) return true;
                }
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return Equipment.UserPermissionTypes.OPERATOR;
        }
    }
}
