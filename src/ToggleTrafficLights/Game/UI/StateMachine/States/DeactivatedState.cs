namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.StateMachine.States
{
    public class Deactivated : ButtonStateBase
    {
        #region Overrides of ButtonStateBase
        protected override string ButtonName
        {
            get { return ButtonBaseName + "Deactivated"; }
        }
        #endregion

        #region Overrides of StateBase
        public override State State
        {
            get { return State.Deactivated; }
        }

        public override void OnEnable()
        {
            base.OnEnable();
        }

        public override void OnDisable()
        {
            base.OnDisable();
        }

        public override void Update()
        {
            base.Update();
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