using System.Collections.Generic;

namespace Tracer.Serialization.Yaml
{
    public class YamlTraceResultDto
    {
        public List<YamlThreadDto> Threads { get; set; }
    }

    public class YamlThreadDto
    {
        public int Id { get; set; }
        public string Time { get; set; }
        public List<YamlMethodDto> Methods { get; set; }
    }

    public class YamlMethodDto
    {
        public string Name { get; set; }
        public string Class { get; set; }
        public string Time { get; set; }
        public List<YamlMethodDto> Methods { get; set; }
    }
}
