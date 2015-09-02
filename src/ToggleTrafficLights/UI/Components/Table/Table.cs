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

        internal static Table CreateEmpty([NotNull] UIComponent root) => new Table(root, new Row[0]);
        internal static Table PrependRow([NotNull] Table table, [NotNull] Row row) => new Table(table.Root, table.Rows.ImmutablePrepend(row));
        internal static Table AppendRow([NotNull] Table table, [NotNull] Row row) => new Table(table.Root, table.Rows.ImmutableAppend(row));

        #region Overrides of Object

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Table");

            foreach (var row in Rows)
            {
                sb.Append("/t")
                  .AppendLine(row.ToString());
            }

            return sb.ToString();
        }

        #endregion
    }
}