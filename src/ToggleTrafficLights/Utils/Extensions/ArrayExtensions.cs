namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions
{
    public static class ArrayExtensions
    {
        public static T[] ImmutableAppend<T>(this T[] array, T value)
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
    }
}