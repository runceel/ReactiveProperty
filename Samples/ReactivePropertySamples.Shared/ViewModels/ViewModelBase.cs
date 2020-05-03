using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using System.Text;

namespace ReactivePropertySamples.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        protected CompositeDisposable Disposables { get; } = new CompositeDisposable();
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        public void Dispose() => Disposables.Dispose();
    }
}
