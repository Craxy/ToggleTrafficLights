using System.Text;
using JetBrains.Annotations;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions
{
    public static class Extensions
    {
        // ReSharper disable InconsistentNaming

        [StringFormatMethod("format")]
        public static StringBuilder printfn(this StringBuilder sb, string format, params object[] args)
        {
            return sb.AppendFormat(format, args).AppendLine();
        }
        [StringFormatMethod("format")]
        public static StringBuilder printf(this StringBuilder sb, string format, params object[] args)
        {
            return sb.AppendFormat(format, args);
        }
        public static StringBuilder printn(this StringBuilder sb, string value)
        {
            return sb.AppendLine(value);
        }

        public static StringBuilder print(this StringBuilder sb, string value)
        {
            return sb.Append(value);
        }

        // ReSharper restore InconsistentNaming

    }

}