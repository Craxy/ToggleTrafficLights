using ColossalFramework;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions
{
  public static class InputKeyExtensions
  {
    public static void Deconstruct(this InputKey key, out KeyCode keyCode, out bool control, out bool shift, out bool alt)
    {
      const int MASK_KEY = 268435455;
      const int MASK_CONTROL = 1073741824;
      const int MASK_SHIFT = 536870912;
      const int MASK_ALT = 268435456;
      
      var num = (int) key;
      keyCode = (KeyCode) (num & MASK_KEY);
      control = (num & MASK_CONTROL) != 0;
      shift = (num & MASK_SHIFT) != 0;
      alt = (num & MASK_ALT) != 0;
    }

    public static bool IsPressed(this InputKey key, Event e)
    {
      if (e.type != EventType.KeyDown)
      {
        return false;
      }

//      var (keyCode, control, shift, alt) = key;  // [CS8179] Predefined type 'System.ValueTuple`4' is not defined or imported
      key.Deconstruct(out var keyCode, out var control, out var shift, out var alt);
      return keyCode != KeyCode.None && keyCode == e.keyCode
             && e.control == control
             && e.shift == shift
             && e.alt == alt;
    }
  }
}
