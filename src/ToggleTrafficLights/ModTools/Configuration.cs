using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.ModTools
{
    public class Configuration
    {

        public static Rect mainWindowRect = new Rect(128, 128, 356, 300);
        public static bool mainWindowVisible = false;

        public static Rect consoleRect = new Rect(16.0f, 16.0f, 512.0f, 256.0f);
        public static bool consoleVisible = false;

        public static int consoleMaxHistoryLength = 1024;
        public static string consoleFormatString = "[{{type}}] {{caller}}: {{message}}";
        public static bool showConsoleOnMessage = false;
        public static bool showConsoleOnWarning = false;
        public static bool showConsoleOnError = true;
        public static bool consoleAutoScrollToBottom = true;

        public static bool improvedWorkshopIntegration = true;

        public static Rect sceneExplorerRect = new Rect(128, 440, 800, 500);
        public static bool sceneExplorerVisible = false;

        public static Rect watchesRect = new Rect(504, 128, 800, 300);
        public static bool watchesVisible = false;

        public static bool logExceptionsToConsole = true;
        public static bool evaluatePropertiesAutomatically = true;
        public static bool extendGamePanels = true;
        public static bool useModToolsConsole = true;
        public static bool hookUnityLogging = true;

        public static Color backgroundColor = new Color(0.321f, 0.321f, 0.321f, 1.0f);
        public static Color titlebarColor = new Color(0.247f, 0.282f, 0.364f, 1.0f);
        public static Color titlebarTextColor = new Color(0.85f, 0.85f, 0.85f, 1.0f);

        public static Color gameObjectColor = new Color(165.0f / 255.0f, 186.0f / 255.0f, 229.0f / 255.0f, 1.0f);
        public static Color enabledComponentColor = Color.white;
        public static Color disabledComponentColor = new Color(127.0f / 255.0f, 127.0f / 255.0f, 127.0f / 255.0f, 1.0f);
        public static Color selectedComponentColor = new Color(233.0f / 255.0f, 138.0f / 255.0f, 23.0f / 255.0f, 1.0f);

        public static Color nameColor = new Color(148.0f / 255.0f, 196.0f / 255.0f, 238.0f / 255.0f, 1.0f);
        public static Color typeColor = new Color(58.0f / 255.0f, 179.0f / 255.0f, 58.0f / 255.0f, 1.0f);
        public static Color keywordColor = new Color(233.0f / 255.0f, 102.0f / 255.0f, 47.0f / 255.0f, 1.0f);
        public static Color modifierColor = new Color(84.0f / 255.0f, 109.0f / 255.0f, 57.0f / 255.0f, 1.0f);
        public static Color memberTypeColor = new Color(86.0f / 255.0f, 127.0f / 255.0f, 68.0f / 255.0f, 1.0f);
        public static Color valueColor = Color.white;

        public static Color consoleMessageColor = Color.white;
        public static Color consoleWarningColor = Color.yellow;
        public static Color consoleErrorColor = new Color(0.7f, 0.1f, 0.1f, 1.0f);
        public static Color consoleExceptionColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);

        public static bool sceneExplorerShowFields = true;
        public static bool sceneExplorerShowProperties = true;
        public static bool sceneExplorerShowMethods = false;
        public static bool sceneExplorerShowModifiers = false;
        public static bool sceneExplorerShowInheritedMembers = false;
        public static bool sceneExplorerEvaluatePropertiesAutomatically = true;
        public static bool sceneExplorerSortAlphabetically = true;

        public static string fontName = "Courier New Bold";
        public static int fontSize = 14;

        public static int hiddenNotifications = 0;
    }
}