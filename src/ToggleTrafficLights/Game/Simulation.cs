using System;
using System.Diagnostics;
using System.Reflection;
using ColossalFramework;
using Craxy.CitiesSkylines.ToggleTrafficLights.Game.Behaviours;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;
using ICities;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game
{
  //Order of Events in Unity: https://docs.unity3d.com/Manual/ExecutionOrder.html

  //C:S does not work with a class implementing two Interfaces at once:
  // it creates for each Interface one instance
  // therefore ILoadingExtension AND IThreadingExtension can not live together in the same instance

  public sealed class Simulation
  {
    public static readonly Simulation Instance = new Simulation();

    public ILoading LoadingManager { get; private set; }
    public IManagers Managers => LoadingManager.managers;
    public Options Options { get; } = new Options();
    public MainMachine MainMachine { get; private set; }

    [Conditional("DEBUG")]
    private void DebugShortcuts()
    {
      if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.P))
      {
      }
      else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.I))
      {
        var msg = $"Current Tool: {ToolsModifierControl.toolController.CurrentTool?.GetType().Name}";
        msg += $"\nCurrent State: {MainMachine.Current}";
        var im = Singleton<InfoManager>.instance;
        msg +=
          $"\nInfoManager: CurrentMode={im.CurrentMode}; CurrentSubMode={im.CurrentSubMode}; NextMode={im.NextMode}; NextSubMode={im.NextSubMode}";

        DebugLog.Info(msg);
      }
      else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.O))
      {
      }
    }

    #region Implementation of IThreadingExtension

    public void OnUpdate(float realTimeDelta, float simulationTimeDelta)
    {
      DebugShortcuts();
    }

    #endregion IThreadingExtension

    #region Implementation of LoadingExtensionBase

    public void OnCreated(ILoading loading)
    {
      LoadingManager = loading;

      DebugLog.Message("Created v.{0} at {1}", Assembly.GetExecutingAssembly().GetName().Version, DateTime.Now);
    }

    public void OnReleased()
    {
      LoadingManager = null;
      DebugLog.Message("Released v.{0} at {1}", Assembly.GetExecutingAssembly().GetName().Version, DateTime.Now);
    }

    public void OnLevelLoaded(LoadMode mode)
    {
      if (IsGameMode())
      {
        Log.Info("Level loading");
        OnGameLevelLoaded();
        Log.Info("Level loaded");
      }
      else
      {
        Log.Info("In Editor->mod is disabled");
      }
    }

    public void OnLevelUnloading()
    {
      if (IsGameMode())
      {
        Log.Message("Level unloading");
        OnGameLevelUnloading();
        Log.Info("Level unloaded");
      }
    }

    #endregion LoadingExtensionBase

    #region helpers

    private bool IsGameMode()
    {
      if (LoadingManager != null)
      {
        return LoadingManager.IsGameMode();
      }
      //don't know -> go on
      DebugLog.Warning("IsGameMode: unknown -- default to true");
      var st = new StackTrace();
      DebugLog.Warning(st.ToString());
      return true;
    }

    #endregion

    public void OnGameLevelLoaded()
    {
      var go = new GameObject("TTLMachine");
      MainMachine = go.AddComponent<MainMachine>();
      MainMachine.Options = Options;
    }

    public void OnGameLevelUnloading()
    {
      if (MainMachine != null)
      {
        MainMachine.enabled = false;
        MainMachine.Options = null;
        GameObject.Destroy(MainMachine.gameObject);
        MainMachine = null;
      }
    }
  }

  public sealed class Threading : ThreadingExtensionBase
  {
    public Simulation Simulation => Simulation.Instance;

    public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
    {
      base.OnUpdate(realTimeDelta, simulationTimeDelta);

      Simulation.OnUpdate(realTimeDelta, simulationTimeDelta);
    }
  }

  public sealed class Loading : LoadingExtensionBase
  {
    public Simulation Simulation => Simulation.Instance;

    public override void OnCreated(ILoading loading)
    {
      Log.Info("created");
      
      base.OnCreated(loading);

      Simulation.OnCreated(loading);
    }

    public override void OnReleased()
    {      
      base.OnReleased();

      Simulation.OnReleased();
      
      Log.Info("released");
    }

    public override void OnLevelLoaded(LoadMode mode)
    {
      base.OnLevelLoaded(mode);

      Simulation.OnLevelLoaded(mode);
    }

    public override void OnLevelUnloading()
    {
      base.OnLevelUnloading();

      Simulation.OnLevelUnloading();
    }
  }
}
