using System;
using System.Net;
using Codeplex.Reactive.Notifier;
using System.Diagnostics.Contracts;
using System.ComponentModel;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#else
using System.Reactive.Linq;
using System.Reactive.Disposables;
#endif

namespace Codeplex.Reactive.Asynchronous
{
    public static class WebClientExtensions
    {
        static Func<IDisposable> RegisterDownloadProgress(WebClient client, IProgress<DownloadProgressChangedEventArgs> progress)
        {
            return () =>
                Observable.FromEvent<DownloadProgressChangedEventHandler, DownloadProgressChangedEventArgs>(
                        h => (sender, e) => h(e), h => client.DownloadProgressChanged += h, h => client.DownloadProgressChanged -= h)
                    .Subscribe(progress.Report);
        }

        static Func<IDisposable> RegisterUploadProgress(WebClient client, IProgress<UploadProgressChangedEventArgs> progress)
        {
            return () =>
                Observable.FromEvent<UploadProgressChangedEventHandler, UploadProgressChangedEventArgs>(
                        h => (sender, e) => h(e), h => client.UploadProgressChanged += h, h => client.UploadProgressChanged -= h)
                    .Subscribe(progress.Report);
        }

        static IObservable<TEventArgs> RegisterAsyncEvent<TDelegate, TEventArgs>(WebClient client,
            Func<Action<TEventArgs>, TDelegate> conversion, Action<TDelegate> addHandler, Action<TDelegate> removeHandler,
            Func<IDisposable> progressSubscribe, Action startAsync)
            where TEventArgs : AsyncCompletedEventArgs
        {
            var result = Observable.Create<TEventArgs>(observer =>
            {
                var isCompleted = false;
                var subscription = Observable.FromEvent<TDelegate, TEventArgs>(conversion, addHandler, removeHandler)
                    .Take(1)
                    .Do(e =>
                    {
                        isCompleted = true;
                        if (e.Error != null) throw e.Error;
                    })
                    .Subscribe(observer);
                var progress = progressSubscribe();
                startAsync();
                return () =>
                {
                    subscription.Dispose();
                    progress.Dispose();
                    if (!isCompleted) client.CancelAsync();
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
                (progress != null) ? RegisterDownloadProgress(client, progress) : () => Disposable.Empty,
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
                (progress != null) ? RegisterDownloadProgress(client, progress) : () => Disposable.Empty,
                () => client.DownloadFileAsync(address, fileName));
        }

        public static IObservable<DownloadStringCompletedEventArgs> DownloadStringObservableAsync(this WebClient client, string address)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(address));
            Contract.Ensures(Contract.Result<IObservable<DownloadStringCompletedEventArgs>>() != null);

            return DownloadStringObservableAsync(client, new Uri(address));
        }

        public static IObservable<DownloadStringCompletedEventArgs> DownloadStringObservableAsync(this WebClient client, Uri address)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Ensures(Contract.Result<IObservable<DownloadStringCompletedEventArgs>>() != null);

            return DownloadStringObservableAsyncCore(client, address, null);
        }

        public static IObservable<DownloadStringCompletedEventArgs> DownloadStringObservableAsync(this WebClient client, Uri address, IProgress<DownloadProgressChangedEventArgs> progress)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Requires<ArgumentNullException>(progress != null);
            Contract.Ensures(Contract.Result<IObservable<DownloadStringCompletedEventArgs>>() != null);

            return DownloadStringObservableAsyncCore(client, address, progress);
        }

        static IObservable<DownloadStringCompletedEventArgs> DownloadStringObservableAsyncCore(WebClient client, Uri address, IProgress<DownloadProgressChangedEventArgs> progress)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Ensures(Contract.Result<IObservable<DownloadStringCompletedEventArgs>>() != null);

            return RegisterAsyncEvent<DownloadStringCompletedEventHandler, DownloadStringCompletedEventArgs>(
                client,
                h => (sender, e) => h(e),
                h => client.DownloadStringCompleted += h,
                h => client.DownloadStringCompleted -= h,
                (progress != null) ? RegisterDownloadProgress(client, progress) : () => Disposable.Empty,
                () => client.DownloadStringAsync(address));
        }

        public static IObservable<OpenReadCompletedEventArgs> OpenReadObservableAsync(this WebClient client, string address)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(address));
            Contract.Ensures(Contract.Result<IObservable<OpenReadCompletedEventArgs>>() != null);

            return OpenReadObservableAsync(client, new Uri(address));
        }

        public static IObservable<OpenReadCompletedEventArgs> OpenReadObservableAsync(this WebClient client, Uri address)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Ensures(Contract.Result<IObservable<OpenReadCompletedEventArgs>>() != null);

            return RegisterAsyncEvent<OpenReadCompletedEventHandler, OpenReadCompletedEventArgs>(
                client,
                h => (sender, e) => h(e),
                h => client.OpenReadCompleted += h,
                h => client.OpenReadCompleted -= h,
                () => Disposable.Empty,
                () => client.OpenReadAsync(address));
        }

        public static IObservable<OpenWriteCompletedEventArgs> OpenWriteObservableAsync(this WebClient client, string address, string method = null)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(address));
            Contract.Ensures(Contract.Result<IObservable<OpenWriteCompletedEventArgs>>() != null);

            return OpenWriteObservableAsync(client, new Uri(address), method);
        }

        public static IObservable<OpenWriteCompletedEventArgs> OpenWriteObservableAsync(this WebClient client, Uri address, string method = null)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Ensures(Contract.Result<IObservable<OpenWriteCompletedEventArgs>>() != null);

            return RegisterAsyncEvent<OpenWriteCompletedEventHandler, OpenWriteCompletedEventArgs>(
                client,
                h => (sender, e) => h(e),
                h => client.OpenWriteCompleted += h,
                h => client.OpenWriteCompleted -= h,
                () => Disposable.Empty,
                () => client.OpenWriteAsync(address, null));
        }

        public static IObservable<WebResponse> UploadDataObservableAsync(this WebClient client)
        {
            
            // TODO:
            return null;
        }

        public static IObservable<WebResponse> UploadFileObservableAsync(this WebClient client)
        {
            // TODO:
            return null;
        }

        public static IObservable<WebResponse> UploadStringObservableAsync(this WebClient client)
        {
            // TODO:
            return null;
        }

        public static IObservable<WebResponse> UploadValuesObservableAsync(this WebClient client)
        {
            // TODO:
            return null;
        }
    }
}