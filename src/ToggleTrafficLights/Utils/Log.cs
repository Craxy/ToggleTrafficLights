using System;
using System.Diagnostics;
using ColossalFramework;
using ColossalFramework.Plugins;
using JetBrains.Annotations;
using Debug = UnityEngine.Debug;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils
{
    public static class Log
    {
        internal static readonly string Prefix = "TrafficLights";

        internal static string AppendPrefix(string text)
        {
            return $"[{Prefix}] {text}";
        }

        internal static string FormatMessage(PluginManager.MessageType messageType, string text)
        {
            return $"[{Prefix}][{messageType}] {text}";
        }

        public static void Generic(PluginManager.MessageType messageType, string text)
        {
//            var msg = string.Format("[{0}] {1}", Prefix, text);
            var msg = AppendPrefix(text);
            DebugOutputPanel.AddMessage(messageType, msg);

#if DEBUG
            CODebugBase<LogChannel>.Log(LogChannel.Modding, $"{messageType.ToString("G")}: {msg}");
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
            var msg = Log.AppendPrefix(text);

            switch (messageType)
            {
                case PluginManager.MessageType.Error:
                    Debug.LogError(msg);
                    break;
                case PluginManager.MessageType.Warning:
                    Debug.LogWarning(msg);
                    break;
                case PluginManager.MessageType.Message:
                    Debug.Log(msg);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(messageType));
            }

//            CODebugBase<LogChannel>.Log(LogChannel.Modding, string.Format("{0}: {1}", messageType.ToString("G"), msg));
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

    public static class FileLog
    {
        public static void Generic(ErrorLevel errorLevel, string text)
        {
            var msg = $"[{"ToggleTrafficLights"}] {text}";
            CODebugBase<LogChannel>.Log(LogChannel.Modding, msg, errorLevel);
        }

        public static void Error(string text)
        {
            Generic(ErrorLevel.Error, text);
        }

        [StringFormatMethod("format")]
        public static void Error(string format, params object[] args)
        {
            Error(string.Format(format, args));
        }

        public static void Warning(string text)
        {
            Generic(ErrorLevel.Warning, text);
        }
        [StringFormatMethod("format")]
        public static void Warning(string format, params object[] args)
        {
            Warning(string.Format(format, args));
        }

        public static void Message(string text)
        {
            Generic(ErrorLevel.Info, text);
        }
        [StringFormatMethod("format")]
        public static void Message(string format, params object[] args)
        {
            Message(string.Format(format, args));
        }
        public static void Info(string text)
        {
            Generic(ErrorLevel.Info, text);
        }
        [StringFormatMethod("format")]
        public static void Info(string format, params object[] args)
        {
            Info(string.Format(format, args));
        }
    }
}