using ColossalFramework.UI;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.StateMachine.States
{
    public class HiddenState : StateBase
    {
        #region Implementation of IState

        public override State State
        {
            get { return State.Hidden;}
        }

        public override void OnEntry()
        {
            base.OnEntry();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override Command? CheckCommand()
        {
            if (RoadsPanel != null && RoadsPanel.isVisible)
            {
                return Command.DisplayRoadsPanel;
            }

            if (KeyHelper.IsInvisibleToolKeyPressed())
            {
                return Command.PressInvisibleShortcut;
            }

            if (KeyHelper.IsToolKeyPressed())
            {
                return Command.PressShortcut;
            }

            return null;
        }

        #endregion
    }
}