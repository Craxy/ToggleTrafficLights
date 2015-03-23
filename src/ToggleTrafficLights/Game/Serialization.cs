using System;
using System.Reflection;
using Craxy.CitiesSkylines.ToggleTrafficLights.Serializer;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;
using ICities;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game
{
    public class Serialization : SerializableDataExtensionBase
    {
        private static readonly SerializerManager SerializerManager = new SerializerManager
        {
            Serializers =
            {
                //dammit....c# does not have proper tuples...
                {1, new Serializer.Serializer(SerializerV1.SerializeData, SerializerV1.DeserializeData)}
            }
        };

        public override void OnCreated(ISerializableData serializableData)
        {
            base.OnCreated(serializableData);

            DebugLog.Warning("Serialization: Created v.{0} at {1}", Assembly.GetExecutingAssembly().GetName().Version, DateTime.Now);

        }

        public override void OnReleased()
        {
            base.OnReleased();

            DebugLog.Warning("Serialization: Released v.{0}", Assembly.GetExecutingAssembly().GetName().Version);
        }

        public override void OnLoadData()
        {
            base.OnLoadData();

            DebugLog.Message("OnLoadData: Data Ids: {0}", string.Join(", ", this.serializableDataManager.EnumerateData()));

            if (managers.loading.IsGameMode())
            {
                SerializerManager.Deserialize(serializableDataManager);
            }
            else
            {
                DebugLog.Message("No Loading - in editor");
            }
        }

        public override void OnSaveData()
        {
            base.OnSaveData();

            DebugLog.Message("OnSaveData: Data Ids: {0}", string.Join(", ", this.serializableDataManager.EnumerateData()));

            //save netnode flags
            //TODO: sollte eigentlich automaitsch serialisert und deserialisiert werden (->NetManager). Warum wird das überschrieben?
            if (managers.loading.IsGameMode())
            {
                SerializerManager.Serialize(serializableDataManager);
            }
            else
            {
                DebugLog.Message("No Saving - in editor");
            }
            
        }
    }
}