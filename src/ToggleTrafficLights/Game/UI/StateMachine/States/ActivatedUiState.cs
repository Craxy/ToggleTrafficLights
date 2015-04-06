using ColossalFramework;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.Options;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.StateMachine.States
{
    public class ActivatedUiState : ActivatedState
    {
        #region fields
        private BatchUi _ui = null;
        #endregion

        #region Overrides of StateBase

        public override State State
        {
            get { return State.ActivatedUiState; }
        }


        public override void OnEntry()
        {
            base.OnEntry();

            if (_ui == null)
            {
                //gets enabled automatically
                var toolControl = Singleton<ToolManager>.instance;
                _ui = toolControl.gameObject.AddComponent<BatchUi>();
            }
            _ui.enabled = true;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (Button != null)
            {
                SetActivedUiStateSprites(Button);
            }
        }

        public override void OnExit()
        {
            if (_ui != null)
            {
                _ui.enabled = false;
                Object.Destroy(_ui);
            }
            _ui = null;

            base.OnExit();
        }


        public override Command? CheckCommand()
        {
            var cmd = base.CheckCommand();

            if (cmd.HasValue && cmd.Value != Command.RightClickToolButton)
            {
                return cmd;
            }

            if (LeftClick)
            {
                return Command.LeftClickOnToolButton;
            }

            return null;
        }

        #endregion

        protected void SetActivedUiStateSprites(UIButton btn)
        {
            btn.normalFgSprite = "Selected";
            btn.disabledFgSprite = "Selected";
            btn.hoveredFgSprite = "Selected";
            btn.pressedFgSprite = "Selected";
            btn.focusedFgSprite = "Selected";

            btn.normalBgSprite = "OptionBaseFocusedRed";
            btn.disabledBgSprite = "OptionBaseFocusedRed";
            btn.hoveredBgSprite = "OptionBaseFocusedRed";
            btn.pressedBgSprite = "OptionBaseFocusedRed";
            btn.focusedBgSprite = "OptionBaseFocusedRed";
        }

    }
}