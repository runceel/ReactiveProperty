using System.ComponentModel;

namespace Reactive.Bindings.Internals
{
    public static class SingletonPropertyChangedEventArgs
    {
        public static readonly PropertyChangedEventArgs Value = new PropertyChangedEventArgs(nameof(ReactiveProperty<object>.Value));
    }
}
