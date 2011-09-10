// <copyright file="StreamExtensionsTest.cs" company="Microsoft">Copyright © Microsoft 2011</copyright>
using System;
using System.IO;
using Codeplex.Reactive.Asynchronous;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;
using System.Reactive.Linq;
using System.Text;
using System.Reactive;
using System.Linq;
using Codeplex.Reactive.Notifier;
using System.Collections.Generic;
using Microsoft.Pex.Engine.Exceptions;

namespace Codeplex.Reactive.Asynchronous
{
    /// <summary>This class contains parameterized unit tests for StreamExtensions</summary>
    [PexClass(typeof(StreamExtensions))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class StreamExtensionsTest
    {
        [PexMethod]
        public void ReadAsObservableAndSubscribe(
            [PexAssumeNotNull]string s
        )
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            var stream = new MemoryStream(bytes);
            var buffer = new byte[bytes.Length];

            IObservable<int> result
               = StreamExtensions.ReadAsObservable(stream, buffer, 0, bytes.Length);
            var disp = result.Single();

            disp.Is(bytes.Length);
            buffer.Is(bytes);
        }

        [PexMethod]
        public void WriteAsObservableAndSubscribe(
            [PexAssumeNotNull]string s
        )
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            var stream = new MemoryStream();

            var result = stream.WriteAsObservable(bytes, 0, bytes.Length);
            var disp = result.Single();

            disp.Is(Unit.Default);
            stream.ToArray().Is(bytes);
        }

        [PexMethod(MaxStack = 200, MaxBranches = 20000)]
        public void WriteAsyncAndSubscribe(
            [PexAssumeNotNull]string s
        )
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            var stream = new MemoryStream();

