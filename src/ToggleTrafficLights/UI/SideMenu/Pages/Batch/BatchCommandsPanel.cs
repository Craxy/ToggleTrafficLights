﻿using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Game;
using Craxy.CitiesSkylines.ToggleTrafficLights.Tools;
using Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table;
using Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table.Extensions;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.SideMenu.Pages.Batch
{
    internal sealed class BatchCommandsPanel : UIPanel
    {
        #region settings
        public ITabSettings Settings { get; set; } = TabBase.DefaultSettings;
        public float HideChangedStatisticsAfterNSeconds { get; set; } = 5.0f;
        #endregion

        #region Overrides of UIComponent

        public override void Start()
        {
            base.Start();

            name = "BatchCommandsPanel";
            width = parent.width;

            Action<UILabel> setupHeader = lbl => lbl.TextScale(Settings.HeaderRowTextScale).Ignore();
            var buttonPadding = Settings.ContentRowIndentation*2.0f;
            Action<UIButton> setupButton = btn => btn.TextScale(0.9f).Width(width - buttonPadding * 2.0f).Height(32.0f).HoveredTextColor(new Color32(7,132,255,255)).NormalBgSprite("ButtonMenu").Ignore();
            var statisticsColor = new Color32(150, 150, 150, 255);
            Action<UILabel> setupStatisticsHeader = lbl => lbl.TextScale(Settings.HeaderRowTextScale).TextColor(statisticsColor).Ignore();
            Action<UILabel> setupStatisticsRow = lbl => lbl.TextScale(Settings.ContentRowTextScale).TextColor(statisticsColor).Ignore();


            var statisticsTable = this.CreateTable()
                .AddHeaderRow("Action", setupStatisticsHeader)
                .AddPropertyValueRow(name: "Lights changed", value: "1234", separator: Settings.DefaultRowSeparator, indention: Settings.ContentRowIndentation, setup: setupStatisticsRow)
                .AddPropertyValueRow(name: "Lights added", value: "1234", separator: Settings.DefaultRowSeparator, indention: Settings.ContentRowIndentation, setup: setupStatisticsRow)
                .AddPropertyValueRow(name: "Lights removed", value: "1234", separator: Settings.DefaultRowSeparator, indention: Settings.ContentRowIndentation, setup: setupStatisticsRow)

                .IndentAll(Settings.ContentRowIndentation * 7.0f)

                .SpreadVertical(Settings.VerticalSpaceBetweenLines)
                .SpreadHorizontal(Settings.IndentationBetweenColumns)
                .AlignEntriesInColumns(Settings.IndentationBetweenColumns, RowTag.SelectRowWithTag(RowTag.PropertyValue))
                .LimitWithToRootWidth(wrapTooLongLabels: false) //action gets changed and should not wrap
                .SpreadVertical(Settings.VerticalSpaceBetweenLines)

                ;

            var table = this.CreateTable()
                .AddHeaderRow("Commands", setupHeader)
                .AddRow(row => row.AppendHorizontalSpace(buttonPadding).AppendButton("Remove all traffic lights", RemoveAllTrafficLights, setupButton))
                .AddRow(row => row.AppendHorizontalSpace(buttonPadding).AppendButton("Add all traffic lights", AddAllTrafficLights, setupButton))
                .AddRow(row => row.AppendHorizontalSpace(buttonPadding).AppendButton("Reset all to default", ResetAllTrafficLights, setupButton))
                .AddVerticalSpace(Settings.VerticalSpaceBetweenLines * 2.5f)

                .SpreadVertical(Settings.VerticalSpaceBetweenLines)
                .SpreadHorizontal(Settings.IndentationBetweenColumns)
                .LimitWithToRootWidth(wrapTooLongLabels: true)

                .Concat(statisticsTable)
                .SpreadVertical(Settings.IndentationBetweenColumns)

                .UpdateHeightOfRootToRowsHeight()

//                .DebugLog(nameof(BatchCommandsPanel))
                ;


            _statisticsRows = statisticsTable.Rows;
            HideChangedStatistics();
        }

        public override void Update()
        {
            base.Update();

            // hide update statistics after some time
            if (_isChangedStatisticsShown)
            {
                _hideChangedStatisticsIn -= Time.deltaTime;

                if (_hideChangedStatisticsIn <= 0)
                {
                    HideChangedStatistics();
                }
            }
        }

        #region Overrides of UIComponent

        protected override void OnVisibilityChanged()
        {
            base.OnVisibilityChanged();

            HideChangedStatistics();
        }

        #endregion

        #endregion



        #region commands
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

            SetChangedStatistics($"Traffic lights testes. Iterations: {iterations}", totalChanges);
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

            if (changes.NumberOfChanges > 0)
            {
                Options.HighlightIntersections.RequestRecalculateColorForAllIntersections();
            }

            return changes;
        }
        #endregion

        #region statistics

        private bool _isChangedStatisticsShown = false;
        private float _hideChangedStatisticsIn = 0;

        private IList<Row> _statisticsRows;
        private void UpdateChangedStatistics(ChangedStatistics stats)
        {
            var i = 0;
            foreach (var s in new[] { stats.Action, stats.NumberOfChanges.ToString(), stats.NumberOfAddedLights.ToString(), stats.NumberOfRemovedLights.ToString() })
            {
                ((UILabel) _statisticsRows[i++].Entries.Last().Component).text = s;
            }
            ShowChangedStatistics();
        }

        private void SetChangedStatistics(string action, ChangedStatistics stats)
        {
            stats.Action = action;
            UpdateChangedStatistics(stats);
        }

        public void ShowChangedStatistics() => ChangeVisibilityOfChangedStatistics(true);
        public void HideChangedStatistics() => ChangeVisibilityOfChangedStatistics(false);
        public void ChangeVisibilityOfChangedStatistics(bool visible)
        {
            foreach (var row in _statisticsRows)
            {
                foreach (var e in row.Entries)
                {
                    e.Component.isVisible = visible;
                }
            }

            _isChangedStatisticsShown = visible;
            if (_isChangedStatisticsShown)
            {
                _hideChangedStatisticsIn = HideChangedStatisticsAfterNSeconds;
            }
            else
            {
                _hideChangedStatisticsIn = float.MinValue;
            }
        }



        public class ChangedStatistics
        {
            public string Action = string.Empty;
            public int NumberOfChanges = 0;
            public int NumberOfAddedLights = 0;
            public int NumberOfRemovedLights = 0;
        }

        #endregion
    }
}