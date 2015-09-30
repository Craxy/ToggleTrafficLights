using System;
using System.Linq;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table.Entries;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;
using JetBrains.Annotations;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table.Extensions
{
    /// <summary>
    /// Naming convention for adding:
    ///     Add: Generic position (-> for delegate
    ///     Append: end
    ///     Prepend: front
    /// partial application muesste man koennen...
    /// </summary>
    public static class RowExtensions
    {
        #region creation

        #region basics

        internal static T AddUIComponent<T>([NotNull] this Row row)
            where T : UIComponent
        {
            return row.Root.AddUIComponent<T>();
        }

        public delegate Row AddEntry([NotNull] Row row, [NotNull] Entry entry);

        public static Row Empty([NotNull] this Row row)
        {
            return Row.CreateEmpty(row.Root);
        }

        public static Row PrependEntry([NotNull] this Row row, [NotNull] Entry entry)
        {
            return Row.PrependEntry(row, entry);
        }

        public static Row AppendEntry([NotNull] this Row row, [NotNull] Entry entry)
        {
            return Row.AppendEntry(row, entry);
        }

        public static Row Concat([NotNull] this Row left, [NotNull] Row right)
        {
            return Row.Concat(left, right);
        }

        #endregion

        public static Row AddTag([NotNull] this Row row, [NotNull] string tag)
        {
            return Row.ChangeTag(row, tag);
        }

        public static Row Tag([NotNull] this Row row, [NotNull] string tag)
        {
            return row.AddTag(tag);
        }

        public static Row AddLabel([NotNull] this Row row, [NotNull] AddEntry add, [NotNull] string text,
            [CanBeNull] Action<UILabel> setup = null)
        {
            return add(row, Create.Label(row, text, setup));
        }

        public static Row AppendLabel([NotNull] this Row row, [NotNull] string text, [CanBeNull] Action<UILabel> setup = null)
        {
            return row.AddLabel(AppendEntry, text, setup);
        }

        public static Row PrependLabel([NotNull] this Row row, [NotNull] string text, [CanBeNull] Action<UILabel> setup = null)
        {
            return row.AddLabel(PrependEntry, text, setup);
        }

        public static Row AddButton([NotNull] this Row row, [NotNull] AddEntry add, [NotNull] string text, [CanBeNull] Action onClick,
            [CanBeNull] Action<UIButton> setup = null)
        {
            return add(row, Create.Button(row, text, onClick, setup));
        }

        public static Row AppendButton([NotNull] this Row row, [NotNull] string text, [CanBeNull] Action onClick,
            [CanBeNull] Action<UIButton> setup = null)
        {
            return row.AddButton(AppendEntry, text, onClick, setup);
        }

        public static Row PrependButton([NotNull] this Row row, [NotNull] string text, [CanBeNull] Action onClick,
            [CanBeNull] Action<UIButton> setup = null)
        {
            return row.AddButton(PrependEntry, text, onClick, setup);
        }

        public static Row AddHorizontalSpace([NotNull] this Row row, [NotNull] AddEntry add, float width,
            [CanBeNull] Action<UIPanel> setup = null)
        {
            return add(row, Create.HorizontalSpace(row, width, setup));
        }

        public static Row AppendHorizontalSpace([NotNull] this Row row, float width, [CanBeNull] Action<UIPanel> setup = null)
        {
            return row.AddHorizontalSpace(AppendEntry, width, setup);
        }

        public static Row PrependHorizontalSpace([NotNull] this Row row, float width, [CanBeNull] Action<UIPanel> setup = null)
        {
            return row.AddHorizontalSpace(PrependEntry, width, setup);
        }

        public static Row AddVerticalSpace([NotNull] this Row row, [NotNull] AddEntry add, float height, Action<UIPanel> setup = null)
        {
            return add(row, Create.VerticalSpace(row, height, setup));
        }

        public static Row AppendVerticalSpace([NotNull] this Row row, float height, Action<UIPanel> setup = null)
        {
            return row.AddVerticalSpace(AppendEntry, height, setup);
        }

        public static Row PrependVerticalSpace([NotNull] this Row row, float height, Action<UIPanel> setup = null)
        {
            return row.AddVerticalSpace(PrependEntry, height, setup);
        }

        public static Row AddCustomPanel<T>([NotNull] this Row row, [NotNull] AddEntry add, Action<T> setup = null)
            where T : UIPanel
        {
            return add(row, Create.CustomPanel(row, setup));
        }

        public static Row AppendCustomPanel<T>([NotNull] this Row row, Action<T> setup = null)
            where T : UIPanel
        {
            return row.AddCustomPanel(AppendEntry, setup);
        }

        public static Row PrependCustomPanel<T>([NotNull] this Row row, Action<T> setup = null)
            where T : UIPanel
        {
            return row.AddCustomPanel(PrependEntry, setup);
        }

        public static Row AddColorField([NotNull] this Row row, [NotNull] AddEntry add, Color initialColor,
            [CanBeNull] Action<Color> onColorChanged, float width, float height, [CanBeNull] Action<UIColorField> setup = null)
        {
            return add(row, Create.ColorField(row, initialColor, onColorChanged, width, height, setup));
        }

        public static Row AppendColorField([NotNull] this Row row, Color initialColor, [CanBeNull] Action<Color> onColorChanged, float width,
            float height, [CanBeNull] Action<UIColorField> setup = null)
        {
            return row.AddColorField(AppendEntry, initialColor, onColorChanged, width, height, setup);
        }

        public static Row PrependColorField([NotNull] this Row row, Color initialColor, [CanBeNull] Action<Color> onColorChanged,
            float width, float height, [CanBeNull] Action<UIColorField> setup = null)
        {
            return row.AddColorField(PrependEntry, initialColor, onColorChanged, width, height, setup);
        }

        public static Row AddDropDown([NotNull] this Row row, [NotNull] AddEntry add, [NotNull] string[] values, int selectedIndex,
            [CanBeNull] Action<int> onSelectedIndexChanged, float width, float height, [CanBeNull] Action<UIDropDown> setupDropDown = null,
            [CanBeNull] Action<UIButton> setupDropDownButton = null)
        {
            return add(row,
                Create.DropDown(row, values, selectedIndex, onSelectedIndexChanged, width, height, setupDropDown, setupDropDownButton));
        }

        public static Row AppendDropDown([NotNull] this Row row, [NotNull] string[] values, int selectedIndex,
            [CanBeNull] Action<int> onSelectedIndexChanged, float width, float height, [CanBeNull] Action<UIDropDown> setupDropDown = null,
            [CanBeNull] Action<UIButton> setupDropDownButton = null)
        {
            return row.AddDropDown(AppendEntry, values, selectedIndex, onSelectedIndexChanged, width, height, setupDropDown,
                setupDropDownButton);
        }

        public static Row PrependDropDown([NotNull] this Row row, [NotNull] string[] values, int selectedIndex,
            [CanBeNull] Action<int> onSelectedIndexChanged, float width, float height, [CanBeNull] Action<UIDropDown> setupDropDown = null,
            [CanBeNull] Action<UIButton> setupDropDownButton = null)
        {
            return row.AddDropDown(PrependEntry, values, selectedIndex, onSelectedIndexChanged, width, height, setupDropDown,
                setupDropDownButton);
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