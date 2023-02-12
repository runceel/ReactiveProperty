using System;
using Reactive.Bindings.Disposables;
using ReactivePropertyCoreSample.WPF.Models;

namespace ReactivePropertyCoreSample.WPF.ViewModels
{
    public class ViewModelBase : BindableBase, IDisposable
    {
        protected CompositeDisposable Disposables { get; } = new CompositeDisposable();

        public void Dispose() => Disposables.Dispose();
    }
}
