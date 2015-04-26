using ColossalFramework.Steamworks;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils
{
    public struct ChangedValue<T>
    {
        public readonly T Value;
        public readonly bool WasChanged;

        public ChangedValue(T value, bool wasChanged) : this()
        {
            Value = value;
            WasChanged = wasChanged;
        }
    }

    public static class ChangedValue
    {
        public static ChangedValue<T> Changed<T>(T value)
        {
            return new ChangedValue<T>(value, true);
        }

        public static ChangedValue<T> Unchanged<T>(T value)
        {
            return new ChangedValue<T>(value, false);
        }
    }
}