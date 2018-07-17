using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ReactiveProperty.Internals
{
    internal static class CompletionHelper
    {
        public static void AppendContinuation(ref object continuationField, Action newContinuation)
        {
            TRY_AGAIN:
            var field = continuationField;

            if (field == null)
            {
                var ret = Interlocked.CompareExchange(ref continuationField, newContinuation, null);
                if (ret == null) return; // ok to replace.
            }

            if (field is List<Action> list)
            {
                lock (field)
                {
                    list.Add(newContinuation);
                    if (field == continuationField)
                    {
                        return; // ok
                    }
                    goto TRY_AGAIN;
                }
            }
            else
            {
                list = new List<Action>(2) { (Action)field };
                list.Add(newContinuation);

                if (Interlocked.CompareExchange(ref continuationField, (object)list, field) == field)
                {
                    return;
                }
                goto TRY_AGAIN;
            }
        }

        public static void InvokeContinuation(ref object continuationField)
        {
            if (continuationField == null) return;
            var continuation = Interlocked.Exchange(ref continuationField, null);

            if (continuation is Action action)
            {
                action();
            }
            else
            {
                var list = (List<Action>)continuation;
                lock (list)
                {
                    var len = list.Count;
                    for (int i = 0; i < len; i++)
                    {
                        list[i].Invoke();
                    }
                }
            }
        }
    }
}
