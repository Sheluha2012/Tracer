using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tracer.Core;
using System.Threading;
using System.Diagnostics;

namespace Tracer.Core.Tests
{
    [TestClass]
    public class TracerTests
    {
        [TestMethod]
        public void SingleMethod_ShouldBeTraced()
        {
            ITracer tracer = new Tracer();

            tracer.StartTrace();
            Thread.Sleep(50);
            tracer.StopTrace();

            var result = tracer.GetTraceResult();

            Assert.AreEqual(1, result.Threads.Count);
            Assert.AreEqual(1, result.Threads[0].Methods.Count);
            Assert.IsTrue(result.Threads[0].Methods[0].ExecutionTime > 0);
        }

        [TestMethod]
        public void NestedMethods_ShouldBeTracedCorrectly()
        {
            ITracer tracer = new Tracer();

            tracer.StartTrace();
            Thread.Sleep(20);

            tracer.StartTrace();
            Thread.Sleep(30);
            tracer.StopTrace();

            tracer.StopTrace();

            var result = tracer.GetTraceResult();
            var method = result.Threads[0].Methods[0];

            Assert.AreEqual(1, method.Methods.Count);
            Assert.IsTrue(method.ExecutionTime >= method.Methods[0].ExecutionTime);
        }

        [TestMethod]
        public void MultipleRootMethods_ShouldBeOnSameLevel()
        {
            ITracer tracer = new Tracer();

            tracer.StartTrace();
            Thread.Sleep(10);
            tracer.StopTrace();

            tracer.StartTrace();
            Thread.Sleep(20);
            tracer.StopTrace();

            var result = tracer.GetTraceResult();

            Assert.AreEqual(2, result.Threads[0].Methods.Count);
        }

        [TestMethod]
        public void MethodsFromDifferentThreads_ShouldBeSeparated()
        {
            ITracer tracer = new Tracer();

            var thread = new Thread(() =>
            {
                tracer.StartTrace();
                Thread.Sleep(50);
                tracer.StopTrace();
            });

            thread.Start();
            thread.Join();

            tracer.StartTrace();
            Thread.Sleep(30);
            tracer.StopTrace();

            var result = tracer.GetTraceResult();

            Assert.AreEqual(2, result.Threads.Count);
        }

    }
}
