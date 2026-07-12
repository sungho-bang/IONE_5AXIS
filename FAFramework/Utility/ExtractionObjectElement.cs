using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Reflection;
using FALibrary;
using System.Text.RegularExpressions;
using System.Linq;

namespace FAFramework.Utility
{
    public class ExceptExtractProperty : Attribute
    {
    }

    public class ObjectElementExtractor
    {
        private class PropertyValueCreatorParam
        {
            public object RootObject { get; set; }
            public string ParentPropertyName { get; set; }
            public object Object { get; set; }
            public string PropertyName { get; set; }
            public Type PropertyType { get; set; }
            public object PropertyValue { get; set; }
            public PropertyInfo PropertyInfo { get; set; }
            public bool IncludeMethod { get; set; }
            public Type[] ExceptTypes { get; set; }
            public Type[] ExceptAttributeType { get; set; }
            public string[] AttributeNames { get; set; }
        }

        private static List<Func<PropertyValueCreatorParam, dynamic>> CreatePropertyValueFuncList { get; set; } = new List<Func<PropertyValueCreatorParam, dynamic>>
        {
            CreateBooleanPropertyValue,
            CreatePropertyValue,
            CreateListPropertyValue,
            CreatePropertyContainerOfDictionary,
            CreateEnumPropertyValue,
            CreateGeneralPropertyValue
        };

        private static bool IsHaveType(Type[] list, Type type)
        {
            foreach (var item in list)
            {
                if (item == type) return true;
            }

            return false;
        }

        public static List<object> ExtractElement(object root, string propertyName, bool includeMethod, Type[] exceptTypes, Type[] exceptAttributeType, string[] attributeNames)
        {
            Dictionary<string, object> objectReferences = new Dictionary<string, object>();
            List<object> result = new List<object>();

            object obj;
            if (string.IsNullOrEmpty(propertyName))
                obj = root;
            else
                obj = GetPropertyValue(root, propertyName);

            if (obj == null) return result;

            PropertyInfo[] propList = obj.GetType().GetProperties();

            foreach (var prop in propList)
            {
                if (exceptTypes != null)
                    if (IsHaveType(exceptTypes, prop.PropertyType) == true) continue;

                Attribute[] attr = Attribute.GetCustomAttributes(prop);

                if (exceptAttributeType != null)
                {
                    bool existExceptAttribute = Array.Exists(exceptAttributeType,
                        targetType => Array.Exists(attr, attribute => attribute.GetType() == targetType));
                    if (existExceptAttribute == true) continue;
                }

                FAAttribute faAttr = GetFAPropertyAttribute(attr);
                if (faAttr != null)
                {
                    if (attributeNames != null &&
                        Array.IndexOf(attributeNames, faAttr.GroupName) < 0) continue;

                    var value = prop.GetValue(obj, null);

                    dynamic addedNewContainer;
                    dynamic attributeGroup = GetGroup(faAttr.GroupName, ref objectReferences, out addedNewContainer);
                    if (addedNewContainer != null)
                        result.Add(addedNewContainer);

                    var param = new PropertyValueCreatorParam
                    {
                        RootObject = root,
                        ParentPropertyName = propertyName,
                        Object = obj,
                        PropertyName = prop.Name,
                        PropertyType = prop.PropertyType,
                        PropertyValue = value,
                        PropertyInfo = prop,
                        IncludeMethod = includeMethod,
                        ExceptTypes = exceptTypes,
                        ExceptAttributeType = exceptAttributeType,
                        AttributeNames = attributeNames
                    };

                    if (param.PropertyName == "MultiBarcodeLengthList")
                    {
                    }

                    dynamic propertyValue = CreatePropertyValueFuncList.Select(x => x(param)).Where(x => x != null).First();

                    if (attributeGroup == null)
                        result.Add(propertyValue);
                    else
                        attributeGroup.Value.Add(propertyValue);
                }
            }

            if (includeMethod)
                AddMethodInfo(obj, result);

            return result;
        }

        private static void AddMethodInfo(object obj, List<object> list)
        {
            MethodInfo[] methodList = obj.GetType().GetMethods();
            foreach (MethodInfo info in methodList)
            {
                Attribute[] attr = Attribute.GetCustomAttributes(info);
                foreach (Attribute at in attr)
                {
                    if (at is FAAttribute)
                    {
                        ParameterInfo[] pars = info.GetParameters();

                        if (pars.Length == 0 && info.ReturnType == typeof(void))
                        {
                            list.Add(MakeAction(obj, info, pars, attr));
                        }
                        else if (pars.Length > 0 &&
                            pars[0].ParameterType == typeof(object) &&
                            info.ReturnType == typeof(void))
                        {
                            list.Add(MakeAction(obj, info, pars, attr));
                        }
                    }
                }
            }
        }

