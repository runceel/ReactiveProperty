using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Codeplex.Reactive.Notifier;
using System.Diagnostics.Contracts;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#else
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.ComponentModel;
#endif

namespace Codeplex.Reactive.Asynchronous
{
    // TODO:other methods

    public static class WebClientExtensions
    {
        static Func<IDisposable> RegisterDownloadProgress(WebClient client, IProgress<DownloadProgressChangedEventArgs> progress)
        {
            if (progress != null)
            {
                return () =>
                    Observable.FromEvent<DownloadProgressChangedEventHandler, DownloadProgressChangedEventArgs>(
                            h => (sender, e) => h(e), h => client.DownloadProgressChanged += h, h => client.DownloadProgressChanged -= h)
                        .Subscribe(progress.Report);
            }
            else
            {
                return () => Disposable.Empty;
            }
        }

        static Func<IDisposable> RegisterUploadProgress(WebClient client, IProgress<UploadProgressChangedEventArgs> progress)
        {
            if (progress != null)
            {
                return () =>
                    Observable.FromEvent<UploadProgressChangedEventHandler, UploadProgressChangedEventArgs>(
                            h => (sender, e) => h(e), h => client.UploadProgressChanged += h, h => client.UploadProgressChanged -= h)
                        .Subscribe(progress.Report);
            }
            else
            {
                return () => Disposable.Empty;
            }
        }

        static IObservable<TEventArgs> RegisterAsyncEvent<TDelegate, TEventArgs>(WebClient client,
            Func<Action<TEventArgs>, TDelegate> conversion, Action<TDelegate> addHandler, Action<TDelegate> removeHandler,
            Func<IDisposable> progressSubscribe, Action startAsync)
            where TEventArgs : AsyncCompletedEventArgs
        {
            var result = Observable.Create<TEventArgs>(observer =>
            {
                var subscription = Observable.FromEvent<TDelegate, TEventArgs>(conversion, addHandler, removeHandler)
                    .Take(1)
                    .Do(e => { if (e.Error != null) throw e.Error; })
                    .Subscribe(observer);
                var progress = progressSubscribe();
                startAsync();
                return () =>
                {
                    subscription.Dispose();
                    progress.Dispose();
                    client.CancelAsync();
                };
            });

            Contract.Assume(result != null);
            return result;
        }

        public static IObservable<DownloadDataCompletedEventArgs> DownloadDataObservableAsync(this WebClient client, string address)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(address));
            Contract.Ensures(Contract.Result<IObservable<DownloadDataCompletedEventArgs>>() != null);

            return DownloadDataObservableAsync(client, new Uri(address));
        }

        public static IObservable<DownloadDataCompletedEventArgs> DownloadDataObservableAsync(this WebClient client, Uri address)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Ensures(Contract.Result<IObservable<DownloadDataCompletedEventArgs>>() != null);

            return DownloadDataObservableAsyncCore(client, address, null);
        }

        public static IObservable<DownloadDataCompletedEventArgs> DownloadDataObservableAsync(this WebClient client, Uri address, IProgress<DownloadProgressChangedEventArgs> progress)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Requires<ArgumentNullException>(progress != null);
            Contract.Ensures(Contract.Result<IObservable<DownloadDataCompletedEventArgs>>() != null);

            return DownloadDataObservableAsyncCore(client, address, progress);
        }

        static IObservable<DownloadDataCompletedEventArgs> DownloadDataObservableAsyncCore(this WebClient client, Uri address, IProgress<DownloadProgressChangedEventArgs> progress)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Ensures(Contract.Result<IObservable<DownloadDataCompletedEventArgs>>() != null);

            return RegisterAsyncEvent<DownloadDataCompletedEventHandler, DownloadDataCompletedEventArgs>(
                client,
                h => (sender, e) => h(e),
                h => client.DownloadDataCompleted += h,
                h => client.DownloadDataCompleted -= h,
                RegisterDownloadProgress(client, progress),
                () => client.DownloadDataAsync(address));
        }

        public static IObservable<AsyncCompletedEventArgs> DownloadFileObservableAsync(this WebClient client, string address, string fileName)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(address));
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(fileName));
            Contract.Ensures(Contract.Result<IObservable<AsyncCompletedEventArgs>>() != null);

            return DownloadFileObservableAsync(client, new Uri(address), fileName);    
        }

        public static IObservable<AsyncCompletedEventArgs> DownloadFileObservableAsync(this WebClient client, Uri address, string fileName)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(fileName));
            Contract.Ensures(Contract.Result<IObservable<AsyncCompletedEventArgs>>() != null);

            return DownloadFileObservableAsyncCore(client, address, fileName, null);
        }

        public static IObservable<AsyncCompletedEventArgs> DownloadFileObservableAsync(this WebClient client, Uri address, string fileName, IProgress<DownloadProgressChangedEventArgs> progress)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(fileName));
            Contract.Requires<ArgumentNullException>(progress != null);
            Contract.Ensures(Contract.Result<IObservable<AsyncCompletedEventArgs>>() != null);

            return DownloadFileObservableAsyncCore(client, address, fileName, progress);
        }

        static IObservable<AsyncCompletedEventArgs> DownloadFileObservableAsyncCore(WebClient client, Uri address, string fileName, IProgress<DownloadProgressChangedEventArgs> progress)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(fileName));
            Contract.Ensures(Contract.Result<IObservable<AsyncCompletedEventArgs>>() != null);

            return RegisterAsyncEvent<AsyncCompletedEventHandler, AsyncCompletedEventArgs>(
                client,
                h => (sender, e) => h(e),
                h => client.DownloadFileCompleted += h,
                h => client.DownloadFileCompleted -= h,
                RegisterDownloadProgress(client, progress),
                () => client.DownloadFileAsync(address, fileName));
        }

        public static IObservable<WebResponse> DownloadStringObservableAsync(this WebClient client)
        {
            return null;
        }

        public static IObservable<WebResponse> OpenReadObservableAsync(this WebClient client)
        {
            return null;
        }

        public static IObservable<WebResponse> OpenWriteObservableAsync(this WebClient client)
        {
            return null;
        }

        public static IObservable<WebResponse> UploadDataObservableAsync(this WebClient client)
        {
            return null;
        }

        public static IObservable<WebResponse> UploadFileObservableAsync(this WebClient client)
        {
            return null;
        }
    }
}