using System.Linq;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Tools;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using UnityEngine;
using Object = System.Object;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.StateMachine.States
{
    public class ActivatedState : ButtonStateBase
    {
        #region field
        private ToggleTrafficLightsTool _tool = null;
        private int _originalSelectIndex = 0;
        private bool _selectedIndexChanged = false;
        #endregion

        #region Overrides of StateBase

        public override State State
        {
            get { return State.Activated; }
        }

        #region Overrides of ButtonStateBase

        public override void OnEntry()
        {
            base.OnEntry();

            _tool = ToolsModifierControl.toolController.gameObject.GetComponent<ToggleTrafficLightsTool>() 
                    ?? ToolsModifierControl.toolController.gameObject.AddComponent<ToggleTrafficLightsTool>();
            ToolsModifierControl.toolController.CurrentTool = _tool;
        }

        public override void OnExit()
        {
            //TODO: remember previous tool
            //reset tools
            if (ToolsModifierControl.toolController.CurrentTool == _tool || ToolsModifierControl.toolController.CurrentTool == null)
            {
                var netTool = ToolsModifierControl.GetTool<NetTool>();
                if (netTool != null)
                {
                    ToolsModifierControl.toolController.CurrentTool = netTool;
                }
                else
                {
                    ToolsModifierControl.toolController.CurrentTool = ToolsModifierControl.GetTool<DefaultTool>();
                }

                DebugLog.Info("Tool reset to {0}", ToolsModifierControl.toolController.CurrentTool);
            }

            //reset builtin tab
            if (BuiltinTabstrip != null && BuiltinTabstrip.selectedIndex < 0)
            {
                if (_originalSelectIndex >= 0)
                {
                    BuiltinTabstrip.selectedIndex = _originalSelectIndex;
                }
                else
                {
                    BuiltinTabstrip.selectedIndex = 0;
                }
                DebugLog.Info("Tab.selectedIndex reset to {0}", BuiltinTabstrip.selectedIndex);
            }

            if (_tool != null)
            {
                UnityEngine.Object.Destroy(_tool);
            }
            _tool = null;
            _selectedIndexChanged = false;
            _originalSelectIndex = 0;

            base.OnExit();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (Button != null)
            {
                SetActivedStateSprites(Button);
            }

            _originalSelectIndex = BuiltinTabstrip.selectedIndex;
            if (_originalSelectIndex < 0)
            {
                _originalSelectIndex = 0;
            }
            BuiltinTabstrip.selectedIndex = -1;
        }

        #endregion

        public override Command? CheckCommand()
        {
            if (RoadsPanel == null || !RoadsPanel.isVisible)
            {
                return Command.HideRoadsPanel;
            }

            if (ToolsModifierControl.toolController.CurrentTool != _tool)
            {
                return Command.ActivateOtherTool;
            }

            if (_selectedIndexChanged)
            {
                return Command.ClickToolModeTab;
            }

            return null;
        }
        #endregion

        #region Overrides of ButtonStateBase

        protected override void OnBuiltinTabstripSelectedIndexChanged(UIComponent component, int value)
        {
            base.OnBuiltinTabstripSelectedIndexChanged(component, value);

            if (value >= 0)
            {
                _selectedIndexChanged = true;
            }
        }

        #endregion
    }
}