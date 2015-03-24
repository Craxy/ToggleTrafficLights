namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions
{
    public static class CitiesHelper
    {
        public static bool HasTrafficLights(NetNode.Flags flags)
        {
            return (flags & NetNode.Flags.TrafficLights) == NetNode.Flags.TrafficLights;
        }
    }
}