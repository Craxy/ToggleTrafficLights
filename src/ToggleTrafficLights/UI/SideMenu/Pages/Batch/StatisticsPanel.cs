using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Tools;
using Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table.Extensions;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.SideMenu.Pages.Batch
{
    internal sealed class StatisticsPanel : UIPanel
    {
        #region settings
        public ITabSettings Settings { get; set; } = TabBase.DefaultSettings;
        public int UpdateStatisticsEveryNUpdates { get; set; } = 25;
        #endregion

        #region Overrides of UIComponent
        public override void Start()
        {
            base.Start();

            name = "StatisticsPanel";
            width = parent.width;

            Action<UILabel> setupHeader = lbl => lbl.TextScale(Settings.HeaderRowTextScale).Ignore();
            Action<UILabel> setupRow = lbl => lbl.TextScale(Settings.ContentRowTextScale).Ignore();

            var table = this.CreateTable()
                .AddHeaderRow("Statistics", setupHeader)
#if DEBUG
                .AddPropertyValueRow(name: "# used nodes", value: "1234567", separator: Settings.DefaultRowSeparator, indention: Settings.ContentRowIndentation, setup: setupRow)
                .AddPropertyValueRow(name: "# road nodes", value: "1234567", separator: Settings.DefaultRowSeparator, indention: Settings.ContentRowIndentation, setup: setupRow)
#endif
                .AddPropertyValueRow(name: "# intersections", value: "1234567", separator: Settings.DefaultRowSeparator, indention: Settings.ContentRowIndentation, setup: setupRow)
                .AddPropertyValueRow(name: "# intersections w/ lights", value: "1234567", separator: Settings.DefaultRowSeparator, indention: Settings.ContentRowIndentation, setup: setupRow)
                .AddPropertyValueRow(name: "# intersections w/out lights", value: "1234567", separator: Settings.DefaultRowSeparator, indention: Settings.ContentRowIndentation, setup: setupRow)
                .AddPropertyValueRow(name: "# intersections w/ lights by default", value: "1234567", separator: Settings.DefaultRowSeparator, indention: Settings.ContentRowIndentation, setup: setupRow)
                .AddPropertyValueRow(name: "# intersections w/out lights by default", value: "1234567", separator: Settings.DefaultRowSeparator, indention: Settings.ContentRowIndentation, setup: setupRow)

                .SpreadVertical(Settings.VerticalSpaceBetweenLines)
                .SpreadHorizontal(Settings.IndentationBetweenColumns)
                .AlignEntriesInColumns(Settings.IndentationBetweenColumns, RowTag.SelectRowWithTag(RowTag.PropertyValue))
                .LimitWithToRootWidth(wrapTooLongLabels: false)
                .SpreadVertical(Settings.VerticalSpaceBetweenLines)

                .UpdateHeightOfRootToRowsHeight()

//                .DebugLog(nameof(StatisticsPanel))
                ;

            _statisticsLabels = table.Rows.Skip(1).Select(r => r.Entries.Last().Component).Cast<UILabel>().ToArray();
        }

        public override void OnEnable()
        {
            base.OnEnable();

            //Force update
            _updateStatisticsCounter = UpdateStatisticsEveryNUpdates;
        }
        #endregion

        #region update statistics
        private int _updateStatisticsCounter = 0;
        public override void Update()
        {
            base.Update();

            if (_updateStatisticsCounter++ >= UpdateStatisticsEveryNUpdates)
            {
                UpdateStatistics();
                UpdateStatisticsGui();

                _updateStatisticsCounter = 0;
            }
        }

        private IList<UILabel> _statisticsLabels; 
        public void UpdateStatisticsGui()
        {
            int i = 0;
            _statisticsLabels[i++].text = NumberOfUsedNodes.ToString();
            _statisticsLabels[i++].text = NumberOfRoadNodes.ToString();
            _statisticsLabels[i++].text = NumberOfRoadIntersections.ToString();
            _statisticsLabels[i++].text = NumberOfRoadIntersectionsWithTrafficLights.ToString();
            _statisticsLabels[i++].text = NumberOfRoadIntersectionsWithoutTrafficLights.ToString();
            _statisticsLabels[i++].text = NumberOfRoadIntersectionsWhichWantTrafficLights.ToString();
            _statisticsLabels[i++].text = NumberOfRoadIntersectionsWhichDontWantTrafficLights.ToString();
        }
        #endregion

        #region statistics
        public int NumberOfUsedNodes = 0;
        public int NumberOfRoadNodes = 0;
        public int NumberOfRoadIntersections = 0;
        public int NumberOfRoadIntersectionsWithTrafficLights = 0;
        public int NumberOfRoadIntersectionsWithoutTrafficLights = 0;
        public int NumberOfRoadIntersectionsWhichWantTrafficLights = 0;
        public int NumberOfRoadIntersectionsWhichDontWantTrafficLights = 0;

        public void ResetStatistics()
        {
            NumberOfUsedNodes = 0;
            NumberOfRoadNodes = 0;
            NumberOfRoadIntersections = 0;
            NumberOfRoadIntersectionsWithTrafficLights = 0;
            NumberOfRoadIntersectionsWithoutTrafficLights = 0;
            NumberOfRoadIntersectionsWhichWantTrafficLights = 0;
            NumberOfRoadIntersectionsWhichDontWantTrafficLights = 0;
        }

        public void UpdateStatistics()
        {
            ResetStatistics();

            var netManager = Singleton<NetManager>.instance;
            for (ushort i = 0; i < netManager.m_nodes.m_size; i++)
            {
                var node = netManager.m_nodes.m_buffer[i];

                if (node.m_flags == NetNode.Flags.None)
                {
                    continue;
                }
                NumberOfUsedNodes++;

                if (!ToggleTrafficLightsTool.IsValidRoadNode(node))
                {
                    continue;
                }
                NumberOfRoadNodes++;

                if ((node.m_flags & NetNode.Flags.Junction) != NetNode.Flags.Junction)
                {
                    continue;
                }
                NumberOfRoadIntersections++;

                if (ToggleTrafficLightsTool.WantTrafficLights(i, node))
                {
                    NumberOfRoadIntersectionsWhichWantTrafficLights++;
                }
                else
                {
                    NumberOfRoadIntersectionsWhichDontWantTrafficLights++;
                }

                var hasLights = ToggleTrafficLightsTool.HasTrafficLights(node.m_flags);
                if (hasLights)
                {
                    NumberOfRoadIntersectionsWithTrafficLights++;
                }
                else
                {
                    NumberOfRoadIntersectionsWithoutTrafficLights++;
                }
            }
        }
        #endregion

    }
}