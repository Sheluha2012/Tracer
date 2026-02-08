using System;
using Tracer.Core;
using Tracer.Serialization;

class Program
    {
        static void Main(string[] args)
        {
            ITracer tracer = new Tracer.Core.Tracer();

            MethodA(tracer);

            var thread = new System.Threading.Thread(() =>
            {
                MethodA(tracer);
            });
            thread.Start();
            thread.Join();

            TraceResult result = tracer.GetTraceResult();
            PrintResult(result);
            var loader = new PluginLoader();
            var serializers = loader.Load("Plugins");

            var writer = new TraceResultWriter();
            writer.Write(result, serializers, "Output");

            Console.ReadKey();
        }

        static void MethodA(ITracer tracer)
        {
            tracer.StartTrace();
            MethodB(tracer);
            System.Threading.Thread.Sleep(100);
            tracer.StopTrace();
        }

        static void MethodB(ITracer tracer)
        {
            tracer.StartTrace();
            System.Threading.Thread.Sleep(200);
            tracer.StopTrace();
        }

        static void PrintResult(TraceResult result)
        {
            foreach (var thread in result.Threads)
            {
                Console.WriteLine($"Thread {thread.ThreadId}, time = {thread.ExecutionTime} ms");
                PrintMethods(thread.Methods, 1);
            }
        }

        static void PrintMethods(
            System.Collections.Generic.IReadOnlyList<MethodTraceResult> methods,
            int indent)
        {
            foreach (var method in methods)
            {
                Console.WriteLine(
                    $"{new string(' ', indent * 2)}" +
                    $"{method.ClassName}.{method.MethodName} - {method.ExecutionTime} ms"
                );

                PrintMethods(method.Methods, indent + 1);
            }
        }
    }