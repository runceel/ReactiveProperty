using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Runtime.InteropServices;
using System.Text;
using Prism.Mvvm;

namespace Reactive.Todo.Main.ViewModels
{
    public class ViewModelBase : BindableBase, IDisposable
    {
        protected CompositeDisposable Disposables { get; } = new CompositeDisposable();
        public void Dispose() => Disposables.Dispose();
    }
}
