using System;
using System.Linq;
using ColossalFramework;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.Menu;
using Craxy.CitiesSkylines.ToggleTrafficLights.Tools.Visualization;
using Object = UnityEngine.Object;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.StateMachine.States
{
    public class ActivatedUiState : ActivatedState
    {
        #region fields
        private BatchUi _ui = null;

        public ActivatedUiState(IntersectionHighlighting intersectionHighlighting)
            : base(intersectionHighlighting)
        {
        }

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
                _ui = toolControl.GetComponent<BatchUi>() ?? toolControl.gameObject.AddComponent<BatchUi>();
            }
            _ui.IntersectionHightlighting = IntersectionHighlighting;
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
            }

            base.OnExit();
        }

        #region Overrides of ActivatedState

        public override void Destroy()
        {
            if (_ui != null)
            {
                _ui.enabled = false;
                Object.Destroy(_ui.gameObject);
            }
            _ui = null;

            base.Destroy();
        }

        #endregion

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

        #region Overrides of ActivatedState
        protected override string ButtonName
        {
            get { return ButtonBaseName + "ActivatedUi"; }
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