using System;
using ColossalFramework.UI;
using JetBrains.Annotations;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table.Extensions
{
    public static class ChangeRowExtensions
    {
        public static Row IndentRow([NotNull] this Row row, float indention, [CanBeNull] Action<UIPanel> setup = null)
        {
            return ;

            return table.AddRow(row => row.AddLabel(title).AddLabel(separator).AddLabel(value).SetupAllOfType(setup));
        }

    }
}