            stream.WriteAsync(s).Single().Is(Unit.Default);
            stream.ToArray().Is(bytes);
        }

        [PexMethod(MaxStack = 200, MaxBranches = 20000)]
        public void WriteAsyncAndSubscribe2(
            [PexAssumeNotNull]IEnumerable<byte> data,
            int chunkSize
        )
        {
            var stream = new MemoryStream();

            stream.WriteAsync(data, chunkSize).Single().Is(Unit.Default);
            stream.ToArray().Is(data);
        }

        [PexMethod(MaxStack = 200, MaxBranches = 20000)]
        public void WriteLineAsyncAndSubscribe(
            [PexAssumeNotNull]IEnumerable<string> data
        )
        {
            var stream = new MemoryStream();

            stream.WriteLineAsync(data).Single().Is(Unit.Default);
            Encoding.UTF8.GetString(stream.ToArray()).Is(
                string.Join("", data.Select(s => s + Environment.NewLine))
                );
        }

        [PexMethod(MaxStack = 200, MaxBranches = 20000)]
        public void ReadAsyncAndSubscribe(
            [PexAssumeNotNull]string s,
            int chunkSize
        )
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            var stream = new MemoryStream(bytes);

            var xs = stream.ReadAsync(chunkSize).ToEnumerable().ToArray();
            xs.SelectMany(x => x).Is(bytes);
        }

        [PexMethod(MaxStack = 200, MaxBranches = 20000)]
        public void ReadLineAsyncAndSubscribe(
            [PexAssumeNotNull]string s,
            int chunkSize
        )
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            var stream = new MemoryStream(bytes);

            var xs = stream.ReadLineAsync(chunkSize).ToEnumerable().ToArray();
            if (s == "")
            {
                xs.Length.Is(0);
            }
            else if (s == "\r" || s == "\n" || s == "\r\n")
            {
                xs.Is("");
            }
            else
            {
                xs.Is(s.Split(new[] { '\r', '\n' }, StringSplitOptions.None));
            }
        }

        // generated

        /// <summary>Test stub for ReadAsObservable(Stream, Byte[], Int32, Int32)</summary>
        [PexMethod]
        public IObservable<int> ReadAsObservable(
            Stream stream,
            byte[] buffer,
            int offset,
            int count
        )
        {
            IObservable<int> result
               = StreamExtensions.ReadAsObservable(stream, buffer, offset, count);
            return result;
        }

        /// <summary>Test stub for WriteAsObservable(Stream, Byte[], Int32, Int32)</summary>
        [PexMethod]
        public IObservable<Unit> WriteAsObservable(
            Stream stream,
            byte[] buffer,
            int offset,
            int count
        )
        {
            IObservable<Unit> result
               = StreamExtensions.WriteAsObservable(stream, buffer, offset, count);
            return result;
        }

        [PexMethod]
        public IObservable<Unit> WriteAsync07(
            Stream stream,
            IObservable<byte> data,
            IProgress<ProgressStatus> progressReporter,
            int chunkSize
        )
        {
            IObservable<Unit> result = StreamExtensions.WriteAsync(stream, data, progressReporter, chunkSize);
            return result;
        }
        [PexMethod]
        public IObservable<Unit> WriteAsync06(
            Stream stream,
            IObservable<byte> data,
            int chunkSize
        )
        {
            IObservable<Unit> result = StreamExtensions.WriteAsync(stream, data, chunkSize);
            return result;
        }
        [PexMethod]
        public IObservable<Unit> WriteAsync05(
            Stream stream,
            IEnumerable<byte> data,
            IProgress<ProgressStatus> progressReporter,
            int chunkSize
        )
        {
            IObservable<Unit> result = StreamExtensions.WriteAsync(stream, data, progressReporter, chunkSize);
            return result;
        }
        [PexMethod]
        public IObservable<Unit> WriteAsync04(
            Stream stream,
            IEnumerable<byte> data,
            int chunkSize
        )
        {
            IObservable<Unit> result = StreamExtensions.WriteAsync(stream, data, chunkSize);
            return result;
        }
        [PexMethod]
        public IObservable<Unit> WriteAsync03(
            Stream stream,
            string data,
            Encoding encoding,
            IProgress<ProgressStatus> progressReporter
        )
        {
            IObservable<Unit> result = StreamExtensions.WriteAsync(stream, data, encoding, progressReporter);
            return result;
        }
        [PexMethod]
        public IObservable<Unit> WriteAsync02(
            Stream stream,
            string data,
            Encoding encoding
        )
        {
            IObservable<Unit> result = StreamExtensions.WriteAsync(stream, data, encoding);
            return result;
        }
        [PexMethod]
        public IObservable<Unit> WriteAsync01(
            Stream stream,
            string data,
            IProgress<ProgressStatus> progressReporter
        )
        {
            IObservable<Unit> result = StreamExtensions.WriteAsync(stream, data, progressReporter);
            return result;
        }
        [PexMethod]
        public IObservable<Unit> WriteAsync(Stream stream, string data)
        {
            IObservable<Unit> result = StreamExtensions.WriteAsync(stream, data);
            return result;
        }
        [PexMethod]
        public IObservable<Unit> WriteLineAsync03(
            Stream stream,
            IObservable<string> data,
            Encoding encoding
        )
        {
            IObservable<Unit> result = StreamExtensions.WriteLineAsync(stream, data, encoding);
            return result;
        }
        [PexMethod]
        public IObservable<Unit> WriteLineAsync02(
            Stream stream,
            IEnumerable<string> data,
            Encoding encoding
        )
        {
            IObservable<Unit> result = StreamExtensions.WriteLineAsync(stream, data, encoding);
            return result;
        }
        [PexMethod]
        public IObservable<Unit> WriteLineAsync01(Stream stream, IObservable<string> data)
        {
            IObservable<Unit> result = StreamExtensions.WriteLineAsync(stream, data);
            return result;
        }
        [PexMethod]
        public IObservable<Unit> WriteLineAsync(Stream stream, IEnumerable<string> data)
        {
            IObservable<Unit> result = StreamExtensions.WriteLineAsync(stream, data);
            return result;
        }
        [PexMethod]
        public IObservable<byte[]> ReadAsync01(
            Stream stream,
            IProgress<ProgressStatus> progressReporter,
            int totalLength,
            int chunkSize,
            bool isAggregateAllChunks
        )
        {
            IObservable<byte[]> result = StreamExtensions.ReadAsync
                                             (stream, progressReporter, totalLength, chunkSize, isAggregateAllChunks);
            return result;
        }
        [PexMethod]
        public IObservable<byte[]> ReadAsync(
            Stream stream,
            int chunkSize,
            bool isAggregateAllChunks
        )
        {
            IObservable<byte[]> result = StreamExtensions.ReadAsync(stream, chunkSize, isAggregateAllChunks);
            return result;
        }
        [PexMethod]
        public IObservable<string> ReadLineAsync01(
            Stream stream,
            Encoding encoding,
            int chunkSize
        )
        {
            IObservable<string> result = StreamExtensions.ReadLineAsync(stream, encoding, chunkSize);
            return result;
        }
        [PexMethod]
        public IObservable<string> ReadLineAsync(Stream stream, int chunkSize)
        {
            IObservable<string> result = StreamExtensions.ReadLineAsync(stream, chunkSize);
            return result;
        }

        // promote
        [TestMethod]
        public void WriteLineAsyncAndSubscribe824()
        {
            string[] ss = new string[2];
            this.WriteLineAsyncAndSubscribe((IEnumerable<string>)ss);
        }
        [TestMethod]
        public void WriteLineAsyncAndSubscribe916()
        {
            string[] ss = new string[1];
            this.WriteLineAsyncAndSubscribe((IEnumerable<string>)ss);
        }
        [TestMethod]
        public void WriteLineAsyncAndSubscribe143()
        {
            string[] ss = new string[0];
            this.WriteLineAsyncAndSubscribe((IEnumerable<string>)ss);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteLineAsync03ThrowsArgumentNullException453()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<Unit> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                iObservable = this.WriteLineAsync03
                                  ((Stream)memoryStream, (IObservable<string>)null, (Encoding)null);
                disposables.Dispose();
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteLineAsync03ThrowsArgumentNullException133()
        {
            IObservable<Unit> iObservable;
            iObservable =
              this.WriteLineAsync03((Stream)null, (IObservable<string>)null, (Encoding)null);
        }
        [TestMethod]
        public void WriteLineAsync02316()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<Unit> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                string[] ss = new string[0];
                iObservable = this.WriteLineAsync02
                                  ((Stream)memoryStream, (IEnumerable<string>)ss, Encoding.UTF32);
                disposables.Dispose();
                Assert.IsNotNull((object)iObservable);
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteLineAsync02ThrowsArgumentNullException627()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<Unit> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                string[] ss = new string[0];
                iObservable = this.WriteLineAsync02
                                  ((Stream)memoryStream, (IEnumerable<string>)ss, (Encoding)null);
                disposables.Dispose();
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteLineAsync02ThrowsArgumentNullException577()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<Unit> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                iObservable = this.WriteLineAsync02
                                  ((Stream)memoryStream, (IEnumerable<string>)null, (Encoding)null);
                disposables.Dispose();
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteLineAsync02ThrowsArgumentNullException377()
        {
            IObservable<Unit> iObservable;
            iObservable =
              this.WriteLineAsync02((Stream)null, (IEnumerable<string>)null, (Encoding)null);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteLineAsync01ThrowsArgumentNullException487()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<Unit> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                iObservable =
                  this.WriteLineAsync01((Stream)memoryStream, (IObservable<string>)null);
                disposables.Dispose();
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteLineAsync01ThrowsArgumentNullException292()
        {
            IObservable<Unit> iObservable;
            iObservable = this.WriteLineAsync01((Stream)null, (IObservable<string>)null);
        }
        [TestMethod]
        public void WriteLineAsync289()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<Unit> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                string[] ss = new string[0];
                iObservable =
                  this.WriteLineAsync((Stream)memoryStream, (IEnumerable<string>)ss);
                disposables.Dispose();
                Assert.IsNotNull((object)iObservable);
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteLineAsyncThrowsArgumentNullException606()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<Unit> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                iObservable =
                  this.WriteLineAsync((Stream)memoryStream, (IEnumerable<string>)null);
                disposables.Dispose();
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteLineAsyncThrowsArgumentNullException95()
        {
            IObservable<Unit> iObservable;
            iObservable = this.WriteLineAsync((Stream)null, (IEnumerable<string>)null);
        }
        [TestMethod]
        public void WriteAsyncAndSubscribe2619()
        {
            byte[] bs = new byte[9];
            this.WriteAsyncAndSubscribe2((IEnumerable<byte>)bs, 4);
        }
        [TestMethod]
        public void WriteAsyncAndSubscribe2397()
        {
            byte[] bs = new byte[7];
            this.WriteAsyncAndSubscribe2((IEnumerable<byte>)bs, 3);
        }
        [TestMethod]
        public void WriteAsyncAndSubscribe2827()
        {
            byte[] bs = new byte[3];
            this.WriteAsyncAndSubscribe2((IEnumerable<byte>)bs, 2);
        }
        [TestMethod]
        public void WriteAsyncAndSubscribe286()
        {
            byte[] bs = new byte[3];
            this.WriteAsyncAndSubscribe2((IEnumerable<byte>)bs, 512);
        }
        [TestMethod]
        public void WriteAsyncAndSubscribe2216()
        {
            byte[] bs = new byte[2];
            this.WriteAsyncAndSubscribe2((IEnumerable<byte>)bs, 1);
        }
        [TestMethod]
        public void WriteAsyncAndSubscribe2798()
        {
            byte[] bs = new byte[2];
            this.WriteAsyncAndSubscribe2((IEnumerable<byte>)bs, 512);
        }
        [TestMethod]
        public void WriteAsyncAndSubscribe2408()
        {
            byte[] bs = new byte[1];
            this.WriteAsyncAndSubscribe2((IEnumerable<byte>)bs, 1);
        }
        [TestMethod]
        public void WriteAsyncAndSubscribe2938()
        {
            byte[] bs = new byte[1];
            this.WriteAsyncAndSubscribe2((IEnumerable<byte>)bs, 512);
        }
        [TestMethod]
        public void WriteAsyncAndSubscribe2272()
        {
            byte[] bs = new byte[0];
            this.WriteAsyncAndSubscribe2((IEnumerable<byte>)bs, 1);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WriteAsyncAndSubscribe2ThrowsArgumentException849()
        {
            byte[] bs = new byte[0];
            this.WriteAsyncAndSubscribe2((IEnumerable<byte>)bs, 0);
        }
        [TestMethod]
        public void WriteAsyncAndSubscribe929()
        {
            this.WriteAsyncAndSubscribe("\0\u0800\0\0\0\0\0\0\0\0\0\0\0\0\0");
        }
        [TestMethod]
        public void WriteAsyncAndSubscribe418()
        {
            this.WriteAsyncAndSubscribe("\udc00");
        }
        [TestMethod]
        public void WriteAsyncAndSubscribe57()
        {
            this.WriteAsyncAndSubscribe("");
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteAsync07ThrowsArgumentNullException725()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<Unit> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                iObservable =
                  this.WriteAsync07((Stream)memoryStream, (IObservable<byte>)null,
                                    (IProgress<ProgressStatus>)null, 0);
                disposables.Dispose();
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteAsync07ThrowsArgumentNullException716()
        {
            IObservable<Unit> iObservable;
            iObservable = this.WriteAsync07
                              ((Stream)null, (IObservable<byte>)null, (IProgress<ProgressStatus>)null, 0);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteAsync06ThrowsArgumentNullException219()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<Unit> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                iObservable =
                  this.WriteAsync06((Stream)memoryStream, (IObservable<byte>)null, 0);
                disposables.Dispose();
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteAsync06ThrowsArgumentNullException896()
        {
            IObservable<Unit> iObservable;
            iObservable = this.WriteAsync06((Stream)null, (IObservable<byte>)null, 0);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteAsync05ThrowsArgumentNullException726()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<Unit> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                byte[] bs1 = new byte[0];
                iObservable = this.WriteAsync05((Stream)memoryStream, (IEnumerable<byte>)bs1,
                                                (IProgress<ProgressStatus>)null, 0);
                disposables.Dispose();
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteAsync05ThrowsArgumentNullException178()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<Unit> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                iObservable =
                  this.WriteAsync05((Stream)memoryStream, (IEnumerable<byte>)null,
                                    (IProgress<ProgressStatus>)null, 0);
                disposables.Dispose();
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteAsync05ThrowsArgumentNullException529()
        {
            IObservable<Unit> iObservable;
            iObservable = this.WriteAsync05
                              ((Stream)null, (IEnumerable<byte>)null, (IProgress<ProgressStatus>)null, 0);
        }
        [TestMethod]
        public void WriteAsync04186()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<Unit> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                byte[] bs1 = new byte[0];
                iObservable =
                  this.WriteAsync04((Stream)memoryStream, (IEnumerable<byte>)bs1, 1);
                disposables.Dispose();
                Assert.IsNotNull((object)iObservable);
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WriteAsync04ThrowsArgumentException581()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<Unit> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                byte[] bs1 = new byte[0];
                iObservable =
                  this.WriteAsync04((Stream)memoryStream, (IEnumerable<byte>)bs1, 0);
                disposables.Dispose();
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteAsync04ThrowsArgumentNullException826()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<Unit> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                iObservable =
                  this.WriteAsync04((Stream)memoryStream, (IEnumerable<byte>)null, 0);
                disposables.Dispose();
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteAsync04ThrowsArgumentNullException605()
        {
            IObservable<Unit> iObservable;
            iObservable = this.WriteAsync04((Stream)null, (IEnumerable<byte>)null, 0);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteAsync03ThrowsArgumentNullException17()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<Unit> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                iObservable = this.WriteAsync03((Stream)memoryStream, "",
                                                Encoding.UTF32, (IProgress<ProgressStatus>)null);
                disposables.Dispose();
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteAsync03ThrowsArgumentNullException537()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<Unit> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                iObservable = this.WriteAsync03((Stream)memoryStream, "",
                                                (Encoding)null, (IProgress<ProgressStatus>)null);
                disposables.Dispose();
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteAsync03ThrowsArgumentNullException285()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<Unit> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                iObservable = this.WriteAsync03((Stream)memoryStream, (string)null,
                                                (Encoding)null, (IProgress<ProgressStatus>)null);
                disposables.Dispose();
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteAsync03ThrowsArgumentNullException551()
        {
            IObservable<Unit> iObservable;
            iObservable = this.WriteAsync03((Stream)null, (string)null,
                                            (Encoding)null, (IProgress<ProgressStatus>)null);
        }
        [TestMethod]
        public void WriteAsync02589()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<Unit> iObservable;
                byte[] bs = new byte[3];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                iObservable =
                  this.WriteAsync02((Stream)memoryStream, "\ud801\0\0", Encoding.UTF8);
                disposables.Dispose();
                Assert.IsNotNull((object)iObservable);
            }
        }
        [TestMethod]
        public void WriteAsync02915()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<Unit> iObservable;
                byte[] bs = new byte[1];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                iObservable = this.WriteAsync02((Stream)memoryStream, "\udc00", Encoding.UTF8);
                disposables.Dispose();
                Assert.IsNotNull((object)iObservable);
            }
        }
        [TestMethod]
        public void WriteAsync02195()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<Unit> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                iObservable = this.WriteAsync02((Stream)memoryStream, "", Encoding.UTF8);
                disposables.Dispose();
                Assert.IsNotNull((object)iObservable);
            }
        }
        [TestMethod]
        public void WriteAsync02104()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<Unit> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                iObservable = this.WriteAsync02((Stream)memoryStream, "", Encoding.UTF32);
                disposables.Dispose();
                Assert.IsNotNull((object)iObservable);
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteAsync02ThrowsArgumentNullException268()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<Unit> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                iObservable = this.WriteAsync02((Stream)memoryStream, "", (Encoding)null);
                disposables.Dispose();
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteAsync02ThrowsArgumentNullException566()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<Unit> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                iObservable =
                  this.WriteAsync02((Stream)memoryStream, (string)null, (Encoding)null);
                disposables.Dispose();
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteAsync02ThrowsArgumentNullException967()
        {
            IObservable<Unit> iObservable;
            iObservable = this.WriteAsync02((Stream)null, (string)null, (Encoding)null);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteAsync01ThrowsArgumentNullException32()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<Unit> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                iObservable =
                  this.WriteAsync01((Stream)memoryStream, "", (IProgress<ProgressStatus>)null);
                disposables.Dispose();
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteAsync01ThrowsArgumentNullException260()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<Unit> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                iObservable = this.WriteAsync01
                                  ((Stream)memoryStream, (string)null, (IProgress<ProgressStatus>)null);
                disposables.Dispose();
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteAsync01ThrowsArgumentNullException769()
        {
            IObservable<Unit> iObservable;
            iObservable =
              this.WriteAsync01((Stream)null, (string)null, (IProgress<ProgressStatus>)null);
        }
        [TestMethod]
        public void WriteAsync773()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<Unit> iObservable;
                byte[] bs = new byte[1];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                iObservable = this.WriteAsync((Stream)memoryStream, "\udc00");
                disposables.Dispose();
                Assert.IsNotNull((object)iObservable);
            }
        }
        [TestMethod]
        public void WriteAsync319()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<Unit> iObservable;
                byte[] bs = new byte[1];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                iObservable = this.WriteAsync((Stream)memoryStream, "\0");
                disposables.Dispose();
                Assert.IsNotNull((object)iObservable);
            }
        }
        [TestMethod]
        public void WriteAsync141()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<Unit> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                iObservable = this.WriteAsync((Stream)memoryStream, "");
                disposables.Dispose();
                Assert.IsNotNull((object)iObservable);
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteAsyncThrowsArgumentNullException669()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<Unit> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                iObservable = this.WriteAsync((Stream)memoryStream, (string)null);
                disposables.Dispose();
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteAsyncThrowsArgumentNullException966()
        {
            IObservable<Unit> iObservable;
            iObservable = this.WriteAsync((Stream)null, (string)null);
        }
        [TestMethod]
        public void WriteAsObservableAndSubscribe950()
        {
            this.WriteAsObservableAndSubscribe("\ud801\0\0");
        }
        [TestMethod]
        public void WriteAsObservableAndSubscribe314()
        {
            this.WriteAsObservableAndSubscribe("\0\u0080\0\0\0\0\0\0\0\0\0\0\0\0\0");
        }
        [TestMethod]
        public void WriteAsObservableAndSubscribe418()
        {
            this.WriteAsObservableAndSubscribe("\udc00");
        }
        [TestMethod]
        public void WriteAsObservableAndSubscribe295()
        {
            this.WriteAsObservableAndSubscribe("\0");
        }
        [TestMethod]
        public void WriteAsObservableAndSubscribe57()
        {
            this.WriteAsObservableAndSubscribe("");
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WriteAsObservableThrowsArgumentException930()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<Unit> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                byte[] bs1 = new byte[0];
                iObservable = this.WriteAsObservable((Stream)memoryStream, bs1, 254, 771);
                disposables.Dispose();
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void WriteAsObservableThrowsArgumentOutOfRangeException747()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<Unit> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                byte[] bs1 = new byte[0];
                iObservable =
                  this.WriteAsObservable((Stream)memoryStream, bs1, int.MinValue, 0);
                disposables.Dispose();
            }
        }
        [TestMethod]
        public void WriteAsObservable49301()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<Unit> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, PexSafeHelpers.ByteToBoolean((byte)128));
                disposables.Add((IDisposable)memoryStream);
                byte[] bs1 = new byte[0];
                iObservable = this.WriteAsObservable((Stream)memoryStream, bs1, 0, 0);
                disposables.Dispose();
                Assert.IsNotNull((object)iObservable);
            }
        }
        [TestMethod]
        public void WriteAsObservable493()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<Unit> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                byte[] bs1 = new byte[0];
                iObservable = this.WriteAsObservable((Stream)memoryStream, bs1, 0, 0);
                disposables.Dispose();
                Assert.IsNotNull((object)iObservable);
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteAsObservableThrowsArgumentNullException428()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<Unit> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                iObservable = this.WriteAsObservable((Stream)memoryStream, (byte[])null, 0, 0);
                disposables.Dispose();
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteAsObservableThrowsArgumentNullException168()
        {
            IObservable<Unit> iObservable;
            iObservable = this.WriteAsObservable((Stream)null, (byte[])null, 0, 0);
        }
        [TestMethod]
        public void ReadLineAsyncAndSubscribe16()
        {
            this.ReadLineAsyncAndSubscribe("\0", 1);
        }
        [TestMethod]
        public void ReadLineAsyncAndSubscribe111()
        {
            this.ReadLineAsyncAndSubscribe("\r", 1);
        }
        [TestMethod]
        public void ReadLineAsyncAndSubscribe499()
        {
            this.ReadLineAsyncAndSubscribe("\n", 1);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ReadLineAsyncAndSubscribeThrowsArgumentException217()
        {
            this.ReadLineAsyncAndSubscribe("\udc00", 0);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ReadLineAsyncAndSubscribeThrowsArgumentException543()
        {
            this.ReadLineAsyncAndSubscribe("\ud801", 0);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ReadLineAsyncAndSubscribeThrowsArgumentException420()
        {
            this.ReadLineAsyncAndSubscribe("\0", 0);
        }
        [TestMethod]
        public void ReadLineAsyncAndSubscribe936()
        {
            this.ReadLineAsyncAndSubscribe("", 1);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ReadLineAsyncAndSubscribeThrowsArgumentException646()
        {
            this.ReadLineAsyncAndSubscribe("", 0);
        }
        [TestMethod]
        public void ReadLineAsync01246()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<string> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                iObservable = this.ReadLineAsync01((Stream)memoryStream, (Encoding)null, 1);
                disposables.Dispose();
                Assert.IsNotNull((object)iObservable);
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ReadLineAsync01ThrowsArgumentException501()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<string> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                iObservable = this.ReadLineAsync01((Stream)memoryStream, (Encoding)null, 0);
                disposables.Dispose();
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadLineAsync01ThrowsArgumentNullException451()
        {
            IObservable<string> iObservable;
            iObservable = this.ReadLineAsync01((Stream)null, (Encoding)null, 0);
        }
        [TestMethod]
        public void ReadLineAsync563()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<string> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                iObservable = this.ReadLineAsync((Stream)memoryStream, 2);
                disposables.Dispose();
                Assert.IsNotNull((object)iObservable);
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ReadLineAsyncThrowsArgumentException371()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<string> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                iObservable = this.ReadLineAsync((Stream)memoryStream, 0);
                disposables.Dispose();
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadLineAsyncThrowsArgumentNullException196()
        {
            IObservable<string> iObservable;
            iObservable = this.ReadLineAsync((Stream)null, 0);
        }
        [TestMethod]
        public void ReadAsyncAndSubscribe33()
        {
            this.ReadAsyncAndSubscribe("\0", 2);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ReadAsyncAndSubscribeThrowsArgumentException564()
        {
            this.ReadAsyncAndSubscribe("\0\udc00\0\0\0\0\0\0\0\0\0\0\0\0\0", 0);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ReadAsyncAndSubscribeThrowsArgumentException924()
        {
            this.ReadAsyncAndSubscribe("\ud801", 0);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ReadAsyncAndSubscribeThrowsArgumentException405()
        {
            this.ReadAsyncAndSubscribe(new string('\0', 15), 0);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ReadAsyncAndSubscribeThrowsArgumentException746()
        {
            this.ReadAsyncAndSubscribe("\udc00", 0);
        }
        [TestMethod]
        public void ReadAsyncAndSubscribe936()
        {
            this.ReadAsyncAndSubscribe("", 1);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ReadAsyncAndSubscribeThrowsArgumentException943()
        {
            this.ReadAsyncAndSubscribe("", 0);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadAsync01ThrowsArgumentNullException826()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<byte[]> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                iObservable = this.ReadAsync01
                                  ((Stream)memoryStream, (IProgress<ProgressStatus>)null, 0, 1, false);
                disposables.Dispose();
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ReadAsync01ThrowsArgumentException522()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<byte[]> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                iObservable = this.ReadAsync01
                                  ((Stream)memoryStream, (IProgress<ProgressStatus>)null, 0, 0, false);
                disposables.Dispose();
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadAsync01ThrowsArgumentNullException910()
        {
            IObservable<byte[]> iObservable;
            iObservable =
              this.ReadAsync01((Stream)null, (IProgress<ProgressStatus>)null, 0, 0, false);
        }
        [TestMethod]
        public void ReadAsync977()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<byte[]> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                iObservable = this.ReadAsync((Stream)memoryStream, 2, true);
                disposables.Dispose();
                Assert.IsNotNull((object)iObservable);
            }
        }
        [TestMethod]
        public void ReadAsync742()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<byte[]> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                iObservable = this.ReadAsync((Stream)memoryStream, 2, false);
                disposables.Dispose();
                Assert.IsNotNull((object)iObservable);
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ReadAsyncThrowsArgumentException179()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<byte[]> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                iObservable = this.ReadAsync((Stream)memoryStream, 0, false);
                disposables.Dispose();
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadAsyncThrowsArgumentNullException418()
        {
            IObservable<byte[]> iObservable;
            iObservable = this.ReadAsync((Stream)null, 0, false);
        }
        [TestMethod]
        public void ReadAsObservableAndSubscribe22()
        {
            this.ReadAsObservableAndSubscribe("\ud9c0\udc00");
        }
        [TestMethod]
        public void ReadAsObservableAndSubscribe950()
        {
            this.ReadAsObservableAndSubscribe("\ud801\0\0");
        }
        [TestMethod]
        public void ReadAsObservableAndSubscribe418()
        {
            this.ReadAsObservableAndSubscribe("\udc00");
        }
        [TestMethod]
        public void ReadAsObservableAndSubscribe57()
        {
            this.ReadAsObservableAndSubscribe("");
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ReadAsObservableThrowsArgumentOutOfRangeException879()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<int> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                byte[] bs1 = new byte[0];
                iObservable =
                  this.ReadAsObservable((Stream)memoryStream, bs1, int.MinValue, 0);
                disposables.Dispose();
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ReadAsObservableThrowsArgumentException674()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<int> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                byte[] bs1 = new byte[0];
                iObservable = this.ReadAsObservable((Stream)memoryStream, bs1, 254, 771);
                disposables.Dispose();
            }
        }
        [TestMethod]
        public void ReadAsObservable493()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<int> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                byte[] bs1 = new byte[0];
                iObservable = this.ReadAsObservable((Stream)memoryStream, bs1, 0, 0);
                disposables.Dispose();
                Assert.IsNotNull((object)iObservable);
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadAsObservableThrowsArgumentNullException73()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                MemoryStream memoryStream;
                IObservable<int> iObservable;
                byte[] bs = new byte[0];
                memoryStream = new MemoryStream(bs, false);
                disposables.Add((IDisposable)memoryStream);
                iObservable = this.ReadAsObservable((Stream)memoryStream, (byte[])null, 0, 0);
                disposables.Dispose();
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadAsObservableThrowsArgumentNullException95()
        {
            IObservable<int> iObservable;
            iObservable = this.ReadAsObservable((Stream)null, (byte[])null, 0, 0);
        }
    }
}
