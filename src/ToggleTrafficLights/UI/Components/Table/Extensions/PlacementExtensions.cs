using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table.Extensions
{
    public static class PlacementExtensions
    {
        #region spreading
        public static Row SpreadHorizontal([NotNull] this Row row, [NotNull] Func<Entry, Entry, float> calculateHorizontalSpaceBetweenEntries)
        {
            Entry preEntry = null;

            var left = 0.0f;
            foreach (var entry in row.Entries)
            {
                if (preEntry != null)
                {
                    left += calculateHorizontalSpaceBetweenEntries(preEntry, entry);
                }

                var c = entry.Component;
                c.relativePosition = new Vector3(left, c.relativePosition.y);

                left += c.width;

                preEntry = entry;
            }

            return row;
        }
        public static Row SpreadHorizontal([NotNull] this Row row, float horizontalSpaceBetweenColumns)
        {
            return row.SpreadHorizontal((_, __) => horizontalSpaceBetweenColumns);
        }
        public static Table SpreadHorizontal([NotNull] this Table table, [NotNull] Func<Entry, Entry, float> calculateHorizontalSpaceBetweenEntries)
        {
            table.Rows.ForEach(row => row.SpreadHorizontal(calculateHorizontalSpaceBetweenEntries));
            return table;
        }
        public static Table SpreadHorizontal([NotNull] this Table table, float horizontalSpaceBetweenColumns)
        {
            return table.SpreadHorizontal((_, __) => horizontalSpaceBetweenColumns);
        }
        public static Table SpreadVertical([NotNull] this Table table, [NotNull] Func<Row, Row, float> calculateVerticalSpaceBetweenRows)
        {
            Row preRow = null;

            var top = 0.0f;
            foreach (var row in table.Rows)
            {
                if (preRow != null)
                {
                    top += calculateVerticalSpaceBetweenRows(preRow, row);
                }

                foreach (var c in row.Entries.Select(Component))
                {
                    c.relativePosition = new Vector3(c.relativePosition.x, top);
                }

                top += row.Entries.Max(e => e.Component.height);

                preRow = row;
            }

            return table;
        }
        public static Table SpreadVertical([NotNull] this Table table, float verticalSpaceBetweenRows)
        {
            return table.SpreadVertical((_, __) => verticalSpaceBetweenRows);
        }
        #endregion

        #region alignment
        public static ICollection<Row> AlignEntriesInColumns(this ICollection<Row> rows, [NotNull] Func<Entry, Entry, float> calculateHorizontalSpaceBetweenEntries)
        {
            //determine max length for each column
            var maxs = new float[rows.Max(r => r.NumberOfColumns)];
            foreach (var row in rows)
            {
                var i = 0;
                foreach (var c in row.Entries.Select(Component))
                {
                    var width = c.width;

                    var max = maxs[i];
                    maxs[i] = Mathf.Max(max, width);

                    i++;
                }
            }

            //place elements according to max width of the columns
            foreach (var row in rows)
            {
                Entry preEntry = null;

                var left = 0.0f;
                var i = 0;
                foreach (var entry in row.Entries)
                {
                    if (preEntry != null)
                    {
                        left += calculateHorizontalSpaceBetweenEntries(preEntry, entry);
                    }

                    var c = entry.Component;

                    c.relativePosition = new Vector3(left, c.relativePosition.y);

                    left += maxs[i];
                    preEntry = entry;
                }
            }

            return rows;
        }

        public static ICollection<Row> AlignEntriesInColumns(this ICollection<Row> rows, float horizontalSpaceBetweenColumns)
        {
            return rows.AlignEntriesInColumns((_, __) => horizontalSpaceBetweenColumns);
        }
        public static Table AlignEntriesInColumns([NotNull] this Table table, [NotNull] Func<Entry, Entry, float> calculateHorizontalSpaceBetweenEntries, [CanBeNull] Func<Row, bool> rowSelector = null)
        {
            var selector = rowSelector ?? (_ => true);
            var rows = table.Rows.Where(selector).ToArray();

            AlignEntriesInColumns(rows, calculateHorizontalSpaceBetweenEntries);

            return table;
        }
        public static Table AlignEntriesInColumns([NotNull] this Table table, float horizontalSpaceBetweenColumns, [CanBeNull] Func<Row, bool> rowSelector = null)
        {
            return table.AlignEntriesInColumns((_, __) => horizontalSpaceBetweenColumns, rowSelector);
        }
        #endregion


        #region root related
        public static Row LimitWidthToRootWidth([NotNull] this Row row, bool wrapTooLongLabels = true)
        {
            var w = row.Root.width;

            foreach (var c in row.Entries.Select(Component))
            {
                var start = c.relativePosition.x;
                var maxWidth = Mathf.Max(w - start, 0.0f);

                c.maximumSize = new Vector2(maxWidth, 0.0f);

                if (wrapTooLongLabels)
                {
                    var lbl = c as UILabel;
                    if (lbl != null)
                    {
                        // wordWrap breaks at strange positions
                        //      wants to break quite early (Foo\nBar\n\Baz)
                        // to prevent this: auto width off AND expand the width slightly (1px)
                        // without expanding the wordWrap algorithm decides there's not enough space with the old width...
                        lbl.autoHeight = true; //disables autoSize (and therefore auto width (which is no extra property)
                        lbl.width = lbl.width + 1.0f;   // if to big: gets automatically shortened because of maximumSize
                        lbl.wordWrap = true;
                    }
                }
            }

            return row;
        }

        public static Table LimitWithToRootWidth([NotNull] this Table table, bool wrapTooLongLabels = true)
        {
            table.Rows.ForEach(row => row.LimitWidthToRootWidth(wrapTooLongLabels));

            return table;
        }
        public static Table UpdateHeightOfRootToRowsHeight([NotNull] this Table table)
        {
            var maxHeight = table.Rows.SelectMany(r => r.Entries)
                                      .Select(Component)
                                      .Select(c => c.relativePosition.y + c.height)
                                      .Max();
            table.Root.height = maxHeight;

            return table;
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