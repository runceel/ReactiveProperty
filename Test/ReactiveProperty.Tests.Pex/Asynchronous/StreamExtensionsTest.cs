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

namespace Codeplex.Reactive.Asynchronous
{
    /// <summary>This class contains parameterized unit tests for StreamExtensions</summary>
    [PexClass(typeof(StreamExtensions))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class StreamExtensionsTest
    {
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
            var disp = result.First();

            disp.Is(bytes.Length);
            buffer.Is(bytes);
        }
    }
}
