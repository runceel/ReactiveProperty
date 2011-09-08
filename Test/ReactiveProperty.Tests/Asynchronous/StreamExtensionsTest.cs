using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Codeplex.Reactive.Notifier;
using Codeplex.Reactive.Asynchronous;
using System.Reactive.Linq;
using System.Reactive;
using Microsoft.Reactive.Testing;

namespace ReactiveProperty.Tests.Asynchronous
{
    [TestClass]
    public class StreamExtensionsTest : ReactiveTest
    {
        const string TestString = @"
てすとすとりんぐaiueo迂回四回三十回
開業改行    タブ スペース ワープ
horaana john.";

        [TestMethod]
        public void ReadAsObservable()
        {
            var bytes = Encoding.UTF8.GetBytes(TestString);

            var buffer = new byte[bytes.Length];
            using (var stream = new MemoryStream(bytes))
            {
                var result = stream.ReadAsObservable(buffer, 0, buffer.Length).ToEnumerable().ToArray();

                result.Length.Is(1);
                result[0].Is(bytes.Length);
                buffer.Is(bytes);
            }
        }

        [TestMethod]
        public void WriteAsObservable()
        {
            var bytes = Encoding.UTF8.GetBytes(TestString);
            using (var stream = new MemoryStream())
            {
                var result = stream.WriteAsObservable(bytes, 0, bytes.Length).ToEnumerable().ToArray();

                result.Length.Is(1);
                stream.ToArray().Is(bytes);
            }
        }

        [TestMethod]
        public void WriteAsync()
        {
            var shitfjis = Encoding.GetEncoding("Shift-JIS");
            var bytes = Encoding.UTF8.GetBytes(TestString);

            using (var stream = new MemoryStream())
            {
                stream.WriteAsync(TestString).Single().Is(Unit.Default);
                Encoding.UTF8.GetString(stream.ToArray()).Is(TestString);
            }

            using (var stream = new MemoryStream())
            {
                stream.WriteAsync(TestString, shitfjis).Single().Is(Unit.Default);
                shitfjis.GetString(stream.ToArray()).Is(TestString);
            }

            using (var stream = new MemoryStream())
            {
                var r = stream.WriteAsync(bytes, 3).ToEnumerable().ToArray();
                r.Length.Is(1);
                stream.ToArray().Is(bytes);
            }

            using (var stream = new MemoryStream())
            {
                var r = stream.WriteAsync(bytes.ToObservable()).ToEnumerable().ToArray();
                r.Length.Is(1);
                stream.ToArray().Is(bytes);
            }
        }

        [TestMethod]
        public void WriteAsyncWithProgress()
        {
            var shitfjis = Encoding.GetEncoding("Shift-JIS");
            var bytes = Encoding.UTF8.GetBytes(TestString); // length = 114

            var testScheduler = new TestScheduler();
            var recorder = testScheduler.CreateObserver<ProgressStatus>();
            var notifier = new ScheduledNotifier<ProgressStatus>();
            notifier.Subscribe(recorder);

            using (var stream = new MemoryStream())
            {
                stream.WriteAsync(TestString, notifier).ToEnumerable().ToArray().Length.Is(1);
                Encoding.UTF8.GetString(stream.ToArray()).Is(TestString);

                recorder.Messages.Count.Is(2);
                recorder.Messages[0].Value.Value.Is(x =>
                    x.CurrentLength == 0
                    && x.TotalLength == bytes.Length
                    && x.Percentage == 0);

                recorder.Messages[1].Value.Value.Is(x =>
                    x.CurrentLength == bytes.Length
                    && x.TotalLength == bytes.Length
                    && x.Percentage == 100);
            }

            recorder.Messages.Clear();
            using (var stream = new MemoryStream())
            {
                var shiftjisBytes = shitfjis.GetBytes(TestString);
                stream.WriteAsync(TestString, shitfjis, notifier).ToEnumerable().ToArray().Length.Is(1);
                shitfjis.GetString(stream.ToArray()).Is(TestString);

                recorder.Messages.Count.Is(2);
                recorder.Messages[0].Value.Value.Is(x =>
                    x.CurrentLength == 0
                    && x.TotalLength == shiftjisBytes.Length
                    && x.Percentage == 0);

                recorder.Messages[1].Value.Value.Is(x =>
                    x.CurrentLength == shiftjisBytes.Length
                    && x.TotalLength == shiftjisBytes.Length
                    && x.Percentage == 100);
            }

            recorder.Messages.Clear();
            using (var stream = new MemoryStream())
            {
                stream.WriteAsync(bytes, notifier, 40).ToEnumerable().ToArray().Length.Is(1);
                Encoding.UTF8.GetString(stream.ToArray()).Is(TestString);

                recorder.Messages.Count.Is(4);
                recorder.Messages[0].Value.Value.Is(x =>
                    x.CurrentLength == 0
                    && x.TotalLength == bytes.Length
                    && x.Percentage == 0);

                recorder.Messages[1].Value.Value.Is(x =>
                    x.CurrentLength == 40
                    && x.TotalLength == bytes.Length
                    && x.Percentage == 35);

                recorder.Messages[2].Value.Value.Is(x =>
                    x.CurrentLength == 80
                    && x.TotalLength == bytes.Length
                    && x.Percentage == 70);

                recorder.Messages[3].Value.Value.Is(x =>
                    x.CurrentLength == bytes.Length
                    && x.TotalLength == bytes.Length
                    && x.Percentage == 100);
            }

            recorder.Messages.Clear();
            using (var stream = new MemoryStream())
            {
                stream.WriteAsync(bytes.Select(x => x), notifier, 40).ToEnumerable().ToArray().Length.Is(1);
                Encoding.UTF8.GetString(stream.ToArray()).Is(TestString);

                recorder.Messages.Count.Is(4);
                recorder.Messages[0].Value.Value.Is(x =>
                    x.CurrentLength == 0
                    && x.TotalLength == -1
                    && x.Percentage == 0);

                recorder.Messages[1].Value.Value.Is(x =>
                    x.CurrentLength == 40
                    && x.TotalLength == -1
                    && x.Percentage == 0);

                recorder.Messages[2].Value.Value.Is(x =>
                    x.CurrentLength == 80
                    && x.TotalLength == -1
                    && x.Percentage == 0);

                recorder.Messages[3].Value.Value.Is(x =>
                    x.CurrentLength == bytes.Length
                    && x.TotalLength == -1
                    && x.Percentage == 0);
            }

            recorder.Messages.Clear();
            using (var stream = new MemoryStream())
            {
                stream.WriteAsync(bytes.ToObservable(), notifier, 40).ToEnumerable().ToArray().Length.Is(1);
                Encoding.UTF8.GetString(stream.ToArray()).Is(TestString);

                recorder.Messages.Count.Is(4);
                recorder.Messages[0].Value.Value.Is(x =>
                    x.CurrentLength == 0
                    && x.TotalLength == -1
                    && x.Percentage == 0);

                recorder.Messages[1].Value.Value.Is(x =>
                    x.CurrentLength == 40
                    && x.TotalLength == -1
                    && x.Percentage == 0);

                recorder.Messages[2].Value.Value.Is(x =>
                    x.CurrentLength == 80
                    && x.TotalLength == -1
                    && x.Percentage == 0);

                recorder.Messages[3].Value.Value.Is(x =>
                    x.CurrentLength == bytes.Length
                    && x.TotalLength == -1
                    && x.Percentage == 0);
            }
        }
    }
}
