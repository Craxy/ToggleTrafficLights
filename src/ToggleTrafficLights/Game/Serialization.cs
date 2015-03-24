using System;
using System.Diagnostics;
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

            DebugLog.Message("Serialization: Created v.{0} at {1}", Assembly.GetExecutingAssembly().GetName().Version, DateTime.Now);

#if !SERIALIZE
            DebugLog.Message("Serialization is disabled");
#endif

        }

        public override void OnReleased()
        {
            base.OnReleased();

            DebugLog.Message("Serialization: Released v.{0}", Assembly.GetExecutingAssembly().GetName().Version);
        }

        public override void OnLoadData()
        {
            base.OnLoadData();

//            DebugLog.Message("OnLoadData: Data Ids: {0}", string.Join(", ", this.serializableDataManager.EnumerateData()));
            DebugLog.Message("OnLoadData");

#if NOSERIALIZATION
            DebugLog.Message("Serialization disabled");
#else
            OnLoadDataImpl();
#endif
        }
        private void OnLoadDataImpl()
        {
            if (managers.loading.IsGameMode())
            {
                try
                {
                    SerializerManager.Deserialize(serializableDataManager);
                }
                catch (Exception e)
                {
                    var txt = "Error while loading data: {0}\n" +
                              "Please report the problem to the project page on GitHub https://github.com/Craxy/ToggleTrafficLights/issues \n" +
                              "       or the mod's steam workshop page http://steamcommunity.com/sharedfiles/filedetails/?id=411833858 \n" +
                              @"Please include your output_log.txt ([Steam]\SteamApps\common\Cities_Skylines\Cities_Data\output_log.txt)" + "\n" +
                              "       and probably your used savegame\n" +
                              "Mod version is {1}";

                    var msg = string.Format(txt, e.Message, Assembly.GetExecutingAssembly().GetName().Version);

                    FileLog.Error("{0}\n{1}", msg, e.ToString());
                    Log.Error(msg);
                }
            }
            else
            {
                DebugLog.Message("No Loading - in editor");
            }
        }

        public override void OnSaveData()
        {
            base.OnSaveData();

//            DebugLog.Message("OnSaveData: Data Ids: {0}", string.Join(", ", this.serializableDataManager.EnumerateData()));
            DebugLog.Message("OnSaveData");

#if NOSERIALIZATION
            DebugLog.Message("Serialization disabled");
#else
            OnSaveDataImpl();
#endif
        }
        private void OnSaveDataImpl()
        {
            //save netnode flags
            //TODO: sollte eigentlich automaitsch serialisert und deserialisiert werden (->NetManager). Warum wird das überschrieben?
            if (managers.loading.IsGameMode())
            {
                try
                {
                    SerializerManager.Serialize(serializableDataManager);
                }
                catch (Exception e)
                {
                    var txt = "Error while saving data: {0}\n" +
                              "Please report the problem to the project page on GitHub https://github.com/Craxy/ToggleTrafficLights/issues \n" +
                              "       or the mod's steam workshop page http://steamcommunity.com/sharedfiles/filedetails/?id=411833858 \n" +
                              @"Please include your output_log.txt ([Steam]\SteamApps\common\Cities_Skylines\Cities_Data\output_log.txt)" + "\n" +
                              "       and probably your used savegame\n" +
                              "Mod version is {1}";

                    var msg = string.Format(txt, e.Message, Assembly.GetExecutingAssembly().GetName().Version);

                    FileLog.Error("{0}\n{1}", msg, e.ToString());
                    Log.Error(msg);
                }
            }
            else
            {
                DebugLog.Message("No Saving - in editor");
            }
        }
    }
}