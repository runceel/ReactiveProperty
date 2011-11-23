using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Codeplex.Reactive.Notifiers;
using System.Diagnostics.Contracts;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
using SerialDisposable = Microsoft.Phone.Reactive.MutableDisposable;
#else
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
#endif

namespace Codeplex.Reactive.Asynchronous
{
    public static class StreamExtensions
    {
        /// <summary>
        /// <para>BeginWrite-EndWrite Obserable Wrapper.</para>
        /// <para>Run immediately, Length of return value is always 1.</para>
        /// </summary>
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

        /// <summary>
        /// <para>BeginRead-EndRead Obserable Wrapper.</para>
        /// <para>Run immediately, Length of return value is always 1.</para>
        /// </summary>
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

        /// <summary>
        /// <para>Write string(Encode to UTF8) async.</para>
        /// <para>Run deferred, Length of return value is always 1.</para>
        /// </summary>
        public static IObservable<Unit> WriteAsync(this Stream stream, string data)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

            return WriteAsync(stream, data, Encoding.UTF8);
        }

        /// <summary>
        /// <para>Write string(Encode to UTF8) async.</para>
        /// <para>Run deferred, Length of return value is always 1.</para>
        /// </summary>
        /// <param name="progressReporter">Reporter of progress(such as ScheduledNotifier).</param>
        /// <returns>Length is always 1</returns>
        public static IObservable<Unit> WriteAsync(this Stream stream, string data, IProgress<ProgressStatus> progressReporter)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentNullException>(progressReporter != null);
            Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

