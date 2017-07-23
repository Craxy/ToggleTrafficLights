using System.Text.RegularExpressions;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions
{
  public static class ColorExtensions
  {
    private static string ToHex(this float value)
    {
      System.Diagnostics.Debug.Assert(value >= 0.0f && value <= 1.0f);

      return value.ToByte().ToHex();
    }

    private static byte ToByte(this float value)
    {
      return (byte) (value * 255.0f);
    }

    private static string ToHex(this byte value)
    {
      return value.ToString("X2");
    }

    private static byte ToByte(this string hexValue)
    {
      return byte.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
    }

    private static float ToFloat(this byte value)
    {
      return value / 255.0f;
    }

    private static float FromHex(this string value)
    {
      return value.ToByte().ToFloat();
    }

    public static string ToHex(this Color color, bool leadingHashtag)
    {
      return string.Format("{4}{0:X2}{1:X2}{2:X2}{3:X2}", color.r.ToByte(), color.g.ToByte(), color.b.ToByte(),
        color.a.ToByte(), leadingHashtag ? "#" : string.Empty);
    }


//        public static Color FromHex(string color)
//        {
//            // starts with or without #
//            // 6 (rgb), 7 (#rgb), 8 (rgba) or 9 (#rgba) length
//            if(color.Length < 6 || color.Length  )
//        }

    private static readonly Regex HexColorRegex =
      new Regex(
        "^(?<hashtag>#)?(?<red>[0-9A-Fa-f]{2})(?<green>[0-9A-Fa-f]{2})(?<blue>[0-9A-Fa-f]{2})(?<alpha>[0-9A-Fa-f]{2})?$",
        RegexOptions.Compiled);

    public static bool TryParseHexColor(this string value, out Color color, out string failureReason)
    {
      // starts with or without #
      // 6 (rgb), 7 (#rgb), 8 (rgba) or 9 (#rgba) length
      if (value.Length < 6)
      {
        color = new Color();
        failureReason = "Value is too short for a valid hex color ([#]RRGGBB[AA]).";
        return false;
      }
      if (value.Length > 9)
      {
        color = new Color();
        failureReason = "Value is too long for a valid hex color ([#]RRGGBB[AA]).";
        return false;
      }

      var match = HexColorRegex.Match(value);
      if (match.Success)
      {
        var r = match.Groups["red"].Value.FromHex();
        var g = match.Groups["green"].Value.FromHex();
        var b = match.Groups["blue"].Value.FromHex();
        var a = match.Groups["alpha"].Success ? match.Groups["alpha"].Value.FromHex() : 1.0f;

        color = new Color(r, g, b, a);
        failureReason = string.Empty;
        return false;
      }
      else
      {
        color = new Color();
        failureReason = "Value is not a valid hex color ([#]RRGGBB[AA]).";
        return false;
      }
    }
  }
}
