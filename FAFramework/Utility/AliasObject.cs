using System.Windows;

namespace FAFramework.Utility
{
    public class AliasObject : FrameworkElement
    {
        public static readonly DependencyProperty AliasProperty =
            DependencyProperty.Register("Alias", typeof(string), typeof(AliasObject));

        public static readonly DependencyProperty ObjProperty =
            DependencyProperty.Register("Obj", typeof(object), typeof(AliasObject));

        public string Alias
        {
            get { return (string)GetValue(AliasProperty); }
            set
            {
                SetValue(AliasProperty, value);
            }
        }

        public object Obj
        {
            get { return GetValue(ObjProperty); }
            set
            {
                SetValue(ObjProperty, value);
            }
        }
    }
}
