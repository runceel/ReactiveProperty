using System;

namespace Reactive.Bindings.Interactivity
{
    public class PreviewInvokeEventArgs : EventArgs
    {
        public bool Cancelling { get; set; }
    }
}
