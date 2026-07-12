using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Data;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using FALibrary;
using System.Reflection;

namespace FAFramework.Utility
{
    public static class UtilityClass
    {
        public static string GetStringResource(object sender, string key, string defaultValue)
        {
            try
            {
                var resource = Application.Current.TryFindResource(key);
                if (resource != null)
                {
                    string result = resource as string;
                    return result;
                }
                else
                    return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        public static void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                var textBox = sender as TextBox;

                if ((textBox.Tag is Utility.PropertyValue) == false) return;
                var source = textBox.Tag as Utility.PropertyValue;
                if (source == null) return;
                if (source.PropertyType.IsPrimitive == false ||
                    source.PropertyType == typeof(bool) ||
                    source.PropertyType == typeof(string)) return;

                var binding = BindingOperations.GetBinding(textBox, TextBox.TextProperty);
                if (binding == null) return;

                if (binding.ValidationRules.Count > 1) return;

                var path = binding.Path.Path;

                Utility.WrappedFARange rule = new Utility.WrappedFARange();
                rule.NumberType = source.PropertyType;
                rule.Range = source.Range;
                binding.ValidationRules.Add(rule);
            }
            catch
            {
            }
        }

        public static void AddPartDefine(this object page, FALibrary.Part.FAPart part, Dictionary<string, PartDefineForManualOperation> partDefineList)
        {
            var partDefine = PartToPartDefineConvertor.Convert(part);
            partDefine.Name = GetPartName(part, partDefineList);
            partDefineList.Add(partDefine.Name, partDefine);
        }

        private static string GetPartName(FALibrary.Part.FAPart part, Dictionary<string, PartDefineForManualOperation> partDefineList)
        {
            if (string.IsNullOrEmpty(part.Name) == true)
            {
                int no = 0;

                while (true)
                {
                    string partName = "part_" + no.ToString();
                    if (partDefineList.ContainsKey(partName) == false)
                        return partName;

                    no++;
                }
            }
            else
            {
                return part.Name;
            }
        }

        public static Dictionary<string, string> SplitKeyValueData(string str)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            string pattern = "([^\\s]*=\"[^\"]*\")|([^\\s]*=[^\\s]*)";
            var matches = Regex.Matches(str, pattern);
            foreach (var item in matches)
            {
                if (item == null) continue;
                string[] keyValue = ParsingAttribute(item.ToString());
                if (keyValue == null) continue;

                if (result.ContainsKey(keyValue[0]) == false)
                    result.Add(keyValue[0], keyValue[1]);
            }

            return result;
        }

        public static string[] ParsingAttribute(string str)
        {
            Regex reg = new Regex("=");
            string[] splitResult = reg.Split(str, 2);
            if (splitResult == null || splitResult.Count() < 2)
                return null;

            return splitResult;
        }

        public static void BlockAltF4(Window window)
        {
            var command = new NoActionRoutedCommand();
            var keyBinding = new KeyBinding(command, Key.F4, ModifierKeys.Alt);
            window.InputBindings.Add(keyBinding);
        }

        public static XDocument Serialize(object value)
        {
            if (value == null) return null;

            try
            {
                XmlSerializer xmlSerializer = FALibrary.Utility.FAUtility.GetXmlSerializer(value.GetType());

                XDocument doc = new XDocument();
                using (var writer = doc.CreateWriter())
                {
                    XmlWriterSettings setting = new XmlWriterSettings();
                    setting.Indent = true;
                    setting.IndentChars = "  ";
                    setting.NewLineOnAttributes = true;
                    setting.OmitXmlDeclaration = true;

                    using (XmlWriter xw = XmlWriter.Create(writer, setting))
                        xmlSerializer.Serialize(xw, value);
                }

                return doc;
            }
            catch
            {
                return new XDocument();
            }
        }

        public static void WriteXElement(XmlWriter writer, IEnumerable<XElement> xel)
        {
            if (xel != null)
            {
                foreach (var item in xel)
                {
                    writer.WriteStartElement(item.Name.ToString());

                    if (item.HasElements == true)
                        WriteXElement(writer, item.Elements());
                    else
                    {
                        writer.WriteValue(item.Value);
                    }

                    writer.WriteEndElement();
                }
            }
        }

