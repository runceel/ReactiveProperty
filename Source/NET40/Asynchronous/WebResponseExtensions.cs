using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#else
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using Codeplex.Reactive.Notifier;
#endif

namespace Codeplex.Reactive.Asynchronous
{
    public static class WebResponseExtensions
    {
        public static IObservable<byte[]> DownloadDataAsync(this WebResponse response)
        {
            return Observable.Defer(() => response.GetResponseStream().ReadAsync())
                .Finally(() => response.Close())
                .Aggregate(new List<byte>(), (list, bytes) => { list.AddRange(bytes); return list; })
                .Select(x => x.ToArray());
        }

        public static IObservable<byte[]> DownloadDataAsync(this WebResponse response, IProgress<ProgressStatus> notifier)
        {
            return Observable.Defer(() => response.GetResponseStream().ReadAsync())
                .Finally(() => response.Close())
                .Scan(new
                {
                    Progress = new ProgressStatus(0, response.ContentLength),
                    ReceivedBytes = new byte[0]
                }, (status, bytes) => new
                {
                    Progress = new ProgressStatus(status.Progress.CurrentLength + bytes.Length, response.ContentLength),
                    ReceivedBytes = bytes
                })
                .Do(x => notifier.Report(x.Progress))
                .Select(x => x.ReceivedBytes);
        }

        public static IObservable<string> DownloadStringAsync(this WebResponse response)
        {
            return DownloadStringAsync(response, Encoding.UTF8);
        }

        public static IObservable<string> DownloadStringAsync(this WebResponse response, Encoding encoding)
        {
            return response.DownloadDataAsync().Select(x => encoding.GetString(x, 0, x.Length));
        }

        public static IObservable<string> DownloadStringLineAsync(this WebResponse response)
        {
            return DownloadStringLineAsync(response, Encoding.UTF8);
        }

        public static IObservable<string> DownloadStringLineAsync(this WebResponse response, Encoding encoding)
        {
            return Observable.Defer(() => response.GetResponseStream().ReadLineAsync(encoding))
                .Finally(() => response.Close());
        }
    }
}