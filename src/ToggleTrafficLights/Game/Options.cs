
using System;
using ColossalFramework;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game
{
    public static class Options
    {
        #region Enums
        [Flags]
        public enum GroundMode
        {
            Ground = 1,
            Underground = 2,
            All = Ground | Underground,
        }

        #endregion

        #region Keys

        public static class InputKeys
        {
            public static readonly SavedInputKey ElevationUp = new SavedInputKey(Settings.buildElevationUp, Settings.gameSettingsFile, DefaultSettings.buildElevationUp, true);
            public static readonly SavedInputKey ElevationDown = new SavedInputKey(Settings.buildElevationDown, Settings.gameSettingsFile, DefaultSettings.buildElevationDown, true);
        }
        #endregion

        #region Traffic Lights Tool
        public static class ToggleTrafficLightsTool
        {
            public static readonly ChangingValue<GroundMode> GroundMode = ChangingValue.Create<GroundMode>(Options.GroundMode.Ground);

            public static readonly ChangingValue<Color> HasTrafficLightsColor = ChangingValue.Create(new Color(0.2f, 0.749f, 0.988f, 1.0f));
            public static readonly ChangingValue<Color> HasNoTrafficLightsColor = ChangingValue.Create(new Color(0.0f, 0.369f, 0.525f, 1.0f));
        }
        #endregion

        #region Traffic Lights Highlighting

        public static class HighlightIntersections
        {
            public static readonly ChangingValue<bool> Enabled = ChangingValue.Create(true);
            public static readonly ChangingValue<GroundMode> IntersectionsToHighlight = ChangingValue.Create(GroundMode.Underground | GroundMode.Ground);

            public static readonly ChangingValue<Color> HasTrafficLightsColor = ChangingValue.Create(Color.white);
            public static readonly ChangingValue<Color> HasNoTrafficLightsColor = ChangingValue.Create(Color.black);

            public static readonly ChangingValue<float> MarkerHeight = ChangingValue.Create(3.0f);
            public static readonly ChangingValue<float> MarkerRadius = ChangingValue.Create(5.0f);

        }
        #endregion



        #region Save Options

        public static void Serialize(string path)
        {
            
        }

        public static void Deserialzie(string path)
        {
            
        }
        #endregion
    }
}