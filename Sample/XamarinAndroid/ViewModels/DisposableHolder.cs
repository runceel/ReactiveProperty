using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

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