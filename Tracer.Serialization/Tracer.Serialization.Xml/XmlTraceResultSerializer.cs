using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Tracer.Core;
using Tracer.Serialization.Abstractions;

namespace Tracer.Serialization.Xml
{
    public class XmlTraceResultSerializer : ITraceResultSerializer
    {
        public string Format => "xml";

        public void Serialize(TraceResult traceResult, Stream to)
        {
            var dto = new XmlTraceResultDto
            {
                Threads = traceResult.Threads.Select(t => new XmlThreadDto
                {
                    Id = t.ThreadId,
                    Time = $"{t.ExecutionTime}ms",
                    Methods = t.Methods.Select(MapMethod).ToList()
                }).ToList()
            };

            var serializer = new XmlSerializer(typeof(XmlTraceResultDto));
            serializer.Serialize(to, dto);
        }

        private XmlMethodDto MapMethod(MethodTraceResult method)
        {
            return new XmlMethodDto
            {
                Name = method.MethodName,
                Class = method.ClassName,
                Time = $"{method.ExecutionTime}ms",
                Methods = method.Methods.Select(MapMethod).ToList()
            };
        }
    }
}
