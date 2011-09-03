using System;
using System.Reactive.Subjects;

namespace Codeplex.Reactive.Notifier
{
    // this interface borrow from AsyncCTP
    // will replace in the future

    /// <summary>
    /// Defines a provider for progress updates.
    /// </summary>
    /// <typeparam name="T">The type of progress update value.</typeparam>
    public interface IProgress<in T>
    {
        /// <summary>
        /// Reports a progress update.
        /// </summary>
        /// <param name="value">The value of the updated progress.</param>
        void Report(T value);
    }
}