using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Text;
using BenchmarkDotNet.Attributes;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace ReactivePropertyBenchmark
{
    public class BasicUsages
    {
        [Benchmark]
        public ReactiveProperty<string> CreateReactivePropertyInstance() => new ReactiveProperty<string>();

        [Benchmark]
        public ReactivePropertySlim<string> CreateReactivePropertySlimInstance() => new ReactivePropertySlim<string>();

        [Benchmark]
        public ReadOnlyReactiveProperty<string> BasicForReactiveProperty()
        {
            var rp = new ReactiveProperty<string>();
            var rrp = rp.ToReadOnlyReactiveProperty();

            rp.Value = "xxxx";
            rp.Dispose();
            return rrp;
        }

        [Benchmark]
        public ReadOnlyReactivePropertySlim<string> BasicForReactivePropertySlim()
        {
            var rp = new ReactivePropertySlim<string>();
            var rrp = rp.ToReadOnlyReactivePropertySlim();

            rp.Value = "xxxx";
            rp.Dispose();
            return rrp;
        }

        [Benchmark]
        public IObservable<string> ObserveProperty()
        {
            var p = new Person();
            return p.ObserveProperty(x => x.Name);
        }

        [Benchmark]
        public ReactiveProperty<string> ToReactivePropertyAsSynchronized()
        {
            var p = new Person();
            return p.ToReactivePropertyAsSynchronized(x => x.Name);
        }
    }

    class Person : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return;
            }

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
    }
}
