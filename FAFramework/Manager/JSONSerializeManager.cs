using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;

namespace FAFramework.Manager
{
    /// <summary>
    /// XmlSerializer를 관리하는 클래스
    /// 한 번 생성된 DataContractJsonSerializer 유지하고 있는다.
    /// </summary>
    public class JSONSerializeManager
    {
        private static Dictionary<Type, DataContractJsonSerializer> _xmlSerializers = new Dictionary<Type, DataContractJsonSerializer>();

        /// <summary>
        /// type에 해당되는 DataContractSerializer 반환한다.
        /// type에 해당되는 DataContractSerializer 없으면 생성해서 반환한다.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>type을 Serialize할 수 있는 DataContractSerializer</returns>
        public static DataContractJsonSerializer GetSerializer(Type type)
        {
            if (!_xmlSerializers.ContainsKey(type))
                _xmlSerializers.Add(type, new DataContractJsonSerializer(type));

            return _xmlSerializers[type];
        }
    }
}
