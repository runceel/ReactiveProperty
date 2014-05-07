
using Codeplex.Reactive.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace Codeplex.Reactive
{
    internal class SingletonPropertyChangedEventArgs
    {
        public static readonly PropertyChangedEventArgs Value = new PropertyChangedEventArgs("Value");
    }
    internal class SingletonDataErrorsChangedEventArgs
    {
        public static readonly DataErrorsChangedEventArgs Value = new DataErrorsChangedEventArgs("Value");
    }

    [Flags]
    public enum ReactivePropertyMode
    {
        None = 0x00,
        /// <summary>If next value is same as current, not set and not notify.</summary>
        DistinctUntilChanged = 0x01,
        /// <summary>Push notify on instance created and subscribed.</summary>
        RaiseLatestValueOnSubscribe = 0x02
    }

    // for EventToReactive and Serialization
    public interface IReactiveProperty
    {
        object Value { get; set; }
        IObservable<IEnumerable> ObserveErrorChanged { get; }
    }

    /// <summary>
    /// Two-way bindable IObserable&lt;T&gt;
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReactiveProperty<T> : IObservable<T>, IDisposable, INotifyPropertyChanged, IReactiveProperty, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        T latestValue;
        bool isDisposed = false;
        readonly IScheduler raiseEventScheduler;
        readonly IObservable<T> source;
        readonly Subject<T> anotherTrigger = new Subject<T>();
        readonly IDisposable sourceDisposable;
        readonly IDisposable raiseSubscription;

        // for Validation
        bool isValueChanged = false;
        readonly SerialDisposable validateNotifyErrorSubscription = new SerialDisposable();
        readonly Subject<IEnumerable> errorsTrigger = new Subject<IEnumerable>();
		List<Func<IObservable<T>, IObservable<IEnumerable>>> validatorStore = new List<Func<IObservable<T>, IObservable<IEnumerable>>>();

        /// <summary>PropertyChanged raise on UIDispatcherScheduler</summary>
        public ReactiveProperty()
            : this(default(T), ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe)
        {
        }

        /// <summary>PropertyChanged raise on UIDispatcherScheduler</summary>
        public ReactiveProperty(T initialValue = default(T), ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged|ReactivePropertyMode.RaiseLatestValueOnSubscribe)
            : this(UIDispatcherScheduler.Default, initialValue, mode)
        { }

        /// <summary>PropertyChanged raise on selected scheduler</summary>
        public ReactiveProperty(IScheduler raiseEventScheduler, T initialValue = default(T), ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged|ReactivePropertyMode.RaiseLatestValueOnSubscribe)
            : this(Observable.Never<T>(), raiseEventScheduler, initialValue, mode)
        {
        }

        // ToReactiveProperty Only
        internal ReactiveProperty(IObservable<T> source, T initialValue = default(T), ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged|ReactivePropertyMode.RaiseLatestValueOnSubscribe)
            : this(source, UIDispatcherScheduler.Default, initialValue, mode)
        {
        }

        internal ReactiveProperty(IObservable<T> source, IScheduler raiseEventScheduler, T initialValue = default(T), ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged|ReactivePropertyMode.RaiseLatestValueOnSubscribe)
        {
            this.latestValue = initialValue;
            this.raiseEventScheduler = raiseEventScheduler;

            // create source
            var merge = source.Merge(anotherTrigger);
            if (mode.HasFlag(ReactivePropertyMode.DistinctUntilChanged)) merge = merge.DistinctUntilChanged();
            merge = merge.Do(x =>
            {
                // setvalue immediately
                if (!isValueChanged) isValueChanged = true;
                latestValue = x;
            });

            // publish observable
            var connectable = (mode.HasFlag(ReactivePropertyMode.RaiseLatestValueOnSubscribe))
                ? merge.Publish(initialValue)
                : merge.Publish();
            this.source = connectable.AsObservable();

            // raise notification
            this.raiseSubscription = connectable
                .ObserveOn(raiseEventScheduler)
                .Subscribe(x =>
                {
                    var handler = PropertyChanged;
                    if (handler != null) PropertyChanged(this, SingletonPropertyChangedEventArgs.Value);
                });

            // start source
            this.sourceDisposable = connectable.Connect();
        }

        /// <summary>
        /// Get latestValue or push(set) value.
        /// </summary>
        public T Value
        {
            get { return latestValue; }
            set { anotherTrigger.OnNext(value); }
        }

        object IReactiveProperty.Value
        {
            get { return (T)Value; }
            set { Value = (T)value; }
        }

        /// <summary>
        /// Subscribe source.
        /// </summary>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            return source.Subscribe(observer);
        }

        /// <summary>
        /// Unsubcribe all subscription.
        /// </summary>
        public void Dispose()
        {
            if (isDisposed) return;

            isDisposed = true;
            anotherTrigger.Dispose();
            raiseSubscription.Dispose();
            sourceDisposable.Dispose();
            validateNotifyErrorSubscription.Dispose();
            errorsTrigger.OnCompleted();
            errorsTrigger.Dispose();
        }

        public override string ToString()
        {
            return (latestValue == null)
                ? "null"
                : "{" + latestValue.GetType().Name + ":" + latestValue.ToString() + "}";
        }

        // Validations

        /// <summary>
        /// <para>Checked validation, raised value. If success return value is null.</para>
        /// </summary>
        public IObservable<IEnumerable> ObserveErrorChanged
        {
            get { return errorsTrigger.AsObservable(); }
        }

        // INotifyDataErrorInfo

        IEnumerable currentErrors;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// <para>Set INotifyDataErrorInfo's asynchronous validation, return value is self.</para>
        /// </summary>
        /// <param name="validator">If success return IO&lt;null&gt;, failure return IO&lt;IEnumerable&gt;(Errors).</param>
        /// <returns>Self.</returns>
        public ReactiveProperty<T> SetValidateNotifyError(Func<IObservable<T>, IObservable<IEnumerable>> validator)
        {
            this.validatorStore.Add(validator);		//--- cache validation functions
            var validators	= this.validatorStore
							.Select(x => x(this.source))
							.ToArray();		//--- use copy
            this.validateNotifyErrorSubscription.Disposable
                = Observable.CombineLatest(validators)
				.Select(xs =>
                {
                    if (xs.Count == 0)          return null;
                    if (xs.All(x => x == null)) return null;

                    var strings = xs
                                .OfType<string>()
                                .Where(x => x != null);
                    var others  = xs
                                .Where(x => !(x is string))
                                .Where(x => x != null)
                                .SelectMany(x => x.Cast<object>());
                    return strings.Concat(others);
                })
                .Subscribe(x =>
                {
                    this.currentErrors = x;
                    var handler = this.ErrorsChanged;
                    if (handler != null)
                        this.raiseEventScheduler.Schedule(() => handler(this, SingletonDataErrorsChangedEventArgs.Value));
                    this.errorsTrigger.OnNext(x);
                });
            return this;
        }

        /// <summary>
        /// <para>Set INotifyDataErrorInfo's asynchronous validation, return value is self.</para>
        /// </summary>
        /// <param name="validator">If success return IO&lt;null&gt;, failure return IO&lt;IEnumerable&gt;(Errors).</param>
        /// <returns>Self.</returns>
        public ReactiveProperty<T> SetValidateNotifyError(Func<IObservable<T>, IObservable<string>> validator)
        {
            return this.SetValidateNotifyError(xs => validator(xs).Cast<IEnumerable>());
        }

        /// <summary>
        /// Set INotifyDataErrorInfo's asynchronous validation.
        /// </summary>
        /// <param name="validator">Validation logic</param>
        /// <returns>Self.</returns>
        public ReactiveProperty<T> SetValidateNotifyError(Func<T, Task<IEnumerable>> validator)
        {
			return this.SetValidateNotifyError(xs => xs.SelectMany(x => validator(x)));
        }

        /// <summary>
        /// Set INotifyDataErrorInfo's asynchronous validation.
        /// </summary>
        /// <param name="validator">Validation logic</param>
        /// <returns>Self.</returns>
        public ReactiveProperty<T> SetValidateNotifyError(Func<T, Task<string>> validator)
        {
			return this.SetValidateNotifyError(xs => xs.SelectMany(x => validator(x)));
        }

        /// <summary>
        /// Set INofityDataErrorInfo validation.
        /// </summary>
        /// <param name="validator">Validation logic</param>
        /// <returns>Self.</returns>
        public ReactiveProperty<T> SetValidateNotifyError(Func<T, IEnumerable> validator)
        {
			return this.SetValidateNotifyError(xs => xs.Select(x => validator(x)));
        }

        /// <summary>
        /// Set INofityDataErrorInfo validation.
        /// </summary>
        /// <param name="validator">Validation logic</param>
        /// <returns>Self.</returns>
        public ReactiveProperty<T> SetValidateNotifyError(Func<T, string> validator)
        {
			return this.SetValidateNotifyError(xs => xs.Select(x => validator(x)));
        }

        /// <summary>Get INotifyDataErrorInfo's error store</summary>
        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            return currentErrors;
        }

        /// <summary>Get INotifyDataErrorInfo's error store</summary>
        public bool HasErrors
        {
            get { return currentErrors != null; }
        }
    }

    /// <summary>
    /// Static methods and extension methods of ReactiveProperty&lt;T&gt;
    /// </summary>
    public static class ReactiveProperty
    {
        /// <summary>
        /// <para>Convert plain object to ReactiveProperty.</para>
        /// <para>Value is OneWayToSource(ReactiveProperty -> Object) synchronized.</para>
        /// <para>PropertyChanged raise on UIDispatcherScheduler</para>
        /// </summary>
        public static ReactiveProperty<TProperty> FromObject<TTarget, TProperty>(
            TTarget target,
            Expression<Func<TTarget, TProperty>> propertySelector,
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged|ReactivePropertyMode.RaiseLatestValueOnSubscribe)
        {
            return FromObject(target, propertySelector, UIDispatcherScheduler.Default, mode);
        }

        /// <summary>
        /// <para>Convert plain object to ReactiveProperty.</para>
        /// <para>Value is OneWayToSource(ReactiveProperty -> Object) synchronized.</para>
        /// <para>PropertyChanged raise on selected scheduler</para>
        /// </summary>
        public static ReactiveProperty<TProperty> FromObject<TTarget, TProperty>(
            TTarget target,
            Expression<Func<TTarget, TProperty>> propertySelector,
            IScheduler raiseEventScheduler,
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged|ReactivePropertyMode.RaiseLatestValueOnSubscribe)
        {
            string propertyName; // no use
            var getter = AccessorCache<TTarget>.LookupGet(propertySelector, out propertyName);
            var setter = AccessorCache<TTarget>.LookupSet(propertySelector, out propertyName);

            var result = new ReactiveProperty<TProperty>(raiseEventScheduler, initialValue: getter(target), mode: mode);
            result.Subscribe(x => setter(target, x));

            return result;
        }

        /// <summary>
        /// <para>Convert plain object to ReactiveProperty.</para>
        /// <para>Value is OneWayToSource(ReactiveProperty -> Object) synchronized.</para>
        /// <para>PropertyChanged raise on UIDispatcherScheduler</para>
        /// </summary>
        public static ReactiveProperty<TResult> FromObject<TTarget, TProperty, TResult>(
            TTarget target,
            Expression<Func<TTarget, TProperty>> propertySelector,
            Func<TProperty, TResult> convert,
            Func<TResult, TProperty> convertBack,
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged|ReactivePropertyMode.RaiseLatestValueOnSubscribe)
        {
            return FromObject(target, propertySelector, convert, convertBack, UIDispatcherScheduler.Default, mode);
        }

        /// <summary>
        /// <para>Convert plain object to ReactiveProperty.</para>
        /// <para>Value is OneWayToSource(ReactiveProperty -> Object) synchronized.</para>
        /// <para>PropertyChanged raise on selected scheduler</para>
        /// </summary>
        public static ReactiveProperty<TResult> FromObject<TTarget, TProperty, TResult>(
            TTarget target,
            Expression<Func<TTarget, TProperty>> propertySelector,
            Func<TProperty, TResult> convert,
            Func<TResult, TProperty> convertBack,
            IScheduler raiseEventScheduler,
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged|ReactivePropertyMode.RaiseLatestValueOnSubscribe)
        {
            string propertyName; // no use
            var getter = AccessorCache<TTarget>.LookupGet(propertySelector, out propertyName);
            var setter = AccessorCache<TTarget>.LookupSet(propertySelector, out propertyName);

            var result = new ReactiveProperty<TResult>(raiseEventScheduler, initialValue: convert(getter(target)), mode: mode);
            result.Select(convertBack).Subscribe(x => setter(target, x));

            return result;
        }


        /// <summary>
        /// <para>Convert to two-way bindable IObservable&lt;T&gt;</para>
        /// <para>PropertyChanged raise on UIDispatcherScheduler</para>
        /// </summary>
        public static ReactiveProperty<T> ToReactiveProperty<T>(this IObservable<T> source,
            T initialValue = default(T),
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged|ReactivePropertyMode.RaiseLatestValueOnSubscribe)
        {
            return new ReactiveProperty<T>(source, initialValue, mode);
        }

        /// <summary>
        /// <para>Convert to two-way bindable IObservable&lt;T&gt;</para>
        /// <para>PropertyChanged raise on selected scheduler</para>
        /// </summary>
        public static ReactiveProperty<T> ToReactiveProperty<T>(this IObservable<T> source,
            IScheduler raiseEventScheduler,
            T initialValue = default(T),
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged|ReactivePropertyMode.RaiseLatestValueOnSubscribe)
        {
            return new ReactiveProperty<T>(source, raiseEventScheduler, initialValue, mode);
        }
    }
}