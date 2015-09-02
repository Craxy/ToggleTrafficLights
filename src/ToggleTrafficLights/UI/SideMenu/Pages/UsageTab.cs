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

            try
            {
                var table = this.CreateTable()
                    .AddSingleStringRow("Button in roads menu", setupHeader)
                    .AddPropertyValueRow("Left Click", "Activate TTL tool", Settings.DefaultRowSeparator, setupRow)
                    .AddPropertyValueRow("Right Click", "Activate TTL tool and display this menu", Settings.DefaultRowSeparator, setupRow)
                    .AddVerticalSpace(Settings.VerticalSpaceAfterGroup)
                    .AddSingleStringRow("Toggle intersections", setupHeader)
                    .AddPropertyValueRow("Left Click", "Toggle traffic lights", Settings.DefaultRowSeparator, setupRow)
                    .AddPropertyValueRow("Right Click", "Reset to default", Settings.DefaultRowSeparator, setupRow)
                    .AddVerticalSpace(Settings.VerticalSpaceAfterGroup)
                    .AddSingleStringRow("Shortcuts", setupHeader)
                    .AddPropertyValueRow("Ctrl+T", "(De)Activate TTL (acts like clicking on TTL button)", Settings.DefaultRowSeparator, setupRow)
                    .AddPropertyValueRow("Ctrl+Shift+T", "(De)Activate TTL without opening the Roads Menu", Settings.DefaultRowSeparator, setupRow)
                    .AddPropertyValueRow(Options.InputKeys.ElevationUp.ToString(), "Toggle only Overground", Settings.DefaultRowSeparator, setupRow)
                    .AddPropertyValueRow(Options.InputKeys.ElevationDown.ToString(), "Toggle only Underground", Settings.DefaultRowSeparator, setupRow)
                    .AddPropertyValueRow($"{Options.InputKeys.ElevationDown}+{Options.InputKeys.ElevationUp}", "Toggle both Overground and Underground", Settings.DefaultRowSeparator, setupRow)
                    .AddVerticalSpace(Settings.VerticalSpaceAfterGroup * 10.0f)
                    .AddSingleStringRow("A detailed description is available on the GitHub page of this mod: https://github.com/Craxy/ToggleTrafficLights", setupRow)

                    .SpreadVertical(Settings.VerticalSpaceBetweenLines)
                    .SpreadHorizontal(Settings.IndentationBetweenColumns)
                    .LimitWithToRootWidth(wrapTooLongLabels: true)
                    .SpreadVertical(Settings.VerticalSpaceBetweenLines)
                    ;


                //                var table = this.CreateTable()
                //                    .AddSingleStringRow("Button in roads menu", setupHeader)
                //                    .AddPropertyValueRow("Left Click", "Activate TTL tool", Settings.DefaultRowSeparator, setupRow)
                //                    .AddPropertyValueRow("Right Click", "Activate TTL tool and display this menu", Settings.DefaultRowSeparator, setupRow)
                //                    .AddVerticalSpace(Settings.VerticalSpaceAfterGroup)
                //                    .AddSingleStringRow("Toggle intersections", setupHeader)
                //                    .AddPropertyValueRow("Left Click", "Toggle traffic lights", Settings.DefaultRowSeparator, setupRow)
                //                    .AddPropertyValueRow("Right Click", "Reset to default", Settings.DefaultRowSeparator, setupRow)
                //                    .AddVerticalSpace(Settings.VerticalSpaceAfterGroup)
                //                    .AddSingleStringRow("Shortcuts", setupHeader)
                //                    .AddPropertyValueRow("Ctrl+T", "(De)Activate TTL (acts like clicking on TTL button)", Settings.DefaultRowSeparator, setupRow)
                //                    .AddPropertyValueRow("Ctrl+Shift+T", "(De)Activate TTL without opening the Roads Menu", Settings.DefaultRowSeparator, setupRow)
                //                    .AddPropertyValueRow(Options.InputKeys.ElevationUp.ToString(), "Toggle only Overground", Settings.DefaultRowSeparator, setupRow)
                //                    .AddPropertyValueRow(Options.InputKeys.ElevationDown.ToString(), "Toggle only Underground", Settings.DefaultRowSeparator, setupRow)
                //                    .AddPropertyValueRow($"{Options.InputKeys.ElevationDown}+{Options.InputKeys.ElevationUp}", "Toggle both Overground and Underground", Settings.DefaultRowSeparator, setupRow)
                //                    .AddVerticalSpace(Settings.VerticalSpaceAfterGroup * 10.0f)
                //                    .AddSingleStringRow("A detailed description is available on the GitHub page of this mod: https://github.com/Craxy/ToggleTrafficLights", setupRow)
                //
                //                    .SpreadVertical(Settings.VerticalSpaceBetweenLines)
                //                    .SpreadHorizontal(Settings.IndentationBetweenColumns)
                //                    .AlignEntriesInColumns(Settings.IndentationBetweenColumns, row => row.NumberOfColumns == 3 && row.Entries.OfType<LabelEntry>().Count() == row.NumberOfColumns)
                //                    .LimitWithToRootWidth(wrapTooLongLabels: true)
                //                    .SpreadVertical(Settings.VerticalSpaceBetweenLines)
                //                    ;
            }
            catch (Exception e)
            {
                DebugLog.Error(e.ToString());
            }

            //            var rows = new List<Controls.Row>
            //            {
            //                this.AddHeader("Menu", setupHeader),
            //                this.AddStringRow("Left Click on button".And(Settings.DefaultRowSeparator).And("Activate TTL tool"), setupRow),
            //                this.AddStringRow("Right Click on button".And(Settings.DefaultRowSeparator).And("Activate TTL tool and display this menu"),
            //                    setupRow),
            //                this.AddVerticalSpace(Settings.VerticalSpaceAfterGroup),
            //                this.AddHeader("Toggle intersections", setupHeader),
            //                this.AddStringRow("Left Click".And(Settings.DefaultRowSeparator).And("Toggle Traffic Lights"), setupRow),
            //                this.AddStringRow("Right Click".And(Settings.DefaultRowSeparator).And("Reset to default"), setupRow),
            //                this.AddVerticalSpace(Settings.VerticalSpaceAfterGroup),
            //                this.AddHeader("Shortcuts", setupHeader),
            //                this.AddStringRow("Ctrl+T".And(Settings.DefaultRowSeparator).And("(De)Activate TTL (acts like clicking on TTL button)"),
            //                    setupRow),
            //                this.AddStringRow("Ctrl+Shift+T".And(Settings.DefaultRowSeparator).And("(De)Activate TTL without opening the Roads Menu"),
            //                    setupRow),
            //                this.AddStringRow(Options.InputKeys.ElevationUp.ToString().And(Settings.DefaultRowSeparator).And("Only Overground"),
            //                    setupRow),
            //                this.AddStringRow(Options.InputKeys.ElevationDown.ToString().And(Settings.DefaultRowSeparator).And("Only Underground"),
            //                    setupRow),
            //                this.AddStringRow(
            //                    $"{Options.InputKeys.ElevationDown}+{Options.InputKeys.ElevationUp}".And(Settings.DefaultRowSeparator)
            //                        .And("Overground and Underground"), setupRow),
            //                this.AddVerticalSpace(Settings.VerticalSpaceAfterGroup*10.0f),
            //                this.AddHeader(
            //                    "A detailed description is available on the GitHub page of this mod: https://github.com/Craxy/ToggleTrafficLights",
            //                    setupRow),
            //            };
            //            rows.SpreadVertical(Settings).SpreadHorizontal(Settings);
            //            rows.OfType<Controls.StringRow>().Cast<Controls.Row>().ToList().AlignColumns(Settings).IndentRows(Settings.ContentRowIndentation, Settings);
            //            rows.LimitLastComponentsWidthToParent(this, Settings).SpreadVertical(Settings);

            //            rows.SpreadVertical().LimitLastComponentsWidthToParent(this).SpreadVertical();
        }

        #endregion
    }

}