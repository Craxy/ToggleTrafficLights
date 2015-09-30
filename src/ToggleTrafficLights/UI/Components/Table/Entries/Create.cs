using System;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table.Extensions;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table.Entries
{
    public static class Create
    {
        #region entries

        public static LabelEntry Label([NotNull] Row row, [NotNull] string text, [CanBeNull] Action<UILabel> setup = null)
        {
            return new LabelEntry(row.AddUIComponent<UILabel>())
                .Do(lbl => lbl.Text = text)
                .DoOnComponent(setup)
                ;
        }

        public static ButtonEntry Button([NotNull] Row row, [NotNull] string text, [CanBeNull] Action onClick,
            [CanBeNull] Action<UIButton> setup = null)
        {
            return new ButtonEntry(row.AddUIComponent<UIButton>())
                .Do(btn => btn.Text = text)
                .DoIfNotNull(onClick, btn => btn.Button.eventClick += (_, __) => onClick())
                .DoOnComponent(setup)
                ;
        }

        public static CustomPanelEntry<T> CustomPanel<T>([NotNull] Row row, Action<T> setup = null)
            where T : UIPanel
        {
            return new CustomPanelEntry<T>(row.AddUIComponent<T>())
                .DoOnComponent(setup)
                ;
        }

        public static PanelEntry Panel([NotNull] Row row, float width, float height, [CanBeNull] Action<UIPanel> setup = null)
        {
            return new PanelEntry(row.AddUIComponent<UIPanel>())
                .CWidth(width).CHeight(width)
                .DoOnComponent(setup)
                ;
        }

        public static VerticalSpaceEntry VerticalSpace([NotNull] Row row, float height, [CanBeNull] Action<UIPanel> setup = null)
        {
            return new VerticalSpaceEntry(row.AddUIComponent<UIPanel>())
                .CHeight(height)
                .CWidth(row.Root.width) //"dummy" width for easier selection
                .DoOnComponent(setup)
                ;
        }

        public static HorizontalSpaceEntry HorizontalSpace([NotNull] Row row, float width, [CanBeNull] Action<UIPanel> setup = null)
        {
            return new HorizontalSpaceEntry(row.AddUIComponent<UIPanel>())
                .CWidth(width)
                .CHeight(2.5f)
                .DoOnComponent(setup) //dummy height for easier selection
                ;
        }

        /// <summary>
        /// Must be called when another UIColorField is available to copy (like ingame -- main menu does not work)
        /// </summary>
        public static ColorFieldEntry ColorField([NotNull] Row row, Color initialColor, [CanBeNull] Action<Color> onColorChanged,
            float width, float height, [CanBeNull] Action<UIColorField> setup = null)
        {
            var cf = Object.Instantiate(Object.FindObjectOfType<UIColorField>().gameObject).GetComponent<UIColorField>();
            row.Root.AttachUIComponent(cf.gameObject);
            cf.pickerPosition = UIColorField.ColorPickerPosition.RightAbove;
            cf.selectedColor = initialColor;
            if (onColorChanged != null)
            {
                cf.eventSelectedColorChanged += (_, c) => onColorChanged(c);
            }

            return new ColorFieldEntry(cf)
                .CName("Color")
                .CSize(new Vector2(height, width))
                .DoOnComponent(setup)
                ;
        }

        public static TextFieldEntry TextField([NotNull] Row row, [NotNull] string initialText, float width,
            [CanBeNull] Action<UITextField> setup = null)
        {
            var tb = row.AddUIComponent<UITextField>();
            tb.text = initialText;
            tb.width = width;
            tb.selectionSprite = "EmptySprite";
            tb.normalBgSprite = "TextFieldPanelHovered";
            tb.normalBgSprite = "TextFieldPanelHovered";

            //TODO: implement
            throw new NotImplementedException();

            return new TextFieldEntry(tb);
        }

        public static DropDownEntry DropDown([NotNull] Row row, [NotNull] string[] values, int selectedIndex,
            [CanBeNull] Action<int> onSelectedIndexChanged, float width = 150.0f, float height = 25.0f,
            [CanBeNull] Action<UIDropDown> setupDropDown = null,
            [CanBeNull] Action<UIButton> setupDropDownButton = null
            )
        {
            var cb = CreateDefaultDropDown(row.Root, width, height);
            values.ForEach(v => cb.AddItem(v));
            cb.selectedIndex = selectedIndex;
            if (onSelectedIndexChanged != null)
            {
                cb.eventSelectedIndexChanged += (_, idx) => onSelectedIndexChanged(idx);
            }

            return new DropDownEntry(cb)
                .CName("DropDown")
                .DoOnComponent(setupDropDown)
                .Do(e => setupDropDownButton?.Invoke((UIButton) e.DropDown.triggerButton))
                ;
        }

        private static UIDropDown CreateDefaultDropDown(UIComponent parent, float width, float height)
        {
            var cb = parent.AddUIComponent<UIDropDown>();
            cb.size = new Vector2(width, height);
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
            cb.textFieldPadding = new RectOffset(4, 0, 4, 0);
            cb.itemPadding = new RectOffset(8, 0, 4, 0);

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
            btn.foregroundSpriteMode = UIForegroundSpriteMode.Scale;
            btn.horizontalAlignment = UIHorizontalAlignment.Right;
            btn.verticalAlignment = UIVerticalAlignment.Middle;

            cb.eventSizeChanged += (c, t) =>
            {
                btn.size = t;
                cb.listWidth = (int) t.x;
                DebugLog.Warning("Size changed");
            };

            return cb;
        }

        #endregion
    }
}