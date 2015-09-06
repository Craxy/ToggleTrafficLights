using System;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table.Extensions;
using JetBrains.Annotations;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table.Entries
{
    public static class Create
    {
        #region entries
        public static LabelEntry Label([NotNull] Row row, [NotNull] string text, [CanBeNull] Action<UILabel> setup = null)
        {
            var lbl = row.AddUIComponent<UILabel>();
            lbl.text = text;
            setup?.Invoke(lbl);

            return new LabelEntry(lbl);
        }
        public static ButtonEntry Button([NotNull] Row row, [NotNull] string text, [CanBeNull] Action onClick, [CanBeNull] Action<UIButton> setup = null)
        {
            var btn = row.AddUIComponent<UIButton>();
            btn.text = text;
            if (onClick != null)
            {
                btn.eventClick += (_, __) => onClick();
            }
            setup?.Invoke(btn);

            return new ButtonEntry(btn);
        }

        public static CustomPanelEntry<T> CustomPanel<T>([NotNull] Row row, Action<T> setup = null)
            where T : UIPanel
        {
            var pnl = row.AddUIComponent<T>();
            setup?.Invoke(pnl);

            return new CustomPanelEntry<T>(pnl);
        }
        public static PanelEntry Panel([NotNull] Row row, float width, float height, [CanBeNull] Action<UIPanel> setup = null)
        {
            var pnl = row.AddUIComponent<UIPanel>();
            pnl.width = width;
            pnl.height = height;
            setup?.Invoke(pnl);

            return new PanelEntry(pnl);
        }
        public static VerticalSpaceEntry VerticalSpace([NotNull] Row row, float height, [CanBeNull] Action<UIPanel> setup = null)
        {
            var pnl = row.AddUIComponent<UIPanel>();
            pnl.height = height;
            pnl.width = row.Root.width; //"dummy" width for easier selection
            setup?.Invoke(pnl);

            return new VerticalSpaceEntry(pnl);
        }
        public static HorizontalSpaceEntry HorizontalSpace([NotNull] Row row, float width, [CanBeNull] Action<UIPanel> setup = null)
        {
            var pnl = row.AddUIComponent<UIPanel>();
            pnl.width = width;
            pnl.height = 2.5f;  //dummy height for easier selection
            setup?.Invoke(pnl);

            return new HorizontalSpaceEntry(pnl);
        }
        #endregion
    }
}