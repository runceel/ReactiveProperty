using System;
using R3;

namespace ReactivePropertySamples.Migrated.ViewModels
{
    public class ViewModelBase : Models.BindableBase, IDisposable
    {
        protected CompositeDisposable Disposables { get; } = new CompositeDisposable();

        public void Dispose() => Disposables.Dispose();
    }
}
