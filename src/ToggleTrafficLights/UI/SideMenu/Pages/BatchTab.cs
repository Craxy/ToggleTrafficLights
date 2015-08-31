using System;
using System.Collections.Generic;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components;
using Craxy.CitiesSkylines.ToggleTrafficLights.UI.SideMenu.Pages.Batch;

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

            var rows = new List<Controls.Row>
            {
                this.AddPanel<BatchCommandsPanel>(),
                this.AddVerticalSpace(50.0f),
                this.AddPanel<Batch.StatisticsPanel>(),
            };
            rows.SpreadVertical(Settings)
                .SpreadHorizontal(Settings);
//            rows.OfType<Controls.StringRow>().Cast<Controls.Row>().ToList().AlignColumns(Settings).IndentRows(Settings);
            rows.LimitLastComponentsWidthToParent(this, Settings)
                .SpreadVertical(Settings);
        }

        #endregion
    }
}