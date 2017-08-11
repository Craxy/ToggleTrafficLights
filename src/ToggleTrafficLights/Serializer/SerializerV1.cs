using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Serializer
{
  public static class SerializerV1
  {
    public static IEnumerable<byte> SerializeData()
    {
      return Singleton<NetManager>.instance.m_nodes.m_buffer
        .Select(n => CitiesHelper.HasTrafficLights(n.m_flags))
        .Select(Convert.ToByte);
    }

    public static void DeserializeData(byte[] data)
    {
      var nm = Singleton<NetManager>.instance;
      var nodes = nm.m_nodes;

      int i = 0;
      foreach (var hasLights in data.Select(Convert.ToBoolean))
      {
        var node = nodes.m_buffer[i];
        if (hasLights)
        {
          //this if is utterly unnecessary...
          if (!CitiesHelper.HasTrafficLights(node.m_flags))
          {
            node.m_flags |= NetNode.Flags.TrafficLights;
          }
        }
        else //(!hasLights)
        {
          //"
          if (CitiesHelper.HasTrafficLights(node.m_flags))
          {
            node.m_flags &= ~NetNode.Flags.TrafficLights;
          }
        }

        nodes.m_buffer[i] = node;

        i++;
      }
    }
  }
}
