using System;
using System.ComponentModel;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Tools;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI
{
    public class ToolUi
    {
        #region fields

        private bool _initialized = false;
        private UIComponent _roadsOptionPanel = null;
        private UITabstrip _builtinTabstrip = null;
        private UIButton _button = null;
        private static readonly string ButtonName = "ToggleTrafficLightsButton";

        private UIPanel _roadsPanel;
        private TrafficLightsToolMode _toolMode;
        private int _originalBuiltinTabstripSelectedIndex = -1;
        private NetTool _netTool;

        private ToggleTrafficLightsTool _tool;
        #endregion

        #region properties
        public bool IsVisible { get; private set; }
        #endregion

        #region life span

        public void Destroy()
        {
            DestroyView();
            DestroyTrafficLightsTool();
        }
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
                if (_toolMode != TrafficLightsToolMode.None)
                {
                    DebugLog.Info("Roads panel no longer visible");
                    SetToolMode(TrafficLightsToolMode.None);
                }
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

            //TODO: ui show
            if (!IsVisible)
            {
                Show();
            }

        }
        #endregion

        #region Display

        public void Show()
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

        #region Initialization
        public bool Initialize()
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

            SetDeactivatedStateSprites(_button);

            _builtinTabstrip.eventSelectedIndexChanged += OnBuiltinTabstripSelectedIndexChanged;

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

        public void DestroyView()
        {
            if (_button != null)
            {
                if (_builtinTabstrip != null)
                {
                    _builtinTabstrip.eventSelectedIndexChanged -= OnBuiltinTabstripSelectedIndexChanged;
                }

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

        #region events

        private bool _ignoreBuiltinTabstripEvents = false;
        private void OnBuiltinTabstripSelectedIndexChanged(UIComponent component, int value)
        {
            //TODO: just if this is enabled
            if (value >= 0)
            {
                SetToolMode(TrafficLightsToolMode.None);
            }
        }
        private void OnClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            SetToolMode(TrafficLightsToolMode.Intersection);
        }
        #endregion

        #region Traffic Lights Tool

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

                //TODO: register to enabled changed
            }
        }

        private void DestroyTrafficLightsTool()
        {
            if (_tool != null)
            {
                Object.Destroy(_tool);
                _tool = null;
                DebugLog.Info("Traffic Lights Tool destroyed");
            }
        }
        #endregion

        #region ToolMode

        private void SetToolMode(TrafficLightsToolMode mode)
        {
            if (mode != _toolMode)
            {
                switch (mode)
                {
                    case TrafficLightsToolMode.None:
                        DebugLog.Info("Tool disabled");

                        if (ToolsModifierControl.toolController.CurrentTool == _tool || ToolsModifierControl.toolController.CurrentTool == null)
                        {
                            ToolsModifierControl.toolController.CurrentTool = _netTool;
                        }

                        DestroyTrafficLightsTool();

                        if (_button != null)
                        {
                            SetDeactivatedStateSprites(_button);
                        }
                        
                        if (_builtinTabstrip != null)
                        {
                            if (_builtinTabstrip.selectedIndex < 0 && _originalBuiltinTabstripSelectedIndex >= 0)
                            {
                                _ignoreBuiltinTabstripEvents = true;
                                DebugLog.Info("Setting builtin tabstrip mode: {0}", _originalBuiltinTabstripSelectedIndex);
                                _builtinTabstrip.selectedIndex = _originalBuiltinTabstripSelectedIndex;
                                _ignoreBuiltinTabstripEvents = false;
                            }
                        }

                        break;
                    case TrafficLightsToolMode.Intersection:
                        DebugLog.Info("Tool enabled");

                        if (_button != null)
                        {
                            SetActivedStateSprites(_button);
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException("mode");
                }
            }
            else
            {
                DebugLog.Info("ToolMode already set to {0}", mode.ToString("G"));
            }
        }

        private enum TrafficLightsToolMode
        {
            None,
            Intersection,
        }
        #endregion
    }
}