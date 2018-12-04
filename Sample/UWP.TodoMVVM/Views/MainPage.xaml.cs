using UWP.TodoMVVM.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace UWP.TodoMVVM.Views
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MainPageViewModel ViewModel => this.DataContext as MainPageViewModel;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void TextBoxTodoTitle_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter && e.KeyStatus.RepeatCount == 1)
            {
                if (this.ViewModel.AddTodoItemCommand.CanExecute())
                {
                    this.ViewModel.AddTodoItemCommand.Execute();
                }
            }
        }

        private void CheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            this.ViewModel.ChangeStateAll(checkBox.IsChecked == true);
        }
    }
}
