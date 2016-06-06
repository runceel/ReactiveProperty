using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Media;

namespace Reactive.Bindings.Extensions
{
    public static class ObserveEveryValueChangedExtensions
    {
        /// <summary>
        /// Publish target property when value is changed. If source is destructed, publish OnCompleted.
        /// </summary>
        public static IObservable<TProperty> ObserveEveryValueChanged<TSource, TProperty>(this TSource source, Func<TSource, TProperty> propertySelector, IEqualityComparer<TProperty> comparer = null)
            where TSource : class
        {
            if (source == null) return Observable.Empty<TProperty>();
            comparer = comparer ?? EqualityComparer<TProperty>.Default;

            var reference = new WeakReference(source);
            source = null;

            return Observable.Create<TProperty>(observer =>
            {
                var currentValue = default(TProperty);
                var prevValue = default(TProperty);

                var t = reference.Target;
                if (t != null)
                {
                    try
                    {
                        currentValue = propertySelector((TSource)t);
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                    }
                    finally
                    {
                        t = null;
                    }
                }
                else
                {
                    observer.OnCompleted();
                    return Disposable.Empty;
                }

                observer.OnNext(currentValue);
                prevValue = currentValue;

                return Observable.FromEvent<EventHandler, EventArgs>(
                        h => (sender, e) => h.Invoke(e),
                        h => CompositionTarget.Rendering += h,
                        h => CompositionTarget.Rendering -= h)
                    .Subscribe(_ =>
                    {
                        var target = reference.Target;
                        if (target != null)
                        {
                            try
                            {
                                currentValue = propertySelector((TSource)target);
                            }
                            catch (Exception ex)
                            {
                                observer.OnError(ex);
                            }
                            finally
                            {
                                target = null;
                            }
                        }
                        else
                        {
                            observer.OnCompleted();
                        }

                        if (!comparer.Equals(currentValue, prevValue))
                        {
                            observer.OnNext(currentValue);
                            prevValue = currentValue;
                        }
                    });
            });
        }
    }
}
