using System;
using System.Net;
using Codeplex.Reactive.Notifier;
using System.Diagnostics.Contracts;
using System.ComponentModel;
using System.IO;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#else
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Reactive;
using System.Collections.Specialized;
#endif

namespace Codeplex.Reactive.Asynchronous
{
    public static class WebClientExtensions
    {
        static Func<IDisposable> RegisterDownloadProgress(WebClient client, IProgress<DownloadProgressChangedEventArgs> progress)
        {
            return () =>
                ObservableEx.FromEvent<DownloadProgressChangedEventHandler, DownloadProgressChangedEventArgs>(
                        h => (sender, e) => h(e), h => client.DownloadProgressChanged += h, h => client.DownloadProgressChanged -= h)
                    .Subscribe(progress.Report);
        }

        static Func<IDisposable> RegisterUploadProgress(WebClient client, IProgress<UploadProgressChangedEventArgs> progress)
        {
            return () =>
                ObservableEx.FromEvent<UploadProgressChangedEventHandler, UploadProgressChangedEventArgs>(
                        h => (sender, e) => h(e), h => client.UploadProgressChanged += h, h => client.UploadProgressChanged -= h)
                    .Subscribe(progress.Report);
        }

        static IObservable<TResult> RegisterAsyncEvent<TDelegate, TEventArgs, TResult>(WebClient client,
            Func<Action<TEventArgs>, TDelegate> conversion, Action<TDelegate> addHandler, Action<TDelegate> removeHandler,
            Func<TEventArgs, TResult> resultSelector, Func<IDisposable> progressSubscribe, Action startAsync)
            where TEventArgs : AsyncCompletedEventArgs
        {
            var result = Observable.Create<TResult>(observer =>
            {
                var isCompleted = false;
                var subscription = ObservableEx.FromEvent<TDelegate, TEventArgs>(conversion, addHandler, removeHandler)
                    .Take(1)
                    .Select(e =>
                    {
                        isCompleted = true;
                        if (e.Cancelled) throw new InvalidOperationException("Call AsyncCancel directly is not supported. If you want to cancel, please call Subscript's Dispose");
                        if (e.Error != null) throw e.Error;
                        return resultSelector(e);
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

        public static IObservable<string> DownloadStringObservableAsync(this WebClient client, string address)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(address));
            Contract.Ensures(Contract.Result<IObservable<string>>() != null);

            return DownloadStringObservableAsync(client, new Uri(address));
        }

        public static IObservable<string> DownloadStringObservableAsync(this WebClient client, Uri address)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Ensures(Contract.Result<IObservable<string>>() != null);

            return DownloadStringObservableAsyncCore(client, address, null);
        }

        public static IObservable<string> DownloadStringObservableAsync(this WebClient client, Uri address, IProgress<DownloadProgressChangedEventArgs> progress)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Requires<ArgumentNullException>(progress != null);
            Contract.Ensures(Contract.Result<IObservable<string>>() != null);

            return DownloadStringObservableAsyncCore(client, address, progress);
        }

        static IObservable<string> DownloadStringObservableAsyncCore(WebClient client, Uri address, IProgress<DownloadProgressChangedEventArgs> progress)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Ensures(Contract.Result<IObservable<string>>() != null);

            return RegisterAsyncEvent<DownloadStringCompletedEventHandler, DownloadStringCompletedEventArgs, string>(
                client,
                h => (sender, e) => h(e),
                h => client.DownloadStringCompleted += h,
                h => client.DownloadStringCompleted -= h,
                e => e.Result,
                (progress != null) ? RegisterDownloadProgress(client, progress) : () => Disposable.Empty,
                () => client.DownloadStringAsync(address));
        }

        public static IObservable<Stream> OpenReadObservableAsync(this WebClient client, string address)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(address));
            Contract.Ensures(Contract.Result<IObservable<Stream>>() != null);

