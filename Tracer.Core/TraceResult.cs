using System.Collections.Generic;

namespace Tracer.Core
{
    public class TraceResult
    {
        public IReadOnlyList<ThreadTraceResult> Threads { get; }

        internal TraceResult(IReadOnlyList<ThreadTraceResult> threads)
        {
            Threads = threads;
        }
    }
    
    public class ThreadTraceResult
    {
        public int ThreadId { get; }
        public long ExecutionTime { get; }
        public IReadOnlyList<MethodTraceResult> Methods { get; }

        internal ThreadTraceResult(
            int threadId,
            long executionTime,
            IReadOnlyList<MethodTraceResult> methods)
        {
            ThreadId = threadId;
            ExecutionTime = executionTime;
            Methods = methods;
        }
    }
    
    public class MethodTraceResult
    {
        public string MethodName { get; }
        public string ClassName { get; }
        public long ExecutionTime { get; }
        public IReadOnlyList<MethodTraceResult> Methods { get; }

        internal MethodTraceResult(
            string methodName,
            string className,
            long executionTime,
            IReadOnlyList<MethodTraceResult> methods)
        {
            MethodName = methodName;
            ClassName = className;
            ExecutionTime = executionTime;
            Methods = methods;
        }
    }
}