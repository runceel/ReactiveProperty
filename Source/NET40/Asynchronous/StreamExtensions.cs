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
            return Observable.FromAsyncPattern((ac, o) => stream.BeginWrite(buffer, offset, count, ac, o), stream.EndWrite)();
        }

        public static IObservable<int> ReadAsObservable(this Stream stream, byte[] buffer, int offset, int count)
        {
            return Observable.FromAsyncPattern<int>((ac, o) => stream.BeginRead(buffer, offset, count, ac, o), stream.EndRead)();
        }

        public static IObservable<Unit> WriteAsync(this Stream stream, string data)
        {
            return WriteAsync(stream, data, Encoding.UTF8);
        }

        public static IObservable<Unit> WriteAsync(this Stream stream, string data, Encoding encoding)
        {
            return WriteAsync(stream, encoding.GetBytes(data));
        }

        public static IObservable<Unit> WriteAsync(this Stream stream, IEnumerable<byte> data, int chunkSize = 65536)
        {
            return WriteAsync(stream, data.ToObservable(), chunkSize);
        }

        public static IObservable<Unit> WriteAsync(this Stream stream, IObservable<byte> data, int chunkSize = 65536)
        {
            return Observable.Defer(() => data)
                .Buffer(chunkSize)
                .Select(l => stream.WriteAsObservable(l.ToArray(), 0, l.Count))
                .Concat()
                .Finally(() => stream.Close());
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
    }
}