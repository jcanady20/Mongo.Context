using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mongo.Context.AppHost.Extensions
{
    public static class IEnumerableExtensions
    {
        public static bool HasItems<T>(this IEnumerable<T> collection)
        {
            return collection != null && collection.Any();
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> array, Action<T> action)
        {
            foreach (var i in array)
                action(i);
            return array;
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable array, Action<T> action)
        {
            return array.Cast<T>().ForEach<T>(action);
        }

        public static IEnumerable<RT> ForEach<T, RT>(this IEnumerable<T> array, Func<T, RT> func)
        {
            var list = new List<RT>();
            foreach (var i in array)
            {
                var obj = func(i);
                if (obj != null)
                    list.Add(obj);
            }
            return list;
        }

        public static IEnumerable<T> Pipe<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
                yield return item;
            };
        }

        public static IEnumerable<R> Pipe<T, R>(this IEnumerable<T> source, Func<T, R> transform)
        {
            foreach (var item in source)
            {
                yield return transform(item);
            }
        }

        public static IEnumerable<T> Filter<T>(this IEnumerable<T> source, Predicate<T> filter)
        {
            foreach (var item in source)
            {
                if (filter(item))
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<IEnumerable<T>> SplitIntoBatches<T>(this IEnumerable<T> items, int batchSize)
        {
            var index = 0;
            var count = items.Count();

            while (index < count)
            {
                yield return items.Skip(index).Take(batchSize);
                index += batchSize;
            }
        }
    }
}
