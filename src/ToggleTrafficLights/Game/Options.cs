
using System;
using System.Globalization;
using System.Xml.Linq;
using ColossalFramework;
using Craxy.CitiesSkylines.ToggleTrafficLights.Game.Option;
using Craxy.CitiesSkylines.ToggleTrafficLights.Game.Option.Shortcuts;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using UnityEngine;
using static Craxy.CitiesSkylines.ToggleTrafficLights.Utils.FunctionalHelper;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game
{


    public sealed class Options : OptionsGroup
    {
        #region Default
        private static Options _default;
        private static readonly object Lock = new object();
        public static Options Default
        {
            get
            {
                if (_default == null)
                {
                    lock (Lock)
                    {
                        if (_default == null)
                        {
                            _default = new Options(nameof(Options));
                        }
                    }
                }

                return _default;
            }
        }
        #endregion

        [Flags]
        public enum GroundMode
        {
            None = 0,
            Overground = 1,
            Underground = 2,
            All = Overground | Underground,
        }

        public Options(string name) : base(name) {}

        #region Shortcuts
        public sealed class ShortcutsGroup : OptionsGroup
        {
            public ShortcutsGroup(string name) : base(name) { }

            public class GroundGroup : OptionsGroup
            {
                public GroundGroup(string name) : base(name) { }

                public SavedShortcutOption Overground { get; } = new SavedShortcutOption(nameof(Overground),
                        Shortcut.Create(KeyCode.PageUp),
                        enabled: true, save: true);
                public SavedShortcutOption Underground { get; } = new SavedShortcutOption(nameof(Underground),
                        Shortcut.Create(KeyCode.PageDown),
                        enabled: true, save: true);
                public SavedShortcutOption OvergroundAndUnderground { get; } = new SavedShortcutOption(nameof(OvergroundAndUnderground),
                        Shortcut.Create(KeyCode.PageDown),
                        enabled: true, save: true);
            }
            public GroundGroup Ground { get; } = new GroundGroup(nameof(Ground));

            public class ToolGroup : OptionsGroup
            {
                public ToolGroup(string name) : base(name) { }


                public SavedShortcutOption ActivateTool { get; } = new SavedShortcutOption(nameof(ActivateTool),
                        Shortcut.Create(KeyCode.T).WithControl(),
                        enabled: true, save: true);
                public SavedShortcutOption ActivateToolWithGui { get; } = new SavedShortcutOption(nameof(ActivateToolWithGui),
                        Shortcut.Create(KeyCode.T).WithControl().WithAlt(),
                        enabled: true, save: true);
                public SavedShortcutOption ActivateToolWithoutGui { get; } = new SavedShortcutOption(nameof(ActivateToolWithoutGui),
                        Shortcut.Create(KeyCode.T).WithControl().WithShift(),
                        enabled: true, save: true);
            }
            public ToolGroup Tool { get; } = new ToolGroup(nameof(Tool));

            public class BatchCommandsGroup : OptionsGroup
            {
                public BatchCommandsGroup(string name) : base(name) { }

                public SavedShortcutOption RemoveAllTrafficLights { get; } = new SavedShortcutOption(nameof(RemoveAllTrafficLights),
                        Shortcut.Create(KeyCode.Delete).WithControl().WithAlt(),
                        enabled: true, save: true);
                public SavedShortcutOption AddTrafficLightsToAllIntersections { get; } = new SavedShortcutOption(nameof(AddTrafficLightsToAllIntersections),
                        Shortcut.Create(KeyCode.Insert).WithControl().WithAlt(),
                        enabled: false, save: false);
                public SavedShortcutOption ResetAllTrafficLightsToDefault { get; } = new SavedShortcutOption(nameof(ResetAllTrafficLightsToDefault),
                        Shortcut.Create(KeyCode.Home).WithControl().WithAlt(),
                        enabled: false, save: false);
            }
            public BatchCommandsGroup BatchCommands { get; } = new BatchCommandsGroup(nameof(BatchCommands));
        }
        public ShortcutsGroup Shortcuts { get; } = new ShortcutsGroup(nameof(Shortcuts));
        #endregion

        #region ToggleTrafficLightsTool
        public sealed class ToggleIntersectionsGroup : OptionsGroup
        {
            public ToggleIntersectionsGroup(string name) : base(name) {}
            /// <summary>
            /// Intersections that should be able to toggle (Overground, Underground, both)
            /// </summary>
            public SavedOption<GroundMode> ToggleIntersectionsAtElevation = new SavedOption<GroundMode>(nameof(ToggleIntersectionsAtElevation), 
                defaultValue: Options.GroundMode.Overground, 
                serializeMethod: (name, value) => new XElement(name, value.ToString()), 
                deserializeMethod: (name, xml) => Utils.Option.Some((GroundMode)Enum.Parse(typeof(GroundMode), xml.Value)),
                enabled: true, save: true);

            public SavedOption<Color> HasTrafficLightsColor = new SavedOption<Color>(nameof(HasTrafficLightsColor),
                defaultValue: new Color(0.2f, 0.749f, 0.988f, 1.0f),
                serializeMethod: Option.Serializer.Serialize.Color, 
                deserializeMethod: Option.Serializer.Deserialize.Color,
                enabled: true, save: true);
            public SavedOption<Color> HasNoTrafficLightsColor = new SavedOption<Color>(nameof(HasNoTrafficLightsColor),
                defaultValue: new Color(0.0f, 0.369f, 0.525f, 1.0f),
                serializeMethod: Option.Serializer.Serialize.Color, 
                deserializeMethod: Option.Serializer.Deserialize.Color,
                enabled: true, save: true);
        }
        public ToggleIntersectionsGroup ToggleIntersections { get; } = new ToggleIntersectionsGroup(nameof(ToggleIntersections));
        #endregion

        #region HighlightIntersections

        public sealed class HighlightIntersectionsGroup : OptionsGroup
        {
            public HighlightIntersectionsGroup(string name) : base(name) { }

            public SavedOption<GroundMode> HighlightIntersectionsAtElevation = new SavedOption<GroundMode>(nameof(HighlightIntersectionsAtElevation),
                defaultValue: Options.GroundMode.Overground,
                serializeMethod: (name, value) => new XElement(name, value.ToString()),
                deserializeMethod: (name, xml) => Utils.Option.Some((GroundMode)Enum.Parse(typeof(GroundMode), xml.Value)),
                enabled: true, save: true);

            public SavedOption<Color> HasTrafficLightsColor = new SavedOption<Color>(nameof(HasTrafficLightsColor),
                defaultValue: new Color(0.56f, 1.0f, 0.56f, 1.0f),
                serializeMethod: Option.Serializer.Serialize.Color,
                deserializeMethod: Option.Serializer.Deserialize.Color,
                enabled: true, save: true);
            public SavedOption<Color> HasNoTrafficLightsColor = new SavedOption<Color>(nameof(HasNoTrafficLightsColor),
                defaultValue: new Color(0.56f, 0.0f, 0.0f, 1.0f),
                serializeMethod: Option.Serializer.Serialize.Color,
                deserializeMethod: Option.Serializer.Deserialize.Color,
                enabled: true, save: true);

            public SavedOption<float> MarkerHeight = new SavedOption<float>(nameof(MarkerHeight),
                defaultValue: 2.0f,
                serializeMethod: Option.Serializer.Serialize.Float,
                deserializeMethod: Option.Serializer.Deserialize.Float,
                enabled: true, save: true);
            public SavedOption<float> MarkerRadius = new SavedOption<float>(nameof(MarkerRadius),
                defaultValue: 5.0f,
                serializeMethod: Option.Serializer.Serialize.Float,
                deserializeMethod: Option.Serializer.Deserialize.Float,
                enabled: true, save: true);

            public SavedOption<int> NumberOfMarkerSides = new SavedOption<int>(nameof(NumberOfMarkerSides),
                defaultValue: 13,
                serializeMethod: Option.Serializer.Serialize.Int,
                deserializeMethod: Option.Serializer.Deserialize.Int,
                enabled: true, save: true);
        }
        public HighlightIntersectionsGroup HighlightIntersections { get; } = new HighlightIntersectionsGroup(nameof(HighlightIntersections));

        #endregion

        #region Logging
        //todo: move to logging class/namespace
        public enum LogLevel : byte
        {
            None = 0,
            Critical = 1,
            Error = 2,
            Warning = 3,
            Information = 4,
            Verbose = 5,
        }
        public sealed class LoggingGroup : OptionsGroup
        {
            public LoggingGroup(string name) : base(name) {}

            public SavedOption<LogLevel> LogLevel { get; } = new SavedOption<LogLevel>(nameof(LogLevel),
                defaultValue:Options.LogLevel.Warning,
                serializeMethod: (name, value) => new XElement(name, value.ToString()),
                deserializeMethod: (name, xml) => Utils.Option.Some((LogLevel)Enum.Parse(typeof(LogLevel), xml.Value)),
                enabled: true, save: true); 
        }
        public LoggingGroup Logging { get; } = new LoggingGroup(nameof(Logging));
        #endregion

        #region Debugging
        public sealed class DebuggingGroup : OptionsGroup
        {
            public DebuggingGroup(string name) : base(name) { }

            //todo: implement:
            // disable serialization
            // remove serializied traffic lights (-> remove from savegame) 
            public SavedOption<bool> SaveTrafficLightsInSavegames { get; } = new SavedOption<bool>(nameof(SaveTrafficLightsInSavegames),
                defaultValue: true,
                serializeMethod: (name, value) => new XElement(name, value.ToString()),
                deserializeMethod: (name, xml) => Utils.Option.Some(bool.Parse(xml.Value)),
                enabled: true, save: true);
        }
        public DebuggingGroup Debugging { get; } = new DebuggingGroup(nameof(Debugging));
        #endregion

        #region serialization
        public void SaveToFile(string path)
        {
            this.Serialize().Save(path);
            this.MarkAsSaved();
        }
        #endregion
    }
}