// <copyright file="WebClientExtensionsTest.cs" company="Microsoft">Copyright © Microsoft 2011</copyright>

using System;
using Codeplex.Reactive.Asynchronous;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using Codeplex.Reactive.Notifier;
using Microsoft.Pex.Framework.Generated;
using System.Collections.Specialized;
using System.Net.Cache;
using System.ComponentModel;

namespace Codeplex.Reactive.Asynchronous
{
    [TestClass]
    [PexClass(typeof(WebClientExtensions))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class WebClientExtensionsTest
    {
        [PexMethod]
        public IObservable<DownloadDataCompletedEventArgs> DownloadDataObservableAsync02(
            WebClient client,
            Uri address,
            IProgress<DownloadProgressChangedEventArgs> progress
        )
        {
            IObservable<DownloadDataCompletedEventArgs> result
               = WebClientExtensions.DownloadDataObservableAsync(client, address, progress);
            return result;
        }
        [PexMethod]
        public IObservable<DownloadDataCompletedEventArgs> DownloadDataObservableAsync01(WebClient client, Uri address)
        {
            IObservable<DownloadDataCompletedEventArgs> result
               = WebClientExtensions.DownloadDataObservableAsync(client, address);
            return result;
        }
        [PexMethod(MaxConditions = 2000)]
        public IObservable<DownloadDataCompletedEventArgs> DownloadDataObservableAsync(WebClient client, string address)
        {
            IObservable<DownloadDataCompletedEventArgs> result
               = WebClientExtensions.DownloadDataObservableAsync(client, address);
            return result;
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DownloadDataObservableAsyncThrowsArgumentNullException863()
        {
            IObservable<DownloadDataCompletedEventArgs> iObservable;
            iObservable = this.DownloadDataObservableAsync((WebClient)null, (string)null);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DownloadDataObservableAsync01ThrowsArgumentNullException259()
        {
            IObservable<DownloadDataCompletedEventArgs> iObservable;
            iObservable = this.DownloadDataObservableAsync01((WebClient)null, (Uri)null);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DownloadDataObservableAsync02ThrowsArgumentNullException420()
        {
            IObservable<DownloadDataCompletedEventArgs> iObservable;
            iObservable = this.DownloadDataObservableAsync02((WebClient)null,
                                                             (Uri)null, (IProgress<DownloadProgressChangedEventArgs>)null);
        }
    }
}
