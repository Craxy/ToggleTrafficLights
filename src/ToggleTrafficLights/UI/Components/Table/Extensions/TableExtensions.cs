using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public static Table Concat([NotNull] this Table front, [NotNull] Table back)
        {
            return Table.Concat(front, back);
        }

        public static Table CopyWith([NotNull] this Table table, [NotNull] Row[] rows)
        {
            return Table.CopyWith(table, rows);
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
        #endregion

        #region remove row
        public static Table RemoveLastRow([NotNull] this Table table)
        {
            return Table.RemoveLastRow(table);
        }
        public static Table RemoveFirstRow([NotNull] this Table table)
        {
            return Table.RemoveFirstRow(table);
        }
        #endregion

        #region Linq-ish row handling

        public static Table Do([NotNull] this Table table, [CanBeNull] Action<IEnumerable<Row>> action = null)
        {
            action?.Invoke(table.Rows);

            return table;
        }
        #endregion

        #region Debug
        public static Table DebugLog([NotNull] this Table table, [NotNull] string pre = "")
        {
            Utils.DebugLog.Info("{0}: {1}", pre, table.ToString());

            return table;
        }
        #endregion
    }
}