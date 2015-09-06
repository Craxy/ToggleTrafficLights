using System;
using JetBrains.Annotations;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions
{
    public static class FunctionalishExtensions
    {
        public static TResult Pipe<TSource, TResult>([NotNull] this TSource value, [NotNull] Func<TSource, TResult> mapFunc) => mapFunc(value);
        public static TResult _<TSource, TResult>([NotNull] this TSource value, [NotNull] Func<TSource, TResult> mapFunc) => Pipe(value, mapFunc);
        public static void Ignore<T>(this T _)
        {
            return;
        }

        #region partial application
        #region left
        public static Func<TResult> PartL<TValue, TResult>(this Func<TValue, TResult> func, TValue value)
        {
            return () => func(value);
        }
        public static Func<TValue2, TResult> PartL<TValue1, TValue2, TResult>(this Func<TValue1, TValue2, TResult> func, TValue1 value)
        {
            return v2 => func(value, v2);
        }
        public static Func<TValue2, TValue3, TResult> PartL<TValue1, TValue2, TValue3, TResult>(this Func<TValue1, TValue2, TValue3, TResult> func, TValue1 value)
        {
            return (v2,v3) => func(value, v2, v3);
        }
        public static Func<TValue2, TValue3, TValue4, TResult> PartL<TValue1, TValue2, TValue3, TValue4, TResult>(this Func<TValue1, TValue2, TValue3, TValue4, TResult> func, TValue1 value)
        {
            return (v2, v3, v4) => func(value, v2, v3, v4);
        }
        // no predefined Func class for this...
//        public static Func<TValue2, TValue3, TValue4, TValue5, TResult> PartL<TValue1, TValue2, TValue3, TValue4, TValue5, TResult>(this Func<TValue1, TValue2, TValue3, TValue4, TValue5, TResult> func, TValue1 value)
//        {
//            return (v2, v3, v4, v5) => func(value, v2, v3, v4, v5);
//        }
        #endregion
        #region right
        public static Func<TResult> PartR<TValue, TResult>(this Func<TValue, TResult> func, TValue value)
        {
            return () => func(value);
        }
        public static Func<TValue1, TResult> PartR<TValue1, TValue2, TResult>(this Func<TValue1, TValue2, TResult> func, TValue2 value)
        {
            return v1 => func(v1, value);
        }
        public static Func<TValue1, TValue2, TResult> PartR<TValue1, TValue2, TValue3, TResult>(this Func<TValue1, TValue2, TValue3, TResult> func, TValue3 value)
        {
            return (v1, v2) => func(v1, v2, value);
        }
        public static Func<TValue1, TValue2, TValue3, TResult> PartR<TValue1, TValue2, TValue3, TValue4, TResult>(this Func<TValue1, TValue2, TValue3, TValue4, TResult> func, TValue4 value)
        {
            return (v1, v2, v3) => func(v1, v2, v3, value);
        }
        // no predefined Func class for this...
//        public static Func<TValue1, TValue2, TValue3, TValue4, TResult> PartR<TValue1, TValue2, TValue3, TValue4, TValue5, TResult>(this Func<TValue1, TValue2, TValue3, TValue4, TValue5, TResult> func, TValue5 value)
//        {
//            return (v1, v2, v3, v4) => func(v1, v2, v3, v4, value);
//        }
        #endregion
        #endregion
    }
}