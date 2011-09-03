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
            return ObservableEx.SafeFromAsyncPattern<WebResponse>(request.BeginGetResponse, request.EndGetResponse);
        }

        public static IObservable<Stream> GetRequestStreamAsObservable(this WebRequest request)
        {
            return ObservableEx.SafeFromAsyncPattern<Stream>(request.BeginGetRequestStream, request.EndGetRequestStream);
        }
    }
}
