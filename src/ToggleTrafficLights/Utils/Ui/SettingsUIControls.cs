using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.Menu.Components;
using ICities;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Ui
{
    public static class SettingsUIControls
    {
        public static UIComponent GetRoot(this UIHelperBase helper)
        {
            return (UIComponent) ((UIHelper)helper).self;
        }

        #region wrappers

        public static UIHelperBase AddUIGroup(this UIHelperBase helper, string name)
        {
//            var hb = helper.AddGroup(name);
//
//            DebugLog.Info("UIHelperBase is {0}", hb);
//
//            var h = hb as UIHelper;
//
//            DebugLog.Info("UIHelper is {0}", h);
//
//            if (h != null)
//            {
//                DebugLog.Info("UIHelper.self is {0}", h.self);
//            }
//            else
//            {
//                DebugLog.Info("UIHelper is null :(");
//            }
            
            return helper.AddGroup(name);
        }

        public static UIPanel AddUISpace(this UIHelperBase helper, int height)
        {
//            var pnl = helper.GetRoot().AddUIComponent<UIPanel>();
//            pnl.name = "TestSpace";
//            pnl.isInteractive = false;
//            pnl.height = height;
//            pnl.color = Color.red;
//            return pnl;

            return helper.AddSpace(height) as UIPanel;
        }

        public static UIButton AddUIButton(this UIHelperBase helper, string text, OnButtonClicked onClicked)
        {
            return helper.AddButton(text, onClicked) as UIButton;
        }
        public static UICheckBox AddUICheckbox(this UIHelperBase helper, string text, bool defaultValue, OnCheckChanged onSelectionChanged)
        {
            return helper.AddCheckbox(text, defaultValue, onSelectionChanged) as UICheckBox;
        }
        public static UISlider AddUISlider(this UIHelperBase helper, string text, float min, float max, float step, float defaultValue, OnValueChanged onValueChanged)
        {
            return helper.AddSlider(text, min, max, step, defaultValue, onValueChanged) as UISlider;
        }
        public static UIDropDown AddUIDropdown(this UIHelperBase helper, string text, string[] options, int defaultSelection, OnDropdownSelectionChanged onSelectionChanged)
        {
            return helper.AddDropdown(text, options, defaultSelection, onSelectionChanged) as UIDropDown;
        }
        #endregion

        #region custom

        public static UILabel AddUILabel(this UIHelperBase helper, string text)
        {
            var root = helper.GetRoot();

            var lbl = root.AddUIComponent<UILabel>();
            lbl.text = text;

            return lbl;
        }
        public static UIColorPanel AddUIColorPanel(this UIHelperBase helper, string title, Color defaultColor)
        {
            var root = helper.GetRoot();

            // UIColorPanel is not a available to copy in settings editor...
            var colorPanel = root.AddUIComponent<UIColorPanel>();
            colorPanel.Title = title;
            colorPanel.Color = defaultColor;

            return colorPanel;
        }
        #endregion
    }
}