using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Helpers;
using System.Collections.Specialized;

namespace ReactiveProperty.Tests.Helpers
{
    [TestClass]
    public class FilteredReadOnlyObservableCollectionTest
    {
        [TestMethod]
        public void NormalCase()
        {
            var source = new ObservableCollection<Person>(new[]
            {
                new Person { Name = "tanaka1", IsRemoved = false },
                new Person { Name = "tanaka2", IsRemoved = false },
                new Person { Name = "tanaka3", IsRemoved = true },
                new Person { Name = "tanaka4", IsRemoved = false },
            });

            var filtered = source.ToFilteredReadOnlyObservableCollection(x => !x.IsRemoved);
            filtered.Select(x => x.Name).Is("tanaka1", "tanaka2", "tanaka4");
            filtered.Count.Is(3);

            source.Add(new Person { Name = "tanaka5", IsRemoved = true });
            filtered.Select(x => x.Name).Is("tanaka1", "tanaka2", "tanaka4");
            filtered.Count.Is(3);

            source.RemoveAt(1); // remove tanaka2
            filtered.Select(x => x.Name).Is("tanaka1", "tanaka4");
            filtered.Count.Is(2);

            source.RemoveAt(1); // remove tanaka3
            filtered.Select(x => x.Name).Is("tanaka1", "tanaka4");
            filtered.Count.Is(2);

            source.Last().IsRemoved = false;
            filtered.Select(x => x.Name).Is("tanaka1", "tanaka4", "tanaka5");
            filtered.Count.Is(3);

            source[0] = new Person { Name = "tanaka1 replaced", IsRemoved = false };
            filtered.Select(x => x.Name).Is("tanaka1 replaced", "tanaka4", "tanaka5");
            filtered.Count.Is(3);

            source.Clear();
            filtered.Count.Is(0);
        }

        [TestMethod]
        public void NormalCase2()
        {
            var source = new ObservableCollection<Person>(new[]
            {
                new Person { Name = "tanaka1", IsRemoved = true },
                new Person { Name = "tanaka2", IsRemoved = false },
                new Person { Name = "tanaka3", IsRemoved = false },
            });

            var filtered = source.ToFilteredReadOnlyObservableCollection(x => x.IsRemoved);
            filtered.Select(x => x.Name).Is("tanaka1");
            filtered.Count.Is(1);

            source.Add(new Person { Name = "tanaka4", IsRemoved = false });
            filtered.Select(x => x.Name).Is("tanaka1");
            filtered.Count.Is(1);

            source[3].IsRemoved = true;
            filtered.Select(x => x.Name).Is("tanaka1", "tanaka4");
            filtered.Count.Is(2);

            source.Clear();
            filtered.Count.Is(0);
        }

        [TestMethod]
        public void CollectionChangedAddTest()
        {
            var source = new ObservableCollection<Person>();
            var filtered = source.ToFilteredReadOnlyObservableCollection(x => !x.IsRemoved);
            var buffer = new List<NotifyCollectionChangedEventArgs>();
            filtered.CollectionChangedAsObservable().Subscribe(buffer.Add);

            source.Add(new Person { Name = "a", IsRemoved = true });
            buffer.Count.Is(0);
            source.Add(new Person { Name = "b", IsRemoved = false });
            buffer.Count.Is(1);
            buffer[0].Is(x => x.Action == NotifyCollectionChangedAction.Add && x.NewItems.Cast<Person>().First().Name == "b" && x.NewStartingIndex == 0);

            source[0].IsRemoved = false;
            buffer.Count.Is(2);
            buffer[1].Is(x => x.Action == NotifyCollectionChangedAction.Add && x.NewItems.Cast<Person>().First().Name == "a" && x.NewStartingIndex == 0);
        }

