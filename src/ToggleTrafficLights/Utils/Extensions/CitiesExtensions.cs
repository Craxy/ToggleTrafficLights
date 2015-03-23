using ICities;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions
{
    public static class CitiesExtensions
    {
        public static bool IsGameMode(this ILoading loading)
        {
            return loading.currentMode == AppMode.Game;
        }
    }
}