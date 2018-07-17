using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveProperty.Tests
{
    [TestClass]
    public class AwaiterTest
    {
        [TestMethod]
        public async Task AwaitProperty()
        {
            IReactiveProperty<int> prop = new ReactiveProperty<int>();
            prop.Value = 999;

            var __ = Task.Delay(1000).ContinueWith(_ => prop.Value = 1000);

            var a = await prop;

            a.Is(1000);
        }

        [TestMethod]
        public async Task AwaitCmd()
        {
            ReactiveCommand<int> cmd = new ReactiveCommand<int>();

            var __ = Task.Delay(1000).ContinueWith(_ => cmd.Execute(9999));

            var a = await cmd;

            a.Is(1000);
        }
    }
}
