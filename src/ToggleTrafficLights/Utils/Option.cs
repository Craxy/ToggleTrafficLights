using System;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils
{
    public interface IOption<T>
    {
    }

    public sealed class None<T> : IOption<T>
    {
        
    }
    public sealed class Some<T> : IOption<T>
    {
        public Some(T value)
        {
            Value = value;
        }

        public T Value { get; private set; }
    }

    public static class Option
    {
        public static Some<T> Some<T>(T value)
        {
            return new Some<T>(value);
        }

        public static None<T> None<T>()
        {
            return new None<T>();
        }

        public static bool IsSome<T>(this IOption<T> option)
        {
            return option is Some<T>;
        }
        public static bool IsNone<T>(this IOption<T> option)
        {
            return option is None<T>;
        }
        public static T GetValue<T>(this IOption<T> option)
        {
            var some = option as Some<T>;
            if (some != null)
            {
                return some.Value;
            }
            else
            {
                throw new ArgumentException("Option is not Some", "option");
            }
        }
        
    }
}