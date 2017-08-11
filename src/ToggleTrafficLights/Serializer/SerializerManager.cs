using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using ICities;
using JetBrains.Annotations;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Serializer
{
  internal sealed class Serializer
  {
    public delegate bool ShouldDelete();
    [CanBeNull]
    public delegate IEnumerable<byte> Serialize();
    /// <summary>
    /// <c>null</c> indicates: no data gets serialized == delete existing data
    /// </summary>
    public delegate void Deserialize([NotNull]byte[] data);

    public static readonly ShouldDelete Delete = () => true;
    public static readonly ShouldDelete DontDelete = () => false;
    
    public readonly ShouldDelete ShouldDeleteData;
    public readonly Serialize SerializeData;
    public readonly Deserialize DeserializeData;

    public Serializer(Serialize serializeData, Deserialize deserializeData, ShouldDelete shouldDeleteData)
    {
      SerializeData = serializeData;
      DeserializeData = deserializeData;
      ShouldDeleteData = shouldDeleteData;
    }
    public Serializer(Serialize serializeData, Deserialize deserializeData)
      : this(serializeData, deserializeData, DontDelete)
    {}
  }

  internal sealed class SerializerManager
  {
    public static readonly string Id = "ToggleTrafficLights";

    //uint: Version number
    //i don't think i will use the full range... but better safe than sorry....-.- (1 byte would definitely be enough!)
    public readonly Dictionary<uint, Serializer> Serializers;

    public SerializerManager()
    {
      Serializers = new Dictionary<uint, Serializer>();
    }

    private static bool Contains(string id, ISerializableData serializableDataManager)
      => serializableDataManager.EnumerateData().Contains(id);

    public static void DeleteAllData(ISerializableData serializableDataManager)
    {
      // test for contains is unnecessary
      if (Contains(Id, serializableDataManager))
      {
        Log.Info($"Removing data with ID {Id}");
        serializableDataManager.EraseData(Id);
      }
    }

    public void Deserialize(ISerializableData serializableDataManager)
    {
      if (!Contains(Id, serializableDataManager))
      {
        DebugLog.Message("Save does not contain data with id {0}", Id);
        return;
      }

      var data = serializableDataManager.LoadData(Id);
      DebugLog.Message("Read {0} bytes for Id {1}", data.Length, Id);
      
      const int versionLength = 4;
      Debug.Assert(data.Length > versionLength);
      if (data.Length < versionLength)
      {
        throw new InvalidOperationException($"Data with id {Id} should be at least {versionLength} bytes long, but is only {data.Length}B");
      }
      
      var version = BitConverter.ToUInt32(data.Take(versionLength).ToArray(), 0);
      DebugLog.Message("Deserializer version {0}", version);
      
      Serializer serializer;
      if (!Serializers.TryGetValue(version, out serializer))
      {
        throw new InvalidOperationException($"No Serializer with version {version} found!");
      }

      if (serializer.ShouldDeleteData())
      {
        Log.Info($"Deleting saved data because serializer version {version} requested deletion.");
        serializableDataManager.EraseData(Id);
      }
      else
      {
        var rData = data.Skip(versionLength).ToArray();
        serializer.DeserializeData(rData);
        DebugLog.Message("Deserialized {0} bytes", rData.Length);
      }
    }

    public void Serialize(ISerializableData serializableDataManager)
    {
      //get newest version
      var version = Serializers.Keys.Max();
      Serialize(serializableDataManager, version);
    }

    public void Serialize(ISerializableData serializableDataManager, uint version)
    {
      Serializer serializer;
      if (!Serializers.TryGetValue(version, out serializer))
      {
        throw new InvalidOperationException($"No Serializer with version {version} found!");
      }
      
      DebugLog.Message("Serialize version {0}", version);

      if (serializer.ShouldDeleteData())
      {
        DebugLog.Info($"Deleting saved data because serializer version {version} requested deletion.");
        serializableDataManager.EraseData(Id);
      }
      else
      {
        var data = serializer.SerializeData();
        if (data == null)
        {
          DebugLog.Info($"No data to serialize (-> delete data if present)");
          serializableDataManager.EraseData(Id);
        }
        else
        {
          var bytesVersion = BitConverter.GetBytes(version);
          var dataWithVersion = bytesVersion.Concat(data).ToArray();

          serializableDataManager.SaveData(Id, dataWithVersion);

          DebugLog.Message("Serialized {0} bytes", dataWithVersion.Length - 4);
        }
      }
    }
  }
}
