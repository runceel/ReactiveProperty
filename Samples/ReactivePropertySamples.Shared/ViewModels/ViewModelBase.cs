using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Text;

namespace ReactivePropertySamples.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        protected CompositeDisposable Disposables { get; } = new CompositeDisposable();
        public event PropertyChangedEventHandler PropertyChanged;

        public void Dispose() => Disposables.Dispose();
    }
}
