using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.SideMenu.Pages
{
    public class BatchTab : TabBase
    {
        #region Overrides of TabBase

        public override void Start()
        {
            base.Start();

            name = "UsagePanel";

            Action<UILabel> setupHeader = lbl => lbl.TextScale(Settings.HeaderRowTextScale).Ignore();
            Action<UILabel> setupRow = lbl => lbl.TextScale(Settings.ContentRowTextScale).Ignore();

            var rows = new List<Controls.Row>();
            {
                this.AddHeader("Menu", setupHeader).AddTo(rows);
            }
            rows.SpreadVertical(Settings).SpreadHorizontal(Settings);
            rows.OfType<Controls.StringRow>().Cast<Controls.Row>().ToList().AlignColumns(Settings).IndentRows(Settings);
            rows.LimitLastComponentsWidthToParent(this, Settings).SpreadVertical(Settings);
        }

        #endregion
    }
}