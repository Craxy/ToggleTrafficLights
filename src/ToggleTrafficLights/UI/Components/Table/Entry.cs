using ColossalFramework.UI;
using JetBrains.Annotations;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table
{
    public class Entry
    {
        public UIComponent Component { get; }
        internal Entry([NotNull] UIComponent component)
        {
            Component = component;
        }
    }

    public class Entry<T> : Entry
        where T : UIComponent
    {
        public T UIComponent => (T)Component;
        public Entry([NotNull] T component) : base(component)
        {
        }
    }
}