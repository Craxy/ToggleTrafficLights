using ColossalFramework.UI;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.StateMachine.States
{
    public class Deactivated : ButtonStateBase
    {

        #region Overrides of StateBase
        public override State State
        {
            get { return State.Deactivated; }
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
            if (RoadsPanel == null || !RoadsPanel.isVisible)
            {
                return Command.HideRoadsPanel;
            }

            if(KeyHelper.IsInvisibleToolKeyPressed())
            {
                return Command.PressInvisibleShortcut;
            }

            if(KeyHelper.IsToolKeyPressed())
            {
                return Command.PressShortcut;
            }

            if (LeftClick)
            {
                return Command.LeftClickOnToolButton;
            }
            if (RightClick)
            {
                return Command.RightClickToolButton;
            }

            return null;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (Button != null)
            {
                SetDeactivatedStateSprites(Button);
            }
        }
        #endregion
    }
}