using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace FAFramework.Utility.Converter
{
    public class StateToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value == null) return Visibility.Collapsed;

            if ((value is Equipment.EquipmentState) == false) return Visibility.Collapsed;

            try
            {
                var state = value as Equipment.EquipmentState;
                string[] args = (parameter as string).Split(';');
                if (args.Length < 2) return Visibility.Collapsed;

                var logic = args[0].ToLower();
                var stateNames = new string[args.Length - 1];
                Array.Copy(args, 1, stateNames, 0, stateNames.Length);

                bool searched = false;
                foreach (var item in stateNames)
                {
                    if (state.Name == item)
                    {
                        searched = true;
                        break;
                    }
                }

                if (logic == "is")
                {
                    if (searched == true)
                        return Visibility.Visible;
                    else
                        return Visibility.Collapsed;
                }
                else if (logic == "isnot")
                {
                    if (searched == true)
                        return Visibility.Collapsed;
                    else
                        return Visibility.Visible;
                }
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
            return null;
        }
    }
}
