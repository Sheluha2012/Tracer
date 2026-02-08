using System.Collections.Generic;
using System.Xml.Serialization;

namespace Tracer.Serialization.Xml
{
    [XmlRoot("root")]
    public class XmlTraceResultDto
    {
        [XmlElement("thread")]
        public List<XmlThreadDto> Threads { get; set; }
    }

    public class XmlThreadDto
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("time")]
        public string Time { get; set; }

        [XmlElement("method")]
        public List<XmlMethodDto> Methods { get; set; }
    }

    public class XmlMethodDto
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("class")]
        public string Class { get; set; }

        [XmlAttribute("time")]
        public string Time { get; set; }

        [XmlElement("method")]
        public List<XmlMethodDto> Methods { get; set; }
    }
}
