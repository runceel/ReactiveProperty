using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Diagnostics.Contracts;
using Codeplex.Reactive.Notifier;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#else
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
#endif

namespace Codeplex.Reactive.Asynchronous
{
    public static class WebResponseExtensions
    {
        /// <summary>
        /// <para>Download data async.</para>
        /// <para>Run deferred, Length of return value is if isAggregateAllChunks is true then 1, else reading count.</para>
        /// </summary>
        /// <param name="response">Target WebResponse.</param>
        /// <param name="chunkSize">The size of one reading.</param>
        /// <param name="isAggregateAllChunks">If true, collect all chunks(return length is 1) else return length is reading count.</param>
        public static IObservable<byte[]> DownloadDataAsync(this WebResponse response, int chunkSize = 65536, bool isAggregateAllChunks = true)
        {
            Contract.Requires<ArgumentNullException>(response != null);
            Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

            var result = Observable.Defer(() => response.GetResponseStream()
                    .ReadAsync(chunkSize, isAggregateAllChunks))
                .Finally(() => response.Close());

            Contract.Assume(result != null);
            return result;
        }

        /// <summary>
        /// <para>Download data async.</para>
        /// <para>Run deferred, Length of return value is if isAggregateAllChunks is true then 1, else reading count.</para>
        /// </summary>
        /// <param name="response">Target WebResponse.</param>
        /// <param name="progressReporter">Reporter of progress(such as ScheduledNotifier).</param>
        /// <param name="chunkSize">The size of one reading.</param>
        /// <param name="isAggregateAllChunks">If true, collect all chunks(return length is 1) else return length is reading count.</param>
        public static IObservable<byte[]> DownloadDataAsync(this WebResponse response, IProgress<ProgressStatus> progressReporter, int chunkSize = 65536, bool isAggregateAllChunks = true)
        {
            Contract.Requires<ArgumentNullException>(response != null);
            Contract.Requires<ArgumentNullException>(progressReporter != null);
            Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

            var result = Observable.Defer(() => response.GetResponseStream()
                    .ReadAsync(progressReporter, (int)response.ContentLength, chunkSize, isAggregateAllChunks))
                .Finally(() => response.Close());

            Contract.Assume(result != null);
            return result;
        }

        /// <summary>
        /// <para>Download string async.</para>
        /// <para>Run deferred, Length of return value is always 1.</para>
        /// </summary>
        public static IObservable<string> DownloadStringAsync(this WebResponse response)
        {
            Contract.Requires<ArgumentNullException>(response != null);
            Contract.Ensures(Contract.Result<IObservable<string>>() != null);

            return DownloadStringAsync(response, Encoding.UTF8);
        }

        /// <summary>
        /// <para>Download string async.</para>
        /// <para>Run deferred, Length of return value is always 1.</para>
        /// </summary>
        public static IObservable<string> DownloadStringAsync(this WebResponse response, Encoding encoding)
        {
            Contract.Requires<ArgumentNullException>(response != null);
            Contract.Requires<ArgumentNullException>(encoding != null);
            Contract.Ensures(Contract.Result<IObservable<string>>() != null);

            var result = response.DownloadDataAsync().Select(x => encoding.GetString(x, 0, x.Length));

            Contract.Assume(result != null);
            return result;
        }

        /// <summary>
        /// <para>Download string async.</para>
        /// <para>Run deferred, Length of return value is always 1.</para>
        /// </summary>
        /// <param name="progressReporter">Reporter of progress(such as ScheduledNotifier).</param>
        public static IObservable<string> DownloadStringAsync(this WebResponse response, IProgress<ProgressStatus> progressReporter)
        {
            Contract.Requires<ArgumentNullException>(response != null);
            Contract.Requires<ArgumentNullException>(progressReporter != null);
            Contract.Ensures(Contract.Result<IObservable<string>>() != null);

            return DownloadStringAsync(response, Encoding.UTF8, progressReporter);
        }

        /// <summary>
        /// <para>Download string async.</para>
        /// <para>Run deferred, Length of return value is always 1.</para>
        /// </summary>
        /// <param name="progressReporter">Reporter of progress(such as ScheduledNotifier).</param>
        public static IObservable<string> DownloadStringAsync(this WebResponse response, Encoding encoding, IProgress<ProgressStatus> progressReporter)
        {
            Contract.Requires<ArgumentNullException>(response != null);
            Contract.Requires<ArgumentNullException>(encoding != null);
            Contract.Requires<ArgumentNullException>(progressReporter != null);
            Contract.Ensures(Contract.Result<IObservable<string>>() != null);

            var result = response.DownloadDataAsync(progressReporter).Select(x => encoding.GetString(x, 0, x.Length));

            Contract.Assume(result != null);
            return result;
        }

        /// <summary>
        /// <para>Download string lines async.</para>
        /// <para>Run deferred, Length of return value is lines row count.</para>
        /// </summary>
        public static IObservable<string> DownloadStringLineAsync(this WebResponse response)
        {
            Contract.Requires<ArgumentNullException>(response != null);
            Contract.Ensures(Contract.Result<IObservable<string>>() != null);

            return DownloadStringLineAsync(response, Encoding.UTF8);
        }

        /// <summary>
        /// <para>Download string lines async.</para>
        /// <para>Run deferred, Length of return value is lines row count.</para>
        /// </summary>
        public static IObservable<string> DownloadStringLineAsync(this WebResponse response, Encoding encoding)
        {
            Contract.Requires<ArgumentNullException>(response != null);
            Contract.Requires<ArgumentNullException>(encoding != null);
            Contract.Ensures(Contract.Result<IObservable<string>>() != null);

            var result = Observable.Defer(() => response.GetResponseStream().ReadLineAsync(encoding))
                .Finally(() => response.Close());

            Contract.Assume(result != null);
            return result;
        }
    }
}