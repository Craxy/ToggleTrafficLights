using System.Collections.Generic;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions
{
    public static class ArrayExtension
    {
        public static IEnumerable<T> ToSeq<T>(this Array16<T> arr)
        {
            for (int i = 0; i < arr.m_size; i++)
            {
                yield return arr.m_buffer[i];
            }
        }

        public static IEnumerable<T> ToSeq<T>(this Array32<T> arr)
        {
            for (int i = 0; i < arr.m_size; i++)
            {
                yield return arr.m_buffer[i];
            }
        }

        public static IEnumerable<T> ToSeq<T>(this Array8<T> arr)
        {
            for (int i = 0; i < arr.m_size; i++)
            {
                yield return arr.m_buffer[i];
            }
        }
    }

}