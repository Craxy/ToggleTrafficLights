using System;
using System.Reflection;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions
{
  public static class ReflectionExtensions
  {
    public static TResult GetNonPublicField<TSource, TResult>(this TSource obj, string name)
    {
      var fi = typeof(TSource).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
      if (fi == null)
      {
        throw new ArgumentException("TSource does not contain the specified field", "name");
      }

      return (TResult) fi.GetValue(obj);
    }

    public static void SetNonPublicField<TSource>(this TSource obj, string name, object value)
    {
      var fi = typeof(TSource).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
      if (fi == null)
      {
        throw new ArgumentException("TSource does not contain the specified field", "name");
      }

      fi.SetValue(obj, value);
    }

    public static void CallNonPublicMethod<TSource>(this TSource obj, string name, params object[] args)
    {
      var mi = typeof(TSource).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
      if (mi == null)
      {
        throw new ArgumentException("TSource does not contain the specified method", "name");
      }

      mi.Invoke(obj, args);
    }

    public static TResult CallNonPublicFunction<TSource, TResult>(this TSource obj, string name, params object[] args)
    {
      var mi = typeof(TSource).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
      if (mi == null)
      {
        throw new ArgumentException("TSource does not contain the specified method", "name");
      }

      return (TResult) mi.Invoke(obj, args);
    }

    public static void CallNonPublicStaticMethod<TSource>(this TSource obj, string name, params object[] args)
    {
      var mi = typeof(TSource).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic);
      if (mi == null)
      {
        throw new ArgumentException("TSource does not contain the specified method", "name");
      }

      mi.Invoke(null, args);
    }

    public static TResult CallNonPublicStaticFunction<TSource, TResult>(this TSource obj, string name,
      params object[] args)
    {
      var mi = typeof(TSource).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic);
      if (mi == null)
      {
        throw new ArgumentException("TSource does not contain the specified method", "name");
      }

      return (TResult) mi.Invoke(null, args);
    }

    public static TResult GetNonPublicStaticField<TSource, TResult>(string name)
    {
      var fi = typeof(TSource).GetField(name, BindingFlags.Static | BindingFlags.NonPublic);
      if (fi == null)
      {
        throw new ArgumentException("TSource does not contain the specified method", nameof(name));
      }

      return (TResult) fi.GetValue(null);
    }
  }
}
