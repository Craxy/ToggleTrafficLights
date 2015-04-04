using Craxy.CitiesSkylines.ToggleTrafficLights.Tools;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.StateMachine.States
{
    public class InvisibleActivatedState : StateBase
    {
        #region fields
        private ToggleTrafficLightsTool _tool = null;
        private ToolBase _previousTool = null;
        #endregion

        #region Overrides of StateBase

        public override State State
        {
            get { return State.InvisibleActivatedState; }
        }

        public override void OnEntry()
        {
            base.OnEntry();

            _previousTool = ToolsModifierControl.toolController.CurrentTool;
            _tool = ToolsModifierControl.toolController.gameObject.GetComponent<ToggleTrafficLightsTool>()
                    ?? ToolsModifierControl.toolController.gameObject.AddComponent<ToggleTrafficLightsTool>();
            ToolsModifierControl.toolController.CurrentTool = _tool;
        }

        public override void OnExit()
        {
            base.OnExit();

            if (ToolsModifierControl.toolController.CurrentTool == null || ToolsModifierControl.toolController.CurrentTool == _tool)
            {
                if (_previousTool != null)
                {
                    ToolsModifierControl.toolController.CurrentTool = _previousTool;
                }
                else
                {
                    ToolsModifierControl.SetTool<DefaultTool>();
                }
            }

            if (_tool != null)
            {
                UnityEngine.Object.Destroy(_tool);
            }
            _tool = null;
            _previousTool = null;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override Command? CheckCommand()
        {
            if (ToolsModifierControl.toolController.CurrentTool != _tool)
            {
                return Command.ActivateOtherTool;
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