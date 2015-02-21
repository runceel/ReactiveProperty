using Microsoft.Win32;
using Reactive.Bindings.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Reactive.Linq;

namespace WPF.Views
{
    /// <summary>
    /// EventToReactiveCommandWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EventToReactiveCommandWindow : Window
    {
        public EventToReactiveCommandWindow()
        {
            InitializeComponent();
        }
    }

    public class OpenFileDialogConverter : ReactiveConverter<EventArgs, string>
    {

        protected override IObservable<string> Convert(IObservable<EventArgs> source)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "*.*|*.*";

            return source
                .Select(_ => dlg)
                .Where(x => x.ShowDialog() == true)
                .Select(x => x.FileName);
        }
    }
}
