using System;
using System.Collections.Generic;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Game;
using Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components;
using Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table;
using Craxy.CitiesSkylines.ToggleTrafficLights.UI.SideMenu.Pages.Batch;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.SideMenu.Pages
{
    public class OptionsTab : TabBase
    {
        #region Overrides of TabBase

        public override void Start()
        {
            base.Start();

            name = "OptionsTab";

//            Action<UILabel> setupHeader = lbl => lbl.TextScale(Settings.HeaderRowTextScale).Ignore();
//            Action<UILabel> setupRow = lbl => lbl.TextScale(Settings.ContentRowTextScale).Ignore();
//
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