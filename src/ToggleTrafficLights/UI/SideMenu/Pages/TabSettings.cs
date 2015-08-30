using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.SideMenu.Pages
{
    public interface ITabSettings
    {
        float VerticalSpaceBetweenLines { get; }
        float VerticalSpaceAfterGroup { get; }
        string DefaultRowSeparator { get; }
        float ContentRowIndentation { get; }
        float IndentationBetweenColumns { get; }
        float HeaderRowTextScale { get; }
        float ContentRowTextScale { get; }
    }

    public class TabSettings : ITabSettings
    {
        public virtual float VerticalSpaceBetweenLines => 2.0f;
        public virtual float VerticalSpaceAfterGroup => 5.0f;

        public virtual string DefaultRowSeparator => ":";
        public virtual float ContentRowIndentation => 7.5f;
        public virtual float IndentationBetweenColumns => 2.0f;

        public virtual float HeaderRowTextScale => 1.0f;
        public virtual float ContentRowTextScale => 0.8125f;
    }
}