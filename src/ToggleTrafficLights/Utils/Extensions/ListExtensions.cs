using System.Collections.Generic;
using JetBrains.Annotations;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions
{
    public static class ListExtensions
    {
        public static T AddTo<T>([NotNull] this T element, [NotNull] IList<T> list)
        {
            list.Add(element);

            return element;
        }  
    }
}