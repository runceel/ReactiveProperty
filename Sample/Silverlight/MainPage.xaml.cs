using System;
using System.Collections.Generic;
using System.Linq;
using Codeplex.Reactive;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Reactive.Linq;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel.DataAnnotations;
using Codeplex.Reactive.Extensions;

namespace Silverlight
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = new MainPageViewModel();
        }
    }

    public class MainPageViewModel
    {
        [Required]
        [Range(1, 10)]
        public ReactiveProperty<string> OnException { get; private set; }
        public ReactiveProperty<string> OnDataError { get; private set; }
        public ReactiveProperty<string> OnNotifyError { get; private set; }
        public ReactiveCommand Command1 { get; set; }

        public MainPageViewModel()
        {
            OnException = new ReactiveProperty<string>()
                .SetValidateAttribute(() => OnException);

            OnDataError = new ReactiveProperty<string>()
                .SetValidateError(s => s != null && s.All(Char.IsUpper) ? null : "ERROR!");

            OnNotifyError = new ReactiveProperty<string>()
                .SetValidateNotifyError(self => self
                    .Select(s => s != null && s.All(Char.IsLower) ? null : new[] { "ERROR" }));


            Command1 = OnException.ErrorsChanged
                .CombineLatest(OnDataError.ErrorsChanged, OnNotifyError.ErrorsChanged,
                    (t1, t2, t3) => new[] { t1, t2, t3 }.All(x => x == null))
                .ToReactiveCommand();
        }


    }
}
