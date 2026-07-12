using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace FAFramework.Utility.Converter
{
    public class StateToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value == null) return false;

            if ((value is Equipment.EquipmentState) == false) return false;

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
                        return true;
                    else
                        return false;
                }
                else if (logic == "isnot")
                {
                    if (searched == true)
                        return false;
                    else
                        return true;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
