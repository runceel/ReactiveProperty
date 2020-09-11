using System.Reactive.Subjects;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings.Extensions;

namespace ReactiveProperty.Tests.Extensions
{
    [TestClass]
    public class InverseObservableExtensionsTest : ReactiveTest
    {
        [TestMethod]
        public void Inverse()
        {
            var s = new Subject<bool>();
            var obs = new TestScheduler().CreateObserver<bool>();
            s.Inverse().Subscribe(obs);
            s.OnNext(true);
            s.OnNext(false);

            obs.Messages.Is(
                OnNext(0, false),
                OnNext(0, true));
        }
    }
}
