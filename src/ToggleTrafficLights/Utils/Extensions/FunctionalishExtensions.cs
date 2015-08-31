using System;
using JetBrains.Annotations;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions
{
    public static class FunctionalishExtensions
    {
        public static TResult Pipe<TSource, TResult>([NotNull] this TSource value, [NotNull] Func<TSource, TResult> mapFunc) => mapFunc(value);
        public static void Ignore<T>(this T _)
        {
            return;
        }
    }
}