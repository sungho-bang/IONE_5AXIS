using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace FALibrary.Utility
{
    public class FAReflection
    {
        public static object GetPropertyValue(object obj, string propertyName)
        {
            if (obj == null) return null;

            PropertyInfo[] propList;
            propList = obj.GetType().GetProperties();

            foreach (PropertyInfo info in propList)
            {
                if (propertyName == info.Name)
                {
                    return info.GetValue(obj, null);
                }
            }

            return null;
        }

        public static void SetPropertyValue(object obj, string propertyName, object value)
        {
            if (obj == null) return;

            PropertyInfo[] propList;
            propList = obj.GetType().GetProperties();

            foreach (PropertyInfo info in propList)
            {
                if (propertyName == info.Name)
                {
                    info.SetValue(obj, value, null);
                    return;
                }
            }
        }

        public static Type GetPropertyType(object obj, string propertyName)
        {
            if (obj == null) return null;

            PropertyInfo[] propList;
            propList = obj.GetType().GetProperties();

            foreach (PropertyInfo info in propList)
            {
                if (propertyName == info.Name)
                {
                    return info.PropertyType;                    
                }
            }

            return null;
        }
    }
}
