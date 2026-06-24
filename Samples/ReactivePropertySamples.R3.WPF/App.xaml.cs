using System.Windows;

namespace ReactivePropertySamples.R3.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // The original sample configured ReactivePropertyScheduler here. R3 has no global UI
        // scheduler; WPF installs a DispatcherSynchronizationContext on the UI thread, which
        // ObserveOnCurrentSynchronizationContext() / raiseEventContext rely on instead.
    }
}
