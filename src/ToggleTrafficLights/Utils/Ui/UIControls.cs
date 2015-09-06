using ColossalFramework.UI;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Ui
{
    public static class UIControls
    {
        public static UIDropDown CreateDropDown(UIComponent parent)
        {
            var cb = parent.AddUIComponent<UIDropDown>();
            cb.size = new Vector2(150.0f, 25.0f);
            cb.zOrder = 1;
            cb.listBackground = "GenericPanelLight";
            cb.itemHover = "ListItemHover";
            cb.itemHighlight = "ListItemHighlight";
            cb.normalBgSprite = "ButtonMenu";
            cb.disabledBgSprite = "ButtonMenuDisabled";
            cb.hoveredBgSprite = "ButtonMenuHovered";
            cb.focusedBgSprite = "ButtonMenu";
            cb.itemHeight = (int) cb.height;
            cb.listWidth = (int) cb.width;
            //TODO: list height
            cb.foregroundSpriteMode = UIForegroundSpriteMode.Stretch;
            cb.popupColor = new Color32(45, 52, 61, 255);
            cb.popupTextColor = new Color32(170, 170, 170, 255);
            cb.verticalAlignment = UIVerticalAlignment.Middle;
            cb.horizontalAlignment = UIHorizontalAlignment.Left;
            cb.selectedIndex = 0;
            cb.textFieldPadding = new RectOffset(8, 0, 8, 0);
            cb.itemPadding = new RectOffset(14, 0, 8, 0);


            var btn = cb.AddUIComponent<UIButton>();
            cb.triggerButton = btn;
            btn.text = "";
            btn.zOrder = 0;
            btn.size = cb.size;
            btn.relativePosition = new Vector3(0f, 0f);
            btn.textVerticalAlignment = UIVerticalAlignment.Middle;
            btn.textHorizontalAlignment = UIHorizontalAlignment.Left;
            btn.normalFgSprite = "IconDownArrow";
            btn.hoveredFgSprite = "IconDownArrowHovered";
            btn.pressedFgSprite = "IconDownArrowPressed";
            btn.focusedFgSprite = "IconDownArrowFocused";
            btn.disabledFgSprite = "IconDownArrowDisabled";
            btn.foregroundSpriteMode = UIForegroundSpriteMode.Fill;
            btn.horizontalAlignment = UIHorizontalAlignment.Right;
            btn.verticalAlignment = UIVerticalAlignment.Middle;

            cb.eventSizeChanged += (c, t) =>
            {
	            btn.size = t; 
	            cb.listWidth = (int)t.x;
            };

            return cb;
        }

        public static void DestroyAllComponents(this UIComponent parent)
        {
            if (parent != null)
            {
                foreach (var c in parent.GetComponentsInChildren<UIComponent>())
                {
                    if (c != null)
                    {
                        Object.Destroy(c.gameObject);
                    }
                }
            }
        }
    }
}