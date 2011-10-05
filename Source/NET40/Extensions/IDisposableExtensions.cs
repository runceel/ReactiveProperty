using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#else
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Reactive;
using System.Collections.Specialized;
#endif

namespace Codeplex.Reactive.Extensions
{
    public static class IDisposableExtensions
    {
        /// <summary>Add disposable(self) to CompositeDisposable(or other ICollection)</summary>
        public static void AddTo(this IDisposable disposable, ICollection<IDisposable> container)
        {
            Contract.Requires<ArgumentNullException>(disposable != null);
            Contract.Requires<ArgumentNullException>(container != null);

            container.Add(disposable);
        }
    }
}