            return OpenReadObservableAsync(client, new Uri(address));
        }

        public static IObservable<Stream> OpenReadObservableAsync(this WebClient client, Uri address)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Ensures(Contract.Result<IObservable<Stream>>() != null);

            return RegisterAsyncEvent<OpenReadCompletedEventHandler, OpenReadCompletedEventArgs, Stream>(
                client,
                h => (sender, e) => h(e),
                h => client.OpenReadCompleted += h,
                h => client.OpenReadCompleted -= h,
                e => e.Result,
                () => Disposable.Empty,
                () => client.OpenReadAsync(address));
        }

        public static IObservable<Stream> OpenWriteObservableAsync(this WebClient client, string address, string method = null)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(address));
            Contract.Ensures(Contract.Result<IObservable<Stream>>() != null);

            return OpenWriteObservableAsync(client, new Uri(address), method);
        }

        public static IObservable<Stream> OpenWriteObservableAsync(this WebClient client, Uri address, string method = null)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Ensures(Contract.Result<IObservable<Stream>>() != null);

            return RegisterAsyncEvent<OpenWriteCompletedEventHandler, OpenWriteCompletedEventArgs, Stream>(
                client,
                h => (sender, e) => h(e),
                h => client.OpenWriteCompleted += h,
                h => client.OpenWriteCompleted -= h,
                e => e.Result,
                () => Disposable.Empty,
                () => client.OpenWriteAsync(address, null));
        }

        public static IObservable<string> UploadStringObservableAsync(WebClient client, string address, string data, string method = null)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Ensures(Contract.Result<IObservable<string>>() != null);

            return UploadStringObservableAsync(client, new Uri(address), data, method);
        }

        public static IObservable<string> UploadStringObservableAsync(WebClient client, Uri address, string data, string method = null)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Ensures(Contract.Result<IObservable<string>>() != null);

            return UploadStringObservableAsyncCore(client, address, data, null, method);
        }

        public static IObservable<string> UploadStringObservableAsync(WebClient client, Uri address, string data, IProgress<UploadProgressChangedEventArgs> progress, string method = null)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentNullException>(progress != null);
            Contract.Ensures(Contract.Result<IObservable<string>>() != null);

            return UploadStringObservableAsyncCore(client, address, data, progress, method);
        }

        public static IObservable<string> UploadStringObservableAsyncCore(WebClient client, Uri address, string data, IProgress<UploadProgressChangedEventArgs> progress, string method)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Ensures(Contract.Result<IObservable<string>>() != null);

            return RegisterAsyncEvent<UploadStringCompletedEventHandler, UploadStringCompletedEventArgs, string>(
                client,
                h => (sender, e) => h(e),
                h => client.UploadStringCompleted += h,
                h => client.UploadStringCompleted -= h,
                e => e.Result,
                (progress != null) ? RegisterUploadProgress(client, progress) : () => Disposable.Empty,
                () => client.UploadStringAsync(address, method, data, null));
        }

