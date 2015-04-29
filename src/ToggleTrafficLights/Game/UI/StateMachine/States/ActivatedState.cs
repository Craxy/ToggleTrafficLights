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

        protected ToggleTrafficLightsTool Tool
        {
            get { return _tool; }
        }

        private int _originalSelectIndex = 0;
        private bool _selectedIndexChanged = false;
        #endregion

        #region Overrides of StateBase

        public override State State
        {
            get { return State.Activated; }
        }

        #region Overrides of ButtonStateBase
        protected override string ButtonName
        {
            get { return ButtonBaseName + "Activated"; }
        }

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

            if (RoadsPanel != null && !RoadsPanel.isVisible)
            {
                if (BuiltinTabstrip != null)
                {
                    BuiltinTabstrip.selectedIndex = 0;
                }
            }

            if (_tool != null)
            {
                _tool.ClearRenderOverlay();
                //TODO: destroys tool unsynchronous -> other state gets tool, which then gets destroyed...
//                UnityEngine.Object.Destroy(_tool);
            }
            _tool = null;
            _selectedIndexChanged = false;
            _originalSelectIndex = 0;

            base.OnExit();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (BuiltinTabstrip != null && BuiltinTabstrip.selectedIndex >= 0)
            {
                BuiltinTabstrip.selectedIndex = -1;
            }
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

        #region Overrides of ButtonStateBase

        public override void Destroy()
        {
            if (_tool != null)
            {
                _tool.ClearRenderOverlay();
                UnityEngine.Object.Destroy(_tool);
            }
            _tool = null;

            base.Destroy();
        }

        #endregion

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

            if (KeyHelper.IsInvisibleToolKeyPressed())
            {
                return Command.PressInvisibleShortcut;
            }

            if (KeyHelper.IsToolKeyPressed())
            {
                return Command.PressShortcut;
            }

            if (_selectedIndexChanged)
            {
                return Command.ClickToolModeTab;
            }

            if (RightClick)
            {
                return Command.RightClickToolButton;
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