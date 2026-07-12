using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace FAFramework.Utility.Converter
{
    public class AlarmModulesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if ((value is string) == false) return "NONE";

            string type = (string)value;

            if (type == "SSDLoadingModule")
                return true;
            else if (type == "SSDPreStackModule")
                return true;
            else if (type == "SSDStackModule")
                return true;
            else if (type == "SSDTransferModule")
                return true;
            else if (type == "TraySupply1Module")
                return true;
            else if (type == "TraySupply2Module")
                return true;
            else if (type == "TraySupply3Module")
                return true;
            else if (type == "SSDFirstBandingMachineModule")
                return true;
            else if (type == "BandingTrayTurnModule")
                return true;
            else if (type == "SSDSecondBandingMachineModule")
                return true;
            else
                return false;

        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if ((value is string) == false) return -1;

            string typeName = ((string)value).ToUpper();

            if (typeName == "ALARM")
                return ConfigClasses.GlobalConst.ALARM;
            else if (typeName == "WARNING")
                return ConfigClasses.GlobalConst.WARNING;
            else
                return -1;
        }
    }
}