        [TestMethod]
        public void CollectionChangedRefreshAndAddTest()
        {
            var source = new ObservableCollection<Person>();
            var filtered = source.ToFilteredReadOnlyObservableCollection(x => x.IsRemoved);
            filtered.Refresh(x => !x.IsRemoved);
            var buffer = new List<NotifyCollectionChangedEventArgs>();
            filtered.CollectionChangedAsObservable().Subscribe(buffer.Add);

            source.Add(new Person { Name = "a", IsRemoved = true });
            buffer.Count.Is(0);
            source.Add(new Person { Name = "b", IsRemoved = false });
            buffer.Count.Is(1);
            buffer[0].Is(x => x.Action == NotifyCollectionChangedAction.Add && x.NewItems.Cast<Person>().First().Name == "b" && x.NewStartingIndex == 0);

            source[0].IsRemoved = false;
            buffer.Count.Is(2);
            buffer[1].Is(x => x.Action == NotifyCollectionChangedAction.Add && x.NewItems.Cast<Person>().First().Name == "a" && x.NewStartingIndex == 0);
        }

        [TestMethod]
        public void CollectionChangedRemoveTest()
        {
            var source = new ObservableCollection<Person>(new[]
            {
                new Person { Name = "tanaka1", IsRemoved = false },
                new Person { Name = "tanaka2", IsRemoved = false },
                new Person { Name = "tanaka3", IsRemoved = true },
                new Person { Name = "tanaka4", IsRemoved = false },
            });

            var filtered = source.ToFilteredReadOnlyObservableCollection(x => !x.IsRemoved);
            var buffer = new List<NotifyCollectionChangedEventArgs>();
            filtered.CollectionChangedAsObservable().Subscribe(buffer.Add);

            source.RemoveAt(2); // tanaka3 remove
            buffer.Count.Is(0);
            source.RemoveAt(0); // tanaka1 remove
            buffer.Count.Is(1);
            buffer[0].Is(x => x.Action == NotifyCollectionChangedAction.Remove && x.OldStartingIndex == 0 && x.OldItems.Cast<Person>().First().Name == "tanaka1");
            source[0].IsRemoved = true;
            buffer.Count.Is(2);
        }

        [TestMethod]
        public void CollectionChangedRefreshAndRemoveTest()
        {
            var source = new ObservableCollection<Person>(new[]
            {
                new Person { Name = "tanaka1", IsRemoved = false },
                new Person { Name = "tanaka2", IsRemoved = false },
                new Person { Name = "tanaka3", IsRemoved = true },
                new Person { Name = "tanaka4", IsRemoved = false },
            });

            var filtered = source.ToFilteredReadOnlyObservableCollection(x => x.IsRemoved);
            filtered.Refresh(x => !x.IsRemoved);
            var buffer = new List<NotifyCollectionChangedEventArgs>();
            filtered.CollectionChangedAsObservable().Subscribe(buffer.Add);

            source.RemoveAt(2); // tanaka3 remove
            buffer.Count.Is(0);
            source.RemoveAt(0); // tanaka1 remove
            buffer.Count.Is(1);
            buffer[0].Is(x => x.Action == NotifyCollectionChangedAction.Remove && x.OldStartingIndex == 0 && x.OldItems.Cast<Person>().First().Name == "tanaka1");
            source[0].IsRemoved = true;
            buffer.Count.Is(2);
        }

        [TestMethod]
        public void CollectionChangedReplaceTest()
        {
            var source = new ObservableCollection<Person>(new[]
            {
                new Person { Name = "tanaka1", IsRemoved = false },
                new Person { Name = "tanaka2", IsRemoved = false },
                new Person { Name = "tanaka3", IsRemoved = true },
                new Person { Name = "tanaka4", IsRemoved = false },
            });

            var filtered = source.ToFilteredReadOnlyObservableCollection(x => !x.IsRemoved);
            var buffer = new List<NotifyCollectionChangedEventArgs>();
            filtered.CollectionChangedAsObservable().Subscribe(buffer.Add);

            source[0] = new Person { Name = "tanaka1 replaced", IsRemoved = false };
            buffer.Count.Is(1);
            buffer[0].Is(x => x.Action == NotifyCollectionChangedAction.Replace && x.NewStartingIndex == 0 && x.NewItems.Cast<Person>().First().Name == "tanaka1 replaced");

            source[1] = new Person { Name = "tanaka2 replaced", IsRemoved = true };
            buffer.Count.Is(2);
            buffer[1].Is(x => x.Action == NotifyCollectionChangedAction.Remove && x.OldStartingIndex == 1 && x.OldItems.Cast<Person>().First().Name == "tanaka2");
        }

