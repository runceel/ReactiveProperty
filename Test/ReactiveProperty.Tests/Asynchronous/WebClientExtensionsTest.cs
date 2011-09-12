using System;
using System.ComponentModel;
using System.ComponentModel.Moles;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Moles;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Codeplex.Reactive.Asynchronous;
using Codeplex.Reactive.Notifier;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ReactiveProperty.Tests.Asynchronous
{
    [TestClass]
    public class WebClientExtensions : ReactiveTest
    {
        [TestMethod, HostType("Moles")]
        public void DownloadDataObservableAsync()
        {
            // Data
            var data = Enumerable.Range(1, 200).Select(i => (byte)i).ToArray();

            // Mole
            DownloadDataCompletedEventHandler completeHandler = (s, e) => { };
            MWebClient.AllInstances.DownloadDataCompletedAddDownloadDataCompletedEventHandler =
                (wc, h) => completeHandler += h;
            MWebClient.AllInstances.DownloadDataCompletedRemoveDownloadDataCompletedEventHandler =
                (wc, h) => completeHandler -= h;

            DownloadProgressChangedEventHandler progressHandler = (s, e) => { };
            MWebClient.AllInstances.DownloadProgressChangedAddDownloadProgressChangedEventHandler =
                (wc, h) => progressHandler += h;
            MWebClient.AllInstances.DownloadProgressChangedRemoveDownloadProgressChangedEventHandler =
                (wc, h) => progressHandler -= h;

            MAsyncCompletedEventArgs.AllInstances.ErrorGet = _ => null;
            MAsyncCompletedEventArgs.AllInstances.CancelledGet = _ => false;

            MWebClient.AllInstances.DownloadDataAsyncUri = (wc, uri) =>
            {
                // when error...
                if (uri.OriginalString == "http://error.com/")
                {
                    var d = new MDownloadDataCompletedEventArgs();
                    new MAsyncCompletedEventArgs(d) { ErrorGet = () => new WebException() };
                    completeHandler(wc, d);
                    return;
                }
                // cancel
                if (uri.OriginalString == "http://cancel.com/")
                {
                    var d = new MDownloadDataCompletedEventArgs();
                    new MAsyncCompletedEventArgs(d) { CancelledGet = () => true };
                    completeHandler(wc, d);
                    return;
                }

                // async...
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    progressHandler(wc, new MDownloadProgressChangedEventArgs
                    {
                        BytesReceivedGet = () => 0,
                        TotalBytesToReceiveGet = () => data.Length
                    });
                    progressHandler(wc, new MDownloadProgressChangedEventArgs
                    {
                        BytesReceivedGet = () => 100,
                        TotalBytesToReceiveGet = () => data.Length
                    });
                    progressHandler(wc, new MDownloadProgressChangedEventArgs
                    {
                        BytesReceivedGet = () => data.Length,
                        TotalBytesToReceiveGet = () => data.Length
                    });
                    completeHandler(wc, new MDownloadDataCompletedEventArgs
                    {
                        ResultGet = () => data.ToArray()
                    });
                });
            };

            // act
            var scheduler = new TestScheduler();
            var recorder = scheduler.CreateObserver<DownloadProgressChangedEventArgs>();
            var notifier = new ScheduledNotifier<DownloadProgressChangedEventArgs>();
            notifier.Subscribe(recorder);

            var client = new WebClient();
            var result = client.DownloadDataObservableAsync(new Uri("http://moles.com/dummy"), notifier).First();
            result.Is(data);

            recorder.Messages.Count.Is(3);
            recorder.Messages[0].Value.Value.Is(x =>
                x.BytesReceived == 0 && x.TotalBytesToReceive == result.Length);
            recorder.Messages[1].Value.Value.Is(x =>
                x.BytesReceived == 100 && x.TotalBytesToReceive == result.Length);
            recorder.Messages[2].Value.Value.Is(x =>
                x.BytesReceived == result.Length && x.TotalBytesToReceive == result.Length);

            recorder.Messages.Clear();
            client.DownloadDataObservableAsync("http://moles.com/dummy").First();
            result.Is(data);
            recorder.Messages.Count.Is(0);

            var errorResult = client.DownloadDataObservableAsync("http://error.com/").Materialize().ToEnumerable().ToArray();
            errorResult.Length.Is(1);
            errorResult[0].Is(x => x.Kind == NotificationKind.OnError && x.Exception is WebException);

            // check cancel
            var cancelResult = client.DownloadDataObservableAsync("http://cancel.com/").Materialize().ToEnumerable().ToArray();
            cancelResult.Length.Is(1);
            cancelResult[0].Is(x => x.Kind == NotificationKind.OnError && x.Exception is OperationCanceledException);
        }

        [TestMethod]
        public void DownloadDataObservableAsyncReal()
        {
            var scheduler = new TestScheduler();
            var recorder = scheduler.CreateObserver<DownloadProgressChangedEventArgs>();
            var notifier = new ScheduledNotifier<DownloadProgressChangedEventArgs>();
            notifier.Subscribe(recorder);

            var client = new WebClient();
            var result = client.DownloadDataObservableAsync(new Uri("http://twitter.com/statuses/public_timeline.xml"), notifier).Single();

            recorder.Messages.Count.Is(x => x > 1);
            var xml = XElement.Parse(Encoding.UTF8.GetString(result));
            xml.Elements("status").Count().Is(x => x == 20 || x == 19);

            // exception check
            recorder.Messages.Clear();
            var xs = client.DownloadDataObservableAsync("http://google.co.jp/404")
                .Materialize()
                .ToEnumerable()
                .ToArray();
            xs.Length.Is(1);
            xs[0].Is(x =>
                x.Kind == NotificationKind.OnError && x.Exception is WebException);
        }

        [TestMethod, HostType("Moles")]
        public void DownloadFileObservableAsync()
        {
            // Data
            var data = "hogehogehugahuga";

            // Mole
            AsyncCompletedEventHandler completeHandler = (s, e) => { };
            MWebClient.AllInstances.DownloadFileCompletedAddAsyncCompletedEventHandler =
                (wc, h) => completeHandler += h;
            MWebClient.AllInstances.DownloadFileCompletedRemoveAsyncCompletedEventHandler =
                (wc, h) => completeHandler -= h;

            DownloadProgressChangedEventHandler progressHandler = (s, e) => { };
            MWebClient.AllInstances.DownloadProgressChangedAddDownloadProgressChangedEventHandler =
                (wc, h) => progressHandler += h;
            MWebClient.AllInstances.DownloadProgressChangedRemoveDownloadProgressChangedEventHandler =
                (wc, h) => progressHandler -= h;

            MAsyncCompletedEventArgs.AllInstances.ErrorGet = _ => null;
            MAsyncCompletedEventArgs.AllInstances.CancelledGet = _ => false;

            MWebClient.AllInstances.DownloadFileAsyncUriString = (wc, uri, filepath) =>
            {
                // when error...
                if (uri.OriginalString == "http://error.com/")
                {
                    var d = new MDownloadDataCompletedEventArgs();
                    new MAsyncCompletedEventArgs(d) { ErrorGet = () => new WebException() };
                    completeHandler(wc, d);
                    return;
                }

                // async...
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    progressHandler(wc, new MDownloadProgressChangedEventArgs
                    {
                        BytesReceivedGet = () => 0,
                        TotalBytesToReceiveGet = () => data.Length
                    });
                    progressHandler(wc, new MDownloadProgressChangedEventArgs
                    {
                        BytesReceivedGet = () => 100,
                        TotalBytesToReceiveGet = () => data.Length
                    });
                    progressHandler(wc, new MDownloadProgressChangedEventArgs
                    {
                        BytesReceivedGet = () => data.Length,
                        TotalBytesToReceiveGet = () => data.Length
                    });
                    File.WriteAllText(filepath, data);
                    completeHandler(wc, new MAsyncCompletedEventArgs());
                });
            };

            // act
            var scheduler = new TestScheduler();
            var recorder = scheduler.CreateObserver<DownloadProgressChangedEventArgs>();
            var notifier = new ScheduledNotifier<DownloadProgressChangedEventArgs>();
            notifier.Subscribe(recorder);

            var client = new WebClient();
            var result = client.DownloadFileObservableAsync(new Uri("http://moles.com/dummy"), "data.test", notifier).Single();
            File.ReadAllText("data.test").Is(data);

            recorder.Messages.Count.Is(3);
            recorder.Messages[0].Value.Value.Is(x =>
                x.BytesReceived == 0 && x.TotalBytesToReceive == data.Length);
            recorder.Messages[1].Value.Value.Is(x =>
                x.BytesReceived == 100 && x.TotalBytesToReceive == data.Length);
            recorder.Messages[2].Value.Value.Is(x =>
                x.BytesReceived == data.Length && x.TotalBytesToReceive == data.Length);

            recorder.Messages.Clear();
            client.DownloadFileObservableAsync("http://moles.com/dummy", "data2.test").Single();
            File.ReadAllText("data2.test").Is(data);
            recorder.Messages.Count.Is(0);

            recorder.Messages.Clear();
            var errorResult = client.DownloadFileObservableAsync("http://error.com/", "data3.test").Materialize().ToEnumerable().ToArray();
            errorResult.Length.Is(1);
            errorResult[0].Is(x => x.Kind == NotificationKind.OnError && x.Exception is WebException);
            File.Exists("data3.test").Is(false);
        }

        [TestMethod]
        public void DownloadFileObservableAsyncReal()
        {
            var scheduler = new TestScheduler();
            var recorder = scheduler.CreateObserver<DownloadProgressChangedEventArgs>();
            var notifier = new ScheduledNotifier<DownloadProgressChangedEventArgs>();
            notifier.Subscribe(recorder);

            var client = new WebClient();
            var result = client.DownloadFileObservableAsync(
                new Uri("http://twitter.com/statuses/public_timeline.xml"), "test_public_tl.xml", notifier).Single();

            recorder.Messages.Count.Is(x => x > 1);
            var xml = XElement.Load("test_public_tl.xml");
            xml.Elements("status").Count().Is(20);

            // exception check
            recorder.Messages.Clear();
            var xs = client.DownloadFileObservableAsync("http://google.co.jp/404", "test404.html")
                .Materialize()
                .ToEnumerable()
                .ToArray();
            xs.Length.Is(1);
            xs[0].Is(x =>
                x.Kind == NotificationKind.OnError && x.Exception is WebException);
        }


        [TestMethod, HostType("Moles")]
        public void DownloadStringObservableAsync()
        {
            // Data
            var data = "hogehogehugahuga";

            // Mole
            DownloadStringCompletedEventHandler completeHandler = (s, e) => { };
            MWebClient.AllInstances.DownloadStringCompletedAddDownloadStringCompletedEventHandler =
                (wc, h) => completeHandler += h;
            MWebClient.AllInstances.DownloadStringCompletedRemoveDownloadStringCompletedEventHandler =
                (wc, h) => completeHandler -= h;

            DownloadProgressChangedEventHandler progressHandler = (s, e) => { };
            MWebClient.AllInstances.DownloadProgressChangedAddDownloadProgressChangedEventHandler =
                (wc, h) => progressHandler += h;
            MWebClient.AllInstances.DownloadProgressChangedRemoveDownloadProgressChangedEventHandler =
                (wc, h) => progressHandler -= h;

            MAsyncCompletedEventArgs.AllInstances.ErrorGet = _ => null;
            MAsyncCompletedEventArgs.AllInstances.CancelledGet = _ => false;

            MWebClient.AllInstances.DownloadStringAsyncUri = (wc, uri) =>
            {
                // when error...
                if (uri.OriginalString == "http://error.com/")
                {
                    var d = new MDownloadStringCompletedEventArgs();
                    new MAsyncCompletedEventArgs(d) { ErrorGet = () => new WebException() };
                    completeHandler(wc, d);
                    return;
                }

                // async...
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    progressHandler(wc, new MDownloadProgressChangedEventArgs
                    {
                        BytesReceivedGet = () => 0,
                        TotalBytesToReceiveGet = () => data.Length
                    });
                    progressHandler(wc, new MDownloadProgressChangedEventArgs
                    {
                        BytesReceivedGet = () => 100,
                        TotalBytesToReceiveGet = () => data.Length
                    });
                    progressHandler(wc, new MDownloadProgressChangedEventArgs
                    {
                        BytesReceivedGet = () => data.Length,
                        TotalBytesToReceiveGet = () => data.Length
                    });
                    completeHandler(wc, new MDownloadStringCompletedEventArgs
                    {
                        ResultGet = () => data
                    });
                });
            };

            // act
            var scheduler = new TestScheduler();
            var recorder = scheduler.CreateObserver<DownloadProgressChangedEventArgs>();
            var notifier = new ScheduledNotifier<DownloadProgressChangedEventArgs>();
            notifier.Subscribe(recorder);

            var client = new WebClient();
            var result = client.DownloadStringObservableAsync(new Uri("http://moles.com/dummy"), notifier).First();
            result.Is(data);

            recorder.Messages.Count.Is(3);
            recorder.Messages[0].Value.Value.Is(x =>
                x.BytesReceived == 0 && x.TotalBytesToReceive == result.Length);
            recorder.Messages[1].Value.Value.Is(x =>
                x.BytesReceived == 100 && x.TotalBytesToReceive == result.Length);
            recorder.Messages[2].Value.Value.Is(x =>
                x.BytesReceived == result.Length && x.TotalBytesToReceive == result.Length);

            recorder.Messages.Clear();
            client.DownloadStringObservableAsync("http://moles.com/dummy").First();
            result.Is(data);
            recorder.Messages.Count.Is(0);

            recorder.Messages.Clear();
            var errorResult = client.DownloadStringObservableAsync("http://error.com/").Materialize().ToEnumerable().ToArray();
            errorResult.Length.Is(1);
            errorResult[0].Is(x => x.Kind == NotificationKind.OnError && x.Exception is WebException);
        }

        [TestMethod]
        public void DownloadStringObservableAsyncReal()
        {
            var scheduler = new TestScheduler();
            var recorder = scheduler.CreateObserver<DownloadProgressChangedEventArgs>();
            var notifier = new ScheduledNotifier<DownloadProgressChangedEventArgs>();
            notifier.Subscribe(recorder);

            var client = new WebClient();
            var result = client.DownloadStringObservableAsync(new Uri("http://twitter.com/statuses/public_timeline.xml"), notifier).Single();

            recorder.Messages.Count.Is(x => x > 1);
            var xml = XElement.Parse(result);
            xml.Elements("status").Count().Is(x => x == 20 || x == 19);

            // exception check
            recorder.Messages.Clear();
            var xs = client.DownloadStringObservableAsync("http://google.co.jp/404")
                .Materialize()
                .ToEnumerable()
                .ToArray();
            xs.Length.Is(1);
            xs[0].Is(x =>
                x.Kind == NotificationKind.OnError && x.Exception is WebException);
        }

        [TestMethod, HostType("Moles")]
        public void OpenReadObservableAsync()
        {
            // Mole
            OpenReadCompletedEventHandler completeHandler = (s, e) => { };
            MWebClient.AllInstances.OpenReadCompletedAddOpenReadCompletedEventHandler =
                (wc, h) => completeHandler += h;
            MWebClient.AllInstances.OpenReadCompletedRemoveOpenReadCompletedEventHandler =
                (wc, h) => completeHandler -= h;

            MAsyncCompletedEventArgs.AllInstances.ErrorGet = _ => null;
            MAsyncCompletedEventArgs.AllInstances.CancelledGet = _ => false;

            MWebClient.AllInstances.OpenReadAsyncUri = (wc, uri) =>
            {
                // when error...
                if (uri.OriginalString == "http://error.com/")
                {
                    var d = new MOpenReadCompletedEventArgs();
                    new MAsyncCompletedEventArgs(d) { ErrorGet = () => new WebException() };
                    completeHandler(wc, d);
                    return;
                }

                // async...
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    completeHandler(wc, new MOpenReadCompletedEventArgs
                    {
                        ResultGet = () => new MemoryStream(Encoding.UTF8.GetBytes("hogehogehugahuga"))
                    });
                });
            };

            // act
            var client = new WebClient();
            var result = client.OpenReadObservableAsync(new Uri("http://moles.com/dummy")).First();
            new StreamReader(result).ReadToEnd().Is("hogehogehugahuga");

            result = client.OpenReadObservableAsync("http://moles.com/dummy").First();
            new StreamReader(result).ReadToEnd().Is("hogehogehugahuga");

            var errorResult = client.OpenReadObservableAsync("http://error.com/").Materialize().ToEnumerable().ToArray();
            errorResult.Length.Is(1);
            errorResult[0].Is(x => x.Kind == NotificationKind.OnError && x.Exception is WebException);
        }

        [TestMethod]
        public void OpenReadObservableAsyncReal()
        {
            var client = new WebClient();
            var result = client.OpenReadObservableAsync(new Uri("http://twitter.com/statuses/public_timeline.xml")).Single();

            var xml = XElement.Load(result);
            xml.Elements("status").Count().Is(20);

            // exception check
            var xs = client.OpenReadObservableAsync("http://google.co.jp/404")
                .Materialize()
                .ToEnumerable()
                .ToArray();
            xs.Length.Is(1);
            xs[0].Is(x =>
                x.Kind == NotificationKind.OnError && x.Exception is WebException);
        }

        [TestMethod, HostType("Moles")]
        public void OpenWriteObservableAsync()
        {
            // Mole
            OpenWriteCompletedEventHandler completeHandler = (s, e) => { };
            MWebClient.AllInstances.OpenWriteCompletedAddOpenWriteCompletedEventHandler =
                (wc, h) => completeHandler += h;
            MWebClient.AllInstances.OpenWriteCompletedRemoveOpenWriteCompletedEventHandler =
                (wc, h) => completeHandler -= h;

            MAsyncCompletedEventArgs.AllInstances.ErrorGet = _ => null;
            MAsyncCompletedEventArgs.AllInstances.CancelledGet = _ => false;

            MWebClient.AllInstances.OpenWriteAsyncUriString = (wc, uri, method) =>
            {
                // when error...
                if (uri.OriginalString == "http://error.com/")
                {
                    var d = new MOpenWriteCompletedEventArgs();
                    new MAsyncCompletedEventArgs(d) { ErrorGet = () => new WebException() };
                    completeHandler(wc, d);
                    return;
                }

                // async...
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    completeHandler(wc, new MOpenWriteCompletedEventArgs
                    {
                        ResultGet = () => new MemoryStream()
                    });
                });
            };

            var client = new WebClient();
            var result = client.OpenWriteObservableAsync(new Uri("http://moles.com/dummy")).Single();
            result.IsInstanceOf<MemoryStream>();

            var errorResult = client.OpenWriteObservableAsync("http://error.com/").Materialize().ToEnumerable().ToArray();
            errorResult.Length.Is(1);
            errorResult[0].Is(x => x.Kind == NotificationKind.OnError && x.Exception is WebException);
        }

        [TestMethod]
        public void OpenWriteObservableAsyncReal()
        {
            var client = new WebClient();
            client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            var result = client.OpenWriteObservableAsync(new Uri("http://goo.gl/api/shorten"), "POST").Single();
            using (var sw = new StreamWriter(result))
            {
                sw.Write("url=http%3A%2F%2Fgoogle.com%2F&security_token");
            }
            client.ResponseHeaders[HttpResponseHeader.Location].Is("http://goo.gl/mR2d");
        }
    }
}