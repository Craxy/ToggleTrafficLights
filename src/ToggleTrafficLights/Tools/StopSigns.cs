using System;
using ColossalFramework;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Tools
{
  public static class StopSigns
  {
    // add is completely useless
    // remove and reset are same -> = remove
    public enum ChangeMode : byte
    {
      Remove, 
      Add, 
      Reset,
      Toggle
    }

    public static void ChangeAllFast(ChangeMode mode)
    {
      var netManager = Singleton<NetManager>.instance;
      var nodes = netManager.m_nodes;
      for (ushort i = 0; i < nodes.m_size; i++)
      {
        var node = nodes.m_buffer[i];
        ChangeFast(i, ref node, mode);
        nodes.m_buffer[i] = node;
      }
    }

    public static void ChangeFast(ushort nodeId, ref NetNode node, ChangeMode mode)
    {
      if (!Node.IsValidIntersection(nodeId, ref node))
      {
        return;
      }
      if (!Node.IsInsideBuildableArea(nodeId, ref node))
      {
        return;
      }
      var ai = node.Info.m_netAI as RoadBaseAI;
      if (ai == null)
      {
        return;
      }

      var nm = Singleton<NetManager>.instance;
      for(var index = 0; index < 8; index++)
      {
        var segment = node.GetSegment(index);
        if(segment != 0)
        {
          var info = nm.m_segments.m_buffer[segment].Info;
          if ((info.m_vehicleTypes & VehicleInfo.VehicleType.Car) != VehicleInfo.VehicleType.None)
          {
            var flag3 = nm.m_segments.m_buffer[segment].m_startNode == nodeId;
            var flag4 = (nm.m_segments.m_buffer[segment].m_flags & NetSegment.Flags.Invert) != NetSegment.Flags.None;
            if ((flag3 != flag4 ? (info.m_hasBackwardVehicleLanes ? 1 : 0) : (info.m_hasForwardVehicleLanes ? 1 : 0)) != 0)
            {
              var flags = !flag3 ? NetSegment.Flags.YieldEnd : NetSegment.Flags.YieldStart;
              var sign = (nm.m_segments.m_buffer[segment].m_flags & flags) != NetSegment.Flags.None
                ? NotificationEvent.Type.YieldOn
                : NotificationEvent.Type.YieldOff;
              

              var dir = nm.m_segments.m_buffer[segment].m_startNode != nodeId
                  ? NetSegment.Flags.YieldEnd
                  : NetSegment.Flags.YieldStart;

              switch(mode)
              {
                case ChangeMode.Add:
                  nm.m_segments.m_buffer[segment].m_flags |= dir;
                  break;
                case ChangeMode.Reset:
                case ChangeMode.Remove:
                  nm.m_segments.m_buffer[segment].m_flags &= ~dir;
                  break;
                case ChangeMode.Toggle:
                  nm.m_segments.m_buffer[segment].m_flags ^= dir;
                  break;
                default:
                  throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
              }
            }
          }
        }
      }
    }
  }
}