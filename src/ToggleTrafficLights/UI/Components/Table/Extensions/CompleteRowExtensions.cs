using System;
using ColossalFramework.UI;
using JetBrains.Annotations;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table.Extensions
{
    public static class CompleteRowExtensions
    {
        public static Table AddPropertyValueRow([NotNull] this Table table, [NotNull] string title, [NotNull] string value, [NotNull] string separator = ":", [CanBeNull] Action<UILabel> setup = null)
        {
            return table.AddRow(row => row.AddLabel(title).AddLabel(separator).AddLabel(value).SetupAllOfType(setup));
        }

        public static Table AddSingleStringRow([NotNull] this Table table, [NotNull] string text, [CanBeNull] Action<UILabel> setup = null)
        {
            return table.AddRow(row => row.AddLabel(text, setup));
        }
    }
}