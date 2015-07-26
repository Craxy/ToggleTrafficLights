using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.StateMachine.States
{
    public abstract class ButtonStateBase : StateBase
    {
        #region fields

        private static UITextureAtlas _atlas = null;

        protected UIButton Button = null;
        protected UITabstrip BuiltinTabstrip = null;
        protected UIComponent RoadsOptionPanel = null;
        protected static string ButtonBaseName = "ToggleTrafficLightsButton";

        protected bool Initialized = false;

        #endregion

        #region properties
        #region mouse
        public bool IsMouseDown(int button)
        {
            //button values are 0 for left button, 1 for right button, 2 for the middle button.
            return Button != null && Button.containsMouse && Input.GetMouseButtonDown(button);
        }
        public bool LeftClick
        {
            get { return IsMouseDown(0); }
        }
        public bool RightClick
        {
            get { return IsMouseDown(1); }
        }
        public bool MiddleClick
        {
            get { return IsMouseDown(1); }
        }
        #endregion

        protected abstract string ButtonName { get; }
        #endregion

        #region Overrides of StateBase

        public override void OnEnable()
        {
            base.OnEnable();

            Initialize();
        }

        public override void OnDisable()
        {
            DestroyView();

            base.OnDisable();
        }

        public override void Update()
        {
            base.Update();

            if (!Initialized)
            {
                Initialize();
            }
        }

        public override void OnDestroy()
        {
            if (Button != null)
            {
                Button.Hide();

                if (RoadsOptionPanel != null)
                {
                    RoadsOptionPanel.RemoveUIComponent(Button);
                }

                Destroy(Button.gameObject);
            }
        }

        #endregion

        #region UI
        protected bool Initialize()
        {
            if(Initialized)
            {
                DebugLog.Info("State {0}: Initialize: SelectToolButton already initialized", State);
                return true;
            }

            if (RoadsPanel == null)
            {
                DebugLog.Info("State {0}: Initialize: RoadsPanel is null", State);
                return false;
            }

            if (!RoadsPanel.isVisible)
            {
                DebugLog.Info("State {0}: Initialize: RoadsPanel is not visible", State);
                return false;
            }

            RoadsOptionPanel = UiHelper.FindComponent<UIComponent>("RoadsOptionPanel(RoadsPanel)", null, UiHelper.FindOptions.NameContains);
            if (RoadsOptionPanel == null)
            {
                DebugLog.Info("State {0}: Initialize: RoadsOptionPanel is null", State);
                return false;
            }
            if (!RoadsOptionPanel.gameObject.activeInHierarchy)
            {
                DebugLog.Info("State {0}: Initialize: RoadsOptionPanel is not active in hierarchy", State);
                return false;
            }
            if (!RoadsOptionPanel.isVisible)
            {
                DebugLog.Info("State {0}: Initialize: RoadsOptionPanel is not visible", State);
                return false;
            }

            BuiltinTabstrip = UiHelper.FindComponent<UITabstrip>("ToolMode", RoadsOptionPanel);
            if (BuiltinTabstrip == null)
            {
                DebugLog.Info("State {0}: Initialize: ToolMode is null", State);
                return false;
            }
            if (!BuiltinTabstrip.gameObject.activeInHierarchy)
            {
                DebugLog.Info("State {0}: Initialize: ToolMode is not active in hierarchy", State);
                return false;
            }

            //TODO: doppelt gemoppelt wenn Button schon gesetzt
            Button = UiHelper.FindComponent<UIButton>(ButtonName);

            if (Button != null)
            {
                DebugLog.Info("State {0}: Button {1} created previously", State, ButtonName);
            }
            else
            {
                DebugLog.Info("State {0}: Creating button {1}", State, ButtonName);
                Button = CreateButton(RoadsOptionPanel);
                if (Button == null)
                {
                    DebugLog.Info("State {0}: Initialize: Button is still null after initialization", State);
                    return false;
                }
            }

            if (Button == null)
            {
                DebugLog.Info("State {0}: Initialize: Button is still null after initialization", State);
                return false;
            }

            if (!SubscribeEvents())
            {
                DebugLog.Info("State {0}: Initialize: Subscribing to events failed", State);
                return false;
            }

            DebugLog.Info("State {0}: Initialize: Button initialized", State);

            Button.Show();
            Initialized = true;
            OnInitialized();

            return true;
        }
        #endregion

        #region View
        protected void DestroyView()
        {
            UnsubscribeEvents();

            if (Button != null)
            {
                //do not destroy button for reuse
                Button.Hide();
            }

            RoadsOptionPanel = null;
            BuiltinTabstrip = null;
            Button = null;
            Initialized = false;
        }
        #endregion

        #region button
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
            btn.name = ButtonName;
            btn.tooltip = "Add/remove traffic lights.\nRight click for menu.";
            btn.size = new Vector2(spriteWidth, spriteHeight);
            //add sprites
            if (_atlas == null)
            {
                _atlas = CreateAtlas("icons.png", "ToggleTrafficLightsUI",
                                        UIView.Find<UITabstrip>("ToolMode").atlas.material,
                                        spriteWidth, spriteHeight, new[]
                                        {
                                            "OptionBase",
                                            "OptionBaseDisabled",
                                            "OptionBaseFocused",
                                            "OptionBaseHovered",
                                            "OptionBasePressed",
                                            "Selected",
                                            "Unselected",
                                            "OptionBaseFocusedRed",
                                        });
                DebugLog.Info("ButtonStateBase: Atlas created");
            }
            btn.atlas = _atlas;
            btn.playAudioEvents = true;
            btn.relativePosition = new Vector3(131, 38);

            SetDeactivatedStateSprites(btn);

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

        #region sprites
        protected void SetActivedStateSprites(UIButton btn)
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
        protected void SetDeactivatedStateSprites(UIButton btn)
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

        #endregion

        #region events

        private bool SubscribeEvents()
        {
            if (Button == null)
            {
                return false;
            }
            Button.eventClick += OnButtonClicked;

            if (BuiltinTabstrip == null)
            {
                UnsubscribeEvents();
                return false;
            }
            BuiltinTabstrip.eventSelectedIndexChanged += OnBuiltinTabstripSelectedIndexChanged;

            return true;
        }



        private void UnsubscribeEvents()
        {
            if (Button != null)
            {
                Button.eventClick -= OnButtonClicked;
            }
            if (BuiltinTabstrip != null)
            {
                BuiltinTabstrip.eventSelectedIndexChanged -= OnBuiltinTabstripSelectedIndexChanged;
            }
        }

        protected virtual void OnButtonClicked(UIComponent component, UIMouseEventParameter args)
        {
        }
        protected virtual void OnBuiltinTabstripSelectedIndexChanged(UIComponent component, int value)
        {

        }

        protected virtual void OnInitialized()
        {
        }
        #endregion
    }
}