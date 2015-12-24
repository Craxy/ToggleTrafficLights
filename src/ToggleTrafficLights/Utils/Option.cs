using System;
using JetBrains.Annotations;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils
{
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once UnusedTypeParameter
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
        public Some([NotNull] T value)
        {
            Value = value;
        }

        public T Value { get; private set; }

        #region Overrides of Object

        public override string ToString()
        {
            return $"Some({Value})";
        }

        #endregion
    }

    public static class Option
    {
        public static Some<T> Some<T>([NotNull] T value)
        {
            return new Some<T>(value);
        }

        public static None<T> None<T>()
        {
            return new None<T>();
        }

        public static bool IsSome<T>([NotNull] this Option<T> option)
        {
            return option is Some<T>;
        }
        public static bool IsNone<T>([NotNull] this Option<T> option)
        {
            return option is None<T>;
        }
        public static T GetValue<T>([NotNull] this Option<T> option)
        {
            var some = option as Some<T>;
            if (some != null)
            {
                return some.Value;
            }
            else
            {
                throw new ArgumentException("Option is not Some", nameof(option));
            }
        }

        public static Option<TResult> Bind<TInput, TResult>(Option<TInput> option, Func<TInput, Option<TResult>> func)
        {
            switch (option.IsSome())
            {
                case true:
                    return func(option.GetValue());
                default:
                    return None<TResult>();
            }
        }
    }
}