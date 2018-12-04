using System;

namespace XamarinAndroid.ViewModels
{
    class DisposableHolder : Java.Lang.Object
    {
        private IDisposable d;

        public DisposableHolder(IDisposable d)
        {
            this.d = d;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                this.d.Dispose();
            }
        }
    }
}
