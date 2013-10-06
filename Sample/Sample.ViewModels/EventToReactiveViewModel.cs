using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codeplex.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using System.Windows;
using System.Reactive;

namespace Sample.ViewModels
{
    public class EventToReactiveViewModel
    {
        // binding from UI, event direct bind
        public ReactiveProperty<Unit> MouseDown { get; private set; }
        // binding from UI, event with converter
        public ReactiveProperty<Tuple<int, int>> MouseMove { get; private set; }
        // binding from UI, IgnoreEventArgs = true
        public ReactiveProperty<Unit> MouseEnter { get; private set; }

        public ReactiveProperty<string> CurrentPoint { get; private set; }
        public ReactiveProperty<string> Entered { get; private set; }

        public ReactiveProperty<string> AlertMessage { get; private set; }

        public EventToReactiveViewModel()
        {
            // mode off RaiseLatestValueOnSubscribe, because initialValue is null.
            // mode off DistinctUntilChanged, because if Unit no send any values.
            var none = ReactivePropertyMode.None;

            MouseMove = new ReactiveProperty<Tuple<int, int>>(mode: none);
            MouseDown = new ReactiveProperty<Unit>(mode: none);
            MouseEnter = new ReactiveProperty<Unit>(mode: none);

            CurrentPoint = MouseMove
                .Select(p => string.Format("X:{0} Y:{1}", p.Item1, p.Item2))
                .ToReactiveProperty();

            Entered = MouseEnter
                .Select(_ => Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1)))
                .Switch()
                .Select(x => "entered:" + x + "sec")
                .ToReactiveProperty();

            this.AlertMessage = MouseDown.Select(_ => "MouseDown!").ToReactiveProperty(mode: none);
        }
    }

    // EventToReactive convert functions
    // Converter/IgnoreEventArgs is useful for unit testings
    // for example, MouseMove.Value = new Point(10, 10) is simulate MouseMove
    // MouseEnter.Value = new Unit() is simulate raise MouseEnter event
    public class Converters
    {
        public Func<object, object> MouseEventToPoint { get; private set; }

        public Converters()
        {
            MouseEventToPoint = ev =>
                {
                    var position = ((dynamic)ev).GetPosition(null);
                    return Tuple.Create((int)position.X, (int)position.Y);
                };
        }
    }
}