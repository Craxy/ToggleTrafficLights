using System;
using System.Collections;
using System.Net.Configuration;
using System.Reflection;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI
{
    public class SelectToolButton
    {
        #region members
        //        public static readonly string ButtonName = "TogggleTrafficLightsToolButton";
        private UIButton _btn = null;
        #endregion

        public bool Initialized
        {
            get { return _btn != null; }
        }

        #region Creation
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

        private UIButton CreateButton(UIComponent parent)
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

            var btn = parent.AddUIComponent<UIButton>();

            btn.size = new Vector2(spriteWidth, spriteHeight);
            btn.normalBgSprite = template.normalBgSprite; 
            btn.disabledBgSprite = template.disabledBgSprite; 
            btn.hoveredBgSprite = template.hoveredBgSprite; 
            btn.pressedBgSprite = template.pressedBgSprite; 
            btn.pressedBgSprite = template.pressedBgSprite; 
            btn.focusedBgSprite = template.focusedBgSprite; 
            btn.playAudioEvents = true; 
            //TODO: sprites autauschen
            btn.normalFgSprite = template.normalFgSprite; 
            btn.disabledFgSprite = template.disabledFgSprite; 
            btn.hoveredFgSprite = template.hoveredFgSprite; 
            btn.pressedFgSprite = template.pressedFgSprite; 

            btn.relativePosition = new Vector3(100, 40);

            btn.StartCoroutine(RegisterButtonEvents(btn));

            return btn;
        }
        private IEnumerator RegisterButtonEvents(UIComponent button)
        {
            yield return null;
            button.eventClick += OnClick;
            DebugLog.Info("RegisterButtonEvents: OnClick registered");
        }

        public void Destroy()
        {
            if (_btn != null)
            {
                var roadsOptionPanel = UiHelper.FindComponent<UIComponent>("RoadsOptionPanel", null, UiHelper.FindOptions.NameContains);
                if (roadsOptionPanel != null)
                {
                    roadsOptionPanel.RemoveUIComponent(_btn);
                }
                Object.Destroy(_btn);
            }
            _btn = null;
        }
        #endregion

        #region Toggle
        private void OnClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            DebugLog.Info("Clicked");
        }

        #endregion
    }
}