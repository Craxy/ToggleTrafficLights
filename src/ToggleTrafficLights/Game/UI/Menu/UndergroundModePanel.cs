using System;
using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.Menu
{
    public class UndergroundModePanel : UIPanel
    {
        private UICheckBox _cbUnderground;
        private UICheckBox _cbOverground;

        public UndergroundModePanel()
        {

        }

        #region Overrides of Monobehavior

        public override void OnEnable()
        {
            base.OnEnable();

            Options.instance.GroundModeChanged += OnGroundModeChanged;
            UpdateCheckBoxes();
        }

        public override void OnDisable()
        {
            Options.instance.GroundModeChanged -= OnGroundModeChanged;

            base.OnDisable();

        }

        public static UndergroundModePanel GetOrCreate()
        {
            var v = UIView.GetAView();
            return v.FindUIComponent<UndergroundModePanel>(Name)
                   ?? ((UndergroundModePanel) v.AddUIComponent(typeof (UndergroundModePanel)));

//            var parent = UIView.GetAView().FindUIComponent("Esc");
//            return parent.Find<UndergroundModePanel>(Name) 
//                   ?? parent.AddUIComponent<UndergroundModePanel>();
        }

        public static UndergroundModePanel Get()
        {
            var v = UIView.GetAView();
            if (v == null)
            {
                return null;
            }

            return v.FindUIComponent<UndergroundModePanel>(Name);
        }

        public static string Name = "ToggleTrafficLightsUndergroundModePanel";

        public override void Start()
        {
            base.Start();

            Options.Ensure();

            name = Name;
            backgroundSprite = "InfoPanelBack";
            size = new Vector2(255f, 22f);
//            relativePosition = new Vector3(-235f, 14, 0);
            relativePosition = new Vector3(200f, 14, 0);

            _cbOverground = CreateCheckbox(Name + "Overground", "Overground", "(Dis)Allow toggling above ground", new Vector2(2f, 3f), 120f);
            _cbUnderground = CreateCheckbox(Name + "Underground", "Underground", "(Dis)Allow toggling below ground", new Vector2(_cbOverground.width + 5f, 3f), 120f);

            _cbOverground.eventCheckChanged += OnOvergroundChanged;
            _cbUnderground.eventCheckChanged += OnUndergroundChanged;

            UpdateCheckBoxes();
        }

        private UICheckBox CreateCheckbox(string name, string text, string tooltip, Vector2 position, float length)
        {

            var cb = AddUIComponent<UICheckBox>();
            cb.name = name;
            cb.tooltip = tooltip;
            cb.relativePosition = position;
            cb.size = new Vector2(length, 21f);
            cb.isVisible = true;
            cb.canFocus = true;
            cb.isInteractive = true;

            var spriteUnchecked = cb.AddUIComponent<UISprite>();
            spriteUnchecked.name = name + "SpriteUnchecked";
            spriteUnchecked.size = new Vector2(16f, 16f);
            spriteUnchecked.relativePosition = new Vector3(0, 0);
            spriteUnchecked.spriteName = "ToggleBase";
            spriteUnchecked.isVisible = true;

            var spriteChecked = cb.AddUIComponent<UISprite>();
            spriteChecked.name = name + "SpriteChecked";
            spriteChecked.size = new Vector2(16f, 16f);
            spriteChecked.relativePosition = new Vector3(0, 0);
            spriteChecked.spriteName = "ToggleBaseFocused";
            cb.checkedBoxObject = spriteChecked;

            var lbl = cb.AddUIComponent<UILabel>();
            lbl.name = name + "Label";
            lbl.text = text;
            lbl.relativePosition = new Vector3(27,2,0);
            lbl.size = new Vector3(length - lbl.relativePosition.x - 2,18);
            lbl.isVisible = true;

            cb.isChecked = false;


            return cb;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        #endregion

        private bool _ignoreChangedEvents = false;
        private void OnGroundModeChanged(Options.GroundMode groundMode)
        {
            UpdateCheckBoxes();
        }
        public void UpdateCheckBoxes()
        {
            _ignoreChangedEvents = true;
            try
            {


                if (_cbUnderground != null)
                {
                    _cbUnderground.isChecked = Options.instance.UsedGroundMode.IsFlagSet(Options.GroundMode.Underground);
                }
                if (_cbOverground != null)
                {
                    _cbOverground.isChecked = Options.instance.UsedGroundMode.IsFlagSet(Options.GroundMode.Ground);
                }
            }
            finally
            {
                _ignoreChangedEvents = false;
            }
        }


        private void OnOvergroundChanged(UIComponent _, bool __)
        {
            OnCheckboxChanged(false);
        }        
        private void OnUndergroundChanged(UIComponent _, bool __)
        {
            OnCheckboxChanged(true);
        }
        private void OnCheckboxChanged(bool undergroundChanged)
        {
            if(_ignoreChangedEvents)
            {
                return;
            }

            var o = _cbOverground.isChecked;
            var u = _cbUnderground.isChecked;

            // nothing changed
            if (o == Options.instance.UsedGroundMode.IsFlagSet(Options.GroundMode.Ground) 
                && u == Options.instance.UsedGroundMode.IsFlagSet(Options.GroundMode.Underground))
            {
                return;
            }

            if (!o && !u)
            {
                //not valid, revert
                if (undergroundChanged)
                {
                    _cbUnderground.isChecked = true;
                }
                else
                {
                    _cbOverground.isChecked = true;
                }
                return;
            }

            var mode = Options.GroundMode.Ground;
            if(o && u)
            {
                mode = Options.GroundMode.All;
            }
            else if (o)
            {
                mode = Options.GroundMode.Ground;
            }
            else if (u)
            {
                mode = Options.GroundMode.Underground;
            }

            Options.instance.UsedGroundMode = mode;
        }
    }
}