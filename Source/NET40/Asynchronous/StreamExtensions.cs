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
using Codeplex.Reactive.Notifier;
using System.Diagnostics.Contracts;
#endif

namespace Codeplex.Reactive.Asynchronous
{
    public static class StreamExtensions
    {
        public static IObservable<Unit> WriteAsObservable(this Stream stream, byte[] buffer, int offset, int count)
        {
            Contract.Requires<ArgumentNullException>(stream != (Stream)null);
            Contract.Requires<ArgumentNullException>(buffer != null);
            Contract.Requires<ArgumentOutOfRangeException>(offset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(count >= 0);
            Contract.Requires<ArgumentException>(count <= ((int)(buffer.Length) - offset));
            Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

            var result = Observable.FromAsyncPattern((ac, o) => stream.BeginWrite(buffer, offset, count, ac, o), stream.EndWrite)();

            Contract.Assume(result != null);
            return result;
        }

        public static IObservable<int> ReadAsObservable(this Stream stream, byte[] buffer, int offset, int count)
        {
            Contract.Requires<ArgumentNullException>(stream != (Stream)null);
            Contract.Requires<ArgumentNullException>(buffer != null);
            Contract.Requires<ArgumentOutOfRangeException>(offset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(count >= 0);
            Contract.Requires<ArgumentException>(count <= ((int)(buffer.Length) - offset));
            Contract.Ensures(Contract.Result<IObservable<int>>() != null);

            var result = Observable.FromAsyncPattern<int>((ac, o) => stream.BeginRead(buffer, offset, count, ac, o), stream.EndRead)();

            Contract.Assume(result != null);
            return result;
        }

        public static IObservable<Unit> WriteAsync(this Stream stream, string data)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

            return WriteAsync(stream, data, Encoding.UTF8);
        }

        public static IObservable<Unit> WriteAsync(this Stream stream, string data, IProgress<ProgressStatus> progressReporter)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentNullException>(progressReporter != null);
            Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

            return WriteAsync(stream, data, Encoding.UTF8, progressReporter);
        }

        public static IObservable<Unit> WriteAsync(this Stream stream, string data, Encoding encoding)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentNullException>(encoding != null);
            Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

            return WriteAsync(stream, encoding.GetBytes(data));
        }

        public static IObservable<Unit> WriteAsync(this Stream stream, string data, Encoding encoding, IProgress<ProgressStatus> progressReporter)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentNullException>(encoding != null);
            Contract.Requires<ArgumentNullException>(progressReporter != null);
            Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

