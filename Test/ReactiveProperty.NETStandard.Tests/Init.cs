using System.Net;
using System.Reactive.Concurrency;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings;

namespace ReactiveProperty.Tests
{
    [TestClass]
    public class Init
    {
        [AssemblyInitialize]
        public static void Initialize(TestContext context)
        {
            ServicePointManager.Expect100Continue = false;
            ReactivePropertyScheduler.SetDefault(Scheduler.CurrentThread);
        }
    }
}
