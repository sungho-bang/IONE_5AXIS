using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace FAFramework.Utility
{
    public static class ConfigCreator
    {
        private class ExtractedInfo
        {
            public string SubUnitName { get; set; }
            public XElement PartList { get; set; }
            public XElement PartMetaDataList { get; set; }
        }

        public static void CreateConfig(Type equipmentType, string name)
        {
            foreach (var item in Extract(equipmentType))
            {
                var configPath = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "ExtractConfig",
                    name,
                    "config",
                    item.SubUnitName);

                if (!System.IO.Directory.Exists(configPath))
                    System.IO.Directory.CreateDirectory(configPath);

                var partlistPath = System.IO.Path.Combine(
                    configPath, "PartList.xml");

                item.PartList.Save(partlistPath);


                var metadataPath = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "ExtractConfig",
                    name,
                    "metadata",
                    item.SubUnitName);

                System.IO.Directory.CreateDirectory(metadataPath);

                var metadataFilepath = System.IO.Path.Combine(
                    metadataPath, "PartList.xml");

                item.PartMetaDataList.Save(metadataFilepath);
            }
        }

        private static List<ExtractedInfo> Extract(Type type)
        {
            var result = new List<ExtractedInfo>();

            foreach (var prop in type.GetProperties())
            {
                if (prop.PropertyType.IsSubclassOf(typeof(Equipment.SubUnitBase)) ||
                    prop.PropertyType == typeof(Equipment.SubUnitBase))
                {
                    CreatePartList(prop.PropertyType,
                        prop.Name,
                        out var partlist,
                        out var metadataList);
                    result.Add(
                        new ExtractedInfo
                        {
                            SubUnitName = prop.Name,
                            PartList = partlist,
                            PartMetaDataList = metadataList
                        });
                }
            }

            return result;
        }

        private static void CreatePartList(Type subUnitType,
            string unitName,
            out XElement partList,
            out XElement partMetadataList)
        {
            partList = new XElement("PartList");
            partMetadataList = new XElement("PartList");

            foreach (var prop in subUnitType.GetProperties())
            {
                if (prop.PropertyType.IsSubclassOf(typeof(FALibrary.Part.FAPart)) ||
                    prop.PropertyType == typeof(FALibrary.Part.FAPart))
                {
                    partList.Add(CreatePartXml(prop.Name, $"{unitName}.{prop.Name}"));
                    partMetadataList.Add(CreateXmlNode(prop.Name,
                        string.Join(";", GetAllBaseClassList(prop.PropertyType))));
                }
            }
        }

        private static XElement CreatePartXml(string name, string description)
        {
            var xml = new XElement("Part");
            xml.Add(CreateXmlNode("Name", name));
            xml.Add(CreateXmlNode("Description", description));
            xml.Add(CreateXmlNode("DeviceName", string.Empty));
            xml.Add(CreateXmlNode("Parameters", CreateXmlNode("Property", string.Empty)));

            return xml;
        }

        private static XElement CreateXmlNode(string name, object value)
        {
            var xml = new XElement(name);
            xml.Add(value);
            return xml;
        }

        private static string[] GetAllBaseClassList(Type type)
        {
            var list = new List<string>();

            while (type.BaseType != null)
            {
                list.Add(type.ToString());
                type = type.BaseType;
            }

            return list.ToArray();
        }
    }
}
