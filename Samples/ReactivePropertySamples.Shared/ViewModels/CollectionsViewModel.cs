using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Helpers;
using ReactivePropertySamples.Models;

namespace ReactivePropertySamples.ViewModels
{
    public class CollectionsViewModel : ViewModelBase
    {
        public ReactiveCollection<Guid> ReactiveCollection { get; }
        public ReactiveCommand AddToReactiveCollectionCommand { get; }
        public ReactiveCommand ClearReactiveCollectionCommand { get; }

        private CollectionsModel _model = new CollectionsModel();
        public ReadOnlyReactiveCollection<GuidViewModel> GuidViewModels { get; }
        public ReactiveCommand AddGuidToModelCommand { get; }
        public ReactiveCommand ClearModelGuidsCommand { get; }

        public ReactiveCommand AddToFilterSourceItemsCommand { get; }
        public ReactiveCommand ClearFromFilterSourceItemsCommand { get; }
        public ObservableCollection<FilterSourceItem> FilterSourceItems { get; }
        public IFilteredReadOnlyObservableCollection<FilterSourceItem> FilteredItems { get; }

        public CollectionsViewModel()
        {
            AddToReactiveCollectionCommand = new ReactiveCommand()
                .AddTo(Disposables);
            ReactiveCollection = AddToReactiveCollectionCommand
                .Select(_ => Guid.NewGuid())
                .ToReactiveCollection()
                .AddTo(Disposables);
            ClearReactiveCollectionCommand = new ReactiveCommand()
                .AddTo(Disposables);
            ClearReactiveCollectionCommand.ObserveOn(TaskPoolScheduler.Default)
                .Subscribe(_ =>
                {
                    ReactiveCollection.ClearOnScheduler();
                });

            GuidViewModels = _model.Guids
                .ToReadOnlyReactiveCollection(x => new GuidViewModel(x))
                .AddTo(Disposables);
            AddGuidToModelCommand = new ReactiveCommand()
                .AddTo(Disposables);
            AddGuidToModelCommand.ObserveOn(TaskPoolScheduler.Default)
                .Subscribe(_ => _model.Guids.Add(Guid.NewGuid()))
                .AddTo(Disposables);
            ClearModelGuidsCommand = new ReactiveCommand()
                .AddTo(Disposables);
            ClearModelGuidsCommand.ObserveOn(TaskPoolScheduler.Default)
                .Subscribe(_ => _model.Guids.Clear())
                .AddTo(Disposables);

            FilterSourceItems = new ObservableCollection<FilterSourceItem>();
            FilteredItems = FilterSourceItems
                .ToFilteredReadOnlyObservableCollection(x => x.Count >= 7)
                .AddTo(Disposables);
            AddToFilterSourceItemsCommand = new ReactiveCommand()
                .WithSubscribe(() => FilterSourceItems.Add(new FilterSourceItem()), Disposables.Add)
                .AddTo(Disposables);
            ClearFromFilterSourceItemsCommand = FilterSourceItems.ObserveProperty(x => x.Count)
                .Select(x => x != 0)
                .ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    foreach(var d in FilterSourceItems)
                    {
                        d.Dispose();
                    }

                    FilterSourceItems.Clear();
                }, Disposables.Add)
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
            _timer.ObserveOnUIDispatcher().Subscribe(_ => Count = Random.Next(10))
                .AddTo(Disposables);
            _timer.Start();
        }
    }
}