        public static XElement ObjectToXml(object obj)
        {
            if (obj is bool ||
                obj.GetType().IsPrimitive == true ||
                obj is string ||
                obj.GetType().IsEnum)
            {
                XElement result = new XElement(obj.GetType().Name);
                result.Value = obj.ToString();
                return result;
            }
            else if (obj is System.Collections.IList)
            {
                try
                {
                    XElement list = new XElement("List");

                    foreach (var item in obj as System.Collections.IList)
                    {
                        var xml = ObjectToXml(item);
                        if (xml != null)
                        {
                            var xel = new XElement("Item");
                            xel.Add(xml);
                            list.Add(xel);
                        }
                    }

                    return list;
                }
                catch
                {
                    return null;
                }
            }
            else if (obj is System.Collections.IDictionary)
            {
                XElement list = new XElement("Dictionary");
                var dic = obj as System.Collections.IDictionary;

                foreach (var key in dic.Keys)
                {
                    var xml = ObjectToXml(dic[key]);
                    if (xml != null)
                    {
                        try
                        {
                            var itemXml = new XElement("Item");
                            var keyXml = new XElement("Key");
                            keyXml.Value = key.ToString();

                            var valueXml = new XElement("Value");
                            valueXml.Add(xml);

                            itemXml.Add(keyXml);
                            itemXml.Add(valueXml);
                            list.Add(itemXml);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }

                return list;
            }
            else
            {
                try
                {
                    XElement x1 = new XElement(obj.GetType().Name);

                    foreach (var property in obj.GetType().GetProperties())
                    {
                        var faproperty = Utility.ObjectElementExtractor.GetFAPropertyAttribute(Attribute.GetCustomAttributes(property));
                        if (faproperty == null) continue;

                        object propertyValue = null;
                        try
                        {
                            propertyValue = property.GetValue(obj, null);
                        }
                        catch
                        {
                            continue;
                        }

                        if (propertyValue == null) continue;

                        var xmlResult = ObjectToXml(propertyValue);
                        if (xmlResult != null)
                        {
                            var propertyXml = new XElement(property.Name);
                            propertyXml.Add(xmlResult);
                            x1.Add(propertyXml);
                        }
                    }

                    return x1;
                }
                catch
                {
                    return null;
                }
            }
        }

        public static List<KeyValuePair<string, object>> GetFieldKeyValueList(Type type)
        {
            List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>();
            var fields = type.GetFields();
            return fields.Select(x => new KeyValuePair<string, object>(x.Name, x.GetValue(null))).ToList();
        }

        public static bool IsEqualString(string s1, string s2, bool trim, bool allowNull = false)
        {
            if (s1 == null || s2 == null) return false;

            if (trim)
            {
                s1 = s1.Trim();
                s2 = s2.Trim();
            }

            return s1 == s2;
        }

        public static IEnumerable<T> GetAllPropertiesValue<T>(object obj) where T : class
        {
            if (obj == null) return new List<T>();
            var properties = obj.GetType().GetProperties().Select(x => x);
            var list = properties.Where(x =>
                x.PropertyType == typeof(T) ||
                x.PropertyType.IsSubclassOf(typeof(T))).
                Select(x => x.GetValue(obj, null));

            if (list == null) return new List<T>();
            return list.Where(x => x != null).Select(x => x as T);
        }

        public static IEnumerable<FALibrary.Part.FAPart> GetAllParts(object obj)
        {
            return GetAllPropertiesValue<FALibrary.Part.FAPart>(obj);
        }

        public static IEnumerable<FALibrary.Part.FAPartAction> GetAllPartAction(this FALibrary.Part.FAPart part)
        {
            return GetAllPropertiesValue<FALibrary.Part.FAPartAction>(part);
        }

        public static IEnumerable<FALibrary.Sequence.FASequence> GetAllSequenceOfPart(this FALibrary.Part.FAPart part)
        {
            return part.GetAllPartAction().Select(x => x.Sequence);
        }

        /// <summary>
        /// Module에 존재하는 모든 파트의 Sequence.OnStart Event Handler를 추가한다.
        /// 해당 모듈에서 Sequence가 호출되었을 때 Sequence.MetaPropery("OwnerModule") = module이 되는 메소드
        /// </summary>
        /// <param name="module"></param>
        public static void SetOwnerModuleChangeOfPartSequence(this Module.FAModule module)
        {
            foreach (var part in UtilityClass.GetAllParts(module))
            {
                foreach (var seq in part.GetAllSequenceOfPart())
                {
                    if (seq == null) continue;
                    seq.OnStart +=
                            delegate
                            {
                                if (module.GetAllSequences().Contains(seq.Caller))
                                {
                                    seq.SetMetaPropertyValue("OwnerModule", module);
                                    seq.SetMetaPropertyValue("OwnerModuleName", module.Name);
                                }
                            };
                }
            }
        }

        public static ObjectPropertyInfo GetAllFAAttributePropertiesNames(this object obj,
            bool searchSubObject,
            params Type[] exceptTypes)
        {
            ObjectPropertyInfo objectPropertyInfo = new ObjectPropertyInfo();
            objectPropertyInfo.PropertyName = string.Empty;
            GetAllFAAttributeProperties(obj,
                objectPropertyInfo,
                new List<object>(),
                searchSubObject,
                exceptTypes);

            return objectPropertyInfo;
        }

        private static void GetAllFAAttributeProperties(object obj,
            ObjectPropertyInfo objectPropertyInfo,
            List<object> allObjects,
            bool searchSubObject,
            params Type[] exceptTypes)
        {
            if (obj == null) return;
            if (allObjects.Contains(obj)) return;

            allObjects.Add(obj);

            foreach (var property in obj.GetType().GetProperties())
            {
                var attr = Attribute.GetCustomAttributes(property);
                if (attr.Where(x => typeof(FALibrary.FAAttribute).IsAssignableFrom(x.GetType())).Count() == 0) continue;
                if (exceptTypes.Where(x => x.IsAssignableFrom(property.PropertyType)).Count() > 0) continue;
                if (typeof(System.Collections.IEnumerable).IsAssignableFrom(property.PropertyType)) continue;

                var name = property.Name;
                var value = property.GetValue(obj, null);

                var newObjectPropertyInfo = new ObjectPropertyInfo();
                newObjectPropertyInfo.PropertyName = name;
                newObjectPropertyInfo.Value = value;
                objectPropertyInfo.Properties.Add(newObjectPropertyInfo);

                if (typeof(FALibrary.FAObject).IsAssignableFrom(property.PropertyType))
                {
                    if (searchSubObject && value != null)
                    {
                        GetAllFAAttributeProperties(value,
                            newObjectPropertyInfo,
                            allObjects,
                            searchSubObject,
                            exceptTypes);
                    }
                }
            }
        }

        public static ObjectPropertyInfo GetAllPropertiesInfo(object obj, string filepath)
        {
            var objectPropertyInfo = obj.GetAllFAAttributePropertiesNames(true,
                typeof(FALibrary.Sequence.FASequence),
                typeof(FALibrary.Part.MemoryBasePart.FAMemoryBasePart));

            MergeObjectPropertyInfo(objectPropertyInfo, LoadAllPropertiesInfo(filepath));
            return objectPropertyInfo;
        }

        public static void SaveAllProperties(object obj, string filepath)
        {
            var objectPropertyInfo = obj.GetAllFAAttributePropertiesNames(true,
                typeof(FALibrary.Sequence.FASequence),
                typeof(FALibrary.Part.MemoryBasePart.FAMemoryBasePart));
            var doc = FALibrary.Utility.FAUtility.Serialize(objectPropertyInfo);
            doc.Save(filepath);
        }

        public static ObjectPropertyInfo LoadAllPropertiesInfo(string filepath)
        {
            System.Xml.Linq.XDocument doc = System.Xml.Linq.XDocument.Load(filepath);
            return FALibrary.Utility.FAUtility.Deserialize(doc, typeof(ObjectPropertyInfo)) as ObjectPropertyInfo;
        }

        private static void MergeObjectPropertyInfo(ObjectPropertyInfo source, ObjectPropertyInfo reference)
        {
            source.Observable = reference.Observable;
            source.Description = reference.Description;
            foreach (var property in source.Properties)
            {
                var result = reference.Properties.Where(x => x.PropertyName == property.PropertyName);
                if (result.Count() == 0) continue;
                MergeObjectPropertyInfo(property, result.First());
            }
        }

        public static FALibrary.Sequence.StepInfo AddStep(this FALibrary.Sequence.FASequence seq, string stepName)
        {
            try
            {
                seq.Steps.Add(stepName, new FALibrary.Sequence.StepInfo());
                return seq.Steps[stepName];
            }
            catch
            {
                return null;
            }
        }

        public static FALibrary.Sequence.StepInfo AddStep(this FALibrary.Sequence.FASequence seq, string stepName, int stepIndex)
        {
            try
            {
                seq.Steps.Add(stepName, new FALibrary.Sequence.StepInfo { StepIndex = stepIndex });
                return seq.Steps[stepName];
            }
            catch
            {
                return null;
            }
        }

        public static int AddItem(this FALibrary.Sequence.FASequence seq,
            params FALibrary.Part.FAPartAction[] partActions)
        {
            return seq.AddItem(partActions.Select(x => x.Sequence).ToArray<FALibrary.Sequence.FASequence>());
        }

        public static object GetPropertyValue(object source, string[] paths)
        {
            var sourceTemp = source;

            foreach (var path in paths)
            {
                var sourceType = sourceTemp.GetType();
                var propertyInfo = sourceType.GetProperty(path);
                if (propertyInfo == null)
                    return null;
                else
                    sourceTemp = propertyInfo.GetValue(sourceTemp);
            }

            return sourceTemp;
        }

        public static void SetDefaultValueAtProperty(this object obj)
        {
            PropertyInfo[] propList = obj.GetType().GetProperties();
            foreach (PropertyInfo info in propList)
            {
                var attribute = Attribute.GetCustomAttribute(info,
                    typeof(DefaultValueAttribute));
                if (attribute == null) continue;
                var hasDefaultValueAttribute = attribute as DefaultValueAttribute;
                info.SetValue(obj, hasDefaultValueAttribute.DefaultValue);
            }
        }
    }
}
