using System;
using System.Linq;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table.Entries;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;
using JetBrains.Annotations;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table.Extensions
{
    public static class CompleteRowExtensions
    {
        public static Table AddPropertyValueRow([NotNull] this Table table, [NotNull] string name, [NotNull] string value, [NotNull] string separator = ":", [CanBeNull] Action<UILabel> setup = null)
        {
            return table.AddPropertyValueRow(name, value, separator, 0.0f, setup);
        }
        public static Table AddPropertyValueRow([NotNull] this Table table, [NotNull] string name, [NotNull] string value, [NotNull] string separator = ":", float indention = 0, [CanBeNull] Action<UILabel> setup = null)
        {
            return table.AddRow(row =>
            {
                if (indention > 0.0f)
                {
                    row = row.AppendHorizontalSpace(indention);
                }
                return row.AppendLabel(name, setup)
                          .AppendLabel(separator, setup)
                          .AppendLabel(value, setup)
                          .Tag(RowTag.PropertyValue);
            });
        }
        public static Table AddSingleStringRow([NotNull] this Table table, [NotNull] string text, [CanBeNull] Action<UILabel> setup = null)
        {
            return table.AddRow(row => row.AppendLabel(text, setup));
        }
        public static Table AddHeaderRow([NotNull] this Table table, [NotNull] string text, [CanBeNull] Action<UILabel> setup = null)
        {
            return table.AddRow(row => row.AppendLabel(text, setup).Tag(RowTag.Header));
        }
        public static Table AddVerticalSpace([NotNull] this Table table, float height, [CanBeNull] Action<UIPanel> setup = null)
        {
            return table.AddRow(row => row.AppendVerticalSpace(height, setup).Tag(RowTag.VerticalSpace));
        }
        public static Table AddCustomPanelRow<T>([NotNull] this Table table, [CanBeNull] Action<T> setup = null) 
            where T : UIPanel
        {
            return table.AddRow(row => row.AppendCustomPanel(setup));
        }
        public static Table AddColorFieldRow([NotNull] this Table table, 
            [NotNull] string name, 
            Color initialColor, [CanBeNull] Action<Color> onColorChanged, [CanBeNull] Action<Action<Color>> notifyColorChanged,  
            [NotNull] string separator = ":", 
            float indention = 0, 
            [CanBeNull] Action<UILabel> setupText = null,
            [CanBeNull] Action<UIColorField> setupColorField = null)
        {
            return table.AddRow(row =>
            {
                if(indention > 0.0f)
                {
                    row = row.AppendHorizontalSpace(indention);
                }

                row = row.AppendLabel(name, setupText)
                    .AppendLabel(separator, setupText)
                    .AppendColorField(initialColor, onColorChanged, 20.0f, 20.0f, setupColorField)
                    .AppendLabel(initialColor.ToHex(true), setupText);

                var cf = row.Entries.OfType<ColorFieldEntry>().Single();
                var hex = (LabelEntry)row.Entries.Last();

                //pass color changes to label
                cf.ColorChanged += color =>
                {
                    hex.Text = color.ToHex(true);
                };

                //get notify when color changes somewhere outside
                notifyColorChanged?.Invoke(color => cf.Color = color);

                return row.Tag(RowTag.ColorField);
            });
        }

        public static Table AddDropDownRow([NotNull] this Table table, [NotNull] string name, [NotNull] string[] values, int selectedIndex,
            [CanBeNull] Action<int> onSelectedIndexChanged,
            [NotNull] string separator = ":", float indention = 0, 
            [CanBeNull] Action<UILabel> setupText = null,
            [CanBeNull] Action<UIDropDown> setupDropDown = null,
            [CanBeNull] Action<UIButton> setupDropDownButton = null
            )
        {
            return table.AddRow(row =>
            {
                if (indention > 0.0f)
                {
                    row = row.AppendHorizontalSpace(indention);
                }

                row = row.AppendLabel(name, setupText)
                    .AppendLabel(separator, setupText)
                    .AppendDropDown(values, selectedIndex, onSelectedIndexChanged, 150.0f, 20.0f, setupDropDown, setupDropDownButton);

                return row.Tag(RowTag.DropDown);
            });
        }
    }
}