        [TestMethod]
        public void CollectionChangedRefreshAndReplaceTest()
        {
            var source = new ObservableCollection<Person>(new[]
            {
                new Person { Name = "tanaka1", IsRemoved = false },
                new Person { Name = "tanaka2", IsRemoved = false },
                new Person { Name = "tanaka3", IsRemoved = true },
                new Person { Name = "tanaka4", IsRemoved = false },
            });

            var filtered = source.ToFilteredReadOnlyObservableCollection(x => x.IsRemoved);
            filtered.Refresh(x => !x.IsRemoved);
            var buffer = new List<NotifyCollectionChangedEventArgs>();
            filtered.CollectionChangedAsObservable().Subscribe(buffer.Add);

            source[0] = new Person { Name = "tanaka1 replaced", IsRemoved = false };
            buffer.Count.Is(1);
            buffer[0].Is(x => x.Action == NotifyCollectionChangedAction.Replace && x.NewStartingIndex == 0 && x.NewItems.Cast<Person>().First().Name == "tanaka1 replaced");

            source[1] = new Person { Name = "tanaka2 replaced", IsRemoved = true };
            buffer.Count.Is(2);
            buffer[1].Is(x => x.Action == NotifyCollectionChangedAction.Remove && x.OldStartingIndex == 1 && x.OldItems.Cast<Person>().First().Name == "tanaka2");
        }

        [TestMethod]
        public void CollectionChangedClearTest()
        {
            var source = new ObservableCollection<Person>(new[]
            {
                new Person { Name = "tanaka1", IsRemoved = false },
                new Person { Name = "tanaka2", IsRemoved = false },
                new Person { Name = "tanaka3", IsRemoved = true },
                new Person { Name = "tanaka4", IsRemoved = false },
            });

            var filtered = source.ToFilteredReadOnlyObservableCollection(x => !x.IsRemoved);
            var buffer = new List<NotifyCollectionChangedEventArgs>();
            filtered.CollectionChangedAsObservable().Subscribe(buffer.Add);

            source.Clear();
            buffer.Count.Is(1);
            buffer[0].Is(x => x.Action == NotifyCollectionChangedAction.Reset);
        }

        [TestMethod]
        public void CollectionChangedRefreshAndClearTest()
        {
            var source = new ObservableCollection<Person>(new[]
            {
                new Person { Name = "tanaka1", IsRemoved = false },
                new Person { Name = "tanaka2", IsRemoved = false },
                new Person { Name = "tanaka3", IsRemoved = true },
                new Person { Name = "tanaka4", IsRemoved = false },
            });

            var filtered = source.ToFilteredReadOnlyObservableCollection(x => x.IsRemoved);
            filtered.Refresh(x => !x.IsRemoved);
            var buffer = new List<NotifyCollectionChangedEventArgs>();
            filtered.CollectionChangedAsObservable().Subscribe(buffer.Add);

            source.Clear();
            buffer.Count.Is(1);
            buffer[0].Is(x => x.Action == NotifyCollectionChangedAction.Reset);
        }

