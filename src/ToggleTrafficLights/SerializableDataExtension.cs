using System;
using System.Linq;
using System.Reflection;
using System.Text;
using ColossalFramework;
using ColossalFramework.Steamworks;
using Craxy.CitiesSkylines.ToggleTrafficLights.Serializer;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using ICities;

namespace Craxy.CitiesSkylines.ToggleTrafficLights
{
    public sealed class SerializableDataExtension : SerializableDataExtensionBase
    {
        private static readonly SerializerManager SerializerManager = new SerializerManager
        {
            Serializers =
            {
                //dammit....c# does not have a proper tuple...
                {1, new Serializer.Serializer(SerializerV1.SerializeData, SerializerV1.DeserializeData)}
            }
        };


        public override void OnCreated(ISerializableData serializableData)
        {
            base.OnCreated(serializableData);

            DebugLog.Warning("Serializable: Created v.{0} at {1}", Assembly.GetExecutingAssembly().GetName().Version, DateTime.Now);

        }

        public override void OnReleased()
        {
            base.OnReleased();

            DebugLog.Warning("Serializable: Released v.{0}", Assembly.GetExecutingAssembly().GetName().Version);
        }

        public override void OnLoadData()
        {
            base.OnLoadData();

            DebugLog.Message("OnLoadData: Data Ids: {0}", string.Join(", ", this.serializableDataManager.EnumerateData()));

            //TODO: erase data after usage?

            SerializerManager.Deserialize(serializableDataManager);
        }

        public override void OnSaveData()
        {
            base.OnSaveData();

            DebugLog.Message("OnSaveData: Data Ids: {0}", string.Join(", ", this.serializableDataManager.EnumerateData()));

            //save netnode flags
            //TODO: sollte eigentlich serialisert und deserialisiert werden (->NetManager). Warum wird das überschrieben?

            SerializerManager.Serialize(serializableDataManager);
        }
    }
}