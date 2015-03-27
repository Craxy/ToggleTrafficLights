using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Tools;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI
{
    public class SelectToggleTrafficLightsToolButton
    {
        #region members
        //        public static readonly string ButtonName = "TogggleTrafficLightsToolButton";
        private UIButton _btn = null;
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

        private UIButton CreateButton(UIComponent parent)
        {
            if (parent == null)
            {
                return null;
            }

            //align with Extended road upgrade mod
            const int spriteWidth = 31;
            const int spriteHeight = 31;

            var btn = parent.AddUIComponent<UIButton>();
            btn.tooltip = "Add/remove traffic lights";
            btn.size = new Vector2(spriteWidth, spriteHeight);
            //add sprites
            btn.atlas = CreateAtlas("icons.png", "ToggleTrafficLightsUI", UIView.Find<UITabstrip>("ToolMode").atlas.material,
                            spriteWidth, spriteHeight, new[]
                            {
                                "OptionBase",
                                "OptionBaseDisabled",
                                "OptionBaseFocused",
                                "OptionBaseHovered",
                                "OptionBasePressed",
                                "Selected", 
                                "Unselected",
                            });
            SetDeactivateStateSprites(btn);
            btn.playAudioEvents = true;
            btn.relativePosition = new Vector3(100, 40);

            RegisterToInterferingTabstrips();

            btn.StartCoroutine(RegisterButtonEvents(btn));

            return btn;
        }
        private IEnumerator RegisterButtonEvents(UIButton button)
        {
            yield return null;

            button.eventClick += OnClick;
            DebugLog.Info("RegisterButtonEvents: OnClick registered");
        }
        public void Destroy()
        {
            DeregisterFromInterferingTabstrip();

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

        private UITextureAtlas CreateAtlas(string file, string name, Material baseMaterial, int spriteWidth, int spriteHeight, string[] spriteNames)
        {
            var tex = new Texture2D(spriteWidth*spriteNames.Length, spriteHeight, TextureFormat.ARGB32, false)
            {
                filterMode = FilterMode.Bilinear,
            };

            //load texture
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            using (var textureStream = assembly.GetManifestResourceStream("Craxy.CitiesSkylines.ToggleTrafficLights.Assets." + file))
            {
                var buf = new byte[textureStream.Length];  //declare arraysize
                textureStream.Read(buf, 0, buf.Length); // read from stream to byte array
                tex.LoadImage(buf);
                tex.Apply(true, false);
            }

            var atlas = ScriptableObject.CreateInstance<UITextureAtlas>();
            // Setup atlas
            var material = Object.Instantiate(baseMaterial);
            material.mainTexture = tex;

            atlas.material = material;
            atlas.name = name;

            //add sprites
            for (var i = 0; i < spriteNames.Length; ++i)
            {
                var uw = 1.0f / spriteNames.Length;

                var spriteInfo = new UITextureAtlas.SpriteInfo
                {
                    name = spriteNames[i],
                    texture = tex,
                    region = new Rect(i * uw, 0, uw, 1),
                };

                atlas.AddSprite(spriteInfo);
            }

            return atlas;
        }
        #endregion

        #region Activate

        private void SetActiveStateSprites(UIButton btn)
        {
            btn.normalFgSprite = "Selected";
            btn.disabledFgSprite = "Selected";
            btn.hoveredFgSprite = "Selected";
            btn.pressedFgSprite = "Selected";
            btn.focusedFgSprite = "Selected";

            btn.normalBgSprite = "OptionBaseFocused";
            btn.disabledBgSprite = "OptionBaseFocused";
            btn.hoveredBgSprite = "OptionBaseFocused";
            btn.pressedBgSprite = "OptionBaseFocused";
            btn.focusedBgSprite = "OptionBaseFocused";
        }
         
        private void SetDeactivateStateSprites(UIButton btn)
        {
            btn.normalFgSprite = "Unselected";
            btn.disabledFgSprite = "Unselected";
            btn.hoveredFgSprite = "Unselected";
            btn.pressedFgSprite = "Unselected";
            btn.focusedFgSprite = "Unselected";

            btn.normalBgSprite = "OptionBase";
            btn.disabledBgSprite = "OptionBase";
            btn.hoveredBgSprite = "OptionBaseHovered";
            btn.pressedBgSprite = "OptionBasePressed";
            btn.focusedBgSprite = "OptionBasePressed";
        }

        public bool IsToolActivated()
        {
            return ToolsModifierControl.toolController.CurrentTool == GetTrafficLightsTool();
        }

        public void Activate()
        {
            SetActiveStateSprites(_btn);
            UiHelper.FindComponent<UITabstrip>("ToolMode").selectedIndex = -1;

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
            SetDeactivateStateSprites(_btn); 

            if (!IsToolActivated())
            {
                return;
            }

            //TODO: to previous tool
            //            ToolsModifierControl.SetTool<NetTool>();
        }

        #endregion

        #region handle other tabs in roadsoptionpanel
        private IEnumerable<UITabstrip> GetInterferingTabstrips()
        {
            new []
            {
                UiHelper.FindComponent<UITabstrip>("ToolMode"),
            }
            
            //TODO: test for null?
            //TODO: extended road tool hinzufügen
        }

        private void RegisterToInterferingTabstrips()
        {
            foreach (var t in GetInterferingTabstrips())
            {
                t.eventSelectedIndexChanged += OnInterferingTabstripSelectedIndexChanged;
            }
        }
        private void DeregisterFromInterferingTabstrip()
        {
            foreach (var t in GetInterferingTabstrips())
            {
                t.eventSelectedIndexChanged -= OnInterferingTabstripSelectedIndexChanged;
            }
        }
        private void OnInterferingTabstripSelectedIndexChanged(UIComponent component, int value)
        {
            var ts = (UITabstrip) component;
            
            //TODO: eigenes deselecten

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

        #region Disable RoadTabs
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


    }
}