using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reactive.Bindings;
using System.Reactive.Linq;
using System.Windows.Input;
using System.Windows;
using System.Reactive;
using Reactive.Bindings.Interactivity;

namespace Sample.ViewModels
{
    public class EventToReactivePropertyViewModel
    {
        // binding from UI, event direct bind
        public ReactiveProperty<Unit> MouseDown { get; }
        // binding from UI, event with converter
        public ReactiveProperty<Tuple<int, int>> MouseMove { get; }
        // binding from UI, IgnoreEventArgs = true
        public ReactiveProperty<Unit> MouseEnter { get; }

        public ReactiveProperty<string> CurrentPoint { get; }
        public ReactiveProperty<string> Entered { get; }

        public ReactiveProperty<string> AlertMessage { get; }

        public EventToReactivePropertyViewModel()
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

    // EventToReactiveProperty converter.
    // Converter/IgnoreEventArgs is useful for unit testings.
    // For example, MouseMovoe.Value = new Point(10, 10) is simulate MouseMove
    // MouseEnter.Value = new Unit() is simulate raise MouseEnter event.
    public class MouseEventToPointConverter : ReactiveConverter<dynamic, Tuple<int, int>>
    {
        protected override IObservable<Tuple<int, int>> OnConvert(IObservable<dynamic> source)
        {
            return source
                .Select(x => x.GetPosition(null))
                .Select(x => Tuple.Create((int)x.X, (int)x.Y));
        }
    }
}