using System;
using System.Collections.Generic;
using System.Text;

namespace Reactive.Bindings.Internals;

internal class DelegateObserver : IObserver<object?>
{
    private readonly Action _onNext;
    private readonly Action? _onCompleted;
    private readonly Action<Exception>? _onError;

    public DelegateObserver(Action onNext, Action? onCompleted, Action<Exception>? onError)
    {
        _onNext = onNext;
        _onCompleted = onCompleted;
        _onError = onError;
    }

    public DelegateObserver(Action onNext) : this(onNext, null, null)
    {
    }

    public DelegateObserver(Action onNext, Action onCompleted) : this(onNext, onCompleted, null)
    {
    }

    public void OnCompleted() => _onCompleted?.Invoke();

    public void OnError(Exception error) => _onError?.Invoke(error);

    public void OnNext() => _onNext();

    void IObserver<object?>.OnNext(object? value) => OnNext();
}

internal class DelegateObserver<T> : IObserver<T>
{
    private readonly Action<T> _onNext;
    private readonly Action? _onCompleted;
    private readonly Action<Exception>? _onError;

    public DelegateObserver(Action<T> onNext, Action? onCompleted, Action<Exception>? onError)
    {
        _onNext = onNext;
        _onCompleted = onCompleted;
        _onError = onError;
    }

    public DelegateObserver(Action<T> onNext) : this(onNext, null, null)
    {
    }

    public DelegateObserver(Action<T> onNext, Action onCompleted) : this(onNext, onCompleted, null)
    {
    }

    public void OnCompleted() => _onCompleted?.Invoke();

    public void OnError(Exception error) => _onError?.Invoke(error);

    public void OnNext(T value) => _onNext(value);
}
