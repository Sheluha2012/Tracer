using System.IO;
using System.Linq;
using Tracer.Core;
using Tracer.Serialization.Abstractions;
using YamlDotNet.Serialization;

namespace Tracer.Serialization.Yaml
{
    public class YamlTraceResultSerializer : ITraceResultSerializer
    {
        public string Format => "yaml";

        public void Serialize(TraceResult traceResult, Stream to)
        {
            var dto = new YamlTraceResultDto
            {
                Threads = traceResult.Threads.Select(t => new YamlThreadDto
                {
                    Id = t.ThreadId,
                    Time = $"{t.ExecutionTime}ms",
                    Methods = t.Methods.Select(MapMethod).ToList()
                }).ToList()
            };

            var serializer = new SerializerBuilder().Build();

            using (var writer = new StreamWriter(to))
            {
                serializer.Serialize(writer, dto);
            }
        }

        private YamlMethodDto MapMethod(MethodTraceResult method)
        {
            return new YamlMethodDto
            {
                Name = method.MethodName,
                Class = method.ClassName,
                Time = $"{method.ExecutionTime}ms",
                Methods = method.Methods.Select(MapMethod).ToList()
            };
        }
    }
}
