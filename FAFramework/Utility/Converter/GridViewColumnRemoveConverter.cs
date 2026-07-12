using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls;

namespace FAFramework.Utility.Converter
{
    public class GridViewColumnRemoveConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if ((value is GridView) == false) return null;
            if (parameter == null) return value;
            if ((parameter is string[]) == false) return value;

            string[] parameters = parameter as string[];
            var gridview = (GridView)value;

            foreach (var item in parameters)
            {
                RemoveColumn(gridview, item);
            }

            return gridview;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static void RemoveColumn(GridView gridView, string columnName)
        {
            GridViewColumn column = null;

            if (gridView == null) return;

            foreach (var item in gridView.Columns)
            {
                if (item.Header.ToString() == columnName)
                    column = item;
            }

            if (column != null)
                gridView.Columns.Remove(column);
        }
    }
}