        private static object MakeAction(object obj, MethodInfo info, ParameterInfo[] pars, Attribute[] attr)
        {
            object result;

            object methodOwner = obj;
            MethodInfo aMemberInfo = info;

            var pushButtonMethodAttribute = GetFirstFAAttributePushButtonMethod(attr);
            if (pushButtonMethodAttribute != null)
            {
                var upDownAction = new UpDownAction();
                upDownAction.Name = info.Name;

                object[] parameters = new object[1];
                parameters[0] = methodOwner;

                upDownAction.MouseDownMethod = new Action<object, MouseButtonEventArgs>(
                    delegate (object sender, MouseButtonEventArgs args)
                    {
                        if (methodOwner != null)
                        {
                            aMemberInfo.Invoke(methodOwner, parameters);
                        }
                    });

                MethodInfo upMethod = obj.GetType().GetMethod(pushButtonMethodAttribute.UpMethodName);
                if (upMethod.ReturnType == typeof(void) && upMethod.GetParameters().Length == 0)
                {
                    upDownAction.MouseUpMethod = new Action<object, MouseButtonEventArgs>(
                        delegate (object sender, MouseButtonEventArgs args)
                        {
                            if (methodOwner != null)
                            {
                                upMethod.Invoke(methodOwner, parameters);
                            }
                        });
                }
                else if (upMethod.GetParameters().Length > 0 &&
                        upMethod.GetParameters()[0].ParameterType == typeof(object) &&
                        upMethod.ReturnType == typeof(void))
                {
                    upDownAction.MouseUpMethod = delegate (object sender, MouseButtonEventArgs args)
                    {
                        if (methodOwner != null)
                        {
                            upMethod.Invoke(methodOwner, parameters);
                        }
                    };
                }

                result = upDownAction;
            }
            else
            {
                var clickAction = new ClickAction();
                clickAction.Name = info.Name;

                dynamic parameters;

                if (pars.Length > 0)
                {
                    parameters = new object[pars.Length];
                    for (int i = 0; i < pars.Length; i++)
                        parameters[i] = null;
                }
                else
                {
                    parameters = null;
                }

                clickAction.ClickMethod = delegate (object sender, RoutedEventArgs args)
                {
                    if (methodOwner != null)
                    {
                        aMemberInfo.Invoke(methodOwner, parameters);
                    }
                };

                result = clickAction;
            }

            return result;
        }

        private static FAAttributePushButtonMethod GetFirstFAAttributePushButtonMethod(Attribute[] attr)
        {
            foreach (var item in attr)
            {
                if (item is FAAttributePushButtonMethod)
                    return (FAAttributePushButtonMethod)item;
            }

            return null;
        }

        public static FAAttribute GetFAPropertyAttribute(Attribute[] array)
        {
            foreach (var item in array)
            {
                if (item is FAAttribute)
                    return (FAAttribute)item;
            }

            return null;
        }

        public static string ConcatPropertyNames(string pre, string post)
        {
            if (string.IsNullOrEmpty(pre) == true)
                return post;
            else
                return string.Concat(pre, '.', post);
        }

        public static object GetPropertyValue(object obj, string path)
        {
            string[] splitPath = path.Split('.');

            try
            {
                object propertyValue = obj;
                foreach (var propertyName in splitPath)
                {
                    propertyValue = propertyValue.GetType().GetProperty(propertyName).GetValue(propertyValue, null);
                    if (propertyValue == null) return null;
                }

                return propertyValue;
            }
            catch
            {
                return null;
            }
        }

        private static dynamic GetGroup(string groupName, ref Dictionary<string, object> objectReferences, out dynamic addedNewContainer)
        {
            dynamic attributeGroup = null;
            addedNewContainer = null;

            if (string.IsNullOrEmpty(groupName) == false)
            {
                var splits = Regex.Split(groupName, "\\.");

                if (objectReferences.ContainsKey(splits[0]))
                {
                    attributeGroup = objectReferences[splits[0]];
                }
                else
                {
                    attributeGroup = new PropertyContainer();
                    attributeGroup.Value = new List<object>();
                    attributeGroup.Name = splits[0];
                    objectReferences.Add(splits[0], attributeGroup);
                    addedNewContainer = attributeGroup;
                }

                PropertyContainer subGroup = null;
                for (int i = 1; i < splits.Length; i++)
                {
                    var subGroupName = string.Join(".", splits, 0, i + 1);
                    if (objectReferences.ContainsKey(subGroupName))
                        attributeGroup = objectReferences[subGroupName];
                    else
                    {
                        subGroup = new PropertyContainer();
                        subGroup.Value = new List<object>();
                        subGroup.Name = splits[i];
                        var list = attributeGroup.Value as List<object>;
                        list.Add(subGroup);
                        attributeGroup = subGroup;
                        objectReferences.Add(subGroupName, attributeGroup);
                    }
                }
            }

            return attributeGroup;
        }

