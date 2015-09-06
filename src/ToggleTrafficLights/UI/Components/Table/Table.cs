using System.Runtime.InteropServices;
using System.Text;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;
using JetBrains.Annotations;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table
{
    public sealed class Table
    {
        public UIComponent Root { get; }

        public Row[] Rows { get; }
        public int NumberOfRows => Rows.Length;

        private Table([NotNull] UIComponent root, [NotNull] Row[] rows)
        {
            Root = root;
            Rows = rows;
        }

        internal static Table CreateEmpty([NotNull] UIComponent root) => Table.Create(root, new Row[0]);
        private static Table Create([NotNull] UIComponent root, [NotNull] Row[] rows) => new Table(root, rows);
        internal static Table PrependRow([NotNull] Table table, [NotNull] Row row) => Table.Create(table.Root, table.Rows.ImmutablePrepend(row));
        internal static Table AppendRow([NotNull] Table table, [NotNull] Row row) => Table.Create(table.Root, table.Rows.ImmutableAppend(row));
        internal static Table RemoveLastRow([NotNull] Table table) => Table.Create(table.Root, table.Rows.ImmutableRemoveLast());
        internal static Table RemoveFirstRow([NotNull] Table table) => Table.Create(table.Root, table.Rows.ImmutableRemoveFirst());
        internal static Table Concat([NotNull] Table front, [NotNull] Table back)
        {
            System.Diagnostics.Debug.Assert(front.Root == back.Root);

            return Table.Create(front.Root, front.Rows.ImmutableConcat(back.Rows));
        }
        internal static Table Copy([NotNull] Table table)
        {
            return Table.Create(table.Root, table.Rows);
        }

        internal static Table CopyWith([NotNull] Table table, [NotNull] Row[] rows)
        {
            return Table.Create(table.Root, rows);
        }
        #region Overrides of Object

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Table");

            foreach (var row in Rows)
            {
                sb.Append("\t")
                  .AppendLine(row.ToString());
            }

            return sb.ToString();
        }

        #endregion
    }
}