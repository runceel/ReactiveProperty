using System.Windows;
using System.Windows.Input;
using R3;
using Reactive.Bindings.R3.Interactivity;

namespace ReactivePropertySamples.R3.WPF.Converters
{
    public class MouseEventConverter : ReactiveConverter<MouseEventArgs, string>
    {
        protected override Observable<string> OnConvert(Observable<MouseEventArgs> source) =>
            source.Select(x => x.GetPosition((IInputElement)AssociateObject))
                .Select(x => $"({(int)x.X}, {(int)x.Y})");
    }
}
