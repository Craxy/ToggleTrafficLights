using System;
using System.Linq;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Game;
using Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table;
using Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table.Entries;
using Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table.Extensions;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.SideMenu.Pages
{
    public class UsageTab : TabBase
    {
        #region Overrides of UIComponent

        public override void Start()
        {
            base.Start();

            name = "UsageTab";

            Action<UILabel> setupHeader = lbl => lbl.TextScale(Settings.HeaderRowTextScale).Ignore(); 
            Action<UILabel> setupRow = lbl => lbl.TextScale(Settings.ContentRowTextScale).Ignore();

            this.CreateTable()
                .AddHeaderRow("Button in roads menu", setupHeader)
                .AddPropertyValueRow(name: "Left Click", value: "Activate TTL tool", separator: Settings.DefaultRowSeparator, indention: Settings.ContentRowIndentation, setup: setupRow)
                .AddPropertyValueRow(name: "Right Click", value: "Activate TTL tool and display this menu", separator: Settings.DefaultRowSeparator, indention: Settings.ContentRowIndentation, setup: setupRow)
                .AddVerticalSpace(Settings.VerticalSpaceAfterGroup)
                .AddHeaderRow("Toggle intersections", setupHeader)
                .AddPropertyValueRow(name: "Left Click", value: "Toggle traffic lights", separator: Settings.DefaultRowSeparator, indention: Settings.ContentRowIndentation, setup: setupRow)
                .AddPropertyValueRow(name: "Right Click", value: "Reset to default", separator: Settings.DefaultRowSeparator, indention: Settings.ContentRowIndentation, setup: setupRow)
                .AddVerticalSpace(Settings.VerticalSpaceAfterGroup)
                .AddHeaderRow("Shortcuts", setupHeader)
                .AddPropertyValueRow(name: "Ctrl+T", value: "(De)Activate TTL (acts like clicking on TTL button)", separator: Settings.DefaultRowSeparator, indention: Settings.ContentRowIndentation, setup: setupRow)
                .AddPropertyValueRow(name: "Ctrl+Shift+T", value: "(De)Activate TTL without opening the Roads Menu", separator: Settings.DefaultRowSeparator, indention: Settings.ContentRowIndentation, setup: setupRow)
                .AddPropertyValueRow(name: Options.InputKeys.ElevationUp.ToString(), value: "Toggle only Overground", separator: Settings.DefaultRowSeparator, indention: Settings.ContentRowIndentation, setup: setupRow)
                .AddPropertyValueRow(name: Options.InputKeys.ElevationDown.ToString(), value: "Toggle only Underground", separator: Settings.DefaultRowSeparator, indention: Settings.ContentRowIndentation, setup: setupRow)
                .AddPropertyValueRow(name: $"{Options.InputKeys.ElevationDown}+{Options.InputKeys.ElevationUp}", value: "Toggle both Overground and Underground", separator: Settings.DefaultRowSeparator, indention: Settings.ContentRowIndentation, setup: setupRow)
                .AddVerticalSpace(Settings.VerticalSpaceAfterGroup * 15.0f)
                .AddSingleStringRow("A detailed description is available on the GitHub page of this mod: https://github.com/Craxy/ToggleTrafficLights", setupRow)
                                    
                .SpreadVertical(Settings.VerticalSpaceBetweenLines)
                .SpreadHorizontal(Settings.IndentationBetweenColumns)
                .AlignEntriesInColumns(Settings.IndentationBetweenColumns, RowTag.SelectRowWithTag(RowTag.PropertyValue))
                .LimitWithToRootWidth(wrapTooLongLabels: true)
                .SpreadVertical(Settings.VerticalSpaceBetweenLines)

//                .DebugLog(nameof(UsageTab))
                ;
        }

        #endregion
    }

}