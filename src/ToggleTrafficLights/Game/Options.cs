
using System;
using System.Xml.Linq;
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
            None = 0,
            Overground = 1,
            Underground = 2,
            All = Overground | Underground,
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
            public static readonly ChangingValue<GroundMode> GroundMode = ChangingValue.Create<GroundMode>(Options.GroundMode.Overground);

            public static readonly ChangingValue<Color> HasTrafficLightsColor = ChangingValue.Create(new Color(0.2f, 0.749f, 0.988f, 1.0f));
            public static readonly ChangingValue<Color> HasNoTrafficLightsColor = ChangingValue.Create(new Color(0.0f, 0.369f, 0.525f, 1.0f));
        }
        #endregion

        #region Traffic Lights Highlighting

        public static class HighlightIntersections
        {
            public static readonly ChangingValue<GroundMode> IntersectionsToHighlight = ChangingValue.Create(GroundMode.None);

            public static readonly ChangingValue<Color> HasTrafficLightsColor = ChangingValue.Create(new Color(0.56f, 1.0f, 0.56f, 1.0f));
            public static readonly ChangingValue<Color> HasNoTrafficLightsColor = ChangingValue.Create(new Color(0.56f, 0.0f, 0.0f, 1.0f));

            public static readonly ChangingValue<float> MarkerHeight = ChangingValue.Create(2.0f);
            public static readonly ChangingValue<float> MarkerRadius = ChangingValue.Create(5.0f);

            public static readonly ChangingValue<int> NumberOfMarkerSides = ChangingValue.Create(13);

        }
        #endregion



        #region Save Options

        public static void Serialize(string path)
        {
            throw new NotImplementedException();

//            try
//            {
//                var xml = new XElement(typeof (Options).Name,
//                    new XElement(typeof(Options.ToggleTrafficLightsTool).Name,
//                            Options.ToggleTrafficLightsTool.GroundMode.ToXml()
//                        )
//                    );
//
//            }
//            catch (Exception e)
//            {
//                Log.Error("Error while saving the options to '{0}': {1}", path, e.ToString());
//            }
        }

        public static void Deserialzie(string path)
        {
            throw new NotImplementedException();
        }
        #endregion

        private static XElement ToXml<T>(this ChangingValue<T> v, string name, Func<T,string> toString)
        {
            return new XElement(name, toString(v.Value));
        }

    }
}