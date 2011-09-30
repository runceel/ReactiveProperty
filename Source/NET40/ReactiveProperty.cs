using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Linq;

#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#else
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.ComponentModel;
using System.Reactive.Disposables;
#endif

namespace Codeplex.Reactive
{
    internal class SingletonPropertyChangedEventArgs
    {
        public static readonly PropertyChangedEventArgs Value = new PropertyChangedEventArgs("Value");
    }

    [Flags]
    public enum ReactivePropertyMode
    {
        None = 0x00,
        DistinctUntilChanged = 0x01,
        PropertyChangedInvokeOnUIDispatcher = 0x02,
        RaiseLatestValueOnSubscribe = 0x04,
        /// <summary>DistinctUntilChanged | PropertyChangedInvokeOnUIDispatcher | RaiseLatestValueOnSubscribe</summary>
        All = DistinctUntilChanged | PropertyChangedInvokeOnUIDispatcher | RaiseLatestValueOnSubscribe
    }

    // TODO:WP7版のRxにin/outがないので要修正
    public interface IReactiveProperty<out T> : IObservable<T>, IDisposable
    {
        object Value { get; set; }
    }

    public class ReactiveProperty<T> : INotifyPropertyChanged, IDataErrorInfo, IReactiveProperty<T>
#if SILVERLIGHT
, INotifyDataErrorInfo
#endif
    {
        public event PropertyChangedEventHandler PropertyChanged;

        T latestValue;
        readonly IObservable<T> source;
        readonly Subject<T> anotherTrigger = new Subject<T>();
        readonly IDisposable sourceDisposable;

        // for Validation
        SerialDisposable validateErrorSubscription = new SerialDisposable();
        SerialDisposable validateNotifyErrorSubscription = new SerialDisposable();
        ValidationAttribute[] attributes;
        BehaviorSubject<object> errorsTrigger = new BehaviorSubject<object>(null);
        readonly ValidationContext validationContext;


        public ReactiveProperty(T initialValue = default(T), ReactivePropertyMode mode = ReactivePropertyMode.All)
            : this(Observable.Never<T>(), null, initialValue, mode)
        { }

        public ReactiveProperty(Action<T> parentRaisePropertyChanged, T initialValue = default(T), ReactivePropertyMode mode = ReactivePropertyMode.All)
            : this(Observable.Never<T>(), parentRaisePropertyChanged, initialValue, mode)
        { }

        // ToReactivePropery Only
        internal ReactiveProperty(IObservable<T> source, T initialValue = default(T), ReactivePropertyMode mode = ReactivePropertyMode.All)
            : this(source, null, initialValue, mode)
        { }

        // ToReactivePropery Only
        internal ReactiveProperty(IObservable<T> source, Action<T> parentRaisePropertyChanged, T initialValue = default(T),
            ReactivePropertyMode mode = ReactivePropertyMode.All)
        {
            this.latestValue = initialValue;
            this.validationContext = new ValidationContext(this, null, null) { MemberName = "Value" };

            // create source
            var merge = source.Merge(anotherTrigger);
            if (mode.HasFlag(ReactivePropertyMode.DistinctUntilChanged)) merge = merge.DistinctUntilChanged();

            // publish observable
            var connectable = (mode.HasFlag(ReactivePropertyMode.RaiseLatestValueOnSubscribe))
                ? merge.Publish(initialValue)
                : merge.Publish();
            this.source = connectable.AsObservable();

            // set value immediately
            var setValue = connectable.Do(x => latestValue = x);

            // raise notification
            (mode.HasFlag(ReactivePropertyMode.PropertyChangedInvokeOnUIDispatcher)
                    ? setValue.ObserveOnUIDispatcher()
                    : setValue)
                .Subscribe(x =>
                {
                    var handler = PropertyChanged;
                    if (handler != null) PropertyChanged(this, SingletonPropertyChangedEventArgs.Value);
                    if (parentRaisePropertyChanged != null) parentRaisePropertyChanged(x);
                });

            // start subscription
            this.sourceDisposable = connectable.Connect();
        }

        public T Value
        {
            get { return latestValue; }
            set
            {
                anotherTrigger.OnNext(value);

                if (attributes != null)
                {
                    try
                    {
                        foreach (var item in attributes)
                        {
                            item.Validate(value, validationContext);
                        }
                        errorsTrigger.OnNext(null);
                    }
                    catch (Exception ex)
                    {
                        errorsTrigger.OnNext(ex);
                        throw;
                    }
                }
            }
        }

        object IReactiveProperty<T>.Value
        {
            get { return (T)Value; }
            set { Value = (T)value; }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return source.Subscribe(observer);
        }

        public void Dispose()
        {
            sourceDisposable.Dispose();
            validateErrorSubscription.Dispose();
            validateNotifyErrorSubscription.Dispose();
            errorsTrigger.Dispose();
        }

        public override string ToString()
        {
            return (latestValue == null)
                ? "null"
                : "{" + latestValue.GetType().Name + ":" + latestValue.ToString() + "}";
        }

        // Validations

        public IObservable<object> ErrorsChanged
        {
            get { return errorsTrigger.AsObservable(); }
        }

        // Exception

        public ReactiveProperty<T> SetValidateAttribute(Expression<Func<ReactiveProperty<T>>> selfSelector)
        {
            this.attributes = ((MemberExpression)selfSelector.Body).Member
                .GetCustomAttributes(typeof(ValidationAttribute), true)
                .Cast<ValidationAttribute>()
                .ToArray();
            return this;
        }

        // IDataErrorInfo

        string currentError;

        public ReactiveProperty<T> SetValidateError(Func<T, string> validate)
        {
            validateErrorSubscription.Disposable = source
                .Subscribe(x =>
                {
                    currentError = validate(x);
                    errorsTrigger.OnNext(currentError);
                });
            return this;
        }

        public string Error
        {
            get { return currentError; }
        }

        public string this[string columnName]
        {
            get { return currentError; }
        }

#if SILVERLIGHT
        // INotifyDataErrorInfo

        IEnumerable currentErrors;

        EventHandler<DataErrorsChangedEventArgs> errorsChanged;
        event EventHandler<DataErrorsChangedEventArgs> INotifyDataErrorInfo.ErrorsChanged
        {
            add { errorsChanged += value; }
            remove { errorsChanged -= value; }
        }

        public ReactiveProperty<T> SetValidateNotifyError(Func<IObservable<T>, IObservable<IEnumerable>> validate)
        {
            validateNotifyErrorSubscription.Disposable = validate(source)
                .Subscribe(xs =>
                {
                    currentErrors = xs;
                    errorsTrigger.OnNext(currentErrors);
                });

            return this;
        }

        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            return currentErrors;
        }

        public bool HasErrors
        {
            get { return currentErrors != null; }
        }

#endif
    }

    public static class ReactivePropertyObservableExtensions
    {
        public static ReactiveProperty<T> ToReactiveProperty<T>(this IObservable<T> source,
            T initialValue = default(T),
            ReactivePropertyMode mode = ReactivePropertyMode.All)
        {
            return new ReactiveProperty<T>(source, initialValue, mode);
        }

        public static ReactiveProperty<T> ToReactiveProperty<T>(this IObservable<T> source,
            Action<T> parentRaisePropertyChanged, T initialValue = default(T),
            ReactivePropertyMode mode = ReactivePropertyMode.All)
        {
            return new ReactiveProperty<T>(source, parentRaisePropertyChanged, initialValue, mode);
        }
    }
}