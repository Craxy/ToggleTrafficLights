using System;
using ColossalFramework;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game
{
  public class Options
  {
    public const string SettingsFile = "ToggleTrafficLights";

    static Options()
    {
      GameSettings.AddSettingsFile(new SettingsFile {fileName = SettingsFile});
    }

    private static readonly InputKey DefaultShortcutActivateTTLWithMenu =
      SavedInputKey.Encode(KeyCode.T, true, false, false);

    private static readonly InputKey DefaultShortcutActivateTTLWithoutMenu =
      SavedInputKey.Encode(KeyCode.T, true, false, true);

    private static readonly InputKey DefaultShortcutActivateTrafficRoutesJunctions =
      SavedInputKey.Encode(KeyCode.T, true, true, false);

    public readonly SavedInputKey ShortcutActivateTTLWithMenu = new SavedInputKey(nameof(ShortcutActivateTTLWithMenu),
      SettingsFile, DefaultShortcutActivateTTLWithMenu, true);

    public readonly SavedInputKey ShortcutActivateTTLWithoutMenu =
      new SavedInputKey(nameof(ShortcutActivateTTLWithoutMenu), SettingsFile, DefaultShortcutActivateTTLWithoutMenu,
        true);

    public readonly SavedInputKey ShortcutActivateTrafficRoutesJunctions = new SavedInputKey(
      nameof(ShortcutActivateTrafficRoutesJunctions), SettingsFile, DefaultShortcutActivateTrafficRoutesJunctions,
      true);

    private static readonly bool DefaultKeepInfoMode = true;

    public readonly SavedBool KeepInfoMode =
      new SavedBool(nameof(KeepInfoMode), SettingsFile, DefaultKeepInfoMode, true);

    public void ResetShortcuts()
    {
      ShortcutActivateTTLWithMenu.value = DefaultShortcutActivateTTLWithMenu;
      ShortcutActivateTTLWithoutMenu.value = DefaultShortcutActivateTTLWithoutMenu;
      ShortcutActivateTrafficRoutesJunctions.value = DefaultShortcutActivateTrafficRoutesJunctions;
    }

    internal static string GetShortcutName(string name)
    {
      switch (name)
      {
        case nameof(ShortcutActivateTTLWithMenu):
          return "Junction Tool";
        case nameof(ShortcutActivateTTLWithoutMenu):
          return "Junction Tool (everywhere)";
        case nameof(ShortcutActivateTrafficRoutesJunctions):
          return "Traffic Routes Junctions Info View";
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    internal static string GetShortcutDescription(string name)
    {
      throw new NotImplementedException();
    }
  }
}
