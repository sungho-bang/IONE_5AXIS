using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace FAFramework.Utility.Converter
{
    public class StringToGridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if ((value is string) == false) return new GridLength(0);
            if ((parameter is string) == false) return new GridLength(0);

            string[] parameters = (parameter as string).Split(';');

            string text = value as string;
            if (string.IsNullOrEmpty(text) == true)
                return new GridLength(0);
            else
            {
                double width;
                double.TryParse(parameters[0], out width);

                if (parameters[1].ToLower() == "star")
                    return new GridLength(width, GridUnitType.Star);
                else if (parameters[1].ToLower() == "pixel")
                    return new GridLength(width, GridUnitType.Pixel);
                else
                    return new GridLength(width, GridUnitType.Pixel);
            }
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
