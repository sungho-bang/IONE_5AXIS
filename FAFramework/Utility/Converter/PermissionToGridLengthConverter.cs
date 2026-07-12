using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls;
using FAFramework.Equipment;
using FAFramework.Utility;

namespace FAFramework.Utility.Converter
{
    public class PermissionToGridLengthConverter : IValueConverter
    {
        private static GridLength _defaultGridLength = new GridLength(0);

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null) return _defaultGridLength;
                if ((value is UserPermissionTypes) == false) return _defaultGridLength;
                if (parameter == null) return _defaultGridLength;
                if ((parameter is ObjectKeyValuePair[]) == false) return _defaultGridLength;

                var permission = (UserPermissionTypes)value;

                var parameterArray = parameter as ObjectKeyValuePair[];
                var result = Array.Find(parameterArray,
                    x =>
                    {
                        if (x is ObjectKeyValuePair)
                        {
                            if (x.Key is UserPermissionTypes)
                            {
                                if ((UserPermissionTypes)x.Key == permission)
                                    return true;
                                else
                                    return false;
                            }
                            else
                                return false;
                        }
                        else
                            return false;
                    });

                if (result == null)
                    return _defaultGridLength;
                else
                    return (GridLength)result.Value;
            }
            catch
            {
                return _defaultGridLength;
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
