﻿using System.Reflection;
using Craxy.CitiesSkylines.ToggleTrafficLights.Game;
using Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using ICities;

namespace Craxy.CitiesSkylines.ToggleTrafficLights
{
  public sealed class Mod : IUserMod
  {
    static Mod()
    {
      Log.Info($"Loaded: {Assembly.GetExecutingAssembly().GetName()})");
    }
    
    public string Name => "Toggle Traffic Lights";

    public string Description
    {
      get
      {
        return "Remove or add traffic lights at junctions."
#if DEBUG
               + " v." + Assembly.GetExecutingAssembly().GetName().Version.ToString()
#endif
          ;
      }
    }

    public void OnSettingsUI(UIHelperBase helper)
    {
      SettingsBuilder.MakeSettings((UIHelper) helper, Simulation.Instance.Options);
    }
  }
}
