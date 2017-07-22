using Craxy.CitiesSkylines.ToggleTrafficLights.Serializer;
using ICities;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game
{
  public class Serialization : SerializableDataExtensionBase
  {
    public override void OnLoadData()
    {
      base.OnLoadData();

      // delete previouse saved TTL data
      SerializerManager.DeleteAllData(this.serializableDataManager);
    }
  }
}