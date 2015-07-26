using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.StateMachine.States
{
    public class HiddenToActivatedState : StateBase
    {
        #region Overrides of StateBase

        public override State State
        {
            get { return State.HiddenToActivated; }
        }

        public override void OnEnable()
        {
            base.OnEnable();

            OpenRoadPanel();
        }

        public override void OnDisable()
        {
            base.OnDisable();
        }

        public override void Update()
        {
            base.Update();

            OpenRoadPanel();
        }

        public override Command? CheckCommand()
        {
            if (RoadsPanel != null && RoadsPanel.isVisible)
            {
                return Command.DisplayRoadsPanel;
            }

            return null;
        }

        #endregion

        private void OpenRoadPanel()
        {
            if (RoadsPanel != null && !RoadsPanel.isVisible)
            {
                CitiesHelper.ClickOnRoadsButton();
            }
        }
    }
}