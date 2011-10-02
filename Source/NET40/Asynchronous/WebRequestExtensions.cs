using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
    // TODO:other methods

    public static class WebRequestExtensions
    {
        public static IObservable<WebResponse> GetResponseAsObservable(this WebRequest request)
        {
            return Observable.Create<WebResponse>(observer =>
            {
                Observable.FromAsyncPattern<WebResponse>(request.BeginGetResponse,
                    ar =>
                    {
                        try
                        {
                            return request.EndGetResponse(ar);
                        }
                        catch (WebException ex)
                        {
                            if (ex.Status == WebExceptionStatus.RequestCanceled) return null;
                            throw;
                        }
                    })()
                    .Subscribe(observer);
                return () => request.Abort();
            });
        }

        public static IObservable<Stream> GetRequestStreamAsObservable(this WebRequest request)
        {
            return Observable.Create<Stream>(observer =>
            {
                Observable.FromAsyncPattern<Stream>(request.BeginGetRequestStream,
                    ar =>
                    {
                        try
                        {
                            return request.EndGetRequestStream(ar);
                        }
                        catch (WebException ex)
                        {
                            if (ex.Status == WebExceptionStatus.RequestCanceled) return null;
                            throw;
                        }
                    })()
                    .Subscribe(observer);
                return () => request.Abort();
            });
        }

        public static IObservable<byte[]> DownloadDataAsync(this WebRequest request)
        {
            return request.GetResponseAsObservable().SelectMany(r => r.DownloadDataAsync());
        }

        public static IObservable<byte[]> DownloadDataAsync(this WebRequest request, IProgress<ProgressStatus> notifier)
        {
            return request.GetResponseAsObservable().SelectMany(r => r.DownloadDataAsync(notifier));
        }

        public static IObservable<string> DownloadStringAsync(this WebRequest request)
        {
            return request.GetResponseAsObservable().SelectMany(r => r.DownloadStringAsync());
        }

        public static IObservable<string> DownloadStringAsync(this WebRequest request, Encoding encoding)
        {
            return request.GetResponseAsObservable().SelectMany(r => r.DownloadStringAsync(encoding));
        }

        public static IObservable<string> DownloadStringLineAsync(this WebRequest request)
        {
            return request.GetResponseAsObservable().SelectMany(r => r.DownloadStringLineAsync());
        }

        public static IObservable<string> DownloadStringLineAsync(this WebRequest request, Encoding encoding)
        {
            return request.GetResponseAsObservable().SelectMany(r => r.DownloadStringLineAsync(encoding));
        }

        public static IObservable<WebResponse> UploadStringAsync(this WebRequest request, string data)
        {
            return request.UploadStringAsync(data, Encoding.UTF8);
        }

        public static IObservable<WebResponse> UploadStringAsync(this WebRequest request, string data, Encoding encoding)
        {
            var bytes = encoding.GetBytes(data);
            return request.UploadDataAsync(bytes);
        }

        public static IObservable<WebResponse> UploadStringAsync(this WebRequest request, string data, IProgress<ProgressStatus> progressReporter, int chunkSize = 65536)
        {
            return UploadStringAsync(request, data, Encoding.UTF8, progressReporter, chunkSize);
        }

        public static IObservable<WebResponse> UploadStringAsync(this WebRequest request, string data, Encoding encoding, IProgress<ProgressStatus> progressReporter, int chunkSize = 65536)
        {
            var bytes = encoding.GetBytes(data);
            return request.UploadDataAsync(bytes, progressReporter, chunkSize);
        }

        public static IObservable<WebResponse> UploadValuesAsync(this WebRequest request, IDictionary<string, string> parameters)
        {
            return UploadValuesAsync(request, parameters, Encoding.UTF8);
        }

        public static IObservable<WebResponse> UploadValuesAsync(this WebRequest request, IDictionary<string, string> parameters, Encoding encoding)
        {
            var parameter = parameters.Select(kvp => Uri.EscapeDataString(kvp.Key) + "=" + Uri.EscapeDataString(kvp.Value))
                .Aggregate(new StringBuilder(), (sb, x) => sb.Append(x)).ToString();
            var bytes = encoding.GetBytes(parameter);

            return request.UploadDataAsync(bytes);
        }

        public static IObservable<WebResponse> UploadValuesAsync(this WebRequest request, IDictionary<string, string> parameters, IProgress<ProgressStatus> progressReporter, int chunkSize = 65536)
        {
            return UploadValuesAsync(request, parameters, progressReporter, Encoding.UTF8, chunkSize);
        }

        public static IObservable<WebResponse> UploadValuesAsync(this WebRequest request, IDictionary<string, string> parameters, IProgress<ProgressStatus> progressReporter, Encoding encoding, int chunkSize = 65536)
        {
            var parameter = parameters.Select(kvp => Uri.EscapeDataString(kvp.Key) + "=" + Uri.EscapeDataString(kvp.Value))
               .Aggregate(new StringBuilder(), (sb, x) => sb.Append(x)).ToString();
            var bytes = encoding.GetBytes(parameter);

            return request.UploadDataAsync(bytes, progressReporter, chunkSize);
        }

        public static IObservable<WebResponse> UploadDataAsync(this WebRequest request, byte[] data)
        {
            return request.GetRequestStreamAsObservable()
                .SelectMany(stream => stream.WriteAsObservable(data, 0, data.Length)
                    .Finally(() => { stream.Flush(); stream.Close(); }))
                .TakeLast(1) // call before sequence's finally
                .SelectMany(_ => request.GetResponseAsObservable());
        }

        public static IObservable<WebResponse> UploadDataAsync(this WebRequest request, byte[] data, IProgress<ProgressStatus> progressReporter, int chunkSize = 65536)
        {
            return request.GetRequestStreamAsObservable()
                .SelectMany(stream => stream.WriteAsync(data, progressReporter, chunkSize))
                .SelectMany(_ => request.GetResponseAsObservable());
        }
    }
}