        [TestMethod]
        public void RemoveIndexOutofRangeException()
        {
            var source = new ObservableCollection<Person>(new[]
            {
                new Person { Name = "tanaka1", IsRemoved = true },
                new Person { Name = "tanaka2", IsRemoved = false },
                new Person { Name = "tanaka3", IsRemoved = true },
            });

            var filtered = source.ToFilteredReadOnlyObservableCollection(x => x.IsRemoved);
            var buffer = new List<NotifyCollectionChangedEventArgs>();
            filtered.CollectionChangedAsObservable().Subscribe(buffer.Add);

            source[2].IsRemoved = false;

            buffer.Count.Is(1);
            buffer[0].Action.Is(NotifyCollectionChangedAction.Remove);
            buffer[0].OldItems[0].Is(source[2]);

            filtered.Count.Is(1);
        }
        [TestMethod]
        public void InconsistencyIndexWhenSourceCollectionItemReplace()
        {
            var source = new ObservableCollection<Person>(new[]
            {
                new Person { Name = "tanaka1", IsRemoved = false },
                new Person { Name = "tanaka2", IsRemoved = false },
                new Person { Name = "tanaka3", IsRemoved = false },
            });

            var filtered = source.ToFilteredReadOnlyObservableCollection(x => !x.IsRemoved);
            var buffer = new List<NotifyCollectionChangedEventArgs>();
            filtered.CollectionChangedAsObservable().Subscribe(buffer.Add);

            source[1] = new Person { Name = "tanaka2 replaced", IsRemoved = true };
            buffer.Count().Is(1);
            buffer[0].Is(x => x.Action == NotifyCollectionChangedAction.Remove && x.OldStartingIndex == 1 && x.OldItems.Cast<Person>().First().Name == "tanaka2");

            source.RemoveAt(2); // tanaka3 remove
            buffer.Count().Is(2);
            buffer[1].Is(x => x.Action == NotifyCollectionChangedAction.Remove && x.OldStartingIndex == 1 && x.OldItems.Cast<Person>().First().Name == "tanaka3");
            filtered.ToArray().Length.Is(filtered.Count);
        }

        [TestMethod]
        public void InconsistencyIndexWhenSourceCollectionItemRemove()
        {
            var source = new ObservableCollection<Person>(new[]
            {
                new Person { Name = "tanaka1", IsRemoved = false },
                new Person { Name = "tanaka2", IsRemoved = true },
                new Person { Name = "tanaka3", IsRemoved = false },
                new Person { Name = "tanaka4", IsRemoved = true },
                new Person { Name = "tanaka5", IsRemoved = false },
            });

            var filtered = source.ToFilteredReadOnlyObservableCollection(x => !x.IsRemoved);
            var buffer = new List<NotifyCollectionChangedEventArgs>();
            filtered.CollectionChangedAsObservable().Subscribe(buffer.Add);

            source.RemoveAt(1); // tanaka2 remove
            buffer.Count.Is(0);

            source.RemoveAt(1); // tanaka3 remove
            buffer.Count.Is(1);
            buffer[0].Is(x => x.Action == NotifyCollectionChangedAction.Remove && x.OldStartingIndex == 1 && x.OldItems.Cast<Person>().First().Name == "tanaka3");

            source.RemoveAt(1); //tanaka4 remove
            buffer.Count.Is(1);

            source[1].IsRemoved = true; // tanaka5 logical remove
            buffer.Count.Is(2);
            buffer[1].Is(x => x.Action == NotifyCollectionChangedAction.Remove && x.OldStartingIndex == 1 && x.OldItems.Cast<Person>().First().Name == "tanaka5");
        }

        [TestMethod]
        public void RefreshTest()
        {
            var source = new ObservableCollection<Person>(new[]
            {
                new Person { Name = "tanaka1", IsRemoved = false },
                new Person { Name = "tanaka2", IsRemoved = true },
                new Person { Name = "tanaka3", IsRemoved = false },
                new Person { Name = "tanaka4", IsRemoved = true },
                new Person { Name = "tanaka5", IsRemoved = false },
            });

            var filtered = source.ToFilteredReadOnlyObservableCollection(x => !x.IsRemoved);
            var buffer = new List<NotifyCollectionChangedEventArgs>();

            filtered.Is(source[0], source[2], source[4]);
            filtered.Refresh(x => x.IsRemoved);
            filtered.Is(source[1], source[3]);
        }

        #region private class
        private class Person : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            private void SetProperty<T>(ref T field, T value, [CallerMemberName]string propertyName = null)
            {
                if (object.Equals(field, value)) { return; }
                field = value;
                var h = this.PropertyChanged;
                if (h != null) { h(this, new PropertyChangedEventArgs(propertyName)); }
            }

            private string name;

            public string Name
            {
                get { return this.name; }
                set { this.SetProperty(ref this.name, value); }
            }

            private bool isRemoved;

            public bool IsRemoved
            {
                get { return this.isRemoved; }
                set { this.SetProperty(ref this.isRemoved, value); }
            }

        }

        #endregion
    }
}
