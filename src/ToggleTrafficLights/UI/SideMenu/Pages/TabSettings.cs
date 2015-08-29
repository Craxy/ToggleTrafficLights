using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.SideMenu.Pages
{
    public interface ITabSettings
    {
        Vector4 ContentPadding { get; }
        float VerticalSpaceBetweenLines { get; }
        float VerticalSpaceAfterGroup { get; }
        string DefaultRowSeparator { get; }
        float ContentRowIntendation { get; }
        float HeaderRowTextScale { get; }
        float ContentRowTextScale { get; }
    }

    public class TabSettings : ITabSettings
    {
        public Vector4 ContentPadding => new Vector4(5.0f, 5.0f, 5.0f, 5.0f);

        public virtual float VerticalSpaceBetweenLines => 2.0f;
        public virtual float VerticalSpaceAfterGroup => 5.0f;

        public virtual string DefaultRowSeparator => ":";
        public virtual float ContentRowIntendation => 7.5f;

        public virtual float HeaderRowTextScale => 1.0f;
        public virtual float ContentRowTextScale => 0.8125f;
    }
}