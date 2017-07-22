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

    public static StringBuilder Clear(this StringBuilder sb)
    {
      sb.Length = 0;
      return sb;
    }

    public static StringBuilder AppendNameValueLine(this StringBuilder sb, string name, string content)
    {
      return sb.Append(name).Append(": ").AppendLine(content);
    }

    public static StringBuilder AppendNameValueLine(this StringBuilder sb, string name, int content)
    {
      return sb.Append(name).Append(": ").Append(content).AppendLine();
    }

    public static StringBuilder AppendNameValueLine(this StringBuilder sb, string name, bool content)
    {
      return sb.Append(name).Append(": ").Append(content).AppendLine();
    }

    public static StringBuilder AppendNameValueLine(this StringBuilder sb, string name, object content)
    {
      return sb.Append(name).Append(": ").Append(content).AppendLine();
    }
  }
}