using System;
using System.Collections;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Tools;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI
{
    public class SelectToggleTrafficLightsToolMultiStateButton
    {
        #region members
        //        public static readonly string ButtonName = "TogggleTrafficLightsToolButton";
        private UIMultiStateButton _btn = null;
//        private ToolBase _previousTool = null;
        private ToggleTrafficLightsTool _trafficLightsTool = null;
        #endregion

        #region properties
        public bool Initialized
        {
            get { return _btn != null; }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Call in gameloop untill function returns true as indicator it is initialized.
        /// Necessary, because we need to wait untill RoadsPanel gets opened.
        /// That's pretty change because for all other panels you can add buttons (and events) right away...but for Roads[Option]Panel....nope....
        /// Additional it's not possible to use UIView.Find.
        /// So I used the code from ExtendedRoadUpgrade https://github.com/viakmaky/Skylines-ExtendedRoadUpgrade (MIT licence)
        /// I have no idea how viakmaky found a working solution...but he did it. Thanks pal!
        /// </summary>
        public bool Initialize()
        {
            //already initialized
            if (_btn != null)
            {
                DebugLog.Info("Initialize: SelectToolButton already initialized");
                return true;
            }

            var roadsPanel = UiHelper.FindComponent<UIComponent>("RoadsPanel");
            if (roadsPanel == null)
            {
                DebugLog.Info("Initialize: RoadsPanel is null");
                return false;
            }
            if (!roadsPanel.isVisible)
            {
                return false;
            }

            var roadsOptionPanel = UiHelper.FindComponent<UIComponent>("RoadsOptionPanel", null, UiHelper.FindOptions.NameContains);
            if (roadsOptionPanel == null)
            {
                DebugLog.Info("Initialize: RoadsOptionPanel is null");
                return false;
            }
            if (!roadsOptionPanel.gameObject.activeInHierarchy)
            {
                DebugLog.Info("Initialize: RoadsOptionPanel is not active in hierarchy");
                return false;
            }

            _btn = CreateButton(roadsOptionPanel);

            if (_btn == null)
            {
                DebugLog.Info("Initialize: Button is still null after initialization");
                return false;
            }

            DebugLog.Info("Initialize: Button initialized");

            return true;
        }

        private UIMultiStateButton CreateButton(UIComponent parent)
        {
            if (parent == null)
            {
                return null;
            }

            var builtinTabstrip = parent.Find<UITabstrip>("ToolMode");
            var template = (UIButton)builtinTabstrip.tabs[0];

            //align with Extended road upgrade mod
            const int spriteWidth = 31;
            const int spriteHeight = 31;

            var btn = parent.AddUIComponent<UIMultiStateButton>();

            btn.size = new Vector2(spriteWidth, spriteHeight);
            while (btn.foregroundSprites.Count < Enum.GetValues(typeof(ButtonState)).Length)
            {
                btn.foregroundSprites.AddState();
            }
            while (btn.backgroundSprites.Count < Enum.GetValues(typeof(ButtonState)).Length)
            {
                btn.backgroundSprites.AddState();
            }

            //ButtonState.Deactivated
            {
                var fgSprites = btn.foregroundSprites[(int) ButtonState.Deactivated];
                fgSprites.normal = template.normalFgSprite;
                fgSprites.disabled = template.disabledFgSprite;
                fgSprites.hovered = template.hoveredFgSprite;
                fgSprites.pressed = template.pressedFgSprite;
                fgSprites.focused = template.focusedFgSprite;

                var bgSprites = btn.backgroundSprites[(int)ButtonState.Deactivated];
                bgSprites.normal = template.normalBgSprite;
                bgSprites.disabled = template.disabledBgSprite;
                bgSprites.hovered = template.hoveredBgSprite;
                bgSprites.pressed = template.pressedBgSprite;
                bgSprites.focused = template.focusedBgSprite;
            }
            //ButtonState.Activated
            {
                var fgSprites = btn.foregroundSprites[(int)ButtonState.Activated];
                fgSprites.normal = template.normalBgSprite;
                fgSprites.disabled = template.disabledFgSprite;
                fgSprites.hovered = template.hoveredFgSprite;
                fgSprites.pressed = template.pressedFgSprite;
                fgSprites.focused = template.focusedFgSprite;

                var bgSprites = btn.backgroundSprites[(int)ButtonState.Activated];
                bgSprites.normal = template.focusedBgSprite;
                bgSprites.disabled = template.disabledBgSprite;
                bgSprites.hovered = template.hoveredBgSprite;
                bgSprites.pressed = template.pressedBgSprite;
                bgSprites.focused = template.focusedBgSprite;
            }
            btn.playAudioEvents = true;

            btn.relativePosition = new Vector3(100, 40);

            //AHHH....aus irgend einem grund will es nicht -> wirf nullexcpetion
            //aber sobald sprite padding in scene explorer changed funktioniert es
            //NullReferenceException: Object reference not set to an instance of an object
            //  at ColossalFramework.UI.UIMultiStateButton.GetForegroundRenderOffset (Vector2 renderSize) [0x00000] in <filename unknown>:0 
            //  at ColossalFramework.UI.UIMultiStateButton.RenderForeground () [0x00000] in <filename unknown>:0 
            //  at ColossalFramework.UI.UIMultiStateButton.OnRebuildRenderData () [0x00000] in <filename unknown>:0 
            //  at ColossalFramework.UI.UIComponent.Render () [0x00000] in <filename unknown>:0 
            //  at ColossalFramework.UI.UIView.RenderComponent (ColossalFramework.UI.UIRenderData& buffer, ColossalFramework.UI.UIComponent component, UInt32 checksum, Single opacity) [0x00000] in <filename unknown>:0 
            //  at ColossalFramework.UI.UIView.RenderComponent (ColossalFramework.UI.UIRenderData& buffer, ColossalFramework.UI.UIComponent component, UInt32 checksum, Single opacity) [0x00000] in <filename unknown>:0 
            //  at ColossalFramework.UI.UIView.RenderComponent (ColossalFramework.UI.UIRenderData& buffer, ColossalFramework.UI.UIComponent component, UInt32 checksum, Single opacity) [0x00000] in <filename unknown>:0 
            //  at ColossalFramework.UI.UIView.RenderComponent (ColossalFramework.UI.UIRenderData& buffer, ColossalFramework.UI.UIComponent component, UInt32 checksum, Single opacity) [0x00000] in <filename unknown>:0 
            //  at ColossalFramework.UI.UIView.Render () [0x00000] in <filename unknown>:0 
            //  at ColossalFramework.UI.UIView.LateUpdate () [0x00000] in <filename unknown>:0 
            //(Filename:  Line: -1)

            btn.StartCoroutine(RegisterButtonEvents(btn));

            return btn;
        }
        private IEnumerator RegisterButtonEvents(UIMultiStateButton button)
        {
            yield return null;

            button.eventClick += OnClick;
            button.activeStateIndex = 0;
            DebugLog.Info("RegisterButtonEvents: OnClick registered");
        }
        public void Destroy()
        {
            if (_btn != null)
            {
                _btn.eventClick -= OnClick;
                var roadsOptionPanel = UiHelper.FindComponent<UIComponent>("RoadsOptionPanel", null, UiHelper.FindOptions.NameContains);
                if (roadsOptionPanel != null)
                {
                    roadsOptionPanel.RemoveUIComponent(_btn);
                }
                Object.Destroy(_btn);
            }
            _btn = null;

            if (_trafficLightsTool != null)
            {
                _trafficLightsTool.OnEnabledChanged -= OnTrafficLightsEnabledChanged;
                if (ToolsModifierControl.toolController.CurrentTool == _trafficLightsTool)
                {
                    ToolsModifierControl.SetTool<NetTool>();
                }
                Object.Destroy(_trafficLightsTool);
            }
            _trafficLightsTool = null;
        }
        #endregion

        #region Activate

        public bool IsToolActivated()
        {
            return ToolsModifierControl.toolController.CurrentTool == GetTrafficLightsTool();
        }

        public void Activate()
        {
            _btn.activeStateIndex = (int) ButtonState.Activated;

            if (IsToolActivated())
            {
                return;
            }

            //TODO: RoadsPanel öffnen wenn nicht offen
            //TODO: save previous tool?

            ToolsModifierControl.toolController.CurrentTool = GetTrafficLightsTool();
        }

        public void Deactivate()
        {
            _btn.activeStateIndex = (int)ButtonState.Deactivated;

            if (!IsToolActivated())
            {
                return;
            }

            //TODO: to previous tool
//            ToolsModifierControl.SetTool<NetTool>();
        }
        #endregion

        #region Toggle
        private void OnClick(UIComponent component, UIMouseEventParameter args)
        {
            DebugLog.Info("Clicked");
            Activate();
        }
        private void OnTrafficLightsEnabledChanged(object sender, EventArgs<bool> args)
        {
            var enabled = args.Value;
            DebugLog.Info("OnTrafficLightsEnabledChanged: Enabled changed to {0}", enabled);
            if (enabled)
            {
                Activate();
            }
            else
            {
                Deactivate();
            }

        }

        #endregion

        #region Traffic Lights Tool

        public ToggleTrafficLightsTool GetTrafficLightsTool()
        {
            if (_trafficLightsTool == null)
            {
                _trafficLightsTool = ToolsModifierControl.toolController.gameObject.GetComponent<ToggleTrafficLightsTool>()
                                     ?? ToolsModifierControl.toolController.gameObject.AddComponent<ToggleTrafficLightsTool>();

                //register enabledchanged
                _trafficLightsTool.OnEnabledChanged += OnTrafficLightsEnabledChanged;

                DebugLog.Message("Simulation: ToggleTrafficLightsTool events registered");
            }

            System.Diagnostics.Debug.Assert(_trafficLightsTool != null);

            return _trafficLightsTool;
        }
        #endregion

        #region Button
        private enum ButtonState
        {
            Deactivated = 0,
            Activated = 1,
        }
        #endregion
    }
}