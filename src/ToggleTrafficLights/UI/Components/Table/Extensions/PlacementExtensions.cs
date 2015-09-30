using System;
using System.Collections.Generic;
using System.Linq;
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

                entry.RelativePosition = new Vector3(left, entry.RelativePosition.y);

                left += entry.Width;

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

                foreach (var entry in row.Entries)
                {
                    entry.RelativePosition = new Vector3(entry.RelativePosition.x, top);
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
        public static Table AlignEntriesInColumns([NotNull] this Table table, float horizontalSpaceBetweenColumns,
            [CanBeNull] Func<Row, bool> rowSelector = null)
        {
            return table.AlignFirstNEntriesInColumns(Option.None<int>(), horizontalSpaceBetweenColumns, rowSelector);
        }

        /// <summary>
        /// Difference to AlignEntriesInColumns:
        ///     AlignEntriesInColumns aligns all columns 
        ///     while AlignEntriesInPropertyValueColumns aligns only the first three columns.
        ///     Columns behind that get spread like it's done in <see cref="SpreadHorizontal(Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table.Row,System.Func{Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table.Entry,Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table.Entry,float})"/>
        /// </summary>
        /// <returns></returns>
        public static Table AlignEntriesInPropertyValueColumns([NotNull] this Table table, float horizontalSpaceBetweenColumns,
            [CanBeNull] Func<Row, bool> rowSelector = null)
        {
            return table.AlignFirstNEntriesInColumns(Option.Some(3), horizontalSpaceBetweenColumns, rowSelector);
        }

        #region variable number of columns
        public static ICollection<Row> AlignFirstNEntriesInColumns([NotNull] this ICollection<Row> rows, 
            Option<int> numberOfLeadingColumnsToAlign, 
            float horizontalSpaceBetweenEntries)
        {
            //max doesn't work with empty collections
            if (rows.Count == 0)
            {
                return rows;
            }

            var n = numberOfLeadingColumnsToAlign.IsSome() ? numberOfLeadingColumnsToAlign.GetValue() : rows.Max(r => r.NumberOfColumns);

            //determine max length for first numberOfLeadingColumnsToAlign columns
            var maxs = new float[Math.Max(n, 0)];
            foreach (var row in rows)
            {
                var i = 0;
                foreach (var c in row.Entries)
                {
                    if (i >= maxs.Length)
                    {
                        break;
                    }

                    var width = c.Width;

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
                        left +=  horizontalSpaceBetweenEntries;
                    }

                    entry.RelativePosition = new Vector3(left, entry.RelativePosition.y);

                    if (i < maxs.Length)
                    {
                        left += maxs[i++];
                    }
                    else
                    {
                        left += entry.Width;
                    }

                    preEntry = entry;
                }
            }

            return rows;
        }

        public static Table AlignFirstNEntriesInColumns([NotNull] this Table table, 
            Option<int> numberOfLeadingColumnsToAlign,
            float horizontalSpaceBetweenEntries,
            [CanBeNull] Func<Row, bool> rowSelector = null)
        {
            var rows = table.Rows;
            if (rowSelector != null)
            {
                rows = table.Rows.Where(rowSelector).ToArray();
            }

            rows.AlignFirstNEntriesInColumns(numberOfLeadingColumnsToAlign, horizontalSpaceBetweenEntries);


            return table;
        }

        #endregion
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
                //panels somehow disrespect maximumSize
                // -> set width manually iff out of bounds
                if (c.width > maxWidth)
                {
                    c.width = maxWidth;
                }

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