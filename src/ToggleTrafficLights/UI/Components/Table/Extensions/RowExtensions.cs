using System;
using System.Linq;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table.Entries;
using JetBrains.Annotations;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table.Extensions
{
    public static class RowExtensions
    {
        #region creation
        private static T AddUIComponent<T>([NotNull] this Row row)
            where T : UIComponent
        {
            return row.Root.AddUIComponent<T>();
        }

        public static Row Empty([NotNull] this Row row)
        {
            return Row.CreateEmpty(row.Root);
        }

        public static Row AddEntry([NotNull] this Row row, [NotNull] Entry entry)
        {
            return Row.AppendEntry(row, entry);
        }

        public static Row PrependEntry([NotNull] this Row row, [NotNull] Entry entry)
        {
            return Row.PrependEntry(row, entry);
        }
        public static Row Concat(this Row left, Row right)
        {
            return Row.Concat(left, right);
        }
        public static Row AddLabel([NotNull] this Row row, [NotNull] string text, [CanBeNull] Action<UILabel> setup = null)
        {
            var lbl = row.AddUIComponent<UILabel>();
            lbl.text = text;
            setup?.Invoke(lbl);

            return row.AddEntry(new LabelEntry(lbl));
        }
        public static Row AddButton([NotNull] this Row row, [NotNull] string text, [CanBeNull] Action onClick, [CanBeNull] Action<UIButton> setup = null)
        {
            var btn = row.AddUIComponent<UIButton>();
            btn.text = text;
            if (onClick != null)
            {
                btn.eventClick += (_, __) => onClick();
            }
            setup?.Invoke(btn);

            return row.AddEntry(new ButtonEntry(btn));
        }

        public static Row AddHorizontalSpace([NotNull] this Row row, float width, [CanBeNull] Action<UIPanel> setup = null)
        {
            var pnl = row.AddUIComponent<UIPanel>();
            pnl.width = width;
            setup?.Invoke(pnl);

            return row.AddEntry(new HorizontalSpaceEntry(pnl));
        }
        public static Row AddIndention([NotNull] this Row row, float height, [CanBeNull] Action<UIPanel> setup = null)
        {
            return row.AddHorizontalSpace(height, setup);
        }
        public static Row AddVerticalSpace([NotNull] this Row row, float height, Action<UIPanel> setup = null)
        {
            var pnl = row.AddUIComponent<UIPanel>();
            pnl.height = height;
            pnl.width = row.Root.width;
            setup?.Invoke(pnl);

            return row.AddEntry(new HorizontalSpaceEntry(pnl));
        }
        #endregion

        #region setup
        public static Row SetupAllOfType<TEntry, TComponent>([NotNull] this Row row, [CanBeNull] Action<TComponent> setup = null)
            where TEntry : Entry<TComponent>
            where TComponent : UIComponent
        {
            if (setup != null)
            {
                row.Entries.OfType<TEntry>().Select(UIComponent).ForEach(setup);
            }

            return row;
        }
        public static Row SetupAllOfType<T>([NotNull] this Row row, [CanBeNull] Action<T> setup = null)
            where T : UIComponent
        {
            return row.SetupAllOfType<Entry<T>, T>(setup);
        }
        #endregion

        #region Helper
        private static UIComponent Component<T>([NotNull] T entry)
            where T : Entry
        {
            return entry.Component;
        }

        private static T UIComponent<T>([NotNull] Entry<T> entry)
            where T : UIComponent
        {
            return entry.UIComponent;
        }
        #endregion
    }
}