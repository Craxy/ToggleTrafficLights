using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using ColossalFramework;
using ColossalFramework.Math;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using Harmony;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Tools
{
  public sealed class TrafficLightsHandlingChanger : IDisposable
  {
    #region Patch

    private Patch _patch = null;
    public bool IsPatched => _patch != null;
    public TrafficLights.ChangeMode ChangeMode => _changeMode;
    
    private void Patch(TrafficLights.ChangeMode changeMode)
    {
      if (_patch != null)
      {
        DebugLog.Warning($"{nameof(UndoPatch)}: Already patched with {ChangeMode}");
        
        if (ChangeMode != changeMode)
        {
          DebugLog.Warning($"{nameof(UndoPatch)}: Update mode to {changeMode}");
          _changeMode = changeMode;
        }
        
        return;
      }

      var src = typeof(RoadBaseAI).GetMethod("UpdateNodeFlags", BindingFlags.Public | BindingFlags.Instance);
      var prefix = typeof(TrafficLightsHandlingChanger).GetMethod(nameof(AfterUpdateNodeFlags), BindingFlags.Public | BindingFlags.Static);

      DebugLog.Warning($"{nameof(UndoPatch)}: Patch to {changeMode}");
      _changeMode = changeMode;
      _patch = Harmony.Patch.Apply(src, prefix);
    }
    private void UndoPatch()
    {
      if (_patch == null)
      {
        DebugLog.Warning($"{nameof(UndoPatch)}: Not patched");
        return;
      }
      
      DebugLog.Warning($"{nameof(UndoPatch)}: Restore patch");
      _patch.Restore();
      _patch = null;
      _changeMode = TrafficLights.ChangeMode.Reset;
    }

    private static TrafficLights.ChangeMode _changeMode = TrafficLights.ChangeMode.Reset;
    public static void AfterUpdateNodeFlags(ushort nodeID, ref NetNode data)
    {
      // must be static for Harmony
      
      if (Node.IsValidTrafficLightsIntersection(nodeID, ref data))
      {
        if (!data.m_flags.IsFlagSet(NetNode.Flags.CustomTrafficLights))
        {
          DebugLog.Warning($"{nameof(AfterUpdateNodeFlags)}: Change traffic light at junction {nodeID} to {_changeMode}");
          TrafficLights.ChangeFast(nodeID, ref data, _changeMode);
        }
      }
    }
    #endregion Patch
    
    public void Change(TrafficLights.ChangeMode changeMode)
    {
      switch (changeMode)
      {
        case TrafficLights.ChangeMode.Remove:
        case TrafficLights.ChangeMode.Add:
        case TrafficLights.ChangeMode.Reset:
          Patch(changeMode);
      break;
        default:
          throw new ArgumentOutOfRangeException(nameof(changeMode), changeMode, null);
      }
    }

    public void Reset()
    {
      if (IsPatched)
      {
        UndoPatch();
      }
    }

    public void Dispose()
    {
      Reset();
    }
  }
}
