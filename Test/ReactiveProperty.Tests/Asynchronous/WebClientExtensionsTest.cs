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
using System.Net;
using System.Net.Moles;
using System.Threading;
using System.Xml.Linq;
using System.ComponentModel.Moles;
using System.ComponentModel;

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
            result.Result.Is(data);

            recorder.Messages.Count.Is(3);
            recorder.Messages[0].Value.Value.Is(x =>
                x.BytesReceived == 0 && x.TotalBytesToReceive == result.Result.Length);
            recorder.Messages[1].Value.Value.Is(x =>
                x.BytesReceived == 100 && x.TotalBytesToReceive == result.Result.Length);
            recorder.Messages[2].Value.Value.Is(x =>
                x.BytesReceived == result.Result.Length && x.TotalBytesToReceive == result.Result.Length);

            recorder.Messages.Clear();
            client.DownloadDataObservableAsync("http://moles.com/dummy").First();
            result.Result.Is(data);
            recorder.Messages.Count.Is(0);

            recorder.Messages.Clear();
            var errorResult = client.DownloadDataObservableAsync("http://error.com/").Materialize().ToEnumerable().ToArray();
            errorResult.Length.Is(1);
            errorResult[0].Is(x => x.Kind == NotificationKind.OnError && x.Exception is WebException);
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
            var xml = XElement.Parse(Encoding.UTF8.GetString(result.Result));
            xml.Elements("status").Count().Is(20);

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
    }



}
