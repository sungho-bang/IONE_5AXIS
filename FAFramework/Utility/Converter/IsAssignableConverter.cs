using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace FAFramework.Utility.Converter
{
    public class IsAssignableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var obj = value;
                var parameterText = parameter as string;
                if (obj == null || string.IsNullOrWhiteSpace(parameterText))
                    return false;

                var paramArr = parameterText.Split('/');
                var typeName = paramArr[0];
                var assembly = Assembly.GetExecutingAssembly();
                if (typeName.Split(':').Length > 1)
                {
                    var arr = typeName.Split(':');
                    assembly = Assembly.Load(arr[0]);
                    typeName = arr[1];
                }

                var type = assembly.GetType(typeName as string);
                if (type == null)
                    return false;

                var objType = obj.GetType();
                var result = type.IsAssignableFrom(objType);

                if (paramArr.Length > 1)
                {
                    var propertyInfo = paramArr[1].Split('=');
                    if (propertyInfo.Length == 2)
                    {
                        var propertyName = propertyInfo[0];
                        var property = objType.GetProperty(propertyName);
                        if (property == null)
                            return false;

                        var propertyValue = System.Convert.ChangeType(propertyInfo[1], property.PropertyType);
                        if (!object.Equals(propertyValue, property.GetValue(obj)))
                        {
                            result = false;
                        }
                    }
                }

                return result;
            }
            catch
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("IsAssignableConverter can only be used OneWay.");
        }
    }
}
