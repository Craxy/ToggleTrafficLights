using System;
using System.Diagnostics;
using ColossalFramework;
using Craxy.CitiesSkylines.ToggleTrafficLights.Serializer;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;
using ICities;
using UnityEngine;
using Debug = System.Diagnostics.Debug;
using Level = Craxy.CitiesSkylines.ToggleTrafficLights.Game.Behaviours.Level;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game
{
  //Order of Events in Unity: https://docs.unity3d.com/Manual/ExecutionOrder.html

  //C:S does not work with a class implementing two Interfaces at once:
  // it creates for each Interface one instance
  // therefore ILoadingExtension AND IThreadingExtension can not live together in the same instance

  public sealed class Simulation
  {
    private static Simulation _instance = null;

    public static Simulation Instance
    {
      get
      {
        if (_instance == null)
        {
          _instance = new Simulation();
        }
        return _instance;
      }
    }
    
    
    public IManagers Managers => SimulationManager.instance.m_ManagersWrapper;
    public bool IsGameMode => Managers.IsGameMode();
    public Options Options { get; } = new Options();


    public enum Step
    {
      LoadData,
      LevelLoaded,
      LevelUnloaded,
      SaveData,
    }
    public void Process(Step step)
    {
      if (!IsGameMode)
      {
        return;
      }
      
      //Problem: 
      //  OnLevelLoaded happens after OnLoadData
      //  OnLevelUnloading happens before OnSaveData
      // but: OnLoadData is called even for new maps
      
      // Order:
      // LoadData (always)
      // LevelLoaded (always)
      // 
      // LevelUnloaded (always)
      // SaveData (when saving)
      
      DebugLog.Info($"{nameof(Process)}({step}): CurrentLevel={_currentLevel}, CurrentLevelOptions={_currentSaveGameOptions}");
      
      // in order
      switch (step)
      {
        case Step.LoadData:        //(always)
          Debug.Assert(_currentLevel == null);
          InitializeLevel();
          Debug.Assert(_currentLevel == null);
          Debug.Assert(_currentSaveGameOptions != null);
          LoadLevel(Managers.serializableData);
          break;
        case Step.LevelLoaded:     // (always)
          Debug.Assert(_currentLevel == null);
          Debug.Assert(_currentSaveGameOptions != null);
          StartLevel();
          Debug.Assert(_currentLevel != null);
          break;
        case Step.LevelUnloaded:   // (always)
          Debug.Assert(_currentLevel != null);
          Debug.Assert(_currentSaveGameOptions != null);
          EndLevel();
          Debug.Assert(_currentLevel == null);
          Debug.Assert(_currentSaveGameOptions != null);
          break;
        case Step.SaveData:        // (when saving)
          Debug.Assert(_currentSaveGameOptions != null);
          SaveLevel(Managers.serializableData);
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(step), step, null);
      }
    }
    
    #region Level
    private Level _currentLevel = null;
    private SaveGameOptions _currentSaveGameOptions = null;
    public SaveGameOptions CurrentSaveGameOptions => _currentSaveGameOptions;

    private void InitializeLevel()
    {
      _currentSaveGameOptions = new SaveGameOptions();
    }
    private void StartLevel()
    {
      var go = new GameObject("TTLLevel");
      _currentLevel = go.AddComponent<Level>();
      Debug.Assert(_currentLevel != null);
      _currentLevel.Options = Options;
      _currentLevel.GameOptions = _currentSaveGameOptions;
    }

    private void EndLevel()
    {
      if (_currentLevel != null)
      {
        _currentLevel.enabled = false;
        _currentLevel.Options = null;
        _currentLevel.GameOptions = null;
        GameObject.Destroy(_currentLevel.gameObject);
        _currentLevel = null;
      }
    }
    #endregion Level
    
    #region Save/Load
    private static readonly SerializerManager SerializerManager = new SerializerManager
      {
        Serializers =
        {
          //dammit....c# does not have proper tuples...
          {1, new Serializer.Serializer(SerializerV1.SerializeData, SerializerV1.DeserializeData, Serializer.Serializer.Delete)},
          {2, new Serializer.Serializer(SerializerV2.SerializeData, SerializerV2.DeserializeData, Serializer.Serializer.DontDelete)},
        }
      };

    private void LoadLevel(ISerializableData serializeData)
    {
      // Is called even for new maps
      // Is called before LoadGame
      // serializeData == Managers.serializableData
      
      try
      {
        SerializerManager.Deserialize(serializeData);
      }
      catch (Exception e)
      {
        Log.Error($"Error while loading data: {e.Message}");
      }
    }
    private void SaveLevel(ISerializableData serializeData)
    {
      try
      {
        SerializerManager.Serialize(serializeData);
      }
      catch (Exception e)
      {
        Log.Error($"Error while saving data: {e.Message}");
      }
    }
    #endregion Save/Load
    
    #region Implementation of IThreadingExtension
    [Conditional("DEBUG")]
    public void OnUpdate(float realTimeDelta, float simulationTimeDelta)
    {
      if (IsGameMode)
      {
          DebugShortcuts();
      }
    }
    #endregion IThreadingExtension
    
    #region Debug
    [Conditional("DEBUG")]
    private void DebugShortcuts()
    {
      if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.I))
      {
        var msg = $"Current Tool: {ToolsModifierControl.toolController.CurrentTool?.GetType().Name}";
        msg += $"\nCurrent State: {_currentLevel?.Current}";
        var im = Singleton<InfoManager>.instance;
        msg += $"\nInfoManager: CurrentMode={im.CurrentMode}; CurrentSubMode={im.CurrentSubMode}; NextMode={im.NextMode}; NextSubMode={im.NextSubMode}";
        msg += $"\nSaveGameOptions: {_currentSaveGameOptions}";
        
        DebugLog.Info(msg);
      }
    }
    #endregion Debug
  }
}
