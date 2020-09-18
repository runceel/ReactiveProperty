using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;

namespace MultiUIThreadApp.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        protected CompositeDisposable Disposables { get; } = new CompositeDisposable();
        public event PropertyChangedEventHandler PropertyChanged;

        public void Dispose() => Disposables.Dispose();

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null) => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