            return WriteAsync(stream, encoding.GetBytes(data), progressReporter);
        }

        public static IObservable<Unit> WriteAsync(this Stream stream, IEnumerable<byte> data, int chunkSize = 65536)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentException>(chunkSize > 0);
            Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

            var dataOb = data.ToObservable();
            Contract.Assume(dataOb != null);
            return WriteAsync(stream, dataOb, chunkSize);
        }

        public static IObservable<Unit> WriteAsync(this Stream stream, IEnumerable<byte> data, IProgress<ProgressStatus> progressReporter, int chunkSize = 65536)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentNullException>(progressReporter != null);
            Contract.Requires<ArgumentException>(chunkSize > 0);
            Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

            var collection = data as ICollection<byte>;
            var totalLength = (collection != null) ? collection.Count : -1;

            var dataOb = data.ToObservable();
            Contract.Assume(dataOb != null);
            return WriteAsyncCore(stream, dataOb, progressReporter, chunkSize, totalLength);
        }

        public static IObservable<Unit> WriteAsync(this Stream stream, IObservable<byte> data, int chunkSize = 65536)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentException>(chunkSize > 0);
            Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

            return WriteAsyncCore(stream, data, null, chunkSize, -1);
        }

        public static IObservable<Unit> WriteAsync(this Stream stream, IObservable<byte> data, IProgress<ProgressStatus> progressReporter, int chunkSize = 65536)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentNullException>(progressReporter != null);
            Contract.Requires<ArgumentException>(chunkSize > 0);
            Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

            return WriteAsyncCore(stream, data, progressReporter, chunkSize, -1);
        }

        static IObservable<Unit> WriteAsyncCore(Stream stream, IObservable<byte> data, IProgress<ProgressStatus> progressReporter, int chunkSize, int totalLength)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentException>(chunkSize > 0);
            Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

            var result = Observable.Defer(() =>
                {
                    Report(progressReporter, 0, totalLength);
                    return data;
                })
                .Buffer(chunkSize)
                .Select(l => stream.WriteAsObservable(l.ToArray(), 0, l.Count)
                    .Do(_ => Report(progressReporter, stream.Length, totalLength)))
                .Concat()
                .Finally(() => { stream.Flush(); stream.Close(); })
                .TakeLast(1);

            Contract.Assume(result != null);
            return result;
        }

        public static IObservable<Unit> WriteLineAsync(this Stream stream, string data)
        {
            return WriteLineAsync(stream, data, Encoding.UTF8);
        }

        public static IObservable<Unit> WriteLineAsync(this Stream stream, string data, Encoding encoding)
        {
            return WriteAsync(stream, data + Environment.NewLine, encoding);
        }

        public static IObservable<Unit> WriteLineAsync(this Stream stream, IEnumerable<string> data)
        {
            return WriteLineAsync(stream, data, Encoding.UTF8);
        }

        public static IObservable<Unit> WriteLineAsync(this Stream stream, IObservable<string> data)
        {
            return WriteLineAsync(stream, data, Encoding.UTF8);
        }

        public static IObservable<Unit> WriteLineAsync(this Stream stream, IEnumerable<string> data, Encoding encoding)
        {
            return WriteLineAsync(stream, data.ToObservable(), encoding);
        }

        public static IObservable<Unit> WriteLineAsync(this Stream stream, IObservable<string> data, Encoding encoding)
        {
            return WriteAsync(stream, data.SelectMany(s => encoding.GetBytes(s + Environment.NewLine)));
        }

        public static IObservable<byte[]> ReadAsync(this Stream stream, int chunkSize = 65536)
        {
            return Observable.Defer(() => Observable.Return(new byte[chunkSize], Scheduler.CurrentThread))
                .SelectMany(buffer => stream.ReadAsObservable(buffer, 0, chunkSize),
                    (buffer, readCount) => new { buffer, readCount })
                .Repeat()
                .TakeWhile(a => a.readCount != 0)
                .Select(a =>
                {
                    if (a.readCount == chunkSize) return a.buffer;

                    var newBuffer = new byte[a.readCount];
                    Array.Copy(a.buffer, newBuffer, a.readCount);
                    return newBuffer;
                })
                .Finally(() => stream.Close());
        }

        public static IObservable<string> ReadLineAsync(this Stream stream, int chunkSize = 65536)
        {
            return ReadLineAsync(stream, Encoding.UTF8, chunkSize);
        }

        public static IObservable<string> ReadLineAsync(this Stream stream, Encoding encoding, int chunkSize = 65536)
        {
            return ObservableEx.Create<string>(observer =>
            {
                var decoder = encoding.GetDecoder();
                var bom = encoding.GetChars(encoding.GetPreamble()).FirstOrDefault();
                var sb = new StringBuilder();
                var prev = default(char);

                return stream.ReadAsync(chunkSize)
                    .SelectMany(bytes =>
                    {
                        var charBuffer = new char[encoding.GetMaxCharCount(bytes.Length)];
                        var count = decoder.GetChars(bytes, 0, bytes.Length, charBuffer, 0);
                        return charBuffer.Take(count);
                    })
                    .Subscribe(
                        c =>
                        {
                            if (c == bom) { } // skip bom
                            else if (prev == '\r' && c == '\n') { } // when \r\n do nothing
                            else if (c == '\r' || c == '\n')   // reach at EndOfLine
                            {
                                var str = sb.ToString();
                                sb.Length = 0;
                                observer.OnNext(str);
                            }
                            else sb.Append(c); // normally char

                            prev = c;
                        },
                        observer.OnError,
                        () =>
                        {
                            var str = sb.ToString();
                            if (str != "") observer.OnNext(str);
                            observer.OnCompleted();
                        });
            });
        }

        // report helper
        static void Report(IProgress<ProgressStatus> reporter, long currentLength, long totalLength)
        {
            if (reporter != null) reporter.Report(new ProgressStatus(currentLength, totalLength));
        }
    }
}