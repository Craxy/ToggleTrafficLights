using System;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Ui;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.Menu.Components
{
    public class UIColorPanel : UIPanel
    {
        public UIColorPanel()
        {
            name = "ColorPanel";
            _color = Color.white;
            Initialized = false;
        }

        #region properties

        public bool Initialized { get; private set; }

        private string _title = "Color";
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnTitleChanged(value);
                }
            }
        }
        public event EventHandler<EventArgs<string>> TitleChanged;
        protected virtual void OnTitleChanged(string title)
        {
            var handler = TitleChanged;
            if (handler != null)
            {
                handler(this, new EventArgs<string>(title));
            }
        }

        private Option<float> _customTitleWidth = Option.None<float>();
        public Option<float> CustomTitleWidth
        {
            get { return _customTitleWidth; }
            set
            {
// ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_customTitleWidth != value)
                {
                    _customTitleWidth = value;
                    OnCustomTitleWidthChanged(value);
                }
            }
        }
        public event EventHandler<EventArgs<Option<float>>> CustomTitleWidthChanged;
        protected virtual void OnCustomTitleWidthChanged(Option<float> w)
        {
            var handler = CustomTitleWidthChanged;
            if (handler != null)
            {
                handler(this, new EventArgs<Option<float>>(w));
            }
        }

        public float TitleWidth
        {
            get
            {
                if (_lblTitle != null)
                {
                    return _lblTitle.width;
                }
                else
                {
                    return 0.0f;
                }
            }
        }


        private Color _color;
        public Color Color
        {
            get { return _color; }
            set
            {
                if(_color != value)
                {
                    _color = value;
                    OnColorChanged(value);
                }
            }
        }

        public event EventHandler<EventArgs<Color>> ColorChanged;
        protected virtual void OnColorChanged(Color c)
        {
            var handler = ColorChanged;
            if (handler != null)
            {
                handler(this, new EventArgs<Color>(c));
            }
        }
        #endregion

        #region controls
        private UILabel _lblTitle = null;
        private UIColorField _cfColor = null;
        private UILabel _lblHashtag = null;
        private UITextField _tfHex = null;
        #endregion

        #region Overrides of UIComponent
        public override void Awake()
        {
            base.Awake();

            SetupControls();
        }

        public override void Start()
        {
            base.Start();
        }

        public override void OnEnable()
        {
            base.OnEnable();

            if (!Initialized)
            {
                SetupControls();
            }

            OnLayout();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            UIControls.DestroyAllComponents(this);
        }

        #endregion

        public void SetupControls()
        {
            const float offset = 5.0f;

            height = 25.0f;

            _lblTitle = AddUIComponent<UILabel>();
            _lblTitle.name = "Caption";
            _lblTitle.text = Title;
            _lblTitle.textScale = 0.9f;
            _lblTitle.relativePosition = new Vector3(0.0f, ((height - _lblTitle.height) / 2.0f));

//            _cfColor = AddUIComponent<UIColorField>();
            _cfColor = Instantiate(FindObjectOfType<UIColorField>().gameObject).GetComponent<UIColorField>();
            AttachUIComponent(_cfColor.gameObject);
            _cfColor.name = "Color";
            _cfColor.size = new Vector2(40.0f, height);
            _cfColor.relativePosition = new Vector3(_lblTitle.width + offset, 0.0f);
            _cfColor.pickerPosition = UIColorField.ColorPickerPosition.RightAbove;
            _cfColor.selectedColor = Color;
            _cfColor.eventSelectedColorChanged += (_, c) => Color = c;

            _lblHashtag = AddUIComponent<UILabel>();
            _lblHashtag.name = "Hashtag";
            _lblHashtag.text = "#";
            _lblHashtag.textScale = 0.9f;
            _lblHashtag.relativePosition = new Vector3(_cfColor.relativePosition.x + _cfColor.width + offset, ((height- _lblHashtag.height) / 2.0f));

            _tfHex = AddUIComponent<UITextField>();
            _tfHex.name = "Hex";
            _tfHex.text = Options.ToggleTrafficLightsTool.HasNoTrafficLightsColor.Value.ToHex(leadingHashtag: false);
            _tfHex.isInteractive = true;
            _tfHex.readOnly = false;
            _tfHex.builtinKeyNavigation = true;
            _tfHex.size = new Vector2(85.0f, 20.0f);
            _tfHex.padding = new RectOffset(6, 6, 3, 3);
            _tfHex.selectionSprite = "EmptySprite";
            _tfHex.normalBgSprite = "TextFieldPanelHovered";
            _tfHex.textColor = new Color32(0, 0, 0, 255);
            _tfHex.maxLength = 8;
            _tfHex.horizontalAlignment = UIHorizontalAlignment.Center;
            _tfHex.relativePosition = new Vector3(_lblHashtag.relativePosition.x + _lblHashtag.width + 1.0f, ((height - _tfHex.height) / 2.0f));
            _tfHex.readOnly = true;

            //register events
            ColorChanged += OnColorChanged;
            TitleChanged += OnTitleChanged;
            CustomTitleWidthChanged += OnCustomTitleWidthChanged;
            eventSizeChanged += OnEventSizeChanged;

            Initialized = true;
        }

        public virtual void OnLayout()
        {
            if (!Initialized || !isEnabled)
            {
                return;
            }

            const float offset = 5.0f;

            DebugLog.Info("OnLayout: CustomTitleWidth={0}", CustomTitleWidth);

            //position controls according to lblTitle width
            if (CustomTitleWidth.IsSome())
            {
                _lblTitle.autoSize = false;
                _lblTitle.width = CustomTitleWidth.GetValue();
            }
            else
            {
                _lblTitle.autoSize = true;
            }
            _lblTitle.relativePosition = new Vector3(0.0f, ((height - _lblTitle.height) / 2.0f));
            _cfColor.size = new Vector2(40.0f, height);
            DebugLog.Info("OnLayout: _lblTitle.width={0}", _lblTitle.width);
            _cfColor.relativePosition = new Vector3(_lblTitle.width + offset, 0.0f);
            _lblHashtag.relativePosition = new Vector3(_cfColor.relativePosition.x + _cfColor.width + offset, ((height - _lblHashtag.height) / 2.0f));
            _tfHex.relativePosition = new Vector3(_lblHashtag.relativePosition.x + _lblHashtag.width + 1.0f, ((height - _tfHex.height) / 2.0f));
        }

        private void OnEventSizeChanged(UIComponent component, Vector2 value)
        {
            OnLayout();
        }

        private void OnCustomTitleWidthChanged(object sender, EventArgs<Option<float>> args)
        {
            OnLayout();
        }


        private void OnTitleChanged(object sender, EventArgs<string> args)
        {
            if (_lblTitle != null)
            {
                _lblTitle.text = args.Value;
            }

            OnLayout();
        }

        private void OnColorChanged(object sender, EventArgs<Color> args)
        {
            var c = args.Value;

            //color picker
            if (_cfColor != null && _cfColor.selectedColor != c)
            {
                _cfColor.selectedColor = c;
            }

            // hex color
            if (_tfHex != null)
            {
                Color currentColor;
                string failure;
                var parsable = _tfHex.text.TryParseHexColor(out currentColor, out failure);
                if (!parsable || currentColor != c)
                {
                    var hex = c.ToHex(leadingHashtag: false);
                    _tfHex.text = hex;
                }
            }
        }
    }
}