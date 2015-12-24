using System;
using System.Collections.Generic;
using System.Linq;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Do<T>(this IEnumerable<T> values, Action<T> action)
        {
            foreach (var value in values)
            {
                action(value);
                yield return value;
            }
        }

        public static bool IsEmpty<T>(this IEnumerable<T> values)
        {
            return !values.Any();
        }

        public static void RemoveAll<T>(this IList<T> list, IEnumerable<T> valuesToRemove)
        {
            foreach (var v in valuesToRemove)
            {
                list.Remove(v);
            }
        }

        public static void RemoveAll<T>(this IList<T> list, Func<T, bool> selector)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                var v = list[i];
                if (selector(v))
                {
                    list.RemoveAt(i);
                }
            }
        }
    }
}