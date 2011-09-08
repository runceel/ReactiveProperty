using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Codeplex.Reactive.Asynchronous;
using System.Reactive.Linq;

namespace ReactiveProperty.Tests.Asynchronous
{
    [TestClass]
    public class StreamExtensionsTest
    {
        [TestMethod]
        public void ReadAsObservable()
        {
            var bytes = Encoding.UTF8.GetBytes("てすとすとりんぐ");

            var buffer = new byte[bytes.Length];
            using (var stream = new MemoryStream(bytes))
            {
                var result = stream.ReadAsObservable(buffer, 0, buffer.Length).ToEnumerable().ToArray();

                result.Length.Is(1);
                result[0].Is(bytes.Length);
                buffer.Is(bytes);
            }
        }

        [TestMethod]
        public void WriteAsObservable()
        {
            var bytes = Encoding.UTF8.GetBytes("てすとすとりんぐ");
            using (var stream = new MemoryStream())
            {
                var result = stream.WriteAsObservable(bytes, 0, bytes.Length).ToEnumerable().ToArray();

                result.Length.Is(1);
                stream.ToArray().Is(bytes);
            }
        }
    }
}
