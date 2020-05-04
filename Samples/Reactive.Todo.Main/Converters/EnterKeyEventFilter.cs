using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Input;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Interactivity;

namespace Reactive.Todo.Main.Converters
{
    public class EnterKeyEventFilter : ReactiveConverter<KeyEventArgs, Unit>
    {
        protected override IObservable<Unit> OnConvert(IObservable<KeyEventArgs> source) => source
            .Where(x => x.Key == Key.Return)
            .ToUnit();
    }
}
