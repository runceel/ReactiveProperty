using System;
using System.Linq.Expressions;
using System.Linq;
using System.Collections;
using System.ComponentModel;
using Codeplex.Reactive.Extensions;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
using SerialDisposable = Microsoft.Phone.Reactive.MutableDisposable;
#else
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Disposables;
using System.Reactive.Concurrency;
#endif
#if !WP_COMMON
using System.ComponentModel.DataAnnotations;
#endif

namespace Codeplex.Reactive
{
    internal class SingletonPropertyChangedEventArgs
    {
        public static readonly PropertyChangedEventArgs Value = new PropertyChangedEventArgs("Value");
    }
#if SILVERLIGHT
    internal class SingletonDataErrorsChangedEventArgs
    {
        public static readonly DataErrorsChangedEventArgs Value = new DataErrorsChangedEventArgs("Value");
    }
#endif

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

    // for EventToReactive
    internal interface IValue
    {
        object Value { get; set; }
    }

    public class ReactiveProperty<T> : IObservable<T>, IDisposable, INotifyPropertyChanged, IValue, IDataErrorInfo
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
        readonly SerialDisposable validateNotifyErrorSubscription = new SerialDisposable();
        readonly BehaviorSubject<object> errorsTrigger = new BehaviorSubject<object>(null);

        public ReactiveProperty(T initialValue = default(T), ReactivePropertyMode mode = ReactivePropertyMode.All)
            : this(Observable.Never<T>(), null, initialValue, mode)
        { }

        public ReactiveProperty(Action<T> parentRaisePropertyChanged, T initialValue = default(T), ReactivePropertyMode mode = ReactivePropertyMode.All)
            : this(Observable.Never<T>(), parentRaisePropertyChanged, initialValue, mode)
        { }

        // ToReactiveProperty Only
        internal ReactiveProperty(IObservable<T> source, T initialValue = default(T), ReactivePropertyMode mode = ReactivePropertyMode.All)
            : this(source, null, initialValue, mode)
        { }

        // ToReactiveProperty Only
        internal ReactiveProperty(IObservable<T> source, Action<T> parentRaisePropertyChanged, T initialValue = default(T),
            ReactivePropertyMode mode = ReactivePropertyMode.All)
        {
            this.latestValue = initialValue;

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
            set { anotherTrigger.OnNext(value); }
        }

        object IValue.Value
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

#if! WP_COMMON
        ValidationContext validationContext;
        ValidationAttribute[] attributes;

        public ReactiveProperty<T> SetValidateAttribute(Expression<Func<ReactiveProperty<T>>> selfSelector)
        {
            this.attributes = ((MemberExpression)selfSelector.Body).Member
                .GetCustomAttributes(typeof(ValidationAttribute), true)
                .Cast<ValidationAttribute>()
                .ToArray();
            return this;
        }

        string ValidateException()
        {
            try
            {
                if (validationContext == null)
                {
                    validationContext = new ValidationContext(this, null, null) { MemberName = "Value" };
                }

                foreach (var item in attributes)
                {
                    item.Validate(latestValue, validationContext);
                }

                return null;
            }
            catch (Exception ex)
            {
                errorsTrigger.OnNext(ex);
                return ex.Message;
            }
        }
#endif

        // IDataErrorInfo

        Func<T, string> dataErrorInfoValidate;
        string currentError;

        public ReactiveProperty<T> SetValidateError(Func<T, string> validate)
        {
            this.dataErrorInfoValidate = validate;
            return this;
        }

        public string Error
        {
            get { return currentError; }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
#if! WP_COMMON
                if (attributes != null && columnName == "Value")
                {
                    var exceptionResult = ValidateException();
                    if (exceptionResult != null)
                    {
                        return exceptionResult;
                    }
                }
#endif

                var handler = dataErrorInfoValidate;
                if (handler != null && columnName == "Value")
                {
                    currentError = handler(latestValue);
                    errorsTrigger.OnNext(currentError);
                    return currentError;
                }

                errorsTrigger.OnNext(null);
                return null;
            }
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
                    var handler = errorsChanged;
                    if (handler != null)
                    {
                        UIDispatcherScheduler.Default.Schedule(() =>
                            handler(this, SingletonDataErrorsChangedEventArgs.Value));
                    }
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