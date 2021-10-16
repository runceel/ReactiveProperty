using System.ComponentModel;

namespace Reactive.Bindings.Internals
{
    internal static class SingletonPropertyChangedEventArgs
    {
        public static readonly PropertyChangedEventArgs Value = new(nameof(IReactiveProperty.Value));
        public static readonly PropertyChangedEventArgs HasErrors = new(nameof(INotifyDataErrorInfo.HasErrors));
    }
}
