using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Tools;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI
{
    public class SelectToggleTrafficLightsToolButton
    {
        #region members
        private UIButton _button = null;
        private ToggleTrafficLightsTool _trafficLightsTool = null;
        private int _previousToolModeSelectedIndex = 0;
        #endregion 

        #region properties
        public bool Initialized
        {
            get { return _button != null; }
        }
        public bool Enabled
        {
            get { return _trafficLightsTool != null && _trafficLightsTool.enabled; }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Call in gameloop untill function returns true as indicator it is initialized.
        /// Necessary, because we need to wait untill RoadsPanel gets opened.
        /// That's pretty strange because for all other panels you can add buttons (and events) right away...but for Roads[Option]Panel....nope....
        /// Additional it's not possible to use UIView.Find.
        /// So I used the code from ExtendedRoadUpgrade https://github.com/viakmaky/Skylines-ExtendedRoadUpgrade (MIT licence)
        /// I have no idea how viakmaky found a working solution...but he did it. Thanks pal!
        /// </summary>
        public bool Initialize()
        {
            if (Initialized)
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

            _button = CreateButton(roadsOptionPanel);

            if (_button == null)
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
            btn.playAudioEvents = true;
            btn.relativePosition = new Vector3(131, 38);

            SetDeactivatedStateSprites(btn);

            RegisterToInterferingUis();

            btn.StartCoroutine(RegisterButtonEvents(btn));

            return btn;
        }
        private static UITextureAtlas CreateAtlas(string file, string name, Material baseMaterial, int spriteWidth, int spriteHeight, string[] spriteNames)
        {
            var tex = new Texture2D(spriteWidth * spriteNames.Length, spriteHeight, TextureFormat.ARGB32, false)
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

        #region Destroy
        public void Destroy()
        {
            DeregisterFromInterferingUis();

            if (_button != null)
            {
                _button.eventClick -= OnClick;
                var roadsOptionPanel = UiHelper.FindComponent<UIComponent>("RoadsOptionPanel", null, UiHelper.FindOptions.NameContains);
                if (roadsOptionPanel != null)
                {
                    roadsOptionPanel.RemoveUIComponent(_button);
                }
                Object.Destroy(_button);
            }
            _button = null;

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

        #region events
        private IEnumerator RegisterButtonEvents(UIButton button)
        {
            yield return null;

            button.eventClick += OnClick;
            DebugLog.Info("RegisterButtonEvents: OnClick registered");
        }
        private void RegisterToInterferingUis()
        {
            //register to other mods tabstrips
            foreach (var t in GetInterferingTabstrips())
            {
                t.eventSelectedIndexChanged += OnInterferingTabstripSelectedIndexChanged;
            }

            var roadsPanel = UiHelper.FindComponent<UIComponent>("RoadsPanel");
            if (roadsPanel != null)
            {
                roadsPanel.eventVisibilityChanged += RoadsPanelOnEventVisibilityChanged;
            }
        }

        private void DeregisterFromInterferingUis()
        {
            foreach (var t in GetInterferingTabstrips())
            {
                t.eventSelectedIndexChanged -= OnInterferingTabstripSelectedIndexChanged;
            }
            var roadsPanel = UiHelper.FindComponent<UIComponent>("RoadsPanel");
            if (roadsPanel != null)
            {
                roadsPanel.eventVisibilityChanged -= RoadsPanelOnEventVisibilityChanged;
            }
        }

        private UITabstrip GetToolModeTabstrip()
        {
            return UiHelper.FindComponent<UITabstrip>("ToolMode");
        }
        private IEnumerable<UITabstrip> GetInterferingTabstrips()
        {
            //TODO: wie IEnumerable einfach erstellen? {1;2;3} in F#?
            return new[]
                {
                    UiHelper.FindComponent<UITabstrip>("ToolMode"),
                    UiHelper.FindComponent<UITabstrip>("ExtendedRoadUpgradePanel"),
                }
                .Where(ts => ts != null);
        }

        private void OnInterferingTabstripSelectedIndexChanged(UIComponent component, int value)
        {
            //i'm not interested in deselecting
            if (value < 0)
            {
                return;
            }
            
            //buildin tab
            if (component.name == "ToolMode")
            {
                //store for restoring when closing panel
                _previousToolModeSelectedIndex = value;
            }

            //a selectedIndex >= 0 means its tool is activated
            //I don't know how other tools handle selected -> ignore them
            if (Enabled && component.name == "ToolMode")
            {
                ToolsModifierControl.toolController.CurrentTool = ToolsModifierControl.toolController.Tools.OfType<NetTool>().First();
                DebugLog.Info("OnInterferingTabstripSelectedIndexChanged: back to NetTool because value is {0}", value);
            }

        }

        private void RoadsPanelOnEventVisibilityChanged(UIComponent component, bool value)
        {
            //enabled can not be used because tool gets first disabled then roadspanel hides
            var tab = GetToolModeTabstrip();
            if (tab.selectedIndex < 0)
            {
                tab.selectedIndex = _previousToolModeSelectedIndex;
                DebugLog.Info("RoadsPanelOnEventVisibilityChanged: change selected to {0}", _previousToolModeSelectedIndex);
            }

        }
        #endregion

        #region toggle
        private void OnClick(UIComponent component, UIMouseEventParameter args)
        {
            if (!Enabled)
            {
                Activate();
            }
        }

        private void OnTrafficLightsEnabledChanged(object sender, EventArgs<bool> args)
        {
            if (_button == null)
            {
                return;
            }

            //set button sprites depending on state
            var enabled = args.Value;
            if (enabled)
            {
                SetActivedStateSprites(_button);
            }
            else
            {
                SetDeactivatedStateSprites(_button);

                //reset tab selection if necessary
                if (GetInterferingTabstrips().All(t => t.selectedIndex < 0))
                {
                    GetToolModeTabstrip().selectedIndex = _previousToolModeSelectedIndex;
                }
            }
        }
        #endregion

        #region sprites
        private void SetActivedStateSprites(UIButton btn)
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
        private void SetDeactivatedStateSprites(UIButton btn)
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
            btn.focusedBgSprite = "OptionBase";
        }
        #endregion

        #region Activation

        public void Activate()
        {
            if (Enabled)
            {
                DebugLog.Info("Traffic Lights tool already activated");
                return;
            }

            //Disable all other tabs
            GetInterferingTabstrips().ForEach(t => t.selectedIndex = -1);
            //activate this tool
            ToolsModifierControl.toolController.CurrentTool = GetTrafficLightsTool();
        }

        public void Deactivate()
        {
            //TODO: implement
        }
        #endregion


        #region Traffic Lights tool
        public ToggleTrafficLightsTool GetTrafficLightsTool()
        {
            if (_trafficLightsTool == null)
            {
                _trafficLightsTool = ToolsModifierControl.toolController.gameObject.GetComponent<ToggleTrafficLightsTool>();
                if (_trafficLightsTool == null)
                {
                    var previousTool = ToolsModifierControl.toolController.CurrentTool;
                    _trafficLightsTool = ToolsModifierControl.toolController.gameObject.AddComponent<ToggleTrafficLightsTool>();
                    //reset to previous tool. Somehow AddComponent enabled the added tool...
                    ToolsModifierControl.toolController.CurrentTool = previousTool;
                }

                //register enabledchanged
                _trafficLightsTool.OnEnabledChanged += OnTrafficLightsEnabledChanged;

                DebugLog.Message("Simulation: ToggleTrafficLightsTool events registered");
            }

            System.Diagnostics.Debug.Assert(_trafficLightsTool != null);

            return _trafficLightsTool;
        }

        #endregion

        public void ToggleShow()
        {

            var roadsPanel = UiHelper.FindComponent<UIComponent>("RoadsPanel");
            if (roadsPanel.isVisible)
            {
                if (Enabled)
                {
                    DebugLog.Info("From enabled to closed panel");
                    //close roads panel
                    ClickOnRoadsButton();
                }
                else
                {
                    DebugLog.Info("From opened panel activate");
                    Activate();
                }
            }
            else
            {
                DebugLog.Info("From closed panel activate");
                //open roads panel
                ClickOnRoadsButton();
                Activate();
            }

        }

        internal static void ClickOnRoadsButton()
        {
            //open/close road panel
            //Source: KeyShortcuts.SelectUIButton
            //I want all to enjoy the developers comment/log at the end of the SelectUIButton:
            //"SelectUIButton() was terminated to prevent an infinite loop. This might be some kind of bug... :D"
            //...
            var tutorialUiTag = (TutorialUITag)MonoTutorialTag.Find("Roads");
            tutorialUiTag.target.SimulateClick();
        }
    }
}