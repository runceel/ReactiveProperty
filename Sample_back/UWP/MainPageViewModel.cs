using System;
using System.Collections.ObjectModel;
using Reactive.Bindings.Helpers;
using Windows.UI.Xaml;

namespace UWP
{
    class MainPageViewModel
    {
        public IFilteredReadOnlyObservableCollection<Item> Items { get; }

        public MainPageViewModel()
        {
            var source = new ObservableCollection<Item>();
            this.Items = source.ToFilteredReadOnlyObservableCollection(_ => true);

            var timer = new DispatcherTimer();
            timer.Tick += (_, __) =>
            {
                source.Add(new Item { Value = "item" + source.Count });
            };
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();
        }
    }
}
