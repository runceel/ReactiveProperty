using System;
using System.Collections.Generic;

namespace Reactive.Bindings.Extensions
{
    public static class IDisposableExtensions
    {
        /// <summary>Add disposable(self) to CompositeDisposable(or other ICollection)</summary>
        public static T AddTo<T>(this T disposable, ICollection<IDisposable> container)
            where T : IDisposable
        {
            container.Add(disposable);
            return disposable;
        }
    }
}