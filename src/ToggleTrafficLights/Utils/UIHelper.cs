using System;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils
{
  public sealed class Layout : IDisposable
  {
    //abusing IDisposable......
    private static readonly Layout _horizontal = new Layout(() => GUILayout.BeginHorizontal(), GUILayout.EndHorizontal);

    private static readonly Layout _vertical = new Layout(() => GUILayout.BeginVertical(), GUILayout.EndVertical);

    public static Layout Horizontal()
    {
      return _horizontal.Begin();
    }

    public static Layout Vertical()
    {
      return _vertical.Begin();
    }

    private readonly Action _begin;
    private readonly Action _end;

    private Layout(Action begin, Action end)
    {
      _begin = begin;
      _end = end;
    }

    private Layout Begin()
    {
      _begin();
      return this;
    }

    private Layout End()
    {
      _end();
      return this;
    }

    #region Implementation of IDisposable

    public void Dispose()
    {
      End();
    }

    #endregion
  }
}
