using System.Collections;
using System.Globalization;
using ColossalFramework;
using Craxy.CitiesSkylines.ToggleTrafficLights.Tools;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.Options
{
    public sealed class BatchUi : MonoBehaviour
    {
        #region fields

        private const int UpdateStatisticsEveryNthUpdate = 30;
        private int _updateStatisticsCounter = 0;
        private Statistics _statistics = null;

        private const int ShowChangesForNUpdates = 300;
        private int _updateChangedStatisticsCounter = 0;
        private ChangedStatistics _changedStatistics = null;
        #endregion

        #region MonoBehaviour

        [UsedImplicitly]
        private void Awake()
        {
            DebugLog.Info("BatchUi: Awake");
        }

        [UsedImplicitly]
        private void Start()
        {
            DebugLog.Info("BatchUi: Start");
        }

        [UsedImplicitly]
        private void OnDestroy()
        {
            DebugLog.Info("BatchUi: OnDestroy");
        }

        [UsedImplicitly]
        private void OnEnable()
        {
            DebugLog.Info("BatchUi: OnEnable");

            _updateStatisticsCounter = 0;
            _updateChangedStatisticsCounter = 0;
            _changedStatistics = null;
            _statistics = null;
        }

        [UsedImplicitly]
        private void OnDisable()
        {
            DebugLog.Info("BatchUi: OnDisable");

            _updateStatisticsCounter = 0;
            _updateChangedStatisticsCounter = 0;
            _changedStatistics = null;
            _statistics = null;
        }


        [UsedImplicitly]
        private void OnGUI()
        {

            const float left = 0f;
            const float top = 50f;
            const float width = 225f;
            const float height = 510f;
            const float padding = 5f;


            GUILayout.BeginArea(new Rect(left, top, width, height));

            var bgTexture = new Texture2D(1, 1);
            bgTexture.SetPixel(0, 0, new Color(0.321f, 0.321f, 0.321f, 1.0f));
            bgTexture.Apply();
            GUI.Box(new Rect(0f, 0f, width, height), bgTexture);

            {
                GUILayout.BeginArea(new Rect(padding, padding, width - 2*padding, height - 2*padding));

                GUILayout.Label("<size=15><b>Toggle Traffic Lights tool</b></size>");

                GUILayout.Space(10f);

                GUILayout.Label("<b>Usage</b>:");
                GUILayout.Label("  Left Click : Toggle Traffic Lights");
                GUILayout.Label("  Right Click: Reset to default");

                GUILayout.Space(10f);

                GUILayout.Label("<b>Statistics</b>:");
                GUILayout.Label("Number of");
                if (_statistics == null || _updateStatisticsCounter++ >= UpdateStatisticsEveryNthUpdate)
                {
                    _statistics = Statistics.Collect();
                    _updateStatisticsCounter = 0;
                }
                _statistics.DrawGuiTable();

                GUILayout.Space(10f);

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

                GUILayout.Space(5f);
                if (_changedStatistics != null && _updateChangedStatisticsCounter > 0)
                {
                    _changedStatistics.DrawGuiTable();
                    _updateChangedStatisticsCounter--;
                }

#if DEBUG
                if (GUILayout.Button("Test Traffic Lights"))
                {
                    TestChangingTrafficLights(50);
                }
#endif

                GUILayout.EndArea();
            }

            GUILayout.EndArea();
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