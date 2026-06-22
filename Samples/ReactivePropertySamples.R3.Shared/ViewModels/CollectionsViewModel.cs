using System;
using System.Collections.ObjectModel;
using System.Threading;
using R3;
using Reactive.Bindings.R3;
using Reactive.Bindings.R3.Extensions;
using ReactivePropertySamples.Migrated.Models;

namespace ReactivePropertySamples.Migrated.ViewModels
{
    public class CollectionsViewModel : ViewModelBase
    {
        // R3 has no ReactiveCollection<T>; a plain ObservableCollection<T> mutated on the UI
        // thread is the naive equivalent of ToReactiveCollection()/ClearOnScheduler().
        public ObservableCollection<Guid> ReactiveCollection { get; }
        public ReactiveCommand AddToReactiveCollectionCommand { get; }
        public ReactiveCommand ClearReactiveCollectionCommand { get; }

        private readonly CollectionsModel _model = new CollectionsModel();
        public ReadOnlyReactiveCollection<GuidViewModel> GuidViewModels { get; }
        public ReactiveCommand AddGuidToModelCommand { get; }
        public ReactiveCommand ClearModelGuidsCommand { get; }

        public ReactiveCommand AddToFilterSourceItemsCommand { get; }
        public ReactiveCommand ClearFromFilterSourceItemsCommand { get; }
        public ObservableCollection<FilterSourceItem> FilterSourceItems { get; }
        public IFilteredReadOnlyObservableCollection<FilterSourceItem> FilteredItems { get; }

        public CollectionsViewModel()
        {
            // Captured on the UI thread; the bridge posts collection-changed events back to it
            // (the migration replacement for ReactiveProperty's IScheduler-based UI dispatch).
            var uiContext = SynchronizationContext.Current;

            ReactiveCollection = new ObservableCollection<Guid>();
            AddToReactiveCollectionCommand = new ReactiveCommand()
                .AddTo(Disposables);
            AddToReactiveCollectionCommand
                .Subscribe(_ => ReactiveCollection.Add(Guid.NewGuid()))
                .AddTo(Disposables);
            ClearReactiveCollectionCommand = new ReactiveCommand()
                .AddTo(Disposables);
            ClearReactiveCollectionCommand
                .Subscribe(_ => ReactiveCollection.Clear())
                .AddTo(Disposables);

            GuidViewModels = _model.Guids
                .ToReadOnlyReactiveCollection(x => new GuidViewModel(x), raiseEventContext: uiContext)
                .AddTo(Disposables);
            AddGuidToModelCommand = new ReactiveCommand()
                .AddTo(Disposables);
            AddGuidToModelCommand
                .ObserveOnThreadPool()
                .Subscribe(_ => _model.Guids.Add(Guid.NewGuid()))
                .AddTo(Disposables);
            ClearModelGuidsCommand = new ReactiveCommand()
                .AddTo(Disposables);
            ClearModelGuidsCommand
                .ObserveOnThreadPool()
                .Subscribe(_ => _model.Guids.Clear())
                .AddTo(Disposables);

            FilterSourceItems = new ObservableCollection<FilterSourceItem>();
            FilteredItems = FilterSourceItems
                .ToFilteredReadOnlyObservableCollection(x => x.Count >= 7, uiContext)
                .AddTo(Disposables);
            AddToFilterSourceItemsCommand = new ReactiveCommand()
                .AddTo(Disposables);
            AddToFilterSourceItemsCommand
                .Subscribe(_ => FilterSourceItems.Add(new FilterSourceItem()))
                .AddTo(Disposables);
            ClearFromFilterSourceItemsCommand = FilterSourceItems.ObservePropertyChanged(x => x.Count)
                .Select(x => x != 0)
                .ToReactiveCommand()
                .AddTo(Disposables);
            ClearFromFilterSourceItemsCommand
                .Subscribe(_ =>
                {
                    foreach (var d in FilterSourceItems)
                    {
                        d.Dispose();
                    }

                    FilterSourceItems.Clear();
                })
                .AddTo(Disposables);
        }
    }

    public class GuidViewModel : ViewModelBase
    {
        private readonly Guid _guid;

        public string Text => $"GUID: {_guid}";

        public GuidViewModel(Guid guid)
        {
            _guid = guid;
        }
    }

    public class FilterSourceItem : ViewModelBase
    {
        private static Random Random { get; } = new Random();
        private readonly ReactiveTimer _timer;

        private int _count;
        public int Count
        {
            get { return _count; }
            set { SetProperty(ref _count, value); }
        }

        public FilterSourceItem()
        {
            _timer = new ReactiveTimer(TimeSpan.FromSeconds(1)).AddTo(Disposables);
            _timer.ObserveOnCurrentSynchronizationContext()
                .Subscribe(_ => Count = Random.Next(10))
                .AddTo(Disposables);
            _timer.Start();
        }
    }
}
