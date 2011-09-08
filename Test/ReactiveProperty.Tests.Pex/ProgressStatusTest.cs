// <copyright file="ProgressStatusTest.cs" company="Microsoft">Copyright © Microsoft 2011</copyright>

using System;
using Codeplex.Reactive.Asynchronous;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;

namespace Codeplex.Reactive.Asynchronous
{
    [TestClass]
    [PexClass(typeof(ProgressStatus))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class ProgressStatusTest
    {
        [PexMethod]
        public int PercentageGet([PexAssumeUnderTest]ProgressStatus target)
        {
            int result = target.Percentage;
            return result;
            // TODO: add assertions to method ProgressStatusTest.PercentageGet(ProgressStatus)
        }
        [TestMethod]
        public void PercentageGet710()
        {
            int i;
            ProgressStatus s0 = new ProgressStatus(0L, 0L);
            i = this.PercentageGet(s0);
            Assert.AreEqual<int>(0, i);
            Assert.IsNotNull((object)s0);
            Assert.AreEqual<long>(0L, s0.CurrentLength);
            Assert.AreEqual<long>(0L, s0.TotalLength);
        }
        [TestMethod]
        public void PercentageGet912()
        {
            int i;
            ProgressStatus s0 = new ProgressStatus(0L, 1L);
            i = this.PercentageGet(s0);
            Assert.AreEqual<int>(0, i);
            Assert.IsNotNull((object)s0);
            Assert.AreEqual<long>(0L, s0.CurrentLength);
            Assert.AreEqual<long>(1L, s0.TotalLength);
        }
        [TestMethod]
        public void PercentageGet350()
        {
            int i;
            ProgressStatus s0 = new ProgressStatus(1L, 1L);
            i = this.PercentageGet(s0);
            Assert.AreEqual<int>(100, i);
            Assert.IsNotNull((object)s0);
            Assert.AreEqual<long>(1L, s0.CurrentLength);
            Assert.AreEqual<long>(1L, s0.TotalLength);
        }
        [PexMethod]
        public string ToString01([PexAssumeUnderTest]ProgressStatus target)
        {
            string result = target.ToString();
            return result;
            // TODO: add assertions to method ProgressStatusTest.ToString01(ProgressStatus)
        }
        [TestMethod]
        public void ToString0147()
        {
            string s;
            ProgressStatus s0 = new ProgressStatus(0L, 0L);
            s = this.ToString01(s0);
            Assert.AreEqual<string>("0/0 - 0%", s);
            Assert.IsNotNull((object)s0);
            Assert.AreEqual<long>(0L, s0.CurrentLength);
            Assert.AreEqual<long>(0L, s0.TotalLength);
        }
        [TestMethod]
        public void ToString01652()
        {
            string s;
            ProgressStatus s0 = new ProgressStatus(0L, 1L);
            s = this.ToString01(s0);
            Assert.AreEqual<string>("0/1 - 0%", s);
            Assert.IsNotNull((object)s0);
            Assert.AreEqual<long>(0L, s0.CurrentLength);
            Assert.AreEqual<long>(1L, s0.TotalLength);
        }
        [TestMethod]
        public void ToString01878()
        {
            string s;
            ProgressStatus s0 = new ProgressStatus(1L, 1L);
            s = this.ToString01(s0);
            Assert.AreEqual<string>("1/1 - 100%", s);
            Assert.IsNotNull((object)s0);
            Assert.AreEqual<long>(1L, s0.CurrentLength);
            Assert.AreEqual<long>(1L, s0.TotalLength);
        }
    }
}
