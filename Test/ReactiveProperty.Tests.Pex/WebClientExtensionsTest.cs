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
using System.IO;
using System.Reactive;

namespace Codeplex.Reactive.Asynchronous
{
    [TestClass]
    [PexClass(typeof(WebClientExtensions))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class WebClientExtensionsTest
    {
        [PexMethod]
        public IObservable<byte[]> DownloadDataObservableAsync02(
            WebClient client,
            Uri address,
            IProgress<DownloadProgressChangedEventArgs> progress
        )
        {
            IObservable<byte[]> result
               = WebClientExtensions.DownloadDataObservableAsync(client, address, progress);
            return result;
        }
        [PexMethod]
        public IObservable<byte[]> DownloadDataObservableAsync01(WebClient client, Uri address)
        {
            IObservable<byte[]> result
               = WebClientExtensions.DownloadDataObservableAsync(client, address);
            return result;
        }
        [PexMethod(MaxConditions = 2000)]
        public IObservable<byte[]> DownloadDataObservableAsync(WebClient client, string address)
        {
            IObservable<byte[]> result
               = WebClientExtensions.DownloadDataObservableAsync(client, address);
            return result;
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DownloadDataObservableAsyncThrowsArgumentNullException863()
        {
            IObservable<byte[]> iObservable;
            iObservable = this.DownloadDataObservableAsync((WebClient)null, (string)null);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DownloadDataObservableAsync01ThrowsArgumentNullException259()
        {
            IObservable<byte[]> iObservable;
            iObservable = this.DownloadDataObservableAsync01((WebClient)null, (Uri)null);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DownloadDataObservableAsync02ThrowsArgumentNullException420()
        {
            IObservable<byte[]> iObservable;
            iObservable = this.DownloadDataObservableAsync02((WebClient)null,
                                                             (Uri)null, (IProgress<DownloadProgressChangedEventArgs>)null);
        }
        [PexMethod(MaxConditions = 10000, MaxBranches = 20000)]
        public IObservable<Unit> DownloadFileObservableAsync02(
            WebClient client,
            Uri address,
            string fileName,
            IProgress<DownloadProgressChangedEventArgs> progress
        )
        {
            IObservable<Unit> result
               = WebClientExtensions.DownloadFileObservableAsync(client, address, fileName, progress);
            result.Subscribe();
            return result;
        }
        [PexMethod(MaxConditions = 10000, MaxBranches = 20000)]
        public IObservable<Unit> DownloadFileObservableAsync01(
            WebClient client,
            Uri address,
            string fileName
        )
        {
            IObservable<Unit> result
               = WebClientExtensions.DownloadFileObservableAsync(client, address, fileName);
            result.Subscribe();
            return result;
        }
        [PexMethod(MaxConditions = 10000, MaxBranches = 20000)]
        public IObservable<Unit> DownloadFileObservableAsync(
            WebClient client,
            string address,
            string fileName
        )
        {
            IObservable<Unit> result
               = WebClientExtensions.DownloadFileObservableAsync(client, address, fileName);
            result.Subscribe();
            return result;
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DownloadFileObservableAsyncThrowsArgumentNullException75()
        {
            IObservable<Unit> iObservable;
            iObservable =
              this.DownloadFileObservableAsync((WebClient)null, (string)null, (string)null);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DownloadFileObservableAsync01ThrowsArgumentNullException422()
        {
            IObservable<Unit> iObservable;
            iObservable =
              this.DownloadFileObservableAsync01((WebClient)null, (Uri)null, (string)null);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DownloadFileObservableAsync02ThrowsArgumentNullException222()
        {
            IObservable<Unit> iObservable;
            iObservable = this.DownloadFileObservableAsync02((WebClient)null, (Uri)null,
                                                             (string)null, (IProgress<DownloadProgressChangedEventArgs>)null);
        }
        [PexMethod(MaxConditions = 10000, MaxBranches = 20000), PexAllowedException(typeof(UriFormatException))]
        public IObservable<string> DownloadStringObservableAsync(WebClient client, string address)
        {
            IObservable<string> result
               = WebClientExtensions.DownloadStringObservableAsync(client, address);
            result.Subscribe().IsNotNull();
            return result;
        }
        [PexMethod(MaxConditions = 10000, MaxBranches = 20000)]
        public IObservable<string> DownloadStringObservableAsync01(
            WebClient client,
            Uri address,
            IProgress<DownloadProgressChangedEventArgs> progress
        )
        {
            IObservable<string> result
               = WebClientExtensions.DownloadStringObservableAsync(client, address, progress);
            result.Subscribe().IsNotNull();
            return result;
        }
        [PexMethod(MaxConditions = 10000, MaxBranches = 20000)]
        public IObservable<string> DownloadStringObservableAsync(WebClient client, Uri address)
        {
            IObservable<string> result
               = WebClientExtensions.DownloadStringObservableAsync(client, address);
            result.Subscribe().IsNotNull();
            return result;
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DownloadStringObservableAsyncThrowsArgumentNullException936()
        {
            IObservable<string> iObservable;
            iObservable = this.DownloadStringObservableAsync((WebClient)null, (string)null);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DownloadStringObservableAsync01ThrowsArgumentNullException266()
        {
            IObservable<string> iObservable;
            iObservable = this.DownloadStringObservableAsync01((WebClient)null,
                                                               (Uri)null, (IProgress<DownloadProgressChangedEventArgs>)null);
        }
        [PexMethod(MaxConditions = 10000, MaxBranches = 20000)]
        public IObservable<Stream> OpenReadObservableAsync01(WebClient client, Uri address)
        {
            IObservable<Stream> result
               = WebClientExtensions.OpenReadObservableAsync(client, address);
            return result;
        }
        [PexMethod(MaxConditions = 10000, MaxBranches = 20000)]
        public IObservable<Stream> OpenReadObservableAsync(WebClient client, string address)
        {
            IObservable<Stream> result
               = WebClientExtensions.OpenReadObservableAsync(client, address);
            return result;
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OpenReadObservableAsyncThrowsArgumentNullException364()
        {
            IObservable<Stream> iObservable;
            iObservable = this.OpenReadObservableAsync((WebClient)null, (string)null);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OpenReadObservableAsync01ThrowsArgumentNullException141()
        {
            IObservable<Stream> iObservable;
            iObservable = this.OpenReadObservableAsync01((WebClient)null, (Uri)null);
        }
        [PexMethod(MaxConditions = 10000, MaxBranches = 20000)]
        public IObservable<Stream> OpenWriteObservableAsync(
            WebClient client,
            string address,
            string method
        )
        {
            IObservable<Stream> result
               = WebClientExtensions.OpenWriteObservableAsync(client, address, method);
            result.Subscribe().IsNotNull();
            return result;
        }
        [PexMethod(MaxConditions = 10000, MaxBranches = 20000)]
        public IObservable<Stream> OpenWriteObservableAsync01(
            WebClient client,
            Uri address,
            string method
        )
        {
            IObservable<Stream> result
               = WebClientExtensions.OpenWriteObservableAsync(client, address, method);
            result.Subscribe().IsNotNull();
            return result;
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OpenWriteObservableAsync01ThrowsArgumentNullException473()
        {
            IObservable<Stream> iObservable;
            iObservable =
              this.OpenWriteObservableAsync01((WebClient)null, (Uri)null, (string)null);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OpenWriteObservableAsyncThrowsArgumentNullException109()
        {
            IObservable<Stream> iObservable;
            iObservable =
              this.OpenWriteObservableAsync((WebClient)null, (string)null, (string)null);
        }
    }
}