using System.Collections;
using System.Reflection;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.StateMachine.States
{
    public abstract class ButtonStateBase : StateBase
    {
        #region fields

        protected UIButton Button = null;
        protected UITabstrip BuiltinTabstrip = null;
        protected UIComponent RoadsOptionPanel = null;
        private const string ButtonName = "ToggleTrafficLightsButton";

        protected bool Initialized = false;
        #endregion

        #region Overrides of StateBase

        public override void OnEntry()
        {
            base.OnEntry();

            Initialize();
        }

        public override void OnExit()
        {
            DestroyView();

            base.OnExit();
        }

        public override void OnUpdate()
        {
            if (!Initialized)
            {
                Initialize();
            }
        }
        #endregion

        #region UI
//        protected bool OpenRoadsPanelIfNotVisible()
//        {
//            if (RoadsOptionPanel == null)
//            {
//                return false;
//            }
//
//            if (!RoadsOptionPanel.isVisible)
//            {
//                CitiesHelper.ClickOnRoadsButton();
//            }
//
//            return true;
//        }
//
//        protected bool CloseRoadsPanelIfVisible()
//        {
//            
//        }
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

//            DebugLog.Info("RoadsPanel: {0}", );

//            RoadsOptionPanel = UiHelper.FindComponent<UIComponent>("RoadsOptionPanel", null, UiHelper.FindOptions.NameContains);
            RoadsOptionPanel = UiHelper.FindComponent<UIComponent>("RoadsOptionPanel(RoadsPanel)", null, UiHelper.FindOptions.NameContains);
//            RoadsOptionPanel = UIView.Find<UIComponent>("RoadsOptionPanel");
//            {
//                var fi = typeof(RoadsPanel).GetField("m_RoadsOptionPanel", BindingFlags.Instance | BindingFlags.NonPublic);
//                RoadsOptionPanel = fi == null ? null : (UIComponent) fi.GetValue(RoadsPanel);
//            }
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
            if (!RoadsOptionPanel.gameObject.activeInHierarchy)
            {
                DebugLog.Info("State {0}: Initialize: ToolMode is not active in hierarchy", State);
                return false;
            }

            Button = UiHelper.FindComponent<UIButton>(ButtonName);
            if (Button != null)
            {
                DestroyView();
            }
            Button = CreateButton(RoadsOptionPanel);

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
                Button.Hide();
                if (RoadsOptionPanel != null)
                {
                    RoadsOptionPanel.RemoveUIComponent(Button);
                }
                //Delay destroying of button -- otherwise if Tooltip is visible it will not be removed and stays open until another tooltip opens
                Button.StartCoroutine(DeleteButton(Button));
//                Object.Destroy(Button);
            }

            RoadsOptionPanel = null;
            BuiltinTabstrip = null;
            Button = null;
            Initialized = false;
        }

        private IEnumerator DeleteButton(UIButton button)
        {
            if (button != null)
            {
                yield return null;
                Object.Destroy(button);
            }
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

        protected virtual void OnButtonClicked(UIComponent component, UIMouseEventParameter eventParam)
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