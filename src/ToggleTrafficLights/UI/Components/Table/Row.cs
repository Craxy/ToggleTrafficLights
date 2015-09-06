using System.Linq;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;
using JetBrains.Annotations;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table
{
    public sealed class Row
    {
        public UIComponent Root { get; }

        public Entry[] Entries { get; }
        public int NumberOfColumns => Entries.Length;

        public static readonly string EmptyTag = string.Empty;
        public string Tag { get; }

        private Row([NotNull] UIComponent root, [NotNull] Entry[] entries, [NotNull] string tag)
        {
            Entries = entries;
            Root = root;
            Tag = tag;
        }

        internal static Row CreateEmpty([NotNull] UIComponent root) => Row.CreateEmpty(root, Row.EmptyTag);
        internal static Row CreateEmpty([NotNull] UIComponent root, [NotNull] string tag) => Row.Create(root, new Entry[0], tag);
        private static Row Create([NotNull] UIComponent root, [NotNull] Entry[] entries, [NotNull] string tag) => new Row(root, entries, tag);
        internal static Row PrependEntry([NotNull] Row row, [NotNull] Entry entry) => Row.Create(row.Root, row.Entries.ImmutablePrepend(entry), row.Tag);
        internal static Row AppendEntry([NotNull] Row row, [NotNull] Entry entry) => Row.Create(row.Root, row.Entries.ImmutableAppend(entry), row.Tag);
        internal static Row Concat([NotNull] Row left, [NotNull] Row right)
        {
            System.Diagnostics.Debug.Assert(left.Root == right.Root);

            var tag = Row.EmptyTag;
            if (left.Tag != Row.EmptyTag)
            {
                tag = left.Tag;
            }
            else if (right.Tag != Row.EmptyTag)
            {
                tag = left.Tag;
            }

            return Row.Create(left.Root, left.Entries.ImmutableConcat(right.Entries), tag);
        }
        internal static Row Copy([NotNull] Row row)
        {
            return Row.Create(row.Root, row.Entries, row.Tag);
        }
        internal static Row ChangeTag([NotNull] Row row, [NotNull] string tag)
        {
            return Row.Create(row.Root, row.Entries, tag);
        }

        #region Overrides of Object

        public override string ToString()
        {
            return $"Row({Tag}) [ {string.Join("; ", Entries.Select(e => e.ToString()).ToArray())} ]";
        }

        #endregion
    }
}