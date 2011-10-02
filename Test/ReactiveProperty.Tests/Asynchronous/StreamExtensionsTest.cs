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
                var r = stream.WriteAsync("").ToEnumerable().ToArray();
                r.Length.Is(1);
                stream.ToArray().Is(new byte[0]);
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
        }

        [TestMethod]
        public void WriteLineAsync()
        {
            using (var stream = new MemoryStream())
            {
                stream.WriteLineAsync(new[] { "foo", "bar", "baz" }).Single().Is(Unit.Default);
                Encoding.UTF8.GetString(stream.ToArray()).Is(@"foo
bar
baz
");
            }

            var shiftjis = Encoding.GetEncoding("shift-jis");

            using (var stream = new MemoryStream())
            {
                stream.WriteLineAsync(new[] { "foo", "bar", "baz" }, shiftjis).Single().Is(Unit.Default);
                shiftjis.GetString(stream.ToArray()).Is(@"foo
bar
baz
");
            }
        }

        [TestMethod]
        public void ReadAsync()
        {
            var bytes = Encoding.UTF8.GetBytes(TestString); // length = 114

            using (var ms = new MemoryStream(bytes))
            {
                var xs = ms.ReadAsync(5, isAggregateAllChunks: true).Single();
                xs.Is(bytes);
            }

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes("")))
            {
                var xs = ms.ReadAsync(isAggregateAllChunks: false).ToEnumerable().ToArray();
                xs.Length.Is(0);
            }

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes("")))
            {
                var xs = ms.ReadAsync(isAggregateAllChunks: true).ToEnumerable().ToArray();
                xs.Length.Is(1);
                xs[0].Is(new byte[0]);
            }

            using (var ms = new MemoryStream(bytes))
            {
                var xs = ms.ReadAsync(30, isAggregateAllChunks: false).ToEnumerable().ToArray();
                xs.Length.Is(4);
                xs[0].Is(bytes.Take(30));
                xs[1].Is(bytes.Skip(30).Take(30));
                xs[2].Is(bytes.Skip(60).Take(30));
                xs[3].Is(bytes.Skip(90).Take(30));
            }

            var testScheduler = new TestScheduler();
            var recorder = testScheduler.CreateObserver<ProgressStatus>();
            var notifier = new ScheduledNotifier<ProgressStatus>();
            notifier.Subscribe(recorder);

            using (var ms = new MemoryStream(bytes))
            {
                ms.ReadAsync(notifier, bytes.Length, 40).Single().Is(bytes);
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
            using (var ms = new MemoryStream(bytes))
            {
                var xs = ms.ReadAsync(notifier, bytes.Length, 40, isAggregateAllChunks: false).ToEnumerable().ToArray();
                xs.Length.Is(3);
                xs[0].Is(bytes.Take(40));
                xs[1].Is(bytes.Skip(40).Take(40));
                xs[2].Is(bytes.Skip(80).Take(40));

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
            using (var ms = new MemoryStream())
            {
                var xs = ms.ReadAsync(notifier, 0, isAggregateAllChunks: false).ToEnumerable().ToArray();
                xs.Length.Is(0);

                recorder.Messages.Count.Is(1);
                recorder.Messages[0].Value.Value.Is(x =>
                    x.CurrentLength == 0
                    && x.TotalLength == 0
                    && x.Percentage == 0);
            }
        }

        [TestMethod]
        public void ReadLineAsync()
        {
            var strings = TestString.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes("")))
            {
                var xs = ms.ReadLineAsync().ToEnumerable().ToArray();
                xs.Length.Is(0);
            }

            using (var ms = new MemoryStream(new byte[] { 239, 187, 191 }
                .Concat(Encoding.UTF8.GetBytes("test")).ToArray()))
            {
                var xs = ms.ReadLineAsync().ToEnumerable().ToArray();
                xs.Is("test");
            }

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(TestString)))
            {
                ms.ReadLineAsync(3).ToEnumerable().Is(strings);
            }

            var sjis = Encoding.GetEncoding("Shift-JIS");
            using (var ms = new MemoryStream(sjis.GetBytes(TestString)))
            {
                ms.ReadLineAsync(sjis, 3).ToEnumerable().Is(strings);
            }

            var utf8nobom = new UTF8Encoding(false);
            using (var ms = new MemoryStream(utf8nobom.GetBytes(TestString)))
            {
                ms.ReadLineAsync(utf8nobom, 3).ToEnumerable().Is(strings);
            }

            var str = "test1あ\rtぅぉest2\nteおいst3\r\ntなest4\n\r\nteめst5\r\n\r\n";
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(str)))
            {
                ms.ReadLineAsync(1).ToEnumerable().Is(
                    "test1あ",
                    "tぅぉest2",
                    "teおいst3",
                    "tなest4",
                    "",
                    "teめst5",
                    "");
            }
        }
    }
}