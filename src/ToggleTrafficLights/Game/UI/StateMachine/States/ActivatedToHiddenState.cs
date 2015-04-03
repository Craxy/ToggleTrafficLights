using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.StateMachine.States
{
    public class ActivatedToHiddenState : StateBase
    {
        #region Overrides of StateBase

        public override State State
        {
            get { return State.ActivatedToHidden; }
        }

        public override void OnEntry()
        {
            base.OnEntry();

            CloseRoadPanel();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            CloseRoadPanel();
        }

        public override Command? CheckCommand()
        {
            if (RoadsPanel != null && !RoadsPanel.isVisible)
            {
                return Command.HideRoadsPanel;
            }

            return null;
        }

        #endregion

        private void CloseRoadPanel()
        {
            if (RoadsPanel != null && RoadsPanel.isVisible)
            {
                CitiesHelper.ClickOnRoadsButton();
            }
        }
 
    }
}