#if !SILVERLIGHT
        public static IObservable<byte[]> DownloadDataObservableAsync(this WebClient client, string address)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(address));
            Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

            return DownloadDataObservableAsync(client, new Uri(address));
        }

        public static IObservable<byte[]> DownloadDataObservableAsync(this WebClient client, Uri address)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

            return DownloadDataObservableAsyncCore(client, address, null);
        }

        public static IObservable<byte[]> DownloadDataObservableAsync(this WebClient client, Uri address, IProgress<DownloadProgressChangedEventArgs> progress)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Requires<ArgumentNullException>(progress != null);
            Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

            return DownloadDataObservableAsyncCore(client, address, progress);
        }

        static IObservable<byte[]> DownloadDataObservableAsyncCore(this WebClient client, Uri address, IProgress<DownloadProgressChangedEventArgs> progress)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

            return RegisterAsyncEvent<DownloadDataCompletedEventHandler, DownloadDataCompletedEventArgs, byte[]>(
                client,
                h => (sender, e) => h(e),
                h => client.DownloadDataCompleted += h,
                h => client.DownloadDataCompleted -= h,
                e => e.Result,
                (progress != null) ? RegisterDownloadProgress(client, progress) : () => Disposable.Empty,
                () => client.DownloadDataAsync(address));
        }

        public static IObservable<Unit> DownloadFileObservableAsync(this WebClient client, string address, string fileName)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(address));
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(fileName));
            Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

            return DownloadFileObservableAsync(client, new Uri(address), fileName);
        }

        public static IObservable<Unit> DownloadFileObservableAsync(this WebClient client, Uri address, string fileName)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(fileName));
            Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

            return DownloadFileObservableAsyncCore(client, address, fileName, null);
        }

        public static IObservable<Unit> DownloadFileObservableAsync(this WebClient client, Uri address, string fileName, IProgress<DownloadProgressChangedEventArgs> progress)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(fileName));
            Contract.Requires<ArgumentNullException>(progress != null);
            Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

            return DownloadFileObservableAsyncCore(client, address, fileName, progress);
        }

        static IObservable<Unit> DownloadFileObservableAsyncCore(WebClient client, Uri address, string fileName, IProgress<DownloadProgressChangedEventArgs> progress)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(fileName));
            Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

            return RegisterAsyncEvent<AsyncCompletedEventHandler, AsyncCompletedEventArgs, Unit>(
                client,
                h => (sender, e) => h(e),
                h => client.DownloadFileCompleted += h,
                h => client.DownloadFileCompleted -= h,
                e => Unit.Default,
                (progress != null) ? RegisterDownloadProgress(client, progress) : () => Disposable.Empty,
                () => client.DownloadFileAsync(address, fileName));
        }

        public static IObservable<byte[]> UploadDataObservableAsync(this WebClient client, string address, byte[] data, string method = null)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

            return UploadDataObservableAsync(client, new Uri(address), data, method);
        }

        public static IObservable<byte[]> UploadDataObservableAsync(this WebClient client, Uri address, byte[] data, string method = null)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

            return UploadDataObservableAsyncCore(client, address, data, null, method);
        }

        public static IObservable<byte[]> UploadDataObservableAsync(this WebClient client, Uri address, byte[] data, IProgress<UploadProgressChangedEventArgs> progress, string method = null)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentNullException>(progress != null);
            Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

            return UploadDataObservableAsyncCore(client, address, data, progress, method);
        }

        public static IObservable<byte[]> UploadDataObservableAsyncCore(WebClient client, Uri address, byte[] data, IProgress<UploadProgressChangedEventArgs> progress, string method)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

            return RegisterAsyncEvent<UploadDataCompletedEventHandler, UploadDataCompletedEventArgs, byte[]>(
                client,
                h => (sender, e) => h(e),
                h => client.UploadDataCompleted += h,
                h => client.UploadDataCompleted -= h,
                e => e.Result,
                (progress != null) ? RegisterUploadProgress(client, progress) : () => Disposable.Empty,
                () => client.UploadDataAsync(address, method, data, null));
        }

        public static IObservable<byte[]> UploadFileObservableAsync(this WebClient client, string address, string fileName, string method = null)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Requires<ArgumentNullException>(fileName != null);
            Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

            return UploadFileObservableAsync(client, new Uri(address), fileName, method);
        }

        public static IObservable<byte[]> UploadFileObservableAsync(this WebClient client, Uri address, string fileName, string method = null)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Requires<ArgumentNullException>(fileName != null);
            Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

            return UploadFileObservableAsyncCore(client, address, fileName, null, method);
        }

        public static IObservable<byte[]> UploadFileObservableAsync(this WebClient client, Uri address, string fileName, IProgress<UploadProgressChangedEventArgs> progress, string method = null)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Requires<ArgumentNullException>(fileName != null);
            Contract.Requires<ArgumentNullException>(progress != null);
            Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

            return UploadFileObservableAsyncCore(client, address, fileName, progress, method);
        }

        public static IObservable<byte[]> UploadFileObservableAsyncCore(WebClient client, Uri address, string fileName, IProgress<UploadProgressChangedEventArgs> progress, string method)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Requires<ArgumentNullException>(fileName != null);
            Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

            return RegisterAsyncEvent<UploadFileCompletedEventHandler, UploadFileCompletedEventArgs, byte[]>(
                client,
                h => (sender, e) => h(e),
                h => client.UploadFileCompleted += h,
                h => client.UploadFileCompleted -= h,
                e => e.Result,
                (progress != null) ? RegisterUploadProgress(client, progress) : () => Disposable.Empty,
                () => client.UploadFileAsync(address, method, fileName, null));
        }

        public static IObservable<byte[]> UploadValuesObservableAsync(WebClient client, string address, NameValueCollection data, string method = null)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Ensures(Contract.Result<IObservable<string>>() != null);

            return UploadValuesObservableAsync(client,new Uri(address), data, method);
        }

        public static IObservable<byte[]> UploadValuesObservableAsync(WebClient client, Uri address, NameValueCollection data, string method = null)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Ensures(Contract.Result<IObservable<string>>() != null);

            return UploadValuesObservableAsyncCore(client, address, data, null, method);
        }

        public static IObservable<byte[]> UploadValuesObservableAsync(WebClient client, Uri address, NameValueCollection data, IProgress<UploadProgressChangedEventArgs> progress, string method = null)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentNullException>(progress != null);
            Contract.Ensures(Contract.Result<IObservable<string>>() != null);

            return UploadValuesObservableAsyncCore(client, address, data, progress, method);
        }

        public static IObservable<byte[]> UploadValuesObservableAsyncCore(WebClient client, Uri address, NameValueCollection data, IProgress<UploadProgressChangedEventArgs> progress, string method)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            Contract.Requires<ArgumentNullException>(address != null);
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Ensures(Contract.Result<IObservable<string>>() != null);

            return RegisterAsyncEvent<UploadValuesCompletedEventHandler, UploadValuesCompletedEventArgs, byte[]>(
                client,
                h => (sender, e) => h(e),
                h => client.UploadValuesCompleted += h,
                h => client.UploadValuesCompleted -= h,
                e => e.Result,
                (progress != null) ? RegisterUploadProgress(client, progress) : () => Disposable.Empty,
                () => client.UploadValuesAsync(address, method, data, null));
        }

#endif
    }
}