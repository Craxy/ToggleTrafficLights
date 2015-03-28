using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private ToolBase _previousTool = null;
        private ToggleTrafficLightsTool _trafficLightsTool = null;
        private Tab _previousTab = null;
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

            RegisterToInterferingUis();

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
            DeregisterFromInterferingUis();

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

            //Select currently selected tab (or default to first tab tool (straight road)
            RememberCurrentActiveTab();

            //deselect all other roads option panel tools
            GetInterferingTabstrips()
                .Where(ts => ts.selectedIndex >= 0)
                .ForEach(ts => ts.selectedIndex = -1);

            if (IsToolActivated())
            {
                return;
            }

            //TODO: RoadsPanel öffnen wenn nicht offen
            SetPreviousTool(ToolsModifierControl.toolController.CurrentTool); //TODO: previous tool is TrafficLightsTool
            ToolsModifierControl.toolController.CurrentTool = GetTrafficLightsTool();
        }

        public void Deactivate()
        {
            SetDeactivateStateSprites(_btn); 

            //Select previous tab tool
            //this is only necessary, if another tool outside the road panel is activated (like other build tab or close roadpanel)
            if (GetInterferingTabstrips().All(t => t.selectedIndex < 0))
            {
                RestorePreviousActiveTab();
            }

            if (!IsToolActivated())
            {
                return;
            }

            DebugLog.Info("Deactivate and go to tool {0}", _previousTool == null ? "null" : _previousTool.GetType().ToString());

            //TODO: problem beim schließen von road panel: erst wird traffic tools disabled registriert -> back to nettool, dann registriert schließen -> NetTool bleibt aktiviert obwohl eigentlich select tool!

            if (_previousTool == null)
            {
                //set to default tool
                ToolsModifierControl.SetTool<DefaultTool>();
            }
            else if (_previousTool is NetTool)
            {
                //enabled traffic lights tool through roads panel -> no ui to change
                ToolsModifierControl.toolController.CurrentTool = _previousTool;
            }
            else if (ToolsModifierControl.toolController.Tools.Contains(_previousTool))
            {
                //enabled from some buildin tool -> reenable that ui
                SetTool(_previousTool);
            }
            else
            {
                //possible mod tool
                //I can't know how to best activate it
                //so I just set the CurrentTool without hiding anything
                ToolsModifierControl.toolController.CurrentTool = _previousTool;
            }


            //TODO: register esc
            //TODO: to previous tool
            //TODO: do something when closing roadsoptionpane
            //            ToolsModifierControl.SetTool<NetTool>();
        }

        private static void SetTool(ToolBase tool)
        {
            //copied from ToolsModifierControl.SetTool
            //necessary for custom tools AND buildin tools for which I don't have the specific type (just ToolBase)

            //not necessary -- tools are already collected
//            if (ToolsModifierControl.m_Tools == null) 
//                ToolsModifierControl.CollectTools();

            //I already have the tool
//            ToolBase toolBase;
//            if (!(ToolsModifierControl.toolController != null) || !ToolsModifierControl.m_Tools.TryGetValue(typeof (T), out toolBase))
//            {
//                return null;
//            }

            if (!ToolsModifierControl.keepThisWorldInfoPanel)
            {
                WorldInfoPanel.HideAllWorldInfoPanels();
            }
            GameAreaInfoPanel.Hide();
            ToolsModifierControl.keepThisWorldInfoPanel = false;
            if (ToolsModifierControl.toolController.CurrentTool != tool)
            {
                ToolsModifierControl.toolController.CurrentTool = tool;
            }

 
        }

        #endregion

        #region handle other tabs in roadsoptionpanel
        private IEnumerable<UITabstrip> GetInterferingTabstrips()
        {
            //TODO: wie IEnumerable einfach erstellen? {1;2;3} in F#?
            return new[]
                {
                    UiHelper.FindComponent<UITabstrip>("ToolMode"),
                    //TODO: extended road tool hinzufügen
                }
                .Where(ts => ts != null); 
        }

        private void RegisterToInterferingUis()
        {
            foreach (var t in GetInterferingTabstrips())
            {
                t.eventSelectedIndexChanged += OnInterferingTabstripSelectedIndexChanged;
            }

            var roadsPanel = UiHelper.FindComponent<UIComponent>("RoadsPanel");
            if(roadsPanel != null)
            {
                roadsPanel.eventVisibilityChanged += RoadsPanelOnEventVisibilityChanged;
            }

            RememberCurrentActiveTab();

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
        private void OnInterferingTabstripSelectedIndexChanged(UIComponent component, int value)
        {
            //-1: nothing selected -> some other option was selected (for example: this tool) 
            if (value >= 0 && IsToolActivated())
            {
                //other tool was selected -> deactivate this tool
                DebugLog.Info("Some other tool was selected -> deselecting Traffic Lights Tool");
                Deactivate();
            }
        }
        private void RoadsPanelOnEventVisibilityChanged(UIComponent component, bool value)
        {
            //wenn von visible->invisible und tool selected -> change to previous road tab tool
            DebugLog.Message("Changing visibility on RoadsPanel to {0}", value);
            if (value == false && IsToolActivated())
            {
                //set selected tab to previous one
                RestorePreviousActiveTab();
            }
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

        private void SetPreviousTool(ToolBase tool)
        {
            DebugLog.Info("Previous tool set to {0}", tool == null ? "null" : tool.GetType().Name);

            _previousTool = tool;
        }
        #endregion

        #region Traffic Lights Tool
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

        #region selected tab

        private void RememberCurrentActiveTab()
        {
            var selected = GetInterferingTabstrips().FirstOrDefault(ts => ts.selectedIndex >= 0);
            _previousTab = selected != null
                           ? new Tab(selected, selected.selectedIndex)
                           : new Tab(GetInterferingTabstrips().First(), 0);
        }

        private void RestorePreviousActiveTab()
        {
            if (_previousTab == null)
            {
                //default to first tab tool (straight tool)
                GetInterferingTabstrips().First().selectedIndex = 0;
            }
            else
            {
                var tab = _previousTab;
                tab.Tabstrip.selectedIndex = tab.SelectedIndex;
            }
        }

        private class Tab
        {
            public readonly UITabstrip Tabstrip;
            public readonly int SelectedIndex;

            public Tab(UITabstrip tabstrip, int selectedIndex)
            {
                Tabstrip = tabstrip;
                SelectedIndex = selectedIndex;
            }
        }
        #endregion
    }
}