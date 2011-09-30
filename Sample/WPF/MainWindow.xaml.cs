using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Codeplex.Reactive.Notifier;
using Codeplex.Reactive;
using GalaSoft.MvvmLight;
using System.Reactive.Linq;
using Codeplex.Reactive.Extensions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WPF
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            DataContext = new MainWindowViewModel();







        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() => { throw new Exception(); }));
        }
    }

    public class A : IServiceProvider
    {

        public object GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }


    public class MainWindowViewModel
    {
        [Required]
        [Range(1, 10, ErrorMessage = "hogehoge")]
        public ReactiveProperty<string> TadanoText { get; set; }


        public ReactiveCommand MessageBoxCommand { get; private set; }

        public MainWindowViewModel()
        {


            // IDataErrorInfoは   T -> string
            // INotifyErrorInfoは IO<T> -> IO<IEnumerable>
            // で設定させるつもり、これはIDataErrorInfoのほう。
            TadanoText = new ReactiveProperty<string>()
                .SetValidateError(s =>
                {
                    return null;
                    //if (s == null) return null;
                    //return s.All(Char.IsUpper)
                    //    ? null
                    //    : "全部大文字じゃありません！";
                })
                .SetValidateAttribute(() => TadanoText);

            

            

            var vc = new ValidationContext(this, null, null) { MemberName = "a" };


            // 検証の結果はErrorsChangedに送られてくる
            // それをそのままICommandのCanExecuteに流す生成
            MessageBoxCommand = TadanoText.ErrorsChanged
                .Select(o => o == null)
                .ToReactiveCommand();

            // CommandのExecuteの設定はSubscribeでやる
            MessageBoxCommand.Subscribe(_ => MessageBox.Show("ほむほむ"));
        }
    }










}
