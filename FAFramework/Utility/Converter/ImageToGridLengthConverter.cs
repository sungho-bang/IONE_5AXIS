using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;

namespace FAFramework.Utility.Converter
{
    public class ImageToGridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if ((value is ImageSource) == false) return new GridLength(0);
            if ((parameter is string) == false) return new GridLength(0);

            string[] parameters = (parameter as string).Split(';');

            if (value == null)
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
