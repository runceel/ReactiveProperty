using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codeplex.Reactive.Interactivity
{
    public class PreviewInvokeEventArgs : EventArgs
    {
        public bool Cancelling { get; set; }
    }
}
