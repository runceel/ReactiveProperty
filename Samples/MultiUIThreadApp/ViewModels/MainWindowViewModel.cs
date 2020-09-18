using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using MultiUIThreadApp.Models;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace MultiUIThreadApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ReadOnlyReactiveCollection<Item> Guids { get; }

        public ReactiveCommand AddNewGuidCommand { get; }

        public MainWindowViewModel()
        {
            Guids = GuidList.Items.ToReadOnlyReactiveCollection()
                .AddTo(Disposables);
            AddNewGuidCommand = new ReactiveCommand()
                .WithSubscribe(() => GuidList.AddNewGuid())
                .AddTo(Disposables);
        }
    }
}
