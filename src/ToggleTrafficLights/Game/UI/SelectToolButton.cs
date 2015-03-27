//using System;
//using System.Collections;
//using System.Net.Configuration;
//using System.Reflection;
//using ColossalFramework.UI;
//using Craxy.CitiesSkylines.ToggleTrafficLights.Tools;
//using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
//using UnityEngine;
//using Object = UnityEngine.Object;
//
//namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI
//{
//    public class SelectToolButton
//    {
//        #region members
//        //        public static readonly string ButtonName = "TogggleTrafficLightsToolButton";
//        private UIMultiStateButton _btn = null;
//        private ToolBase _previousTool = null;
//        private ToggleTrafficLightsTool _trafficLightsTool = null;
//        #endregion
//
//        public bool Initialized
//        {
//            get { return _btn != null; }
//        }
//
//        #region Creation
//        /// <summary>
//        /// Call in gameloop untill function returns true as indicator it is initialized.
//        /// Necessary, because we need to wait untill RoadsPanel gets opened.
//        /// That's pretty change because for all other panels you can add buttons (and events) right away...but for Roads[Option]Panel....nope....
//        /// Additional it's not possible to use UIView.Find.
//        /// So I used the code from ExtendedRoadUpgrade https://github.com/viakmaky/Skylines-ExtendedRoadUpgrade (MIT licence)
//        /// I have no idea how viakmaky found a working solution...but he did it. Thanks pal!
//        /// </summary>
//        public bool Initialize()
//        {
//            //already initialized
//            if (_btn != null)
//            {
//                DebugLog.Info("Initialize: SelectToolButton already initialized");
//                return true;
//            }
//
//            var roadsPanel = UiHelper.FindComponent<UIComponent>("RoadsPanel");
//            if (roadsPanel == null)
//            {
//                DebugLog.Info("Initialize: RoadsPanel is null");
//                return false;
//            }
//            if (!roadsPanel.isVisible)
//            {
//                return false;
//            }
//
//            var roadsOptionPanel = UiHelper.FindComponent<UIComponent>("RoadsOptionPanel", null, UiHelper.FindOptions.NameContains);
//            if (roadsOptionPanel == null)
//            {
//                DebugLog.Info("Initialize: RoadsOptionPanel is null");
//                return false;
//            }
//            if (!roadsOptionPanel.gameObject.activeInHierarchy)
//            {
//                DebugLog.Info("Initialize: RoadsOptionPanel is not active in hierarchy");
//                return false;
//            }
//
//            _btn = CreateButton(roadsOptionPanel);
//
//            if (_btn == null)
//            {
//                DebugLog.Info("Initialize: Button is still null after initialization");
//                return false;
//            }
//
//            DebugLog.Info("Initialize: Button initialized");
//
//            return true;
//        }
//
//        private UIMultiStateButton CreateButton(UIComponent parent)
//        {
//            if (parent == null)
//            {
//                return null;
//            }
//
//            var builtinTabstrip = parent.Find<UITabstrip>("ToolMode");
//            var template = (UIButton)builtinTabstrip.tabs[0];
//
//            //align with Extended road upgrade mod
//            const int spriteWidth = 31;
//            const int spriteHeight = 31;
//
//            var btn = parent.AddUIComponent<UIMultiStateButton>();
//
//            btn.size = new Vector2(spriteWidth, spriteHeight);
//            btn.normalBgSprite = template.normalBgSprite; 
//            btn.disabledBgSprite = template.disabledBgSprite; 
//            btn.hoveredBgSprite = template.hoveredBgSprite; 
//            btn.pressedBgSprite = template.pressedBgSprite; 
//            btn.pressedBgSprite = template.pressedBgSprite; 
//            btn.focusedBgSprite = template.focusedBgSprite; 
//            btn.playAudioEvents = true; 
//            //TODO: sprites autauschen
//            btn.normalFgSprite = template.normalFgSprite; 
//            btn.disabledFgSprite = template.disabledFgSprite; 
//            btn.hoveredFgSprite = template.hoveredFgSprite; 
//            btn.pressedFgSprite = template.pressedFgSprite; 
//
//            btn.relativePosition = new Vector3(100, 40);
//
//            btn.StartCoroutine(RegisterButtonEvents(btn));
//
//            return btn;
//        }
//        private IEnumerator RegisterButtonEvents(UIComponent button)
//        {
//            yield return null;
//            button.eventClick += OnClick;
//            DebugLog.Info("RegisterButtonEvents: OnClick registered");
//        }
//
//        public void Destroy()
//        {
//            if (_btn != null)
//            {
//                var roadsOptionPanel = UiHelper.FindComponent<UIComponent>("RoadsOptionPanel", null, UiHelper.FindOptions.NameContains);
//                if (roadsOptionPanel != null)
//                {
//                    roadsOptionPanel.RemoveUIComponent(_btn);
//                }
//                Object.Destroy(_btn);
//            }
//            _btn = null;
//
//            if (_trafficLightsTool != null)
//            {
//                if (ToolsModifierControl.toolController.CurrentTool == _trafficLightsTool)
//                {
//                    ToolsModifierControl.SetTool<NetTool>();
//                }
//                Object.Destroy(_trafficLightsTool);
//            }
//            _trafficLightsTool = null;
//        }
//        #endregion
//
//        #region Toggle
//        private void OnClick(UIComponent component, UIMouseEventParameter eventParam)
//        {
//            var btn = (UIButton) component;
//
//            DebugLog.Info("Clicked");
//        }
//        private void ToggleToggleTrafficLightsTool()
//        {
//            var controller = ToolsModifierControl.toolController;
//
//            var currentTool = controller.CurrentTool;
//            var tool = GetTrafficLightsTool();
//
//            if (currentTool == tool)
//            {
//                //switch to previous tool
//                if (_previousTool == null)
//                {
//                    //fallback: back to defaulttool (selection tool)
//                    ToolsModifierControl.SetTool<DefaultTool>();
//                }
//                else
//                {
//                    controller.CurrentTool = _previousTool;
//                }
//
//                DebugLog.Message("Switched to previous tool: {0}", _previousTool == null ? "[DefaultTool]" : _previousTool.GetType().Name);
//
//                _previousTool = null;
//                System.Diagnostics.Debug.Assert(_previousTool == null);
//
//            }
//            else
//            {
//                //switch to traffic lights tool
//                _previousTool = currentTool;
//                controller.CurrentTool = tool;
//
//                DebugLog.Message("Switched to ToggleTrafficLightsTool from {0}", _previousTool == null ? "[DefaultTool]" : _previousTool.GetType().Name);
//
//                System.Diagnostics.Debug.Assert(_previousTool != null);
//            }
//        }
//        private void OnTrafficLightsEnabledChanged(object sender, EventArgs<bool> args)
//        {
//            var enabled = args.Value;
//
//            var tool = GetTrafficLightsTool();
//
//            DebugLog.Info("OnTrafficLightsEnabledChanged: Enabled changed");
//
//            //set button style regarding enabled
//            if (_btn != null)
//            {
//                _btn.ActiveStatesCount()
//            }
//        }
//        #endregion
//
//        #region Traffic Lights Tool
//
//        public ToggleTrafficLightsTool GetTrafficLightsTool()
//        {
//            if (_trafficLightsTool == null)
//            {
//                _trafficLightsTool = ToolsModifierControl.toolController.gameObject.GetComponent<ToggleTrafficLightsTool>() 
//                                     ?? ToolsModifierControl.toolController.gameObject.AddComponent<ToggleTrafficLightsTool>();
//
//                //register enabledchanged
//                _trafficLightsTool.OnEnabledChanged += OnTrafficLightsEnabledChanged;
//
//                DebugLog.Message("Simulation: ToggleTrafficLightsTool events registered");
//            }
//
//            System.Diagnostics.Debug.Assert(_trafficLightsTool != null);
//
//            return _trafficLightsTool;
//        }
//
//
//
////        public static ToolBase GetTrafficLightsTool<T>() where T : ToolBase
////        {
////            var tool = ToolsModifierControl.toolController.gameObject.GetComponent<T>();
////            if (tool == null)
////            {
////                tool = ToolsModifierControl.toolController.gameObject.AddComponent<T>();
////                DebugLog.Message("Simulation: ToggleTrafficLightsTool created");
////            }
////
////            System.Diagnostics.Debug.Assert(tool != null);
////
////            return tool;
////        }
////
////        public static void DestroyTrafficLightsTool<T>() where T : ToolBase
////        {
////            if (ToolsModifierControl.toolController != null)
////            {
////                var tool = ToolsModifierControl.toolController.gameObject.GetComponent<T>();
////                if (tool != null)
////                {
////                    Object.Destroy(tool);
////                }
////            }
////        }
//        #endregion
//    }
//}