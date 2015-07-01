using System;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils
{
// ReSharper disable once InconsistentNaming
    public interface Option<T>
    {
    }

    public sealed class None<T> : Option<T>
    {
        #region Overrides of Object

        public override string ToString()
        {
            return "None";
        }

        #endregion
    }
    public sealed class Some<T> : Option<T>
    {
        public Some(T value)
        {
            Value = value;
        }

        public T Value { get; private set; }

        #region Overrides of Object

        public override string ToString()
        {
            return string.Format("Some({0})", Value);
        }

        #endregion
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

        public static bool IsSome<T>(this Option<T> option)
        {
            return option is Some<T>;
        }
        public static bool IsNone<T>(this Option<T> option)
        {
            return option is None<T>;
        }
        public static T GetValue<T>(this Option<T> option)
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