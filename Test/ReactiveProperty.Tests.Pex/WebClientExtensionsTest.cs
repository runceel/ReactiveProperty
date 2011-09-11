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
        [PexMethod(MaxConditions = 10000, MaxBranches = 20000)]
        public IObservable<AsyncCompletedEventArgs> DownloadFileObservableAsync02(
            WebClient client,
            Uri address,
            string fileName,
            IProgress<DownloadProgressChangedEventArgs> progress
        )
        {
            IObservable<AsyncCompletedEventArgs> result
               = WebClientExtensions.DownloadFileObservableAsync(client, address, fileName, progress);
            result.Subscribe();
            return result;
        }
        [PexMethod(MaxConditions = 10000, MaxBranches = 20000)]
        public IObservable<AsyncCompletedEventArgs> DownloadFileObservableAsync01(
            WebClient client,
            Uri address,
            string fileName
        )
        {
            IObservable<AsyncCompletedEventArgs> result
               = WebClientExtensions.DownloadFileObservableAsync(client, address, fileName);
            result.Subscribe();
            return result;
        }
        [PexMethod(MaxConditions = 10000, MaxBranches = 20000)]
        public IObservable<AsyncCompletedEventArgs> DownloadFileObservableAsync(
            WebClient client,
            string address,
            string fileName
        )
        {
            IObservable<AsyncCompletedEventArgs> result
               = WebClientExtensions.DownloadFileObservableAsync(client, address, fileName);
            result.Subscribe();
            return result;
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DownloadFileObservableAsyncThrowsArgumentNullException75()
        {
            IObservable<AsyncCompletedEventArgs> iObservable;
            iObservable =
              this.DownloadFileObservableAsync((WebClient)null, (string)null, (string)null);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DownloadFileObservableAsync01ThrowsArgumentNullException422()
        {
            IObservable<AsyncCompletedEventArgs> iObservable;
            iObservable =
              this.DownloadFileObservableAsync01((WebClient)null, (Uri)null, (string)null);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DownloadFileObservableAsync02ThrowsArgumentNullException222()
        {
            IObservable<AsyncCompletedEventArgs> iObservable;
            iObservable = this.DownloadFileObservableAsync02((WebClient)null, (Uri)null,
                                                             (string)null, (IProgress<DownloadProgressChangedEventArgs>)null);
        }
    }
}
