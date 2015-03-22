using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using ICities;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Serializer
{
    internal sealed class Serializer
    {
        public readonly Func<IEnumerable<byte>> SerializeData;
        public readonly Action<IEnumerable<byte>> DeserializeData;

        public Serializer(Func<IEnumerable<byte>> serializeData, Action<IEnumerable<byte>> deserializeData)
        {
            SerializeData = serializeData;
            DeserializeData = deserializeData;
        }
    }
    internal sealed class SerializerManager
    {
        public static readonly string Id = "ToggleTrafficLights";

        //uint: Version number
        //i don't think i will use the full range... but better safe than sorry....-.- (1 byte would definitely be enough!)
        public readonly Dictionary<uint,Serializer> Serializers;

        public SerializerManager()
        {
            Serializers = new Dictionary<uint, Serializer>();
        }

        public void Deserialize(ISerializableData serializableDataManager)
        {
            if (!serializableDataManager.EnumerateData().Contains(Id))
            {
                return;
            }

            var data = serializableDataManager.LoadData(Id);

            const int versionLength = 4;

            Debug.Assert(data.Length > versionLength);

            var version = BitConverter.ToUInt32(data.Take(versionLength).ToArray(), 0);
            DebugLog.Message("Deserializer version {0}", version);

            Serializer s;
            if (!Serializers.TryGetValue(version, out s))
            {
                //TODO: Fehlerbehandlung
                throw new InvalidOperationException(string.Format("No Serializer with version {0} found!", version));
            }

            //TODO: Fehlerhandlung?
            s.DeserializeData(data.Skip(versionLength));

            DebugLog.Message("Deserialized {0} bytes", data.Length-4);
        }

        public void Serialize(ISerializableData serializableDataManager)
        {
            //get newest version
            var version = Serializers.Keys.Max();
            Serialize(serializableDataManager, version);
        }

        public void Serialize(ISerializableData serializableDataManager, uint version)
        {
            Serializer s;
            if (!Serializers.TryGetValue(version, out s))
            {
                //TODO: Fehlerbehandlung
                throw new InvalidOperationException(string.Format("No Serializer with version {0} found!", version));
            }

            DebugLog.Message("Serialize version {0}", version);

            var data = s.SerializeData();

            var bytesVersion = BitConverter.GetBytes(version);
            var dataWithVersion = bytesVersion.Concat(data).ToArray();

            serializableDataManager.SaveData(Id, dataWithVersion);

            DebugLog.Message("Serialized {0} bytes", dataWithVersion.Length - 4);
        }
        
    }
}