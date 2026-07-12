using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace FAFramework.Utility
{
    public static class VirtualKeyboardAttached
    {
        private static Point ZeroPoint = new Point(0, 0);
        public static readonly DependencyProperty InputBindingsProperty =
            DependencyProperty.RegisterAttached("InputBindings", typeof(InputBindingCollection), typeof(VirtualKeyboardAttached),
                new FrameworkPropertyMetadata(new InputBindingCollection(),
                (sender, e) =>
                {
                    var element = sender as UIElement;
                    if (element == null) return;

                    foreach (InputBinding item in (InputBindingCollection)e.NewValue)
                    {
                        element.CommandBindings.Add(
                            new CommandBinding(item.Command, ExecuteTextBoxClickCommand));
                    }

                    element.InputBindings.Clear();
                    element.InputBindings.AddRange((InputBindingCollection)e.NewValue);
                }));

        public static readonly DependencyProperty PasswordInputBindingsProperty =
            DependencyProperty.RegisterAttached("PasswordInputBindings", typeof(InputBindingCollection), typeof(VirtualKeyboardAttached),
                new FrameworkPropertyMetadata(new InputBindingCollection(),
                (sender, e) =>
                {
                    var element = sender as UIElement;
                    if (element == null) return;

                    foreach (InputBinding item in (InputBindingCollection)e.NewValue)
                    {
                        element.CommandBindings.Add(
                            new CommandBinding(item.Command, ExecutePasswordBoxClickCommand));
                    }

                    element.InputBindings.Clear();
                    element.InputBindings.AddRange((InputBindingCollection)e.NewValue);
                }));

        private static DependencyObject GetTopLevelControl(DependencyObject control)
        {
            DependencyObject tmp = control;
            DependencyObject parent = null;
            while ((tmp = VisualTreeHelper.GetParent(tmp)) != null)
            {
                parent = tmp;
            }
            return parent;
        }

        private static void ExecuteTextBoxClickCommand(object sender, ExecutedRoutedEventArgs e)
        {
            TextBox textBox = null;

            if (Equipment.MainEquipment.Instance.UseVirtualKeyboard == false)
            {
                if (e.OriginalSource == null) return;
                if ((e.OriginalSource is TextBox) == false) return;
                textBox = e.OriginalSource as TextBox;
                textBox.Focus();
                return;
            }

            if ((e.OriginalSource is TextBox) == false) return;
            if (e.OriginalSource == null) return;

            textBox = e.OriginalSource as TextBox;
            var textBoxPosition = textBox.PointToScreen(new Point(textBox.ActualWidth / 2, textBox.ActualHeight / 2));
            var topControl = GetTopLevelControl(textBox);

            bool isString;
            var rangeRule = GetNumberRangeRule(textBox, out isString);

            if (isString)
            {
                var keyboard = new GUI.KeyboardWindow();

                //keyboard.Left = textBoxPosition.X;
                //keyboard.Top = textBoxPosition.Y;
                keyboard.InputString = textBox.Text;

                if (topControl is Window)
                    keyboard.Owner = (Window)topControl;

                if (keyboard.ShowDialog() == true)
                {
                    textBox.Text = keyboard.InputString;
                    try
                    {
                        var bx = textBox.GetBindingExpression(TextBox.TextProperty);
                        bx.UpdateSource();
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.ToString());
                    }
                }
            }
            else
            {
                textBox.PointToScreen(ZeroPoint);

                var keyboard = new GUI.NumericKeyboardWindow();
                keyboard.Min = rangeRule.Range.Min;
                keyboard.Max = rangeRule.Range.Max;
                keyboard.NumType = rangeRule.NumberType;
                //keyboard.Left = textBoxPosition.X;
                //keyboard.Top = textBoxPosition.Y;
                keyboard.InputValueString = textBox.Text;

                if (topControl is Window)
                    keyboard.Owner = (Window)topControl;

                if (keyboard.ShowDialog() == true)
                {
                    textBox.Text = keyboard.Value.ToString();
                    try
                    {
                        var bx = textBox.GetBindingExpression(TextBox.TextProperty);
                        bx.UpdateSource();
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.ToString());
                    }
                }
            }
        }

        private static void ExecutePasswordBoxClickCommand(object sender, ExecutedRoutedEventArgs e)
        {
            PasswordBox textBox = null;

            if (Equipment.MainEquipment.Instance.UseVirtualKeyboard == false)
            {
                if (e.OriginalSource == null) return;
                if ((e.OriginalSource is PasswordBox) == false) return;
                textBox = e.OriginalSource as PasswordBox;
                textBox.Focus();
                return;
            }

            if ((e.OriginalSource is PasswordBox) == false) return;
            if (e.OriginalSource == null) return;

            textBox = e.OriginalSource as PasswordBox;
            var textBoxPosition = textBox.PointToScreen(new Point(textBox.ActualWidth / 2, textBox.ActualHeight / 2));
            var topControl = GetTopLevelControl(textBox);

            var keyboard = new GUI.KeyboardWindow();

            keyboard.Left = textBoxPosition.X;
            keyboard.Top = textBoxPosition.Y;
            keyboard.PasswordMode = true;

            if (topControl is Window)
                keyboard.Owner = (Window)topControl;

            if (keyboard.ShowDialog() == true)
            {
                textBox.Password = keyboard.InputString;
            }
        }

        public static InputBindingCollection GetInputBindings(UIElement element)
        {
            return (InputBindingCollection)element.GetValue(InputBindingsProperty);
        }

        public static void SetInputBindings(UIElement element, InputBindingCollection inputBindings)
        {
            element.SetValue(InputBindingsProperty, inputBindings);
        }

        public static InputBindingCollection GetPasswordInputBindings(UIElement element)
        {
            return (InputBindingCollection)element.GetValue(PasswordInputBindingsProperty);
        }

        public static void SetPasswordInputBindings(UIElement element, InputBindingCollection inputBindings)
        {
            element.SetValue(PasswordInputBindingsProperty, inputBindings);
        }

        private static Utility.FANumberRangeRule GetNumberRangeRule(TextBox textBox, out bool isString)
        {
            try
            {
                isString = false;
                var bindingEx = BindingOperations.GetBindingExpression(textBox, TextBox.TextProperty);
                object source;
                string propertyName;
                GetObjectAndProperty(bindingEx.DataItem, bindingEx.ParentBinding.Path.Path, out source, out propertyName);

                if (source == null || string.IsNullOrEmpty(propertyName)) return null;

                var propertyType = source.GetType().GetProperty(propertyName).PropertyType;
                if (propertyType.IsPrimitive == false)
                {
                    isString = true;
                    return null;
                }

                FANumberRangeRule rangeRule = new FANumberRangeRule(propertyType);

                var rangeListPropertyInfo = source.GetType().GetProperty("RangeList");
                var propertyValue = rangeListPropertyInfo.GetValue(source, null);
                if (propertyValue == null) return rangeRule;

                var rangeList = (FALibrary.Utility.SerializableDictionary<string, FALibrary.FARange>)propertyValue;
                if (rangeList == null) return rangeRule;

                if (rangeList.ContainsKey(propertyName))
                {
                    rangeRule = new FANumberRangeRule(propertyType);
                    rangeRule.Range = rangeList[propertyName];
                }

                return rangeRule;
            }
            catch
            {
                isString = true;
                return null;
            }
        }

        private static void GetObjectAndProperty(object source, string path, out object obj, out string propertyName)
        {
            obj = null;
            propertyName = string.Empty;

            object dataItem = source;
            string[] strs = path.Split('.');
            bool firstSearched = false;
            string property = string.Empty;
            foreach (var item in strs)
            {
                if (firstSearched)
                {
                    dataItem = dataItem.GetType().GetProperty(property).GetValue(dataItem, null);
                }
                else
                    firstSearched = true;

                property = dataItem.GetType().GetProperty(item).Name;
            }

            obj = dataItem;
            propertyName = property;
        }
    }
}
