using System;
using System.Collections.Generic;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Game;
using Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components;
using Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table;
using Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table.Extensions;
using Craxy.CitiesSkylines.ToggleTrafficLights.UI.SideMenu.Pages.Batch;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.SideMenu.Pages
{
    public class OptionsTab : TabBase
    {
        #region Overrides of TabBase

        public override void Start()
        {
            base.Start();

            name = "OptionsTab";

            Action<UILabel> setupHeader = lbl => lbl.TextScale(Settings.HeaderRowTextScale).Ignore();
            var txtScale = 0.90f;
            Action<UILabel> setupText = lbl => lbl.TextScale(txtScale).Ignore();

            //todo: hex labels get sometimes cut: hex changes to something longer

            var table = this.CreateTable()
                .AddHeaderRow("Mouse over: highlighting of current intersection", setupHeader)
                .AddColorFieldRow("with lights", 
                        initialColor: Options.ToggleTrafficLightsTool.HasTrafficLightsColor.Value, 
                        onColorChanged: c => Options.ToggleTrafficLightsTool.HasTrafficLightsColor.Value = c,
                        notifyColorChanged: action => Options.ToggleTrafficLightsTool.HasTrafficLightsColor.ValueChanged += (_, c) => action(c),
                        separator: Settings.DefaultRowSeparator,
                        indention: Settings.ContentRowIndentation,
                        setupText: setupText
                        )
                .AddColorFieldRow("without lights", 
                        initialColor: Options.ToggleTrafficLightsTool.HasNoTrafficLightsColor.Value, 
                        onColorChanged: c => Options.ToggleTrafficLightsTool.HasNoTrafficLightsColor.Value = c,
                        notifyColorChanged: action => Options.ToggleTrafficLightsTool.HasNoTrafficLightsColor.ValueChanged += (_, c) => action(c),
                        separator: Settings.DefaultRowSeparator,
                        indention: Settings.ContentRowIndentation,
                        setupText: setupText
                        )
                .AddVerticalSpace(5.0f)
                .AddHeaderRow("Intersections: highlighting of all traffic lights", setupHeader)
                .AddDropDownRow("intersections to highlight", 
                    values: Enum.GetNames(typeof(Options.GroundMode)),
                    selectedIndex: Array.IndexOf(((Options.GroundMode[])Enum.GetValues(typeof(Options.GroundMode))), Options.HighlightIntersections.IntersectionsToHighlight.Value),
                    onSelectedIndexChanged: idx => Options.HighlightIntersections.IntersectionsToHighlight.Value = ((Options.GroundMode[])Enum.GetValues(typeof(Options.GroundMode)))[idx],
                    separator: Settings.DefaultRowSeparator,
                    indention: Settings.ContentRowIndentation,
                    setupText: setupText,
                    setupDropDown: cb => cb.TextScale(txtScale).Width(120f).Height(19.0f),
                    setupDropDownButton: btn => btn.TextScale(txtScale)
                    )
                .AddColorFieldRow("with lights",
                        initialColor: Options.HighlightIntersections.HasTrafficLightsColor.Value,
                        onColorChanged: c => Options.HighlightIntersections.HasTrafficLightsColor.Value = c,
                        notifyColorChanged: action => Options.HighlightIntersections.HasTrafficLightsColor.ValueChanged += (_, c) => action(c),
                        separator: Settings.DefaultRowSeparator,
                        indention: Settings.ContentRowIndentation,
                        setupText: setupText
                        )
                .AddColorFieldRow("without lights",
                        initialColor: Options.HighlightIntersections.HasNoTrafficLightsColor.Value,
                        onColorChanged: c => Options.HighlightIntersections.HasNoTrafficLightsColor.Value = c,
                        notifyColorChanged: action => Options.HighlightIntersections.HasNoTrafficLightsColor.ValueChanged += (_, c) => action(c),
                        separator: Settings.DefaultRowSeparator,
                        indention: Settings.ContentRowIndentation,
                        setupText: setupText
                        )

                .SpreadVertical(Settings.VerticalSpaceBetweenLines)
                .SpreadHorizontal(Settings.IndentationBetweenColumns)
                .LimitWithToRootWidth(false)
                .SpreadVertical(Settings.VerticalSpaceBetweenLines)
                .SpreadHorizontal(Settings.IndentationBetweenColumns)
                .AlignEntriesInPropertyValueColumns(Settings.IndentationBetweenColumns, r => r.NumberOfColumns > 1)
                .LimitWithToRootWidth(wrapTooLongLabels: true)
                .SpreadVertical(Settings.VerticalSpaceBetweenLines)
                ;


//            var rows = new List<Controls.Row>
//            {
//                this.AddHeader("Mouse over: highlighting of current intersection", setupHeader),
//                this.AddColorPanel("with lights:", Options.ToggleTrafficLightsTool.HasTrafficLightsColor.Value, cp => 
//                {
//                    cp.height = 25.0f;
//                    cp.width = width;
//                    cp.ColorChanged += (_, args) => Options.ToggleTrafficLightsTool.HasTrafficLightsColor.Value = args.Value;
//                    Options.ToggleTrafficLightsTool.HasTrafficLightsColor.ValueChanged += (_, c) => cp.Color = c;
//                }),
//                this.AddColorPanel("without lights:", Options.ToggleTrafficLightsTool.HasTrafficLightsColor.Value, cp => 
//                {
//                    cp.height = 25.0f;
//                    cp.width = width;
//                    cp.ColorChanged += (_, args) => Options.ToggleTrafficLightsTool.HasTrafficLightsColor.Value = args.Value;
//                    Options.ToggleTrafficLightsTool.HasTrafficLightsColor.ValueChanged += (_, c) => cp.Color = c;
//                }),
//            };
//            rows.SpreadVertical(Settings)
//                .SpreadHorizontal(Settings);
//            //            rows.OfType<Controls.StringRow>().Cast<Controls.Row>().ToList().AlignColumns(Settings).IndentRows(Settings);
//            rows.LimitLastComponentsWidthToParent(this, Settings)
//                .SpreadVertical(Settings);
        }

        #endregion

    }
}