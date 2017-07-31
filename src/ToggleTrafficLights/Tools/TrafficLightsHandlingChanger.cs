using System;
using System.Collections.Generic;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Tools
{
  public class TrafficLightsHandlingChanger : IDisposable
  {
    //GetInstanceId(): simpler ID, but no (easy) way to retrieve object by ID
    //private readonly IDictionary<int, bool> _defaults = new Dictionary<int, bool>();
    private readonly Dictionary<RoadBaseAI, bool> _defaults = new Dictionary<RoadBaseAI, bool>();
    private void Remember(RoadBaseAI ai, bool hasLights) => _defaults[ai] = hasLights;
    private bool Remembered(RoadBaseAI ai) => _defaults.ContainsKey(ai);
    private bool TryGetRemembered(RoadBaseAI ai, out bool hasLights) => _defaults.TryGetValue(ai, out hasLights);
    private void Forget(RoadBaseAI ai) => _defaults.Remove(ai);
    private bool TryRemember(RoadBaseAI ai, bool hasLights)
    {
      if (!Remembered(ai))
      {
        Remember(ai, hasLights);
        return true;
      }
      else
      {
        return false;
      }
    }
    private void RememberIfNew(RoadBaseAI ai, bool hasLights)
    {
      if (!Remembered(ai))
      {
        Remember(ai, hasLights);
      }
    }

    public void Change(RoadBaseAI ai, bool hasLights)
    {
      RememberIfNew(ai, ai.m_trafficLights);
      ai.m_trafficLights = hasLights;
    }
    public void ChangeAll(bool haveLights)
    {
      foreach (var roadInfo in GetAllRoads())
      {
        var ai = roadInfo.m_netAI as RoadBaseAI;
        if (ai != null)
        {
          Change(ai, haveLights);
        }
      }
    }

    private static List<NetInfo> GetAllRoads()
    {
      var roads = new List<NetInfo>();

//      // iterate buttons in roads building menu and collect displayed roads
//      foreach (var btn in UIView.Find<UIComponent>("RoadsPanel").GetComponentsInChildren<UIButton>())
//      {
//        var info = btn.objectUserData as NetInfo;
//        if (info != null)
//        {
//          var ai = info.m_netAI as RoadBaseAI;
//          if (ai != null)
//          {
//            roads.Add(info);
//          }
//        }
//      }
      
//      // interate NetInfos and collect all roads with a category not "Default". These are used by slopes etc.
//      // this returns 2 more roads than via RoadsPanels:
//      //  Harbor Road, Bus Depot Road
//      var count = (uint)PrefabCollection<NetInfo>.LoadedCount();
//      for(uint i = 0; i < count; i++)
//      {
//        var info = PrefabCollection<NetInfo>.GetLoaded(i);
//        if(info != null)
//        {
//          var serviceValid = info.GetService() == ItemClass.Service.Road 
//                             && (info.m_vehicleTypes & VehicleInfo.VehicleType.Car) != VehicleInfo.VehicleType.None;
//          var categoryValid = !string.IsNullOrEmpty(info.category) && info.category != "Default";
//          
//          // tests for service and category are sufficient, but better safe than sorry
//          var ai = info.m_netAI as RoadBaseAI;
//          if(serviceValid && categoryValid && ai != null)
//          {
//            roads.Add(info);
//          }
//        }
//      }

      // slopes, tunnels and bridges have special ais -> traffic lights must be changed for them too
      var count = (uint)PrefabCollection<NetInfo>.LoadedCount();
      for(uint i = 0; i < count; i++)
      {
        var info = PrefabCollection<NetInfo>.GetLoaded(i);
        if(info != null)
        {
          var serviceValid = info.GetService() == ItemClass.Service.Road 
                             && (info.m_vehicleTypes & VehicleInfo.VehicleType.Car) != VehicleInfo.VehicleType.None;
          
          var ai = info.m_netAI as RoadBaseAI;
          if(serviceValid && ai != null)
          {
            roads.Add(info);
          }
        }
      }
      
      return roads;
    }

    public void Reset(RoadBaseAI ai, bool forget = false)
    {
      if (TryGetRemembered(ai, out var hasLights))
      {
        ai.m_trafficLights = hasLights;
        if (forget)
        {
          Forget(ai);
        }
      }
    }
    public void ResetAllRemembered(bool forget = true)
    {
      foreach (var pair in _defaults)
      {
        var ai = pair.Key;
        var hasLights = pair.Value;

        ai.m_trafficLights = hasLights;
      }
      
      if (forget)
      {
        _defaults.Clear();
      }
    }

    public void Dispose()
    {
      ResetAllRemembered(forget: true);
    }
  }
}
