using System.Diagnostics;
using ColossalFramework;
using ColossalFramework.Plugins;
using JetBrains.Annotations;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils
{
    public static class Log
    {
        private static readonly string Prefix = "TrafficLight";

        public static void Generic(PluginManager.MessageType messageType, string text)
        {
            var msg = string.Format("[{0}] {1}", Prefix, text);
            DebugOutputPanel.AddMessage(messageType, msg);

#if DEBUG
            CODebugBase<LogChannel>.Warn(LogChannel.Modding, string.Format("ToggleTrafficLights: {0}: {1}", messageType.ToString("G"), msg));
#endif
        }

        public static void Error(string text)
        {
            Generic(PluginManager.MessageType.Error, text);
        }

        [StringFormatMethod("format")]
        public static void Error(string format, params object[] args)
        {
            Error(string.Format(format, args));
        }

        public static void Warning(string text)
        {
            Generic(PluginManager.MessageType.Warning, text);
        }
        [StringFormatMethod("format")]
        public static void Warning(string format, params object[] args)
        {
            Warning(string.Format(format, args));
        }

        public static void Message(string text)
        {
            Generic(PluginManager.MessageType.Message, text);
        }
        [StringFormatMethod("format")]
        public static void Message(string format, params object[] args)
        {
            Message(string.Format(format, args));
        }
        public static void Info(string text)
        {
            Generic(PluginManager.MessageType.Message, text);
        }
        [StringFormatMethod("format")]
        public static void Info(string format, params object[] args)
        {
            Info(string.Format(format, args));
        }
    }

    public static class DebugLog
    {
        [Conditional("DEBUG")]
        public static void Generic(PluginManager.MessageType messageType, string text)
        {
            Log.Generic(messageType, text);
        }
        [Conditional("DEBUG")]
        public static void Error(string text)
        {
            Generic(PluginManager.MessageType.Error, text);
        }
        [Conditional("DEBUG")]
        [StringFormatMethod("format")]
        public static void Error(string format, params object[] args)
        {
            Error(string.Format(format, args));
        }
        [Conditional("DEBUG")]
        public static void Warning(string text)
        {
            Generic(PluginManager.MessageType.Warning, text);
        }
        [Conditional("DEBUG")]
        [StringFormatMethod("format")]
        public static void Warning(string format, params object[] args)
        {
            Warning(string.Format(format, args));
        }

        [Conditional("DEBUG")]
        public static void Message(string text)
        {
            Generic(PluginManager.MessageType.Message, text);
        }
        [Conditional("DEBUG")]
        [StringFormatMethod("format")]
        public static void Message(string format, params object[] args)
        {
            Message(string.Format(format, args));
        }
        [Conditional("DEBUG")]
        public static void Info(string text)
        {
            Generic(PluginManager.MessageType.Message, text);
        }
        [Conditional("DEBUG")]
        [StringFormatMethod("format")]
        public static void Info(string format, params object[] args)
        {
            Info(string.Format(format, args));
        }
    }

}