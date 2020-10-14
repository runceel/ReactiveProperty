using System;
using System.Collections.Generic;
using System.Text;
using MultiUIThreadApp.Models;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace MultiUIThreadApp.ViewModels
{
    public class ListBoxWindowViewModel : ViewModelBase
    {
        public ReadOnlyReactiveCollection<Item> Guids { get; }

        public ListBoxWindowViewModel()
        {
            Guids = GuidList.Items.ToReadOnlyReactiveCollection()
                .AddTo(Disposables);
        }
    }
}
