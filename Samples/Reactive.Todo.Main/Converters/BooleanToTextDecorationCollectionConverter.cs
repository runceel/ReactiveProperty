using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Reactive.Todo.Main.Converters
{
    public class BooleanToTextDecorationCollectionConverter : IValueConverter
    {
        private static TextDecorationCollection Strikethrough { get; }

        static BooleanToTextDecorationCollectionConverter()
        {
            Strikethrough = new TextDecorationCollection
            {
                TextDecorations.Strikethrough,
            };
            Strikethrough.Freeze();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => 
            (bool)value ? Strikethrough : null;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
