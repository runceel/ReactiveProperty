using System;
using System.Collections.Generic;
using System.Text;

namespace Reactive.Bindings.Internals
{
    internal static class InternalDisposable
    {
        public static readonly IDisposable Empty = new EmptyDisposable();

        class EmptyDisposable : IDisposable
        {
            public void Dispose() { }
        }
    }
}
