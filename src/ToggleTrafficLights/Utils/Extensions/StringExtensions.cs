using JetBrains.Annotations;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions
{
    public static class StringExtensions
    {
        [StringFormatMethod("format")]
        public static string Format([NotNull] this string format, object arg0)
        {
            return string.Format(format, arg0);
        }
        [StringFormatMethod("format")]
        public static string Format([NotNull] this string format, object arg0, object arg1)
        {
            return string.Format(format, arg0, arg1);
        }
        [StringFormatMethod("format")]
        public static string Format([NotNull] this string format, object arg0, object arg1, object arg2)
        {
            return string.Format(format, arg0, arg1, arg2);
        }
        [StringFormatMethod("format")]
        public static string Format([NotNull] this string format, [NotNull] params object[] args)
        {
            return string.Format(format, args);
        }
    }
}