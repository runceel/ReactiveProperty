using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace Reactive.Bindings.Notifiers
{
    /// <summary>
    /// Notify of busy.
    /// </summary>
    public class BusyNotifier : INotifyPropertyChanged, IObservable<bool>
    {

        private static readonly PropertyChangedEventArgs IsBusyPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(IsBusy));

        /// <summary>
        /// property changed event handler
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private int ProcessCounter { get; set; }

        private Subject<bool> IsBusySubject { get; } = new Subject<bool>();

        private object LockObject { get; } = new object();

        private bool isBusy;

        /// <summary>
        /// Is process running.
        /// </summary>
        public bool IsBusy
        {
            get { return this.isBusy; }
            set
            {
                if (this.isBusy == value) { return; }
                this.isBusy = value;
                this.PropertyChanged?.Invoke(this, IsBusyPropertyChangedEventArgs);
                this.IsBusySubject.OnNext(this.isBusy);
            }
        }

        /// <summary>
        /// Process start.
        /// </summary>
        /// <returns>Call dispose method when process end.</returns>
        public IDisposable ProcessStart()
        {
            lock (this.LockObject)
            {
                this.ProcessCounter++;
                this.IsBusy = this.ProcessCounter != 0;
                return Disposable.Create(() =>
                {
                    lock (this.LockObject)
                    {
                        this.ProcessCounter--;
                        this.IsBusy = this.ProcessCounter != 0;
                    }
                });
            }
        }

        /// <summary>
        /// Subscribe busy.
        /// </summary>
        /// <param name="observer">observer</param>
        /// <returns>disposable</returns>
        public IDisposable Subscribe(IObserver<bool> observer)
        {
            observer.OnNext(this.IsBusy);
            return this.IsBusySubject.Subscribe(observer);
        }
    }
}
