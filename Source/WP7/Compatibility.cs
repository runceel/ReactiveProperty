using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using Microsoft.Phone.Reactive;

namespace Codeplex.Reactive
{
    // Compatible with .NET
    internal static class Compatibility
    {
        public static bool HasFlag(this Enum self, Enum flags)
        {
            if (self.GetType() != flags.GetType()) throw new ArgumentException("not sampe type");

            var sval = Convert.ToUInt64(self);
            var fval = Convert.ToUInt64(flags);

            return (sval & fval) == fval;
        }

        public static IObservable<TSource> Concat<TSource>(this IObservable<IObservable<TSource>> sources)
        {
            if (sources == null) throw new ArgumentNullException("sources");

            return Observable.CreateWithDisposable<TSource>(observer =>
            {
                var lockObject = new object();
                var isCompleted = false;
                var isActive = false;
                var q = new Queue<IObservable<TSource>>();
                var disposables = new CompositeDisposable();

                Action<IObservable<TSource>> subscribe = null;
                subscribe = innerSource =>
                {
                    lock (lockObject)
                    {
                        var innerSubscription = new MutableDisposable();
                        disposables.Add(innerSubscription);

                        innerSubscription.Disposable = innerSource.Subscribe(x =>
                        {
                            lock (lockObject)
                            {
                                observer.OnNext(x);
                            }
                        }
                        , (exception) =>
                        {
                            lock (lockObject)
                            {
                                observer.OnError(exception);
                            }
                        }
                        , () =>
                        {
                            disposables.Remove(innerSubscription);

                            lock (lockObject)
                            {
                                if (q.Count > 0)
                                {
                                    var obj = q.Dequeue();
                                    subscribe(obj);
                                }
                                else
                                {
                                    isActive = false;
                                    if (isCompleted)
                                    {
                                        observer.OnCompleted();
                                    }
                                }
                            }
                        });
                    }
                };

                var outerSubscription = sources.Subscribe(innerSource =>
                {
                    lock (lockObject)
                    {
                        if (!isActive)
                        {
                            isActive = true;
                            subscribe(innerSource);
                        }
                        else
                        {
                            q.Enqueue(innerSource);
                        }
                    }
                }
                , (exception) =>
                {
                    lock (lockObject)
                    {
                        observer.OnError(exception);
                    }
                }, () =>
                {
                    lock (lockObject)
                    {
                        isCompleted = true;
                        if (!isActive)
                        {
                            observer.OnCompleted();
                        }
                    }
                });

                disposables.Add(outerSubscription);
                return disposables;
            });
        }
    }
}