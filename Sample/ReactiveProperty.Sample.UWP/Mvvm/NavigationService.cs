using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace ReactiveProperty.Sample.UWP.Mvvm
{
    public interface INavigationService
    {
        void Navigate(string viewName, object parameter);
    }

    public class NavigationService : INavigationService
    {
        private static readonly Dictionary<string, Type> _viewTypeMap = new Dictionary<string, Type>();

        public static void RegisteryViewType(string viewName, Type type) => _viewTypeMap[viewName] = type;

        private readonly Frame _frame;

        public NavigationService(Frame frame)
        {
            _frame = frame;
        }
        public void Navigate(string viewName, object parameter)
        {
            _frame.Navigate(_viewTypeMap[viewName], parameter);
        }
    }
}
