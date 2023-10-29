using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;

namespace ReactiveProperty.WPF.ManualTests;
internal class ValidationViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    [Required]
    public ReactiveProperty<string> RequiredText { get; }

    public ValidationViewModel()
    {
        RequiredText = new ReactiveProperty<string>("")
            .SetValidateAttribute(() => RequiredText);
    }
}
