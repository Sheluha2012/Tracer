using System.Collections.Generic;
using System.IO;
using Tracer.Core;
using Tracer.Serialization.Abstractions;

namespace Tracer.Serialization
{
    public class TraceResultWriter
    {
        public void Write(
            TraceResult traceResult,
            IReadOnlyList<ITraceResultSerializer> serializers,
            string outputPath)
        {
            Directory.CreateDirectory(outputPath);

            foreach (var serializer in serializers)
            {
                var extension = serializer.Format ?? "txt";
                var filePath = Path.Combine(outputPath, $"result.{extension}");

                using (var file = File.Create(filePath))
                {
                    serializer.Serialize(traceResult, file);
                }
            }
        }
    }
}