        private static dynamic CreateBooleanPropertyValue(PropertyValueCreatorParam param)
        {
            if (param.PropertyType == typeof(bool))
            {
                var propertyValue = new BooleanPropertyValue();
                propertyValue.Name = param.PropertyName;
                propertyValue.SetObject(param.RootObject, ConcatPropertyNames(param.ParentPropertyName,
                    param.PropertyName), param.Object, param.PropertyName);
                return propertyValue;
            }
            else
                return null;
        }

        private static dynamic CreatePropertyValue(PropertyValueCreatorParam param)
        {
            if (param.PropertyType.IsPrimitive || param.PropertyType == typeof(string))
            {
                if (param.PropertyInfo.GetSetMethod(false) == null)
                {
                    var propertyValue = new ReadOnlyPropertyValue();
                    propertyValue.Name = param.PropertyName;
                    propertyValue.SetObject(param.RootObject, ConcatPropertyNames(param.ParentPropertyName,
                        param.PropertyName));
                    return propertyValue;
                }
                else
                {
                    var propertyValue = new PropertyValue();
                    propertyValue.Name = param.PropertyName;
                    propertyValue.SetObject(param.RootObject,
                        ConcatPropertyNames(param.ParentPropertyName,
                            param.PropertyName),
                        param.Object,
                        param.PropertyName);
                    return propertyValue;
                }
            }
            else
                return null;
        }

        private static dynamic CreateListPropertyValue(PropertyValueCreatorParam param)
        {
            if (param.PropertyValue is System.Collections.IList)
            {
                var propertyValue = new ListPropertyValue();
                propertyValue.Name = param.PropertyName;
                propertyValue.Value = param.PropertyValue;
                return propertyValue;
            }
            else
                return null;
        }

        private static dynamic CreatePropertyContainerOfDictionary(PropertyValueCreatorParam param)
        {
            if (param.PropertyValue is System.Collections.IDictionary)
            {
                var propertyValue = new PropertyContainer();
                propertyValue.Name = param.PropertyName;

                List<object> propertyValuesList = new List<object>();
                foreach (System.Collections.DictionaryEntry item in ((System.Collections.IDictionary)param.PropertyValue))
                {
                    PropertyContainer subPropertyValue = new PropertyContainer();
                    subPropertyValue.Name = item.Key.ToString();
                    subPropertyValue.Value = ExtractElement(item.Value,
                        string.Empty,
                        param.IncludeMethod,
                        param.ExceptTypes,
                        param.ExceptAttributeType,
                        param.AttributeNames);
                    propertyValuesList.Add(subPropertyValue);
                }

                propertyValue.Value = propertyValuesList;

                return propertyValue;
            }
            else
                return null;
        }

        private static dynamic CreateEnumPropertyValue(PropertyValueCreatorParam param)
        {
            if (param.PropertyType.IsEnum)
            {
                var propertyValue = new EnumPropertyValue();
                propertyValue.Name = param.PropertyName;
                propertyValue.Value = param.PropertyValue;
                propertyValue.SetObject(param.RootObject,
                    ConcatPropertyNames(param.ParentPropertyName, param.PropertyName),
                    param.Object,
                    param.PropertyName);

                return propertyValue;
            }
            else
                return null;
        }

        private static dynamic CreateGeneralPropertyValue(PropertyValueCreatorParam param)
        {
            var list = ExtractElement(param.RootObject,
                ConcatPropertyNames(param.ParentPropertyName, param.PropertyName),
                param.IncludeMethod,
                null,
                param.ExceptAttributeType,
                param.AttributeNames);

            if (list.Count > 0)
            {
                var propertyValue = new PropertyContainer();
                propertyValue.Name = param.PropertyName;
                propertyValue.Value = list;
                return propertyValue;
            }
            else
            {
                var propertyValue = new ReadOnlyPropertyValue();
                propertyValue.Name = param.PropertyName;
                propertyValue.SetObject(param.RootObject,
                    ConcatPropertyNames(param.ParentPropertyName, param.PropertyName));
                return propertyValue;
            }
        }
    }
}
