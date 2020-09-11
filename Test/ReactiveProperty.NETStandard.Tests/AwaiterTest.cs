using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings;

namespace ReactiveProperty.Tests
{
    [TestClass]
    public class AwaiterTest
    {
        [TestMethod]
        public async Task AwaitProperty()
        {
            IReactiveProperty<int> prop = new ReactiveProperty<int>
            {
                Value = 999
            };

            var __ = Task.Delay(1000).ContinueWith(_ => prop.Value = 1000);

            var a = await prop;

            a.Is(1000);
        }

        [TestMethod]
        public async Task AwaitCmd()
        {
            var cmd = new ReactiveCommand<int>();

            var __ = Task.Delay(1000).ContinueWith(_ => cmd.Execute(9999));

            var a = await cmd;

            a.Is(9999);
        }

        [TestMethod]
        public async Task AwaitPropertyHandler()
        {
            IReactiveProperty<int> prop = new ReactiveProperty<int>
            {
                Value = 999
            };

            using (var handler = prop.GetAsyncHandler(CancellationToken.None))
            {
                { var __ = Task.Delay(1000).ContinueWith(_ => prop.Value = 1000); }
                var v1 = await handler;
                v1.Is(1000);

                { var __ = Task.Delay(1000).ContinueWith(_ => prop.Value = 1001); }
                var v2 = await handler;
                v2.Is(1001);

                { var __ = Task.Delay(1000).ContinueWith(_ => prop.Value = 1002); }
                var v3 = await handler;
                v3.Is(1002);
            }
        }
    }
}
