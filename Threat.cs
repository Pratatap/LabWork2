using System;
using System.Collections.Generic;

namespace LabWork2._1
{
    public class Threat
    {
        public string[] info;

        public Threat(int gen)
        {
            info = new string[gen];
        }
        public Threat(string[] info)
        {
            this.info = info;
        }
        public class ShortThreatInfo
        {
            public string ID { get; set; }
            public string Name { get; set; }

            public ShortThreatInfo(string id, string name)
            {
                ID = id;
                Name = name;
            }
        }
        public class FullThreatInfo
        {
            public FullThreatInfo() { }
            public List<Tuple<string, string>> list = new List<Tuple<string, string>>();
            public FullThreatInfo(Tuple<string, string>[] data)
            {
                foreach (var item in data)
                {
                    list.Add(item);
                }
            }
        }
    }
}
