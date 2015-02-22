using Reactive.Bindings.Interactivity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace StoreApp.Views
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class EventToReactiveCommand : Page
    {
        public EventToReactiveCommand()
        {
            this.InitializeComponent();
        }

    }

    public class SelectFileConverter : ReactiveConverter<RoutedEventArgs, string>
    {
        protected override IObservable<string> OnConvert(IObservable<RoutedEventArgs> source)
        {
            return source
                .Select(_ => new FileOpenPicker()) // create picker
                .Do(x => x.FileTypeFilter.Add(".txt")) // set extensions
                .SelectMany(x => x.PickSingleFileAsync().AsTask().ToObservable()) // convert task to iobservable
                .Where(x => x != null) // filter
                .Select(x => x.Path); // convert
        }
    }

}
