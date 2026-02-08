using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Tracer.Serialization.Abstractions;

namespace Tracer.Serialization
{
    public class PluginLoader
    {
        public IReadOnlyList<ITraceResultSerializer> Load(string path)
        {
            var result = new List<ITraceResultSerializer>();

            if (!Directory.Exists(path))
                return result;

            var dllFiles = Directory.GetFiles(path, "*.dll");

            foreach (var dll in dllFiles)
            {
                Assembly assembly;

                try
                {
                    assembly = Assembly.LoadFrom(dll);
                }
                catch
                {
                    continue;
                }

                var serializers = assembly.GetTypes()
                    .Where(t =>
                        typeof(ITraceResultSerializer).IsAssignableFrom(t) &&
                        !t.IsInterface &&
                        !t.IsAbstract);

                foreach (var type in serializers)
                {
                    if (Activator.CreateInstance(type) is ITraceResultSerializer serializer)
                    {
                        result.Add(serializer);
                    }
                }
            }

            return result;
        }
    }
}
