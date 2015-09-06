using Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table.Extensions;
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