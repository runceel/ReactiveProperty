using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using Microsoft.Win32;
using Reactive.Bindings.Interactivity;

namespace ReactivePropertySamples.WPF.Converters
{
    public class OpenFileConverter : ReactiveConverter<EventArgs, string>
    {
        protected override IObservable<string> OnConvert(IObservable<EventArgs> source) => source
            .Select(_ =>
            {
                var dlg = new OpenFileDialog();
                if (!dlg.ShowDialog() ?? false)
                {
                    return null;
                }

                return dlg.FileName;
            })
            .Where(x => !string.IsNullOrWhiteSpace(x));
    }
}
