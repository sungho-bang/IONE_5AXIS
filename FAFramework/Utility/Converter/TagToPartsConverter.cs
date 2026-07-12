using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls;

namespace FAFramework.Utility.Converter
{
    public class TagToPartsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (values[0] == null)
                    return null;
                if (values[1] == null)
                    return null;

                if (!(values[0] is UIElementCollection))
                    return null;

                var children = values[0] as UIElementCollection;
                var source = values[1];

                List<string> partPaths = new List<string>();
                foreach (dynamic item in children)
                {
                    var partNames = (item.Tag as string).Split(';').Select(x => x.Trim());
                    foreach (var partName in partNames)
                    {
                        if (!partPaths.Contains(partName))
                            partPaths.Add(partName);
                    }
                }

                return partPaths.Select(x => UtilityClass.GetPropertyValue(source, x.Split('.'))).Where(x => x != null);
            }
            catch
            {
                return false;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
