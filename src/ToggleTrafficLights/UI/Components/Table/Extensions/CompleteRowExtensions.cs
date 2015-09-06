using System;
using ColossalFramework.UI;
using JetBrains.Annotations;

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

    }
}