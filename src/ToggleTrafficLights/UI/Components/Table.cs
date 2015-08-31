using System;
using System.Linq;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;
using JetBrains.Annotations;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components
{
    public static class TableCreator
    {
        public class Table
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
            internal static Table AddRow([NotNull] Table table, [NotNull] Row row) => new Table(table.Root, table.Rows.ImmutableAppend(row));
            internal Table AddRow([NotNull] Row row) => Table.AddRow(this, row);
        }
        public class Row
        {
            public UIComponent Root { get; }

            public Entry[] Entries { get; }
            public int NumberOfColumns => Entries.Length;

            private Row([NotNull] UIComponent root, [NotNull] Entry[] entries)
            {
                Entries = entries;
                Root = root;
            }

            internal static Row CreateEmpty([NotNull] UIComponent root) => new Row(root, new Entry[0]);
            internal static Row AddEntry([NotNull] Row row, [NotNull] Entry entry) => new Row(row.Root, row.Entries.ImmutableAppend(entry));
            internal Row AddEntry([NotNull] Entry entry) => Row.AddEntry(this, entry);
        }

        public class Entry
        {
            public UIComponent Value { get; }
            internal Entry([NotNull] UIComponent value)
            {
                Value = value;
            }
        }

        #region Entries
        public class LabelEntry : Entry
        {
            public UILabel Label => (UILabel) Value;
            public LabelEntry([NotNull] UILabel value) : base(value)
            {
            }
        }
        public class ButtonEntry : Entry
        {
            public UIButton Button => (UIButton)Value;
            public ButtonEntry([NotNull] UIButton value) : base(value)
            {
            }
        }
        public class PanelEntry : Entry
        {
            public UIPanel Panel => (UIPanel)Value;
            public PanelEntry([NotNull] UIPanel value) : base(value)
            {
            }
        }

        // not inherited from PanelEntry so it's easier to filter
        public class HorizontalSpaceEntry : Entry
        {
            // and ignoring immutability....well done....
            public float Width
            {
                get { return Panel.width; }
                set { Panel.width = Mathf.Max(0.0f, value); }
            }
            public UIPanel Panel => (UIPanel)Value;
            public HorizontalSpaceEntry([NotNull] UIPanel value) : base(value)
            {
            }
        }
        // not inherited from PanelEntry so it's easier to filter
        public class VerticalSpaceEntry : Entry
        {
            // and ignoring immutability....well done....
            public float Height
            {
                get { return Panel.height; }
                set { Panel.height = Mathf.Max(0.0f, value); }
            }
            public UIPanel Panel => (UIPanel)Value;
            public VerticalSpaceEntry([NotNull] UIPanel value) : base(value)
            {
            }
        }

        public class ColorFieldEntry : Entry
        {
            public UIColorField ColorField => (UIColorField) Value;
            public ColorFieldEntry([NotNull] UIColorField value) : base(value)
            {
            }
        }
        //todo: text field etc
        #endregion


        #region Create Table

        public static Table CreateTable<T>([NotNull] this T parent)
            where T : UIComponent
        {
            return Table.CreateEmpty(parent);   
        }

        public static Table AddRow([NotNull] this Table table, [NotNull] Func<Row, Row> fillRow)
        {
            var row = Row.CreateEmpty(table.Root).Pipe(fillRow);
            return Table.AddRow(table, row);
        }
        #endregion

        #region Create Row

        private static T AddUIComponent<T>([NotNull] this Row row)
            where T : UIComponent
        {
            return row.Root.AddUIComponent<T>();
        }
        public static Row AddLabel([NotNull] this Row row, [NotNull] string text, Action<UILabel> setup = null)
        {
            var lbl = row.AddUIComponent<UILabel>();
            lbl.text = text;
            setup?.Invoke(lbl);

            return row.AddEntry(new LabelEntry(lbl));
        }
        public static Row AddButton([NotNull] this Row row, [NotNull] string text, [CanBeNull] Action onClick, Action<UIButton> setup = null)
        {
            var btn = row.AddUIComponent<UIButton>();
            btn.text = text;
            if (onClick != null)
            {
                btn.eventClick += (_, __) => onClick();
            }
            setup?.Invoke(btn);

            return row.AddEntry(new ButtonEntry(btn));
        }

        public static Row AddHorizontalSpace([NotNull] this Row row, float width, Action<UIPanel> setup = null)
        {
            var pnl = row.AddUIComponent<UIPanel>();
            pnl.width = width;
            setup?.Invoke(pnl);

            return row.AddEntry(new HorizontalSpaceEntry(pnl));
        }
        public static Row AddIndention([NotNull] this Row row, float height, Action<UIPanel> setup = null)
        {
            return row.AddHorizontalSpace(height, setup);
        }
        public static Row AddVerticalSpace([NotNull] this Row row, float height, Action<UIPanel> setup = null)
        {
            var pnl = row.AddUIComponent<UIPanel>();
            pnl.height = height;
            setup?.Invoke(pnl);

            return row.AddEntry(new HorizontalSpaceEntry(pnl));
        }
        #endregion

        #region placement
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

                var c = entry.Value;

                c.relativePosition = new Vector3(left, c.relativePosition.y);

                left += c.relativePosition.x + c.width;

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

                top += row.Entries.Max(e => e.Value.height);

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

        public static Table AlignColumns([NotNull] this Table table, Func<Row, bool> rowSelector)
        {
            var rows = 

            return table;
        }
        #endregion

        #region root related
        public static Row LimitWidthToRootWidth([NotNull] this Row row, bool wordWrapLabels = true)
        {
            var w = row.Root.width;

            foreach (var c in row.Entries.Select(Component))
            {
                var start = c.relativePosition.x;
                var maxWidth = Mathf.Max(w - start, 0.0f);

                c.maximumSize = new Vector2(maxWidth, 0.0f);

                if(wordWrapLabels)
                {
                    var lbl = c as UILabel;
                    if(lbl != null)
                    {
                        lbl.wordWrap = true;
                    }
                }
            }

            return row;
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
        #endregion

        #region UIComponent
        public static T ToolTip<T>(this T component, string text)
            where T : UIComponent
        {
            component.tooltip = text;
            return component;
        }
        public static T Width<T>(this T component, float width)
            where T : UIComponent
        {
            component.width = width;
            return component;
        }
        public static T Height<T>(this T component, float height)
            where T : UIComponent
        {
            component.height = height;
            return component;
        }
        #endregion

        #region TextComponent
        public static T TextScale<T>(this T txt, float textScale)
            where T : UITextComponent
        {
            txt.textScale = textScale;
            return txt;
        }
        public static T TextColor<T>(this T txt, Color32 color)
            where T : UITextComponent
        {
            txt.textColor = color;
            return txt;
        }
        #endregion

        #region Helper

        private static UIComponent Component(Entry entry)
        {
            return entry.Value;
        }

        #endregion
    }
}