using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Linq;
using Codeplex.Reactive.Extensions;

namespace ReactiveProperty.Extensions
{
    public static class INotifyDataErrorInfoExtensions
    {
        public static IObservable<DataErrorsChangedEventArgs> ErrorsChangedAsObservable<T>(this T source)
            where T : INotifyDataErrorInfo
        {
            return Observable.FromEvent<EventHandler<DataErrorsChangedEventArgs>, DataErrorsChangedEventArgs>(
                h => (sender, e) => h(e),
                h => source.ErrorsChanged += h,
                h => source.ErrorsChanged -= h);
        }

        public static IObservable<TProperty> ObserveErrorInfo<TSubject, TProperty>(
            this TSubject subject, Expression<Func<TSubject, TProperty>> propertySelector)
            where TSubject : INotifyDataErrorInfo
        {
            string propertyName;
            var accessor = AccessorCache<TSubject>.Lookup(propertySelector, out propertyName);

            return subject.ErrorsChangedAsObservable()
                .Where(e => e.PropertyName == propertyName)
                .Select(_ => ((Func<TSubject, TProperty>)accessor).Invoke(subject));
        }
    }
}
