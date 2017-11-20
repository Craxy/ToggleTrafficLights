using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Craxy.CitiesSkylines.ToggleTrafficLights.Game;
using Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using ICities;

namespace Craxy.CitiesSkylines.ToggleTrafficLights
{
  // Strange behaviour of IUserMod with other interfaces:
  // For every class with C:S mod interface a new instance is created, even if same class implements multiple interfaces.
  //   Except: for IUserMod: if class with IUserMod implements other C:S mod interfaces, IUserMod instance is reused
   
  public sealed class Mod 
    : IUserMod
      , ISerializableDataExtension, ILoadingExtension
#if DEBUG
      , IThreadingExtension
#endif
  {
    public Simulation Simulation => Simulation.Instance;
    
    #region IUserMod
    private static Version Version 
      => Assembly.GetExecutingAssembly().GetName().Version;
    
    static Mod() 
      => Log.Info($"Loaded: {Assembly.GetExecutingAssembly().GetName()})");

    public string Name 
      => "Toggle Traffic Lights"
#if DEBUG
         + " (DEBUG)"
#endif
          ;
    public string Description 
      => "Remove or add traffic lights at junctions.";
    
    public void OnEnabled() 
      => Log.Message($"Mod {Name} v.{Version} enabled at {DateTime.Now}");
    public void OnDisabled() 
      => Log.Message($"Mod {Name} v.{Version} disabled at {DateTime.Now}");

    public void OnSettingsUI(UIHelperBase helper) 
      => SettingsBuilder.MakeSettings((UIHelper) helper, Simulation.Options);

    #endregion IUserMode
    
    #region ISerializableDataExtension
    void ISerializableDataExtension.OnCreated(ISerializableData serializedData) 
      => Trace(nameof(ISerializableDataExtension));
    void ILoadingExtension.OnReleased() 
      => Trace(nameof(ILoadingExtension));
    
    void ILoadingExtension.OnLevelLoaded(LoadMode mode)
    {
      Trace(nameof(ILoadingExtension));
      Simulation.Process(Simulation.Step.LevelLoaded);
    }
    void ILoadingExtension.OnLevelUnloading()
    {
      Trace(nameof(ILoadingExtension));
      Simulation.Process(Simulation.Step.LevelUnloaded);
    }
    #endregion ISerializableDataExtension
    
    #region ILoadingExtension
    void ILoadingExtension.OnCreated(ILoading loading) 
      => Trace(nameof(ILoadingExtension));
    void ISerializableDataExtension.OnReleased() 
      => Trace(nameof(ISerializableDataExtension));
    
    void ISerializableDataExtension.OnLoadData()
    {
      Trace(nameof(ISerializableDataExtension));
      Simulation.Process(Simulation.Step.LoadData);
    }
    void ISerializableDataExtension.OnSaveData()
    {
      Trace(nameof(ISerializableDataExtension));
      Simulation.Process(Simulation.Step.SaveData);
    }
    #endregion ILoadingExtension
    
    #region IThreadingExtension
    #if DEBUG
    void IThreadingExtension.OnCreated(IThreading threading) 
      => Trace(nameof(IThreadingExtension));
    void IThreadingExtension.OnReleased() 
      => Trace(nameof(IThreadingExtension));
    
    void IThreadingExtension.OnUpdate(float realTimeDelta, float simulationTimeDelta)
    {
      Simulation.OnUpdate(realTimeDelta, simulationTimeDelta);
    }
    void IThreadingExtension.OnBeforeSimulationTick()
    {}
    void IThreadingExtension.OnBeforeSimulationFrame()
    {}
    void IThreadingExtension.OnAfterSimulationFrame()
    {}
    void IThreadingExtension.OnAfterSimulationTick()
    {}
    #endif
    #endregion IThreadingExtension

    [Conditional("DEBUG")]
    private void Trace(string interfaceName, [CallerMemberName] string memberName = "")
    {
      #if DEBUG
      DebugLog.Info($"{interfaceName}.{memberName} called at {DateTime.Now} on instance {_uniqueId}.");
      #endif
    }
    #if DEBUG
    private readonly string _uniqueId = Guid.NewGuid().ToString();
    #endif
  }
}
