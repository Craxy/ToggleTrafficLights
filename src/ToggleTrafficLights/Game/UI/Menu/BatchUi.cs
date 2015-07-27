using System.Collections;
using System.Globalization;
using System.Linq;
using ColossalFramework;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.ModTools;
using Craxy.CitiesSkylines.ToggleTrafficLights.Tools;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.Menu
{
    public sealed class BatchUi : MonoBehaviour
    {
        #region fields

        private Texture2D _backgroundTexture = null;
        private Vector2 _scrollPosition;

        private const int UpdateStatisticsEveryNthUpdate = 30;
        private int _updateStatisticsCounter = 0;
        private Statistics _statistics = null;

        private const int ShowChangesForNUpdates = 300;
        private int _updateChangedStatisticsCounter = 0;
        private ChangedStatistics _changedStatistics = null;

        private UIPanel _clickCatcher = null;
        private readonly WindowSize _size = new WindowSize
        {
             Left = 0f,
             Top = 50f,
             Width = 300f,
             Height = 600f,
             Padding = 5f, 
        };

        private ColorPicker _hasTrafficLightsColorPicker;
        private ColorPicker _doesNotHaveTrafficLightsColor;
        #endregion

        #region MonoBehaviour

        [UsedImplicitly]
        private void Start()
        {
            DebugLog.Info("BatchUi: Start");

            name = "ToggleTrafficLightsBatchUi";

            _backgroundTexture = new Texture2D(1, 1);
            _backgroundTexture.SetPixel(0, 0, new Color(0.321f, 0.321f, 0.321f, 1.0f));
            _backgroundTexture.Apply();
        }

        [UsedImplicitly]
        private void OnDestroy()
        {
            DebugLog.Info("BatchUi: OnDestroy");

            if (_clickCatcher != null)
            {
                Destroy(_clickCatcher.gameObject);
            }
            _clickCatcher = null;

            if (_doesNotHaveTrafficLightsColor != null)
            {
                Destroy(_doesNotHaveTrafficLightsColor.gameObject);
            }
            _doesNotHaveTrafficLightsColor = null;
            if (_hasTrafficLightsColorPicker != null)
            {
                Destroy(_hasTrafficLightsColorPicker.gameObject);
            }
            _hasTrafficLightsColorPicker = null;
        }

        [UsedImplicitly]
        private void OnEnable()
        {
            DebugLog.Info("BatchUi: OnEnable");

            _updateStatisticsCounter = 0;
            _updateChangedStatisticsCounter = 0;
            _changedStatistics = null;
            _statistics = null;

            if (_clickCatcher == null)
            {
                var uiView = UIView.GetAView();
                _clickCatcher = (UIPanel)uiView.AddUIComponent(typeof(UIPanel));
                _clickCatcher.name = "ToggleTrafficLightsToolBatchUiBachground";
                //without background sprite it's invisible
//                _clickCatcher.backgroundSprite = "GenericPanel";


                //adjust _size from unity pixels to C:S pixels via GetUIView().ratio
                var ratio = uiView.ratio;

                _clickCatcher.absolutePosition = new Vector3(_size.Left * ratio, _size.Top * ratio);
                _clickCatcher.size = new Vector2(_size.Width * ratio, _size.Height * ratio);
                _clickCatcher.zOrder = int.MaxValue;
            }
            _clickCatcher.isVisible = true;
            _clickCatcher.isEnabled = true;

            if (_doesNotHaveTrafficLightsColor == null)
            {
                _doesNotHaveTrafficLightsColor = gameObject.AddComponent<ColorPicker>();
            }
            if (_hasTrafficLightsColorPicker == null)
            {
                _hasTrafficLightsColorPicker = gameObject.AddComponent<ColorPicker>();

            }
        }

        [UsedImplicitly]
        private void OnDisable()
        {
            DebugLog.Info("BatchUi: OnDisable");

            _updateStatisticsCounter = 0;
            _updateChangedStatisticsCounter = 0;
            _changedStatistics = null;
            _statistics = null;

            if (_clickCatcher != null)
            {
                _clickCatcher.isVisible = false;
                _clickCatcher.isEnabled = false;
            }
        }

        [UsedImplicitly]
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(_size.Left, _size.Top, _size.Width, _size.Height));
            {
                GUI.Box(new Rect(0f, 0f, _size.Width, _size.Height), _backgroundTexture);
                GUILayout.BeginArea(new Rect(_size.Padding, _size.Padding, _size.Width - 2 * _size.Padding, _size.Height - 2 * _size.Padding));
                {
                    GUILayout.BeginVertical();
                    {
                        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
                        {
                            ShowGuiContent();
                        }
                        GUILayout.EndScrollView();
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndArea();
            }
            GUILayout.EndArea();
        }

        private void ShowGuiContent()
        {
            using (Layout.Vertical())
            {
                GUILayout.Label("<size=15><b>Toggle Traffic Lights tool</b></size>");
                GUILayout.Space(8f);
                {
                    if (GUILayout.Button("Options"))
                    {
                        var view = UIView.GetAView();
                        var oui = view.FindUIComponent<OptionsUi>(OptionsUi.Name) ?? view.AddUIComponent(typeof(OptionsUi));
                        oui.Show(true);
                    }

                    GUILayout.Space(8f);
                }
                GUILayout.Label("<b>Usage</b>:");
                {
                    var usages = new[]
                    {
                        Tuple.Create("Left Click", "Toggle Traffic Lights"),
                        Tuple.Create("Right Click", "Reset to default"),
                        Tuple.Create(Options.InputKeys.ElevationDown.ToString(), "Only Underground"),
                        Tuple.Create(Options.InputKeys.ElevationUp.ToString(), "Only Overground"),
                        Tuple.Create(string.Format("{0}+{1}", Options.InputKeys.ElevationDown.ToString(), Options.InputKeys.ElevationUp.ToString()), "Both Overground & Underground"),
                    };

                    var skin = new GUIStyle(GUI.skin.label) {fontSize = 11};

                    var maxHeader = usages.Select(u => new GUIContent(u.Item1))
                                          .Select(skin.CalcSize)
                                          .Select(s => s.x)
                                          .Max();

                    foreach (var u in usages)
                    {
                        using (Layout.Horizontal())
                        {
                            GUILayout.Space(5f);
                            GUILayout.Label(u.Item1, skin, GUILayout.Width(maxHeader));
                            GUILayout.Space(1.5f);
                            GUILayout.Label(":", skin);
                            GUILayout.Space(1.5f);
                            GUILayout.Label(u.Item2, skin);
                            GUILayout.FlexibleSpace();
                        }
                    }
                }

                GUILayout.Space(10f);
                ShowStatisticsGui();

                GUILayout.Space(10f);
                ShowBatchCommandsGui();
            }
        }

        private void ShowStatisticsGui()
        {
            using (Layout.Vertical())
            {
                GUILayout.Label("<b>Statistics</b>:");
                GUILayout.Label("Number of");
                if (_statistics == null || _updateStatisticsCounter++ >= UpdateStatisticsEveryNthUpdate)
                {
                    _statistics = Statistics.Collect();
                    _updateStatisticsCounter = 0;
                }
                _statistics.DrawGuiTable();
            }
        }

        private void ShowBatchCommandsGui()
        {
            using(Layout.Vertical())
            {
                GUILayout.Label("<b>Batch Commands</b>:");
                if (GUILayout.Button("Remove all Traffic Lights"))
                {
                    RemoveAllTrafficLights();
                }
                if (GUILayout.Button("Add all Traffic Lights"))
                {
                    AddAllTrafficLights();
                }
                if (GUILayout.Button("Reset all to default"))
                {
                    ResetAllTrafficLights();
                }
#if DEBUG
                if (GUILayout.Button("Test Traffic Lights"))
                {
                    TestChangingTrafficLights(50);
                }
#endif

                GUILayout.Space(5f);
                if (_changedStatistics != null && _updateChangedStatisticsCounter > 0)
                {
                    _changedStatistics.DrawGuiTable();
                    _updateChangedStatisticsCounter--;
                }
            }
        }
        #endregion

        public void RemoveAllTrafficLights()
        {
            DebugLog.Info("Clicked: Remove all Traffic Lights");

            var changes = ChangeTrafficLights((_, __, ___) => false);
            SetChangedStatistics("Traffic Lights removed", changes);
        }

        public void AddAllTrafficLights()
        {
            DebugLog.Info("Clicked: Add all Traffic Lights");

            var changes = ChangeTrafficLights((_, __, ___) => true);
            SetChangedStatistics("Traffic Lights added", changes);
        }

        public void ResetAllTrafficLights()
        {
            DebugLog.Info("Clicked: Reset all to default");

            var changes = ChangeTrafficLights((id, node, ___) => ToggleTrafficLightsTool.WantTrafficLights(id, node));
            SetChangedStatistics("Traffic Lights reset", changes);
        }

        public void TestChangingTrafficLights(int iterations)
        {
            DebugLog.Info("Clicked: Test traffic lights");

            var totalChanges = new ChangedStatistics();
            for (var i = 0; i < iterations; i++)
            {
                //change all
                var changes = ChangeTrafficLights((_, __, hasLights) => !hasLights);

                totalChanges.NumberOfChanges += changes.NumberOfChanges;
                totalChanges.NumberOfAddedLights += changes.NumberOfAddedLights;
                totalChanges.NumberOfRemovedLights += changes.NumberOfRemovedLights;
            }

            SetChangedStatistics(string.Format("Traffic lights testes. Iterations: {0}", iterations), totalChanges);
        }

        private IEnumerator ChangeLights(string action, ShouldHaveLights toggleLights)
        {
            return null;
        }

        private void SetChangedStatistics(string action, ChangedStatistics stats)
        {
            stats.Action = action;
            _changedStatistics = stats;
            _updateChangedStatisticsCounter = ShowChangesForNUpdates;
        }
        public delegate bool ShouldHaveLights(ushort id, NetNode node, bool hasLights);
        public ChangedStatistics ChangeTrafficLights(ShouldHaveLights shouldHaveLights)
        {
            var changes = new ChangedStatistics();

            var netManager = Singleton<NetManager>.instance;
            for (ushort i = 0; i < netManager.m_nodes.m_size; i++)
            {
                var node = netManager.m_nodes.m_buffer[i];

                if (node.m_flags == NetNode.Flags.None)
                {
                    continue;
                }
                if (!ToggleTrafficLightsTool.IsValidRoadNode(node))
                {
                    continue;
                }
                if ((node.m_flags & NetNode.Flags.Junction) != NetNode.Flags.Junction)
                {
                    continue;
                }

                var hasLights = ToggleTrafficLightsTool.HasTrafficLights(node.m_flags);

                var shouldLights = shouldHaveLights(i, node, hasLights);
                if (shouldLights != hasLights)
                {
                    changes.NumberOfChanges++;

                    if (shouldLights)
                    {
                        node.m_flags = ToggleTrafficLightsTool.SetTrafficLights(node.m_flags);
                        changes.NumberOfAddedLights++;
                    }
                    else
                    {
                        node.m_flags = ToggleTrafficLightsTool.UnsetTrafficLights(node.m_flags);
                        changes.NumberOfRemovedLights++;
                    }
                    netManager.m_nodes.m_buffer[i] = node;
                }
            }

            if(changes.NumberOfChanges > 0)
            {
                Options.HighlightIntersections.RequestRecalculateColorForAllIntersections();
            }

            return changes;
        }

        private ChangedStatistics ChangeTrafficLightsForAllNodes(ShouldHaveLights shouldHaveLights)
        {
            var changes = new ChangedStatistics();

            var netManager = Singleton<NetManager>.instance;
            for (ushort i = 0; i < netManager.m_nodes.m_size; i++)
            {
                var node = netManager.m_nodes.m_buffer[i];

                if (node.m_flags == NetNode.Flags.None)
                {
                    continue;
                }
                if (!ToggleTrafficLightsTool.IsValidRoadNode(node))
                {
                    continue;
                }

                var hasLights = ToggleTrafficLightsTool.HasTrafficLights(node.m_flags);

                var shouldLights = shouldHaveLights(i, node, hasLights);
                if (shouldLights != hasLights)
                {
                    changes.NumberOfChanges++;

                    if (shouldLights)
                    {
                        node.m_flags = ToggleTrafficLightsTool.SetTrafficLights(node.m_flags);
                        changes.NumberOfAddedLights++;
                    }
                    else
                    {
                        node.m_flags = ToggleTrafficLightsTool.UnsetTrafficLights(node.m_flags);
                        changes.NumberOfRemovedLights++;
                    }
                    netManager.m_nodes.m_buffer[i] = node;
                }
            }

            return changes;
        }

        public struct WindowSize
        {
            public float Left;
            public float Top;
            public float Width;
            public float Height;
            public float Padding; 
        }

        public class Statistics
        {
            public int NumberOfUsedNodes = 0;
            public int NumberOfRoadNodes = 0;
            public int NumberOfRoadIntersections = 0;
            public int NumberOfRoadIntersectionsWithTrafficLights = 0;
            public int NumberOfRoadIntersectionsWithoutTrafficLights = 0;
            public int NumberOfRoadIntersectinsWhichWantTrafficLights = 0;

            protected Statistics()
            {
            }

            public override string ToString()
            {
                var os = new[]
                {
                    string.Format("# Used Nodes                : {0}", NumberOfUsedNodes),
                    string.Format("# Road Nodes                : {0}", NumberOfRoadNodes),
                    string.Format("# Intersections             : {0}", NumberOfRoadIntersections),
                    string.Format("# Intersections w/ Lights   : {0}", NumberOfRoadIntersectionsWithTrafficLights),
                    string.Format("# Intersections w/out Lights: {0}", NumberOfRoadIntersectionsWithoutTrafficLights),
                    string.Format("# Intersections want Lights : {0}", NumberOfRoadIntersectinsWhichWantTrafficLights),
                };
                return string.Join("\n", os);
            }

            public void DrawGuiTable()
            {
//                DrawLine("Used Nodes", NumberOfUsedNodes);
//                DrawLine("Road Nodes", NumberOfRoadNodes);
                DrawLine("Intersections", NumberOfRoadIntersections);
                DrawLine("Intersections w/ Lights", NumberOfRoadIntersectionsWithTrafficLights);
                DrawLine("Intersections w/out Lights", NumberOfRoadIntersectionsWithoutTrafficLights);
                DrawLine("Intersections with Lights by default", NumberOfRoadIntersectinsWhichWantTrafficLights);
            }

            public static void DrawLine(string title, int value)
            {
                //default font is not monospace
                const float size = 40f;

                GUILayout.BeginHorizontal();
                GUILayout.Space(7f);
                GUILayout.Label(title);
                GUILayout.FlexibleSpace();
                GUILayout.Label(":");
                GUILayout.Label(value.ToString(CultureInfo.InvariantCulture), GUILayout.Width(size));
                GUILayout.EndHorizontal();
            }

            public static Statistics Collect()
            {
                var stats = new Statistics();

                var netManager = Singleton<NetManager>.instance;
                for (ushort i = 0; i < netManager.m_nodes.m_size; i++)
                {
                    var node = netManager.m_nodes.m_buffer[i];

                    if (node.m_flags == NetNode.Flags.None)
                    {
                        continue;
                    }
                    stats.NumberOfUsedNodes++;
                    
                    if (!ToggleTrafficLightsTool.IsValidRoadNode(node))
                    {
                        continue;
                    }
                    stats.NumberOfRoadNodes++;

                    if ((node.m_flags & NetNode.Flags.Junction) != NetNode.Flags.Junction)
                    {
                        continue;
                    }
                    stats.NumberOfRoadIntersections++;
                    
                    if (ToggleTrafficLightsTool.WantTrafficLights(i, node))
                    {
                        stats.NumberOfRoadIntersectinsWhichWantTrafficLights++;
                    }

                    var hasLights = ToggleTrafficLightsTool.HasTrafficLights(node.m_flags);
                    if (hasLights)
                    {
                        stats.NumberOfRoadIntersectionsWithTrafficLights++;
                    }
                    else
                    {
                        stats.NumberOfRoadIntersectionsWithoutTrafficLights++;
                    }
                }

                return stats;
            }
        }

        public class ChangedStatistics
        {
            public string Action = "";
            public int NumberOfChanges = 0;
            public int NumberOfAddedLights = 0;
            public int NumberOfRemovedLights = 0;

            public void DrawGuiTable()
            {
                if (!string.IsNullOrEmpty(Action))
                {
                    GUILayout.Label(ApplyColor(string.Format("<b>{0}</b>", Action)));
                }
                GUILayout.Space(0.5f);
                DrawLine("Lights changes", NumberOfChanges);
                DrawLine("Lights added", NumberOfAddedLights);
                DrawLine("Lights removed", NumberOfRemovedLights);
            }

            public static void DrawLine(string title, int value)
            {
                //default font is not monospace
                const float size = 40f;

                GUILayout.BeginHorizontal();
                GUILayout.Space(7f);
                GUILayout.Label(ApplyColor(title));
                GUILayout.FlexibleSpace();
                GUILayout.Label(ApplyColor(":"));
                GUILayout.Label(ApplyColor(value.ToString(CultureInfo.InvariantCulture)), GUILayout.Width(size));
                GUILayout.EndHorizontal();
            }

            public static string ApplyColor(string text)
            {
                const string color = "grey";
                return string.Format("<color={0}>{1}</color>", color, text);
            }
        }
    }
}