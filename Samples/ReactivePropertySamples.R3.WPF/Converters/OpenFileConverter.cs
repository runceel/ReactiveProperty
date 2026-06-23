using System;
using Microsoft.Win32;
using R3;
using Reactive.Bindings.R3.Interactivity;

namespace ReactivePropertySamples.R3.WPF.Converters
{
    public class OpenFileConverter : ReactiveConverter<EventArgs, string>
    {
        protected override Observable<string> OnConvert(Observable<EventArgs> source) => source
            .Select(_ =>
            {
                var dlg = new OpenFileDialog();
                if (!(dlg.ShowDialog() ?? false))
                {
                    return null;
                }

                return dlg.FileName;
            })
            .Where(x => !string.IsNullOrWhiteSpace(x));
    }
}
