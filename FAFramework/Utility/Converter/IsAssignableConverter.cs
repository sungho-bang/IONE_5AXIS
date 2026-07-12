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
                var paramArr = (parameter as string).Split('/');
                var typeName = paramArr[0];
                var assembly = Assembly.GetExecutingAssembly();
                if (typeName.Split(':').Length > 1)
                {
                    var arr = typeName.Split(':');
                    assembly = Assembly.Load(arr[0]);
                    typeName = arr[1];
                }

                var type = assembly.GetType(typeName as string);
                var result = type.IsAssignableFrom(obj.GetType());

                if (paramArr.Length > 1)
                {
                    var propertyInfo = paramArr[1].Split('=');
                    if (propertyInfo.Length == 2)
                    {
                        var propertyName = propertyInfo[0];
                        var property = obj.GetType().GetProperty(propertyName);
                        var propertyValue = System.Convert.ChangeType(propertyInfo[1], property.PropertyType);
                        if (property != null)
                        {
                            if (!ValueType.Equals(propertyValue,
                                property.GetValue(obj)))
                            {
                                result = false;
                            }
                        }
                    }
                }

                return result;
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("IsAssignableConverter can only be used OneWay.");
        }
    }
}
