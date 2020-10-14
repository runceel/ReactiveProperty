using System.ComponentModel;

namespace Reactive.Bindings.Internals
{
    internal static class SingletonPropertyChangedEventArgs
    {
        public static readonly PropertyChangedEventArgs Value = new PropertyChangedEventArgs(nameof(IReactiveProperty.Value));
        public static readonly PropertyChangedEventArgs HasErrors = new PropertyChangedEventArgs(nameof(INotifyDataErrorInfo.HasErrors));
    }
}
