using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;

namespace FAFramework.GEM
{
    [DataContract]
    public class ALIDContainer
    {
        [DataMember]
        public ALIDDefine[] ALIDs { get; set; }

        public static ALIDContainer LoadFromCSVFile(string filename)
        {
            ALIDContainer container = new ALIDContainer();

            if (File.Exists(filename))
            {
                List<ALIDDefine> list = new List<ALIDDefine>();
                var lines = File.ReadAllLines(filename);
                foreach (var item in lines)
                {
                    var splitData = item.Split('\t');
                    if (splitData.Length >= 2)
                    {
                        int id;
                        if (int.TryParse(splitData[0], out id) == false) continue;
                        var name = splitData[1];
                        var alarmID = new ALIDDefine
                        {
                            ID = id,
                            Name = name
                        };

                        list.Add(alarmID);
                    }
                }

                container.ALIDs = list.ToArray();
            }

            return container;
        }
    }
}
