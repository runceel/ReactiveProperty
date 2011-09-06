// <copyright file="SignalNotifierTest.cs" company="Microsoft">Copyright © Microsoft 2011</copyright>
using System;
using Codeplex.Reactive.Notifier;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;
using Microsoft.Pex.Engine.Exceptions;

namespace Codeplex.Reactive.Notifier
{
    /// <summary>This class contains parameterized unit tests for SignalNotifier</summary>
    [PexClass(typeof(SignalNotifier))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class SignalNotifierTest
    {
        // test

        [PexMethod]
        public void Increment(int max, int incrementCount)
        {
            var notifier = new SignalNotifier(max);

            notifier.Increment(incrementCount);
            notifier.Count.Is(incrementCount);
        }

        [PexMethod]
        public void Decrement(int max, int initialIncrementCount, int decrementCount)
        {
            var notifier = new SignalNotifier(max);
            notifier.Increment(initialIncrementCount);
            notifier.Decrement(decrementCount);
            notifier.Count.Is(initialIncrementCount - decrementCount);
        }

        // promote

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void IncrementThrowsArgumentException327()
        {
            this.Increment(0, 0);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void IncrementThrowsArgumentException80()
        {
            this.Increment(1, 0);
        }
        [TestMethod]
        public void Increment476()
        {
            this.Increment(1, 1);
        }
        [TestMethod]
        public void Increment347()
        {
            this.Increment(927, 926);
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void IncrementThrowsInvalidOperationException65()
        {
            this.Increment(2, 3);
        }
        [TestMethod]
        public void Decrement314()
        {
            this.Decrement(1, 1, 1);
        }
        [TestMethod]
        public void Decrement981()
        {
            this.Decrement(257, 257, 128);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DecrementThrowsArgumentException154()
        {
            this.Decrement(0, 0, 0);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DecrementThrowsArgumentException262()
        {
            this.Decrement(514, 3, 0);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DecrementThrowsArgumentException548()
        {
            this.Decrement(1, 0, 0);
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DecrementThrowsInvalidOperationException510()
        {
            this.Decrement(2, 2, 768);
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DecrementThrowsInvalidOperationException589()
        {
            this.Decrement(2, 3, 0);
        }
    }
}
