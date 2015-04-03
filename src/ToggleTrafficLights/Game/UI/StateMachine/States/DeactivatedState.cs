using ColossalFramework.UI;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.StateMachine.States
{
    public class Deactivated : ButtonStateBase
    {
        #region fields

        private bool _buttonClicked = false;
        #endregion

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

            _buttonClicked = false;
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

            if(KeyHelper.IsToolKeyPressed())
            {
                return Command.PressShortcut;
            }

            if (_buttonClicked)
            {
                return Command.ClickToolButton;
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

        #region Overrides of ButtonStateBase

        protected override void OnButtonClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            base.OnButtonClicked(component, eventParam);

            _buttonClicked = true;
        }

        #endregion
    }
}