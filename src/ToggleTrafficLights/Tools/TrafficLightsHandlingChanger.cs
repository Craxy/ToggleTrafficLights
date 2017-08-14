using System;
using System.Linq;
using System.Reflection;
using ColossalFramework;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using Harmony;
using UnityEngine.Networking.Types;

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
        DebugLog.Info($"{nameof(Patch)}: Already patched with {ChangeMode}");
        
        if (ChangeMode != changeMode)
        {
          DebugLog.Info($"{nameof(Patch)}: Update mode to {changeMode}");
          _changeMode = changeMode;
        }
        
        return;
      }

     var src = typeof(RoadBaseAI).GetMethod(nameof(RoadBaseAI.UpdateNode), BindingFlags.Public | BindingFlags.Instance);
     var prefix = typeof(TrafficLightsHandlingChanger).GetMethod(nameof(AfterRoadBaseAiUpdateNode), BindingFlags.Public | BindingFlags.Static);

      DebugLog.Info($"{nameof(Patch)}: Patch to {changeMode}; src={src}; prefix={prefix}");
      _changeMode = changeMode;
      _patch = Harmony.Patch.Apply(src, prefix);
    }
    private void UndoPatch()
    {
      if (_patch == null)
      {
        DebugLog.Info($"{nameof(UndoPatch)}: Not patched");
        return;
      }
      
      DebugLog.Info($"{nameof(UndoPatch)}: Restore patch");
      _patch.Restore();
      _patch = null;
      _changeMode = TrafficLights.ChangeMode.Reset;
    }

    private static TrafficLights.ChangeMode _changeMode = TrafficLights.ChangeMode.Reset;
    public static void AfterRoadBaseAiUpdateNode(ushort nodeID, ref NetNode data)
    {
      if (Node.IsValidTrafficLightsIntersection(nodeID, ref data))
      {
        //todo: is this called at loading of map? -> might change existing behaviour
        if (!data.m_flags.IsFlagSet(NetNode.Flags.CustomTrafficLights))
        {
          DebugLog.Info($"{nameof(AfterRoadBaseAiUpdateNode)}: Change traffic light at junction {nodeID} to {_changeMode}");
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
