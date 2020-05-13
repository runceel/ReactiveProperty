using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Reactive.Bindings.Interactivity;

namespace ReactivePropertySamples.WPF.Converters
{
    public class MouseEventConverter : ReactiveConverter<MouseEventArgs, string>
    {
        protected override IObservable<string> OnConvert(IObservable<MouseEventArgs> source) => 
            source.Select(x => x.GetPosition((IInputElement)AssociateObject))
                .Select(x => $"({(int)x.X}, {(int)x.Y}");
    }
}
