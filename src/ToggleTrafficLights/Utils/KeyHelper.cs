using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils
{
  public static class KeyHelper
  {
    public static bool IsControlDown(KeyCode code) => code == KeyCode.LeftControl || code == KeyCode.RightControl;
    public static bool IsShiftDown(KeyCode code) => code == KeyCode.LeftShift || code == KeyCode.RightShift;
    public static bool IsAltDown(KeyCode code) => code == KeyCode.LeftAlt || code == KeyCode.RightAlt;
    public static bool IsModifierKey(KeyCode code) => IsControlDown(code) || IsShiftDown(code) || IsAltDown(code);
  }
}
