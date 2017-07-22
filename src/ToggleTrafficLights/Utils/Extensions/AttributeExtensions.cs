using System;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions
{
  public static class AttributeExtensions
  {
    public static TAttribute GetAttribute<TSource, TAttribute>()
      where TAttribute : Attribute
    {
      return (TAttribute) Attribute.GetCustomAttribute(typeof(TSource), typeof(TAttribute));
    }
  }
}
