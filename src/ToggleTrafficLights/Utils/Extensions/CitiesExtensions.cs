using ICities;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions
{
  public static class CitiesExtensions
  {
    public static bool IsGameMode(this ILoading loading) 
      => loading.currentMode == AppMode.Game;

    public static bool IsGameMode(this IManagers managers) 
      => managers.loading.IsGameMode();

    public static bool IsGameMode(this SerializableDataExtensionBase extensionBase)
      => extensionBase.managers.IsGameMode();
    public static bool IsGameMode(this LoadingExtensionBase extensionBase)
      => extensionBase.managers.IsGameMode();
    public static bool IsGameMode(this ThreadingExtensionBase extensionBase)
      => extensionBase.managers.IsGameMode();
  }
}
