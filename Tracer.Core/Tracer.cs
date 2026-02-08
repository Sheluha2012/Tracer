using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Tracer.Core
{
    public class Tracer : ITracer
    { 
        private class MethodInfoInternal
        {
            public string MethodName { get; }
            public string ClassName { get; }
            public Stopwatch Stopwatch { get; } = new Stopwatch();
            public List<MethodInfoInternal> Children { get; } = new List<MethodInfoInternal>();

            public MethodInfoInternal(string methodName, string className)
            {
                MethodName = methodName;
                ClassName = className;
            }
        }

        private class ThreadInfoInternal
        {
            public int ThreadId { get; }
            public Stack<MethodInfoInternal> CallStack { get; } = new Stack<MethodInfoInternal>();
            public List<MethodInfoInternal> RootMethods { get; } = new List<MethodInfoInternal>();

            public ThreadInfoInternal(int threadId)
            {
                ThreadId = threadId;
            }
        }

        private readonly ConcurrentDictionary<int, ThreadInfoInternal> _threads = new ConcurrentDictionary<int, ThreadInfoInternal>();

        public void StartTrace()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            var threadInfo = _threads.GetOrAdd(threadId, id => new ThreadInfoInternal(id));
            var stackTrace = new StackTrace();
            var frame = stackTrace.GetFrame(1);
            var method = frame.GetMethod();

            var methodInfo = new MethodInfoInternal(method.Name, method.DeclaringType?.Name ?? "Unknown");

            if (threadInfo.CallStack.Count > 0)
            {
                // вложенный метод
                threadInfo.CallStack.Peek().Children.Add(methodInfo);
            }
            else
            {
                // корневой метод
                threadInfo.RootMethods.Add(methodInfo);
            }

            threadInfo.CallStack.Push(methodInfo);
            methodInfo.Stopwatch.Start();
        }


        public void StopTrace()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            if (!_threads.TryGetValue(threadId, out var threadInfo))
            {
                // Если поток не зарегистрирован — ничего не делаем
                return;
            }

            if (threadInfo.CallStack.Count == 0)
            {
                // Стек пуст — некорректный вызов StopTrace
                return;
            }

            // Достаём метод из стека
            var methodInfo = threadInfo.CallStack.Pop();

            // Останавливаем таймер
            methodInfo.Stopwatch.Stop();
        }

        private MethodTraceResult BuildMethodResult(MethodInfoInternal method)
        {
            var childResults = new List<MethodTraceResult>();
            long childTime = 0;

            foreach (var child in method.Children)
            {
                var childResult = BuildMethodResult(child);
                childResults.Add(childResult);
                childTime += childResult.ExecutionTime;
            }

            return new MethodTraceResult(
                method.MethodName,
                method.ClassName,
                method.Stopwatch.ElapsedMilliseconds,
                childResults
            );
        }

        public TraceResult GetTraceResult()
        {
            var threadResults = new List<ThreadTraceResult>();

            foreach (var threadPair in _threads)
            {
                var threadInfo = threadPair.Value;

                var methods = new List<MethodTraceResult>();
                long totalTime = 0;

                foreach (var rootMethod in threadInfo.RootMethods)
                {
                    var methodResult = BuildMethodResult(rootMethod);
                    methods.Add(methodResult);
                    totalTime += methodResult.ExecutionTime;
                }

                threadResults.Add(
                    new ThreadTraceResult(
                        threadInfo.ThreadId,
                        totalTime,
                        methods
                    )
                );
            }

            return new TraceResult(threadResults);
        }

    }
}

