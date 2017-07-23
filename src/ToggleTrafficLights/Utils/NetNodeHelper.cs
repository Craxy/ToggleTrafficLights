using System;
using System.Reflection;
using ColossalFramework;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils
{
  public static class Node
  {
    public enum Mode : byte
    {
      Add,
      Remove,
      Toggle,
      Reset,
    }

    public static NetNode Get(ushort nodeId) 
      => Singleton<NetManager>.instance.m_nodes.m_buffer[nodeId];

    /// <summary>
    /// NetNode is a struct and therefore it's not enough to just change a NetNode in variable but must be written into NodeManager
    /// </summary>
    public static void UpdateFlags(ushort nodeId, NetNode.Flags flags) 
      => Singleton<NetManager>.instance.m_nodes.m_buffer[nodeId].m_flags = flags;

    public static void UpdateFlags(ushort nodeId, ref NetNode node) 
      => UpdateFlags(nodeId, node.m_flags);

    public static void UpdateNode(ushort nodeId, NetNode node)
    {
      Singleton<NetManager>.instance.m_nodes.m_buffer[nodeId] = node;
    }

    #region Test for Lights

    private delegate bool RoadBaseCanHaveTrafficLight(RoadBaseAI ai, ushort nodeId, ref NetNode node);

    private static RoadBaseCanHaveTrafficLight _canHaveTrafficLightMethod = null;

    /// Calls RoadBaseAI.CanEnableTrafficLights
    public static bool IntersectionCanHaveTrafficLights(RoadBaseAI ai, ushort nodeId, ref NetNode node)
    {
      // use delegate to call RoadBaseAI.CanEnableTrafficLights
      // that's much faster than using just reflection:
      //    https://codeblog.jonskeet.uk/2008/08/09/making-reflection-fly-and-exploring-delegates/
      if (_canHaveTrafficLightMethod == null)
      {
        var mi = typeof(RoadBaseAI).GetMethod("CanEnableTrafficLights", BindingFlags.Instance | BindingFlags.NonPublic);
        // Can't use Func<RoadBaseAi,ushort,NetNode,bool> because of ref parameter
        _canHaveTrafficLightMethod =
          (RoadBaseCanHaveTrafficLight) Delegate.CreateDelegate(typeof(RoadBaseCanHaveTrafficLight), mi);
      }

      return _canHaveTrafficLightMethod(ai, nodeId, ref node);
    }

    /// <summary>
    /// Requires node to be a Junction and have RoadBaseAi.
    /// Test with <see cref="IsValidIntersection"/>
    /// </summary>
    public static bool IntersectionCanHaveTrafficLights(ushort nodeId, ref NetNode node) 
      => IntersectionCanHaveTrafficLights((RoadBaseAI) node.Info.m_netAI, nodeId, ref node);
    public static bool IsValidIntersection(ushort nodeId, ref NetNode node) 
      => TryGetRoadAiIfValidIntersection(nodeId, ref node, out var _);

    public static bool IsInsideBuildableArea(ushort nodeId, ref NetNode node)
      => !GameAreaManager.instance.PointOutOfArea(node.m_position);
    
    public static bool TryGetRoadAiIfValidIntersection(ushort nodeId, ref NetNode node, out RoadBaseAI ai)
    {
      ai = default(RoadBaseAI);
      if (node.m_flags == NetNode.Flags.None)
      {
        return false;
      }
      if ((node.m_flags & NetNode.Flags.Junction) != NetNode.Flags.Junction)
      {
        return false;
      }
      if ((node.m_flags & (NetNode.Flags.Created | NetNode.Flags.Deleted)) != NetNode.Flags.Created)
      {
        return false;
      }

      // Service=Road does not always mean RoadBaseAi: Bus stations are TransportPathAI
      // There's also a case with Service != Road but AI=RoadBaseAI: Dams (DamAI : RoadBaseAI). 
      // But Dams can't have intersections.
      if (node.Info.GetService() != ItemClass.Service.Road)
      {
        return false;
      }
      ai = node.Info.GetAI() as RoadBaseAI;
      return ai != null;
    }

    public static bool CanHaveTrafficLights(ushort nodeId, ref NetNode node) 
      => TryGetRoadAiIfValidIntersection(nodeId, ref node, out var ai)
         && IntersectionCanHaveTrafficLights(ai, nodeId, ref node);

    public static bool IsValidTrafficLightsIntersection(ushort nodeId, ref NetNode node) 
      => CanHaveTrafficLights(nodeId, ref node);

    public static bool WantTrafficLights(ushort nodeId, ref NetNode node)
    {
      var ai = node.Info.GetAI() as RoadBaseAI;
      return ai != null && ai.WantTrafficLights();
    }

    public static bool HasIntersection(ushort nodeId, ref NetNode node) 
      => (node.m_flags & NetNode.Flags.TrafficLights) == NetNode.Flags.TrafficLights;

    #endregion

    public static class TrafficLights
    {
      public static void Change(ushort nodeId, ref NetNode node, Mode mode)
      {
        if (Node.IsValidTrafficLightsIntersection(nodeId, ref node))
        {
          switch (mode)
          {
            case Mode.Add:
              node.m_flags |= NetNode.Flags.TrafficLights;
              node.m_flags |= NetNode.Flags.CustomTrafficLights;
              break;
            case Mode.Remove:
              node.m_flags &= ~NetNode.Flags.TrafficLights;
              node.m_flags |= NetNode.Flags.CustomTrafficLights;
              break;
            case Mode.Toggle:
              node.m_flags ^= NetNode.Flags.TrafficLights;
              node.m_flags |= NetNode.Flags.CustomTrafficLights;
              break;
            case Mode.Reset:
              if (Node.WantTrafficLights(nodeId, ref node))
              {
                node.m_flags |= NetNode.Flags.TrafficLights;
              }
              else
              {
                node.m_flags &= ~NetNode.Flags.TrafficLights;
              }
              node.m_flags &= ~NetNode.Flags.CustomTrafficLights;
              break;
            default:
              throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
          }
          UpdateFlags(nodeId, ref node);
        }
      }

      public static void Toggle(ushort nodeId)
      {
        var node = Node.Get(nodeId);
        Toggle(nodeId, ref node);
      }

      public static void Toggle(ushort nodeId, ref NetNode node)
      {
        Change(nodeId, ref node, Mode.Toggle);
      }

      public static void Reset(ushort nodeId, ref NetNode node)
      {
        Change(nodeId, ref node, Mode.Reset);
      }

      public static void Add(ushort nodeId, ref NetNode node)
      {
        Change(nodeId, ref node, Mode.Add);
      }

      public static void Remove(ushort nodeId, ref NetNode node)
      {
        Change(nodeId, ref node, Mode.Remove);
      }
    }

    public static class StopSign
    {
      private static bool TryGetSegment(ushort nodeId, ref NetNode node, ushort segmentIndex, out ushort segment)
      {
        segment = default(ushort);
        if (segmentIndex >= 8)
        {
          return false;
        }
        if ((node.m_flags & (NetNode.Flags.TrafficLights | NetNode.Flags.OneWayIn)) != NetNode.Flags.None)
        {
          return false;
        }
        if (!Node.IsValidTrafficLightsIntersection(nodeId, ref node))
        {
          return false;
        }

        var sgmnt = node.GetSegment(segmentIndex);
        if (sgmnt == 0)
        {
          return false;
        }
        var nm = Singleton<NetManager>.instance;
        if ((nm.m_segments.m_buffer[sgmnt].Info.m_vehicleTypes & VehicleInfo.VehicleType.Car) ==
            VehicleInfo.VehicleType.None)
        {
          return false;
        }

        segment = sgmnt;
        return true;
      }

      private static NetSegment.Flags GetYieldDirection(ushort nodeId, ushort segment)
      {
        return Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_startNode != nodeId
          ? NetSegment.Flags.YieldEnd
          : NetSegment.Flags.YieldStart;
      }

      public static void Change(ushort nodeId, ref NetNode node, ushort segmentIndex, Mode mode)
      {
        if (TryGetSegment(nodeId, ref node, segmentIndex, out var segment))
        {
          var nm = Singleton<NetManager>.instance;
          var yieldDirection = GetYieldDirection(nodeId, segment);
          switch (mode)
          {
            case Mode.Add:
              nm.m_segments.m_buffer[segment].m_flags |= yieldDirection;
              break;
            case Mode.Reset: // default is no sign
            case Mode.Remove:
              Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_flags &= ~yieldDirection;
              break;
            case Mode.Toggle:
              Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_flags ^= yieldDirection;
              break;
            default:
              throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
          }
          nm.m_segments.m_buffer[segment].UpdateLanes(segment, true);
        }
      }

      public static void Toggle(ushort nodeId, ushort segmentIndex)
      {
        var node = Node.Get(nodeId);
        Toggle(nodeId, ref node, segmentIndex);
      }
      public static void Toggle(ushort nodeId, ref NetNode node, ushort segmentIndex) 
        => Change(nodeId, ref node, segmentIndex, Mode.Toggle);
      public static void Add(ushort nodeId, ref NetNode node, ushort segmentIndex) 
        => Change(nodeId, ref node, segmentIndex, Mode.Add);
      public static void Remove(ushort nodeId, ref NetNode node, ushort segmentIndex) 
        => Change(nodeId, ref node, segmentIndex, Mode.Remove);
      public static void Reset(ushort nodeId, ref NetNode node, ushort segmentIndex) 
        => Change(nodeId, ref node, segmentIndex, Mode.Reset);
    }
  }
}
