
using System;
using ColossalFramework;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;

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

        private GroundMode _usedGroundMode;
        public GroundMode UsedGroundMode
        {
            get { return _usedGroundMode; }
            set
            {
                if (value != _usedGroundMode)
                {
                    _usedGroundMode = value;
                    OnGroundModeChanged(value);
                }
            }
        }

        public event Action<GroundMode> GroundModeChanged;
        private void OnGroundModeChanged(GroundMode gm)
        {
            var handler = GroundModeChanged;
            if (handler != null)
            {
                handler(gm);
            }
        }

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