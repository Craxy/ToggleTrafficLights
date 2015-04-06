using System;
using System.Reflection;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions
{
    public static class ReflectionExtensions
    {
        public static TResult GetNonPublicField<TSource, TResult>(this TSource obj, string name)
        {
            var fi = typeof(TSource).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            if (fi == null)
            {
                throw new ArgumentException("TSource does not contain the specified field", "name");
            }

            return (TResult) fi.GetValue(obj);
        }
    }
}