using System;
using System.Collections.Generic;
using System.Text;

namespace Reactive.Bindings.Internals;
internal class DelegateDisposable : IDisposable
{
    private bool _isDisposed;
    private readonly Action _action;

    public DelegateDisposable(Action action)
    {
        _action = action;
    }
    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;
        _action();
    }
}

internal class DelegateDisposable<TState> : IDisposable
{
    private bool _isDisposed;
    private readonly Action<TState> _action;
    private readonly TState _state;

    public DelegateDisposable(Action<TState> action, TState state)
    {
        _action = action;
        _state = state;
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;
        _action(_state);
    }
}
