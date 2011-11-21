using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codeplex.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using System.Windows;
using System.Reactive;

namespace WPF.ViewModels
{
    public class EventToReactiveViewModel
    {
        // binding from UI, event direct bind
        public ReactiveProperty<MouseEventArgs> MouseDown { get; private set; }
        // binding from UI, event with converter
        public ReactiveProperty<Point> MouseMove { get; private set; }

        public ReactiveProperty<string> CurrentPoint { get; private set; }

        public EventToReactiveViewModel()
        {
            // mode off RaiseLatestValueOnSubscribe, because initialValue is null.
            var mode = ReactivePropertyMode.DistinctUntilChanged;

            MouseMove = new ReactiveProperty<Point>(mode: mode);
            MouseDown = new ReactiveProperty<MouseEventArgs>(mode: mode);

            CurrentPoint = MouseMove
                .Select(p => string.Format("X:{0} Y:{1}", p.X, p.Y))
                .ToReactiveProperty();

            MouseDown.Subscribe(_ => MessageBox.Show("MouseDown!"));
        }
    }

    // EventToReactive convert functions
    public class Converters
    {
        public Func<object, object> MouseEventToPoint { get; private set; }
        public Func<object, object> ToUnit { get; private set; }

        public Converters()
        {
            MouseEventToPoint = ev => ((MouseEventArgs)ev).GetPosition(null);

            // ToUnit is useful for testability
            // for example: MouseDown.Value = new Unit();
            // this simulates raise event
            ToUnit = _ => new Unit();
        }
    }
}