using System;
using System.Collections.Generic;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components;
using Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table;
using Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table.Extensions;
using Craxy.CitiesSkylines.ToggleTrafficLights.UI.SideMenu.Pages.Batch;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.SideMenu.Pages
{
    public class BatchTab : TabBase
    {
        #region Overrides of TabBase

        public override void Start()
        {
            base.Start();

            name = "BatchTab";

            Action<UILabel> setupHeader = lbl => lbl.TextScale(Settings.HeaderRowTextScale).Ignore();
            Action<UILabel> setupRow = lbl => lbl.TextScale(Settings.ContentRowTextScale).Ignore();

            this.CreateTable()
                .AddCustomPanelRow<BatchCommandsPanel>()
                .AddVerticalSpace(50.0f)
                .AddCustomPanelRow<Batch.StatisticsPanel>()

                .SpreadHorizontal(Settings.IndentationBetweenColumns)
                .SpreadVertical(Settings.VerticalSpaceBetweenLines)

//                .DebugLog(nameof(BatchTab))
                ;
        }

        #endregion
    }
}