using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace FAFramework.Utility
{
    public class PropertyValue : DependencyObject
    {
        public Type PropertyType { get; set; }
        public FALibrary.FARange Range { get; set; }
        public string Name { get; set; }

        public object Value
        {
            get { return GetValue(ValueProperty); }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(PropertyValue));

        public void SetObject(object rootObj, string rootPropertyPath, object obj, string propertyName)
        {
            var setMethod = obj.GetType().GetProperty(propertyName).GetSetMethod(false);

            Binding bd = new Binding(rootPropertyPath);
            bd.Source = rootObj;

            if (setMethod != null)
                bd.Mode = BindingMode.TwoWay;
            else
                bd.Mode = BindingMode.OneWay;

            BindingOperations.SetBinding(this, PropertyValue.ValueProperty, bd);

            PropertyType = obj.GetType().GetProperty(propertyName).PropertyType;
            if (obj is FALibrary.FAObject)
            {
                var faObject = obj as FALibrary.FAObject;
                if (faObject.RangeList.ContainsKey(propertyName) == true)
                    Range = faObject.RangeList[propertyName];
            }
        }
    }

    public class ReadOnlyPropertyValue : DependencyObject
    {
        public string Name { get; set; }

        public object Value
        {
            get { return GetValue(ValueProperty); }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(ReadOnlyPropertyValue));

        public void SetObject(object obj, string propertyName)
        {
            Binding bd = new Binding(propertyName);
            bd.Source = obj;
            bd.Mode = BindingMode.OneWay;
            BindingOperations.SetBinding(this, ReadOnlyPropertyValue.ValueProperty, bd);
        }
    }

    public class BooleanPropertyValue : PropertyValue
    {
    }

    public class ListPropertyValue
    {
        public string Name { get; set; }

        public object Value { get; set; }


        public GridView View
        {
            get
            {
                return GetView();
            }
        }

        public GridView GetView()
        {
            var result = new GridView();
            var list = Value as System.Collections.IList;
            if (list.Count > 0)
            {
                var propInfos = list[0].GetType().GetProperties();

                foreach (var info in propInfos)
                {
                    GridViewColumn column = new GridViewColumn();
                    column.Header = info.Name;
                    if (info.PropertyType == typeof(bool))
                    {
                        System.Windows.DataTemplate template = new System.Windows.DataTemplate();
                        System.Windows.FrameworkElementFactory checkBox =
                            new System.Windows.FrameworkElementFactory(typeof(CheckBox));
                        checkBox.SetValue(CheckBox.VerticalAlignmentProperty, System.Windows.VerticalAlignment.Center);

                        Binding bd = new Binding(info.Name);
                        if (info.GetSetMethod(false) != null)
                        {
                            bd.Mode = BindingMode.TwoWay;
                            checkBox.SetValue(CheckBox.IsEnabledProperty, true);
                        }
                        else
                        {
                            bd.Mode = BindingMode.OneWay;
                        }

                        checkBox.SetBinding(CheckBox.IsCheckedProperty, bd);
                        template.VisualTree = checkBox;
                        column.CellTemplate = template;
                    }
                    else if (info.PropertyType == typeof(short) ||
                        info.PropertyType == typeof(ushort) ||
                        info.PropertyType == typeof(int) ||
                        info.PropertyType == typeof(uint) ||
                        info.PropertyType == typeof(long) ||
                        info.PropertyType == typeof(ulong) ||
                        info.PropertyType == typeof(float) ||
                        info.PropertyType == typeof(double))
                    {
                        System.Windows.DataTemplate template = new System.Windows.DataTemplate();
                        System.Windows.FrameworkElementFactory textBox =
                            new System.Windows.FrameworkElementFactory(typeof(TextBox));
                        Binding bd = new Binding(info.Name);
                        if (info.GetSetMethod(false) != null)
                        {
                            bd.Mode = BindingMode.TwoWay;
                            textBox.SetValue(TextBox.IsEnabledProperty, true);
                        }
                        else
                        {
                            bd.Mode = BindingMode.OneWay;
                            textBox.SetValue(TextBox.IsEnabledProperty, false);
                        }

                        textBox.SetBinding(TextBox.TextProperty, bd);

                        template.VisualTree = textBox;
                        column.CellTemplate = template;
                        column.Width = double.NaN;
                    }
                    else
                        column.DisplayMemberBinding = new System.Windows.Data.Binding(info.Name);

                    result.Columns.Add(column);
                }
            }

            return result;
        }
    }

    public class EnumPropertyValue : DependencyObject
    {
        public string Name { get; set; }

        public object Value { get; set; }

        public object EnumNames { get; set; }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(EnumPropertyValue));

        public void SetObject(object rootObj, string rootPropertyPath, object obj, string propertyName)
        {
            var propInfo = obj.GetType().GetProperty(propertyName);
            EnumNames = propInfo.PropertyType.GetEnumNames();

            Binding bd = new Binding(rootPropertyPath);
            bd.Source = rootObj;

            if (propInfo.GetSetMethod(false) != null)
                bd.Mode = BindingMode.TwoWay;
            else
                bd.Mode = BindingMode.OneWay;

            BindingOperations.SetBinding(this, EnumPropertyValue.ValueProperty, bd);
        }
    }

    public class PropertyContainer
    {
        public string Name { get; set; }

        public object Value { get; set; }
    }

    public class ClickAction
    {
        public static Dictionary<ClickAction, ClickAction> Items = new Dictionary<ClickAction, ClickAction>();

        public string Name { get; set; }
        public Action<object, RoutedEventArgs> ClickMethod { get; set; }

        public ClickAction()
        {
            ClickAction.Items.Add(this, this);
        }
    }

    public class UpDownAction
    {
        public static Dictionary<UpDownAction, UpDownAction> Items = new Dictionary<UpDownAction, UpDownAction>();

        public string Name { get; set; }
        public Action<object, MouseButtonEventArgs> MouseDownMethod;
        public Action<object, MouseButtonEventArgs> MouseUpMethod;

        public UpDownAction()
        {
            UpDownAction.Items.Add(this, this);
        }
    }
}
