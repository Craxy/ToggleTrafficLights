using ColossalFramework.UI;
using JetBrains.Annotations;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table.Entries
{
    public class LabelEntry : Entry<UILabel>
    {
        public UILabel Label => UIComponent;
        public LabelEntry([NotNull] UILabel component) : base(component)
        {
        }
    }

    public class ButtonEntry : Entry<UIButton>
    {
        public UIButton Button => UIComponent;
        public ButtonEntry([NotNull] UIButton component) : base(component)
        {
        }
    }

    public class PanelEntry : Entry<UIPanel>
    {
        public UIPanel Panel => UIComponent;
        public PanelEntry([NotNull] UIPanel component) : base(component)
        {
        }
    }

    // not inherited from PanelEntry so it's easier to filter
    public class VerticalSpaceEntry : Entry<UIPanel>
    {
        // and ignoring immutability....well done....
        public float Height
        {
            get { return Panel.height; }
            set { Panel.height = Mathf.Max(0.0f, value); }
        }
        public UIPanel Panel => UIComponent;
        public VerticalSpaceEntry([NotNull] UIPanel component) : base(component)
        {
        }
    }

    // not inherited from PanelEntry so it's easier to filter
    public class HorizontalSpaceEntry : Entry<UIPanel>
    {
        // and ignoring immutability....well done....
        public float Width
        {
            get { return Panel.width; }
            set { Panel.width = Mathf.Max(0.0f, value); }
        }
        public UIPanel Panel => UIComponent;
        public HorizontalSpaceEntry([NotNull] UIPanel component) : base(component)
        {
        }
    }

    public class ColorFieldEntry : Entry<UIColorField>
    {
        public UIColorField ColorField => UIComponent;
        public ColorFieldEntry([NotNull] UIColorField component) : base(component)
        {
        }
    }

    //todo: text field etc
}