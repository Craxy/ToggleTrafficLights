//using System;
//using System.Collections.Generic;
//using System.Linq;
//using ColossalFramework.UI;
//using Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.Menu.Components;
//using Craxy.CitiesSkylines.ToggleTrafficLights.UI.SideMenu.Pages;
//using JetBrains.Annotations;
//using UnityEngine;
//
//namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components
//{
//    public static class Controls
//    {
//        public abstract class Component
//        {
//        }
//        public class Row : Component
//        {
//            public UIComponent[] Columns { get; }
//            public int NumberOfColumns => Columns.Length;
//
//            internal Row([NotNull] UIComponent[] columns)
//            {
//                Columns = columns;
//            }
//        }
//
//        public class VerticalSpace : Row
//        {
//            public UIPanel Space => (UIPanel) Columns.Single();
//            public float Height => Space.height;
//            public VerticalSpace([NotNull] UIPanel space) 
//                : base(new UIComponent[]{space})
//            {
//            }
//        }
//        public class Header : Row
//        {
//            public UILabel Label => (UILabel)Columns.Single();
//
//            internal Header([NotNull] UILabel lbl) 
//                : base(new UIComponent[] { lbl })
//            {
//            }
//        }
//        public class StringRow : Row
//        {
//            public UILabel[] Labels => (UILabel[])Columns;
//
//            internal StringRow([NotNull] UILabel[] columns)
//                : base((UILabel[])columns)
//            {
//            }
//        }
//
//        public class PanelRow : Row
//        {
//            public UIPanel Panel => (UIPanel) Columns.Single();
//
//            public PanelRow([NotNull] UIPanel panel) 
//                : base(new UIComponent[] { panel })
//            {
//            }
//        }
//        public class ButtonRow : Row
//        {
//            public UIButton Button => (UIButton)Columns.Single();
//
//            public ButtonRow([NotNull] UIButton button)
//                : base(new UIComponent[] { button })
//            {
//            }
//        }
//        public class ColorFieldRow : Row
//        {
//            public UILabel Title => (UILabel) Columns.First();
//            public UIColorField ColorPanel => Columns.OfType<UIColorField>().Single();
//
//            public ColorFieldRow([NotNull] UILabel colorRow)
//                : base(new UIComponent[] { colorRow })
//            {
//            }
//        }
//
//        public static VerticalSpace AddVerticalSpace(this UIComponent parent, float height, Action<UIPanel> setup = null)
//        {
//            var pnl = parent.AddUIComponent<UIPanel>();
//            pnl.height = height;
//            pnl.width = parent.width;
//
//            setup?.Invoke(pnl);
//
//            return new VerticalSpace(pnl);
//        }
//
//
//
//        public static PanelRow AddPanel<T>(this UIComponent parent, Action<UIPanel> setup = null)
//            where T : UIPanel
//        {
//            var pnl = parent.AddUIComponent<T>();
//
//            setup?.Invoke(pnl);
//
//            return new PanelRow(pnl);
//        }
//        public static ButtonRow AddButton(this UIComponent parent, string text, Action onClick, Action<UIButton> setup = null)
//        {
//            var btn = parent.AddUIComponent<UIButton>();
//            btn.text = text;
//            btn.normalBgSprite = "ButtonMenu";
//            btn.hoveredTextColor = new Color32(7, 132, 255, 255);
//
//            if (onClick != null)
//            {
//                btn.eventClick += (_, __) => onClick();
//            }
//
//            setup?.Invoke(btn);
//
//            return new ButtonRow(btn);
//        }
//        public static ColorFieldRow AddColorPicker(this UIComponent parent, string title, Color initialColor, Action<UIColorPanel> setup = null)
//        {
//            var cp = parent.AddUIComponent<UIColorPanel>();
//            cp.Title = title;
//            cp.Color = initialColor;
//
//            setup?.Invoke(cp);
//
//            return new ColorFieldRow(cp);
//        }
//
//
//        private static UILabel CreateLabel(this UIComponent parent, string value, Action<UILabel> setup = null)
//        {
//            var lbl = parent.AddUIComponent<UILabel>();
//            lbl.text = value;
//
//            setup?.Invoke(lbl);
//
//            return lbl;
//        }
//
//        public static Header AddHeader(this UIComponent parent, string title, Action<UILabel> setup = null)
//        {
//            var lbl = parent.CreateLabel(title, setup);
//            return new Header(lbl);
//        }
//
//        public static StringRow AddStringRow(this UIComponent parent, IEnumerable<string> values, Action<UILabel> setup = null)
//        {
//            // partial application c# style.....
//            Func<string, UILabel> createLabel = value => parent.CreateLabel(value, setup);
//
//            var lbls = values.Select(createLabel).ToArray();
//            return new StringRow(lbls);
//        }
//
//        #region UIComponent
//
//        public static T ToolTip<T>(this T component, string text)
//            where T : UIComponent
//        {
//            component.tooltip = text;
//            return component;
//        }
//        public static T Width<T>(this T component, float width)
//            where T : UIComponent
//        {
//            component.width = width;
//            return component;
//        }
//        public static T Height<T>(this T component, float height)
//            where T : UIComponent
//        {
//            component.height = height;
//            return component;
//        }
//        #endregion
//
//        #region TextComponent
//        public static T TextScale<T>(this T txt, float textScale)
//            where T : UITextComponent
//        {
//            txt.textScale = textScale;
//            return txt;
//        }
//        public static T TextColor<T>(this T txt, Color32 color)
//            where T : UITextComponent
//        {
//            txt.textColor = color;
//            return txt;
//        }
//        #endregion
//
//        #region placement
//        public static IList<T> SpreadVertical<T>(this IList<T> rows, ITabSettings settings)
//            where T : Row
//        {
//            var top = 0.0f;
//
//            foreach (var row in rows)
//            {
//                foreach (var c in row.Columns)
//                {
//                    c.relativePosition = new Vector3(c.relativePosition.x, top);
//                }
//
//                top += row.Columns.Max(c => c.height);
//
//                top += settings.VerticalSpaceBetweenLines;
//            }
//
//            return rows;
//        }
//
//        public static IList<T> SpreadHorizontal<T>(this IList<T> rows, ITabSettings settings)
//            where T : Row
//
//        {
//
//            foreach (var row in rows)
//                {
//                    var left = 0.0f;
//            
//                    foreach (var c in row.Columns)
//                    {
//                        c.relativePosition = new Vector3(left, c.relativePosition.y);
//                        left += c.relativePosition.x + c.width;
//            
//                        left += settings.IndentationBetweenColumns;
//                    }
//                }
//            
//                return rows;
//        }
//
//        public static IList<T> LimitLastComponentsWidthToParent<T>(this IList<T> rows, UIComponent parent, ITabSettings settings)
//            where T : Row
//        {
//            var w = parent.width;
//
//            foreach (var row in rows)
//            {
//                var c = row.Columns.Last();
//                var lbl = c as UILabel;
//                if (lbl != null)
//                {
//                    var start = lbl.relativePosition.x;
//                    var maxWidth = Mathf.Max(w - start, 0.0f);
//
//                    lbl.autoSize = false;
//                    lbl.autoHeight = true;
//                    lbl.maximumSize = new Vector2(maxWidth, 0.0f);
//                    lbl.width = maxWidth;   // else wordWrap breaks words even if there's enough space...
//                    lbl.wordWrap = true;
//                }
//            }
//
//            return rows;
//        }
//
//        public static IList<T> AlignColumns<T>(this IList<T> rows, ITabSettings settings)
//            where T : Row
//        {
//            //determine max length for each column
//            var maxs = new float[rows.Max(r => r.Columns.Length)];
//            foreach (var row in rows)
//            {
//                var i = 0;
//                foreach (var c in row.Columns)
//                {
//                    var width = c.width;
//
//                    var max = maxs[i];
//                    maxs[i] = Mathf.Max(max, width);
//
//                    i++;
//                }
//            }
//
//            //place elements according to max width of the columns
//            foreach (var row in rows)
//            {
//                var left = 0.0f;
//                var i = 0;
//                foreach (var c in row.Columns)
//                {
//                    c.relativePosition = new Vector3(left, c.relativePosition.y);
//                    
//                    var max = maxs[i];
//                    left += max + settings.IndentationBetweenColumns;
//
//                    i++;
//                }
//            }
//
//            return rows;
//        }
//
//        public static IList<T> IndentRows<T>(this IList<T> rows, float indention, ITabSettings settings)
//            where T : Row
//        {
//            foreach (var row in rows)
//            {
//                foreach (var c in row.Columns)
//                {
//                    c.relativePosition = new Vector3(c.relativePosition.x + indention, c.relativePosition.y);
//                }
//            }
//
//            return rows;
//        }
//
//        public static IList<T> UpdateHeightOfParentToRows<T>(this IList<T> rows, UIComponent parent)
//            where T : Row
//        {
//            var maxHeight = rows.SelectMany(r => r.Columns)
//                                .Select(c => c.relativePosition.y + c.height)
//                                .Max();
//            parent.Height(maxHeight);
//
//            return rows;
//        }
//        #endregion
//
//        #region functional-ish
//        public static IEnumerable<T> And<T>(this IEnumerable<T> values, T value)
//        {
//            return values.Concat(new[] {value});
//        }
//
//        public static IEnumerable<T> And<T>(this T value1, T value2)
//        {
//            return new [] {value1, value2};
//        }
//        #endregion
//    }
//}