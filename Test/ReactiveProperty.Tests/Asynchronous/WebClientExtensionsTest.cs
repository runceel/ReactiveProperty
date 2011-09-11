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

            MWebClient.AllInstances.DownloadDataAsyncUri = (wc, uri) =>
            {
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
        }
    }



}
