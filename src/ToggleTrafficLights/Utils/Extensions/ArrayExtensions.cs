using JetBrains.Annotations;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions
{
    public static class ArrayExtensions
    {
        public static T[] ImmutableCopy<T>([NotNull] this T[] array)
        {
            if (array.Length == 0)
            {
                return new T[0];
            }

            var newArray = new T[array.Length];
            array.CopyTo(newArray, 0);

            return newArray;
        }

        public static T[] ImmutableAppend<T>([NotNull] this T[] array, [NotNull] T value)
        {
            if (array.Length == 0)
            {
                return new[] {value};
            }

            var newArray = new T[array.Length + 1];
            array.CopyTo(newArray, 0);
            newArray[array.Length] = value;

            return newArray;
        }

        public static T[] ImmutablePrepend<T>([NotNull] this T[] array, [NotNull] T value)
        {
            if (array.Length == 0)
            {
                return new[] { value };
            }

            var newArray = new T[array.Length + 1];
            newArray[0] = value;
            array.CopyTo(newArray, 1);

            return newArray;
        }

        public static T[] ImmutableConcat<T>([NotNull] this T[] headArray, [NotNull] T[] tailArray)
        {
            if (headArray.Length == 0)
            {
                
                return tailArray.ImmutableCopy();
            }
            if (tailArray.Length == 0)
            {
                return headArray.ImmutableCopy();
            }

            var newArray = new T[headArray.Length + tailArray.Length];
            headArray.CopyTo(newArray, 0);
            tailArray.CopyTo(newArray, headArray.Length);

            return newArray;
        }
    }
}