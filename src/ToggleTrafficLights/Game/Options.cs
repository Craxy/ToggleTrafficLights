
using System;
using ColossalFramework;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game
{
    public sealed class Options : SingletonLite<Options>
    {
        #region ctor -- required by SingletonLite 
        public Options()
        {
            ElevationUp = new SavedInputKey(Settings.buildElevationUp, Settings.gameSettingsFile, DefaultSettings.buildElevationUp, true);
            ElevationDown = new SavedInputKey(Settings.buildElevationDown, Settings.gameSettingsFile, DefaultSettings.buildElevationDown, true);

            UsedGroundMode = GroundMode.Ground;
        }
        #endregion

        #region ToggleTrafficLightsTool

        #region Underground
        public SavedInputKey ElevationUp { get; private set; }
        public SavedInputKey ElevationDown { get; private set; }

        public GroundMode UsedGroundMode { get; set; }

        [Flags]
        public enum GroundMode
        {
            Ground = 1,
            Underground = 2,
            All = Ground | Underground,
        }

        #endregion

        public bool HighlightAllIntersections = true;
        #endregion
    }
}