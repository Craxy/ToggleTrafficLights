using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;
using JetBrains.Annotations;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components
{
    public static class Controls
    {
        public abstract class Component
        {
        }
        public class Row : Component
        {
            public UIComponent[] Columns { get; }
            public int NumberOfColumns => Columns.Length;

            internal Row([NotNull] UIComponent[] columns)
            {
                Columns = columns;
            }
        }

        public class VerticalSpace : Row
        {
            public UIPanel Space => (UIPanel) Columns.Single();
            public float Height => Space.height;
            public VerticalSpace([NotNull] UIPanel space) 
                : base(new UIComponent[]{space})
            {
            }
        }
        public class Header : Row
        {
            public UILabel Label => (UILabel)Columns.Single();

            internal Header([NotNull] UILabel lbl) 
                : base(new UIComponent[] { lbl })
            {
            }
        }

        public static VerticalSpace AddVerticalSpace(this UIComponent parent, float height, Action<UIPanel> setup = null)
        {
            var pnl = parent.AddUIComponent<UIPanel>();
            pnl.height = height;
            pnl.width = parent.width;

            setup?.Invoke(pnl);

            return new VerticalSpace(pnl);
        }

        public class StringRow : Row
        {
            public UILabel[] Labels => (UILabel[]) Columns;

            internal StringRow([NotNull] UILabel[] columns) 
                : base((UILabel[])columns)
            {
            }
        }

        private static UILabel CreateLabel(this UIComponent parent, string value, Action<UILabel> setup = null)
        {
            var lbl = parent.AddUIComponent<UILabel>();
            lbl.text = value;

            setup?.Invoke(lbl);

            return lbl;
        }

        public static Header AddHeader(this UIComponent parent, string title, Action<UILabel> setup = null)
        {
            var lbl = parent.CreateLabel(title, setup);
            return new Header(lbl);
        }

        public static StringRow AddStringRow(this UIComponent parent, IEnumerable<string> values, Action<UILabel> setup = null)
        {
            // partial application c# style.....
            Func<string, UILabel> createLabel = value => parent.CreateLabel(value, setup);

            var lbls = values.Select(createLabel).ToArray();
            return new StringRow(lbls);
        }

        #region UIComponent

        public static T ToolTip<T>(this T component, string text)
            where T : UIComponent
        {
            component.tooltip = text;
            return component;
        }
        #endregion

        #region UILabel
        public static UILabel TextScale(this UILabel lbl, float textScale)
        {
            lbl.textScale = textScale;
            return lbl;
        }
        #endregion

        #region placement
        public static IList<Row> SpreadVertical(this IList<Row> rows)
        {
            var top = 0.0f;

            foreach (var row in rows)
            {
                foreach (var c in row.Columns)
                {
                    c.relativePosition = new Vector3(c.relativePosition.x, top);
                }

                top += row.Columns.Max(c => c.height);

                top += 3.0f;
            }

            return rows;
        }

        public static IList<Row> SpreadHorizontal(this IList<Row> rows)
        {
            
                foreach (var row in rows)
                {
                    var left = 0.0f;
            
                    foreach (var c in row.Columns)
                    {
                        c.relativePosition = new Vector3(left, c.relativePosition.y);
                        left += c.relativePosition.x + c.width;
            
                        left += 2.0f;
                    }
                }
            
                return rows;
        }

        public static IList<Row> LimitLastLabelsWidthToParent(this IList<Row> rows, UIComponent parent)
        {
            var w = parent.width;

            foreach (var row in rows)
            {
                var c = row.Columns.Last();
                var lbl = c as UILabel;
                if (lbl != null)
                {
                    var start = lbl.relativePosition.x;
                    var maxWidth = Mathf.Max(w - start, 0.0f);

                    lbl.autoSize = false;
                    lbl.autoHeight = true;
                    lbl.maximumSize = new Vector2(maxWidth, 0.0f);
                    lbl.width = maxWidth;   // else wordWrap breaks words even if there's enough space...
                    lbl.wordWrap = true;
                }
            }

            return rows;
        }

        public static IList<Row> AlignColumns(this IList<Row> rows)
        {
            //determine max length for each column
            var maxs = new float[rows.Max(r => r.Columns.Length)];
            foreach (var row in rows)
            {
                var i = 0;
                foreach (var c in row.Columns)
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
                var left = 0.0f;
                var i = 0;
                foreach (var c in row.Columns)
                {
                    c.relativePosition = new Vector3(left, c.relativePosition.y);
                    
                    var max = maxs[i];
                    left += max + 2.0f;

                    i++;
                }
            }

            return rows;
        }
        #endregion

        #region functional-ish
        public static void Ignore<T>(this T _)
        {
            return;
        }

        public static IEnumerable<T> And<T>(this IEnumerable<T> values, T value)
        {
            return values.Concat(new[] {value});
        }

        public static IEnumerable<T> And<T>(this T value1, T value2)
        {
            return new [] {value1, value2};
        }
        #endregion
    }
}