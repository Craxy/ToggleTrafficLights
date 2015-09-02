using System;
using System.Collections.Generic;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;
using JetBrains.Annotations;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table.Extensions
{
    public static class TableExtensions
    {
        #region Create Table
        public static Table CreateTable<T>([NotNull] this T parent)
            where T : UIComponent
        {
            return Table.CreateEmpty(parent);
        }
        #endregion

        #region create row
        public static Table AddRow([NotNull] this Table table, [NotNull] Func<Row, Row> fillRow)
        {
            var row = Row.CreateEmpty(table.Root).Pipe(fillRow);
            return Table.AppendRow(table, row);
        }
        public static Table PrependRow([NotNull] this Table table, [NotNull] Func<Row, Row> fillRow)
        {
            var row = Row.CreateEmpty(table.Root).Pipe(fillRow);
            return Table.PrependRow(table, row);
        }
        public static Table AddVerticalSpace([NotNull] this Table table, float height, [CanBeNull] Action<UIPanel> setup = null)
        {
            return table.AddRow(row => row.AddVerticalSpace(height, setup));
        }
        #endregion

        #region Linq-ish row handling

        public static Table Do([NotNull] this Table table, [CanBeNull] Action<IEnumerable<Row>> action = null)
        {
            action?.Invoke(table.Rows);

            return table;
        }
        #endregion
    }
}