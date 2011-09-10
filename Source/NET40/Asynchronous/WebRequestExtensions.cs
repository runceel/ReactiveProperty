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
#endif

namespace Codeplex.Reactive.Asynchronous
{
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
    }
}