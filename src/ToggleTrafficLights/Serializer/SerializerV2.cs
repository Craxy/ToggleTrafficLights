using System.Collections.Generic;
using System.Linq;
using Craxy.CitiesSkylines.ToggleTrafficLights.Game;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Serializer
{
  public static class SerializerV2
  {
    private static SaveGameOptions SaveGameOptions => Simulation.Instance.CurrentSaveGameOptions;
    public static IEnumerable<byte> SerializeData()
    {
      switch (SaveGameOptions.TrafficLights)
      {
        case SaveGameOptions.TrafficLightsHandling.NoTrafficLights:
        case SaveGameOptions.TrafficLightsHandling.AllTrafficLights:
          DebugLog.Info($"Serialized Options.TrafficLights: {SaveGameOptions.TrafficLights}");
          return new[] { (byte) SaveGameOptions.TrafficLights };
        case SaveGameOptions.TrafficLightsHandling.Default:
        default:
          return null;
      }
    }

    public static void DeserializeData(byte[] data)
    {
      var value = (SaveGameOptions.TrafficLightsHandling)data.Single();
      DebugLog.Info($"Deserialized Options.TrafficLights: {value}");
      SaveGameOptions.TrafficLights = value;
    }
  }
}
