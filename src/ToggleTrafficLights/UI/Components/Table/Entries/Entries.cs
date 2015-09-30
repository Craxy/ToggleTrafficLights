using System;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.ModTools;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;
using JetBrains.Annotations;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table.Entries
{
    public class TextEntry<T> : Entry<T> 
        where T : UITextComponent
    {
        public string Text
        {
            get { return UIComponent.text; }
            set { UIComponent.text = value; }
        }

        protected TextEntry([NotNull] T component) : base(component)
        {
        }
    }

    public class LabelEntry : TextEntry<UILabel>
    {
        public UILabel Label => UIComponent;
        public LabelEntry([NotNull] UILabel component) : base(component)
        {
        }

        #region Overrides of Entry
        public override string ToString()
        {
            return $"{GetType().Name}(\"{Label.text}\")";
        }
        #endregion
    }

    public class ButtonEntry : TextEntry<UIButton>
    {
        public UIButton Button => UIComponent;
        public ButtonEntry([NotNull] UIButton component) : base(component)
        {
        }

        #region Overrides of Entry
        public override string ToString()
        {
            return $"{GetType().Name}(\"{Button.text}\")";
        }
        #endregion
    }

    public class PanelEntry : Entry<UIPanel>
    {
        public UIPanel Panel => UIComponent;
        public PanelEntry([NotNull] UIPanel component) : base(component)
        {
        }

        #region Overrides of Entry
        public override string ToString()
        {
            return $"{GetType().Name}(h:{Panel.height};w:{Panel.width})";
        }
        #endregion
    }

    public class CustomPanelEntry<T> : Entry<T>
        where T : UIPanel
    {
        public T Panel => UIComponent;
        public CustomPanelEntry([NotNull] T component) : base(component)
        {
        }

        #region Overrides of Entry
        public override string ToString()
        {
            return $"{GetType().Name} of {typeof(T).Name}(h:{Panel.height};w:{Panel.width})";
        }
        #endregion
    }

    // not inherited from PanelEntry so it's easier to filter
    public class VerticalSpaceEntry : Entry<UIPanel>
    {
        public UIPanel Panel => UIComponent;
        public VerticalSpaceEntry([NotNull] UIPanel component) : base(component)
        {
        }

        #region Overrides of Entry
        public override string ToString()
        {
            return $"{GetType().Name}(h:{Height})";
        }
        #endregion
    }

    // not inherited from PanelEntry so it's easier to filter
    public class HorizontalSpaceEntry : Entry<UIPanel>
    {
        public UIPanel Panel => UIComponent;
        public HorizontalSpaceEntry([NotNull] UIPanel component) : base(component)
        {
        }

        #region Overrides of Entry
        public override string ToString()
        {
            return $"{GetType().Name}(w:{Width})";
        }
        #endregion
    }

    public class ColorFieldEntry : Entry<UIColorField>
    {
        public Color Color
        {
            get { return ColorPanel.selectedColor; }
            set { ColorPanel.selectedColor = value; }
        }

        public event Action<Color> ColorChanged;
        private void OnColorChanged(UIComponent component, Color color)
        {
            ColorChanged?.Invoke(color);
        }

        public UIColorField ColorPanel => UIComponent;
        public ColorFieldEntry([NotNull] UIColorField component) : base(component)
        {
            component.eventSelectedColorChanged += OnColorChanged;
        }

        #region Overrides of Entry
        public override string ToString()
        {
            return $"{GetType().Name}(color:{ColorPanel.selectedColor})";
        }

        /// <summary>
        /// UIColorField is slightly shifted down -> push ColorField 2px up
        /// </summary>
        public override Vector3 RelativePosition
        {
            get { return new Vector3(Component.relativePosition.x, Component.relativePosition.y + 2.0f); }
            set { Component.relativePosition = new Vector3(value.x, value.y - 2.0f); }
        }

        #endregion
    }

    public class TextFieldEntry : Entry<UITextField>
    {
        public UITextField TextField => UIComponent;
        public TextFieldEntry([NotNull] UITextField component) : base(component)
        {
        }

        #region Overrides of Entry
        public override string ToString()
        {
            return $"{GetType().Name}(text:\"{TextField.text}\")";
        }
        #endregion
    }

    public class DropDownEntry : Entry<UIDropDown>
    {
        public UIDropDown DropDown => UIComponent;
        public DropDownEntry([NotNull] UIDropDown component) : base(component)
        {
        }
    }

    //todo: text field etc
}