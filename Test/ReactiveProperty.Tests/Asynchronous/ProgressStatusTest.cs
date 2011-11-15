using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Codeplex.Reactive.Notifiers;
using Codeplex.Reactive.Asynchronous;
using System.Reactive.Linq;
using System.Reactive;
using Microsoft.Reactive.Testing;

namespace ReactiveProperty.Tests.Asynchronous
{
    [TestClass]
    public class ProgressStatusTest : ReactiveTest
    {
        [TestMethod]
        public void Progress()
        {
            new ProgressStatus(0, 0).Is(x =>
                x.CurrentLength == 0
                && x.TotalLength == 0
                && x.Percentage == 0);

            var p = new ProgressStatus(20, 100);

            new ProgressStatus(20, 100).Is(x =>
                x.CurrentLength == 20
                && x.TotalLength == 100
                && x.Percentage == 20);

            new ProgressStatus(100, 300).Is(x =>
                x.CurrentLength == 100
                && x.TotalLength == 300
                && x.Percentage == 33);

            new ProgressStatus(500, 500).Is(x =>
                x.CurrentLength == 500
                && x.TotalLength == 500
                && x.Percentage == 100);

            new ProgressStatus(25, 50).ToString().Is("25/50 - 50%");
        }

    }
}
