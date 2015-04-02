using System;
using System.Net.Configuration;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Tools;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.New
{
    public sealed class TrafficLightsToolUi
    {
        #region fields
        private ToggleTrafficLightsTool _tool = null;
        private NetTool _netTool = null;

        private UIComponent _roadsPanel = null;
        private UIComponent _roadsOptionPanel = null;
        private UITabstrip _builtinTabstrip = null;
        private int _originalBuiltinTabsripSelectedIndex = 1;
        private UIButton _button;
        private static readonly string ButtonName = "ToggleTrafficLightsButton";
        private bool _initialized = false;

        #endregion

        #region properties
        public bool IsVisible { get; private set; }
        public bool IsActivated { get; private set; }

        #endregion

        #region game loop

        public void OnUpdate()
        {
            if (_roadsPanel == null)
            {
                _roadsPanel = UIView.Find<UIPanel>("RoadsPanel");
            }

            if (_roadsPanel == null || !_roadsPanel.isVisible)
            {
                if(IsActivated)

                return;
            }
        }
        public void Update()
        {
            if (_roadsPanel == null)
            {
                _roadsPanel = UIView.Find<UIPanel>("RoadsPanel");
            }

            if (_roadsPanel == null || !_roadsPanel.isVisible)
            {
                //TODO: disable ui if needed
                return;
            }

            if (_netTool == null)
            {
                foreach (var tool in ToolsModifierControl.toolController.Tools)
                {
                    var nt = tool as NetTool;
                    if (nt != null && nt.m_prefab != null)
                    {
                        DebugLog.Info("NetTool found: {0}", nt.name);
                        _netTool = nt;
                        break;
                    }
                }

                if (_netTool == null)
                {
                    return;
                }
            }

            if (!IsVisible)
            {
                Show();
            }
        }
        private void Show()
        {
            if (!_initialized)
            {
                if (!Initialize())
                {
                    return;
                }
            }

            DebugLog.Info("Showing UI");
            IsVisible = true;
        }
        #endregion

        #region initialization
        private bool Initialize()
        {
            DebugLog.Info("Initializing UI");

            _roadsOptionPanel = UiHelper.FindComponent<UIComponent>("RoadsOptionPanel", null, UiHelper.FindOptions.NameContains);
            if (_roadsOptionPanel == null || !_roadsOptionPanel.gameObject.activeInHierarchy)
            {
                return false;
            }

            _builtinTabstrip = UiHelper.FindComponent<UITabstrip>("ToolMode", _roadsOptionPanel);
            if (_builtinTabstrip == null || !_builtinTabstrip.gameObject.activeInHierarchy)
            {
                return false;
            }

            _button = UiHelper.FindComponent<UIButton>(ButtonName);
            if (_button != null)
            {
                DestroyView();
            }

            CreateView();

            return _button != null;

        }

        private void CreateView()
        {
            DebugLog.Info("Creating view");

            var rootObject = new GameObject(ButtonName);
            _button = rootObject.AddComponent<UIButton>();

            //align with Extended road upgrade mod
            const int spriteWidth = 31;
            const int spriteHeight = 31;

            _button.tooltip = "Add/remove traffic lights";
            _button.size = new Vector2(spriteWidth, spriteHeight);

            //add sprites
            _button.atlas = CreateAtlas("icons.png", "ToggleTrafficLightsUI", UIView.Find<UITabstrip>("ToolMode").atlas.material,
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
            _button.playAudioEvents = true;
            _button.relativePosition = new Vector3(131, 38);


            _builtinTabstrip.eventSelectedIndexChanged += OnBuiltinTabstripSelectedIndexChanged;


            SetDeactivatedStateSprites(_button);
            _button.eventClick += OnClick;

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

        #region destroy
        public void Destroy()
        {
            DestroyView();
            DestroyTrafficLightsTool();
        }

        private void DestroyView()
        {
            if (_button != null)
            {
                if (_builtinTabstrip != null)
                {
                    _builtinTabstrip.eventSelectedIndexChanged -= OnBuiltinTabstripSelectedIndexChanged;
                }

                _button.eventClick -= OnClick;
                Object.Destroy(_button);
                _button = null;
            }
            IsVisible = false;
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

        #region activation

        private void SetActivation(bool activate)
        {
            if (IsActivated == activate)
            {
                return;
            }

            if (activate)
            {
                CreateTrafficLightsTool();
                ToolsModifierControl.toolController.CurrentTool = _tool;

                IsActivated = true;
                DebugLog.Info("Traffic Lights tool activated");
            }
            else //deactivate
            {
                if (ToolsModifierControl.toolController.CurrentTool == _tool || ToolsModifierControl.toolController.CurrentTool == null)
                {
                    ToolsModifierControl.toolController.CurrentTool = _netTool;
                }

                DestroyTrafficLightsTool();

                IsActivated = false;
                DebugLog.Info("Traffic Lights tool disabled");
            }
        }

        private void SetActivationImpl(bool activate)
        {
            IsActivated = activate;

            if (_builtinTabstrip != null)
            {
                if (activate)
                {
                    if (_builtinTabstrip.selectedIndex >= 0)
                    {
                        _builtinTabstrip.selectedIndex = -1;
                        DebugLog.Info("Setting builtin tabstrip mode: -1");
                    }
                }
                else //deactivate
                {
                    if (_builtinTabstrip.selectedIndex >= 0)
                    {
                        DebugLog.Info("Setting builtin tabstrip mode: -1");
                    }
                }
            }
        }

        private void Activate()
        {
            SetActivation(true);
        }
        private void Deactivate()
        {
            SetActivation(false);
        }
        #endregion

        #region events
        private void OnClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            throw new NotImplementedException();
        }

        private void OnTrafficLightsToolEnabledChanged(object sender, EventArgs<bool> args)
        {
            var enabled = args.Value;

            if (enabled)
            {
                SetActivedStateSprites(_button);
            }
            else
            {
                SetDeactivatedStateSprites(_button);
            }

            //TODO: automatisch bnl activieren?

            //TODO: implement
            throw new NotImplementedException();

        }
        #endregion

        #region builtin tabstrip (RoadsOptionPanel)
        private void OnBuiltinTabstripSelectedIndexChanged(UIComponent component, int value)
        {
            if (value >= 0)
            {
                _originalBuiltinTabsripSelectedIndex = value;
            }
        }
        #endregion



        #region traffic lights tool
        private void CreateTrafficLightsTool()
        {
            if (_tool == null)
            {
                _tool = ToolsModifierControl.toolController.gameObject.GetComponent<ToggleTrafficLightsTool>();
                if (_tool == null)
                {
                    _tool = ToolsModifierControl.toolController.gameObject.AddComponent<ToggleTrafficLightsTool>();
                    DebugLog.Info("Tool created: {0}", _tool);
                }
                else
                {
                    DebugLog.Info("Found existing tool: {0}", _tool);
                }

                _tool.EnabledChanged += OnTrafficLightsToolEnabledChanged;
            }
        }
        private void DestroyTrafficLightsTool()
        {
            if (_tool != null)
            {
                _tool.EnabledChanged -= OnTrafficLightsToolEnabledChanged;
                Object.Destroy(_tool);
                _tool = null;
                DebugLog.Info("Traffic Lights Tool destroyed");
            }
        }
        #endregion
    }
}