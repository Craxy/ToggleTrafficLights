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

        public override void OnEnable()
        {
            base.OnEnable();

            CloseRoadPanel();
        }

        public override void OnDisable()
        {
            base.OnDisable();
        }

        public override void Update()
        {
            base.Update();

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