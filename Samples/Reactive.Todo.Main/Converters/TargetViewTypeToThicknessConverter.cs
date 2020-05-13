using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
using Reactive.Todo.Main.Events;

namespace Reactive.Todo.Main.Converters
{
    public class TargetViewTypeToThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var x = (TargetViewType)value;
            var p = (TargetViewType)parameter;
            return x == p ? new Thickness(2) : new Thickness();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
