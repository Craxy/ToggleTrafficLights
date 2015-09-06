using System;
using System.Linq;
using ColossalFramework.UI;
using JetBrains.Annotations;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table.Extensions
{
    public static class ChangeRowExtensions
    {
        public static Row IndentRow([NotNull] this Row row, float indention, [CanBeNull] Action<UIPanel> setup = null)
        {
            return row.PrependHorizontalSpace(indention, setup);
        }

        public static Table IndentLastRow([NotNull] this Table table, float indention, [CanBeNull] Action<UIPanel> setup = null)
        {
            return table.RemoveLastRow().AddRow(r => r.PrependHorizontalSpace(indention, setup));
        }

        public static Table IndentAll([NotNull] this Table table, float indention, [CanBeNull] Action<UIPanel> setup = null)
        {
            return table.CopyWith(rows: table.Rows.Select(r => r.PrependHorizontalSpace(indention, setup)).ToArray());

        }
    }
}