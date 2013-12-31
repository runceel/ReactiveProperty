using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Codeplex.Reactive;
using System.Threading;

namespace ReactiveProperty.Tests
{
    [TestClass]
    public class Init
    {
        [AssemblyInitialize]
        public static void Initialize(TestContext context)
        {
            ServicePointManager.Expect100Continue = false;
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
        }
    }
}
