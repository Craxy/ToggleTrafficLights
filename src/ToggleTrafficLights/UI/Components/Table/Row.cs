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

        private Row([NotNull] UIComponent root, [NotNull] Entry[] entries)
        {
            Entries = entries;
            Root = root;
        }

        internal static Row CreateEmpty([NotNull] UIComponent root) => new Row(root, new Entry[0]);
        internal static Row PrependEntry([NotNull] Row row, [NotNull] Entry entry) => new Row(row.Root, row.Entries.ImmutablePrepend(entry));
        internal static Row AppendEntry([NotNull] Row row, [NotNull] Entry entry) => new Row(row.Root, row.Entries.ImmutableAppend(entry));
        internal static Row Concat([NotNull] Row left, [NotNull] Row right)
        {
            System.Diagnostics.Debug.Assert(left.Root == right.Root);

            return new Row(left.Root, left.Entries.ImmutableConcat(right.Entries));
        }

        #region Overrides of Object

        public override string ToString()
        {
            return string.Format("Row [ {0} ]", string.Join("; ", Entries.Select(e => e.GetType().Name).ToArray()));
        }

        #endregion
    }
}