            return WriteAsync(stream, data, Encoding.UTF8, progressReporter);
        }

        /// <summary>
        /// <para>Write string async.</para>
        /// <para>Run deferred, Length of return value is always 1.</para>
        /// </summary>
        /// <returns>Length is always 1</returns>
        public static IObservable<Unit> WriteAsync(this Stream stream, string data, Encoding encoding)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentNullException>(encoding != null);
            Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

            return WriteAsync(stream, encoding.GetBytes(data));
        }

        /// <summary>
        /// <para>Write string async.</para>
        /// <para>Run deferred, Length of return value is always 1.</para>
        /// </summary>
        /// <param name="progressReporter">Reporter of progress(such as ScheduledNotifier).</param>
        /// <returns>Length is always 1</returns>
        public static IObservable<Unit> WriteAsync(this Stream stream, string data, Encoding encoding, IProgress<ProgressStatus> progressReporter)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentNullException>(encoding != null);
            Contract.Requires<ArgumentNullException>(progressReporter != null);
            Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

            return WriteAsync(stream, encoding.GetBytes(data), progressReporter);
        }

        /// <summary>
        /// <para>Write data async.</para>
        /// <para>Run deferred, Length of return value is always 1.</para>
        /// </summary>
        /// <returns>Length is always 1</returns>
        public static IObservable<Unit> WriteAsync(this Stream stream, IEnumerable<byte> data, int chunkSize = 65536)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentException>(chunkSize > 0);
            Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

            return WriteAsyncCore(stream, data, null, chunkSize, ProgressStatus.Unknown);
        }

        /// <summary>
        /// <para>Write data async.</para>
        /// <para>Run deferred, Length of return value is always 1.</para>
        /// </summary>
        /// <param name="progressReporter">Reporter of progress(such as ScheduledNotifier).</param>
        /// <returns>Length is always 1</returns>
        public static IObservable<Unit> WriteAsync(this Stream stream, IEnumerable<byte> data, IProgress<ProgressStatus> progressReporter, int chunkSize = 65536)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentNullException>(progressReporter != null);
            Contract.Requires<ArgumentException>(chunkSize > 0);
            Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

            var collection = data as ICollection<byte>;
            var totalLength = (collection != null) ? collection.Count : ProgressStatus.Unknown;

            return WriteAsyncCore(stream, data, progressReporter, chunkSize, totalLength);
        }

        static IObservable<Unit> WriteAsyncCore(Stream stream, IEnumerable<byte> data, IProgress<ProgressStatus> progressReporter, int chunkSize, int totalLength)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentException>(chunkSize > 0);
            Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

            var result = EnumerableEx.Defer(() =>
                {
                    Report(progressReporter, 0, totalLength);
                    return data;
                })
                .Buffer(chunkSize)
                .Select((xs, i) => stream
                    .WriteAsObservable(xs, 0, xs.Length)
                    .Do(_ => Report(progressReporter, (i * chunkSize) + xs.Length, totalLength)))
                .Concat()
                .Finally(() => { stream.Flush(); stream.Close(); })
                .StartWith(new Unit()) // length must be 1
                .TakeLast(1);

            Contract.Assume(result != null);
            return result;
        }

        /// <summary>
        /// <para>Write strings that add every lines Environment.NewLine(Encode to UTF8) async.</para>
        /// <para>Run deferred, Length of return value is always 1.</para>
        /// </summary>
        /// <returns>Length is always 1</returns>
        public static IObservable<Unit> WriteLineAsync(this Stream stream, IEnumerable<string> data)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

            return WriteLineAsync(stream, data, Encoding.UTF8);
        }

        /// <summary>
        /// <para>Write strings that add every lines Environment.NewLine(Encode to UTF8) async.</para>
        /// <para>Run deferred, Length of return value is always 1.</para>
        /// </summary>
        /// <returns>Length is always 1</returns>
        public static IObservable<Unit> WriteLineAsync(this Stream stream, IEnumerable<string> data, Encoding encoding)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentNullException>(encoding != null);
            Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

            var writeData = data.SelectMany(s => encoding.GetBytes(s + Environment.NewLine));
            Contract.Assume(writeData != null);
            return WriteAsync(stream, writeData);
        }

        /// <summary>
        /// <para>Read data async.</para>
        /// <para>Run deferred, Length of return value is if isAggregateAllChunks is true then 1, else reading count.</para>
        /// </summary>
        /// <param name="stream">Target stream.</param>
        /// <param name="chunkSize">The size of one reading.</param>
        /// <param name="isAggregateAllChunks">If true, collect all chunks(return length is 1) else return length is reading count.</param>
        /// <returns>If isAggregateAllChunks is true then 1, else reading count.</returns>
        public static IObservable<byte[]> ReadAsync(this Stream stream, int chunkSize = 65536, bool isAggregateAllChunks = true)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires<ArgumentException>(chunkSize > 0);
            Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

            return ReadAsyncCore(stream, null, ProgressStatus.Unknown, chunkSize, isAggregateAllChunks);
        }

        /// <summary>
        /// <para>Read data async.</para>
        /// <para>Run deferred, Length of return value is if isAggregateAllChunks is true then 1, else reading count.</para>
        /// </summary>
        /// <param name="stream">Target stream.</param>
        /// <param name="progressReporter">Reporter of progress(such as ScheduledNotifier).</param>
        /// <param name="totalLength">Target stream's length(use for ProgressReporter)</param>
        /// <param name="chunkSize">The size of one reading.</param>
        /// <param name="isAggregateAllChunks">If true, collect all chunks(return length is 1) else return length is reading count.</param>
        /// <returns>If isAggregateAllChunks is true then 1, else reading count.</returns>
        public static IObservable<byte[]> ReadAsync(this Stream stream, IProgress<ProgressStatus> progressReporter, int totalLength = ProgressStatus.Unknown, int chunkSize = 65536, bool isAggregateAllChunks = true)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires<ArgumentException>(chunkSize > 0);
            Contract.Requires<ArgumentNullException>(progressReporter != null);
            Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

            return ReadAsyncCore(stream, progressReporter, totalLength, chunkSize, isAggregateAllChunks);
        }

        static IObservable<byte[]> ReadAsyncCore(Stream stream, IProgress<ProgressStatus> progressReporter, int totalLength, int chunkSize, bool isAggregateAllChunks)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires<ArgumentException>(chunkSize > 0);
            Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

            var currentLength = 0;
            var query = Observable.Defer(() =>
                {
                    if (currentLength == 0) Report(progressReporter, currentLength, totalLength);
                    return Observable.Return(new byte[chunkSize], Scheduler.CurrentThread);
                })
                .SelectMany(buffer => stream.ReadAsObservable(buffer, 0, chunkSize),
                    (buffer, readCount) => new { buffer, readCount })
                .Repeat()
                .TakeWhile(a => a.readCount != 0)
                .Select(a =>
                {
                    currentLength += a.readCount;
                    Report(progressReporter, currentLength, totalLength);

                    if (a.readCount == chunkSize) return a.buffer;

                    var newBuffer = new byte[a.readCount];
                    Array.Copy(a.buffer, newBuffer, a.readCount);
                    return newBuffer;
                })
                .Finally(() => stream.Close());

            var result = (isAggregateAllChunks)
                ? query
                    .Aggregate(new List<byte>(), (list, bytes) => { list.AddRange(bytes); return list; })
                    .Select(list => list.ToArray())
                : query;

            Contract.Assume(result != null);
            return result;
        }

        /// <summary>
        /// <para>Read string lines(Encode to UTF8) async.</para>
        /// <para>Run deferred, Length of return value is lines row count.</para>
        /// </summary>
        /// <returns>Length is lines row count</returns>
        public static IObservable<string> ReadLineAsync(this Stream stream, int chunkSize = 65536)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires<ArgumentException>(chunkSize > 0);
            Contract.Ensures(Contract.Result<IObservable<string>>() != null);

            return ReadLineAsync(stream, Encoding.UTF8, chunkSize);
        }

        /// <summary>
        /// <para>Read string lines async.</para>
        /// <para>Run deferred, Length of return value is lines row count.</para>
        /// </summary>
        /// <returns>Length is lines row count</returns>
        public static IObservable<string> ReadLineAsync(this Stream stream, Encoding encoding, int chunkSize = 65536)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires<ArgumentException>(chunkSize > 0);
            Contract.Ensures(Contract.Result<IObservable<string>>() != null);

            var result = ObservableEx.Create<string>(observer =>
            {
                var decoder = encoding.GetDecoder();
                var bom = encoding.GetChars(encoding.GetPreamble()).FirstOrDefault();
                var sb = new StringBuilder();
                var prev = default(char);

                var disposable = new SerialDisposable(); // SingleAssignment
                disposable.Disposable = stream.ReadAsync(chunkSize, isAggregateAllChunks: false)
                    .Subscribe(
                        bytes =>
                        {
                            var charBuffer = new char[encoding.GetMaxCharCount(bytes.Length)];
                            var count = decoder.GetChars(bytes, 0, bytes.Length, charBuffer, 0);

                            for (int i = 0; i < count; i++)
                            {
                                var c = charBuffer[i];

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
                                if (disposable.IsDisposed) return;
                            }
                        },
                        observer.OnError,
                        () =>
                        {
                            var str = sb.ToString();
                            if (str != "") observer.OnNext(str);
                            observer.OnCompleted();
                        });
                return disposable;
            });

            Contract.Assume(result != null);
            return result;
        }

        // report helper
        static void Report(IProgress<ProgressStatus> reporter, long currentLength, long totalLength)
        {
            if (reporter != null) reporter.Report(new ProgressStatus(currentLength, totalLength));
        }
    }
}