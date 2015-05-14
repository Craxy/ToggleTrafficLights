using System;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.Menu.Components
{
    public class RadioButtonGroup<T>
    {
        public RadioButtonGroup()
        {
            Title = string.Empty;
            Items = new T[0];
            SelectedIndex = 0;
            CalcItemName = item => item.ToString();
        }

        public string Title { get; set; }
        public T[] Items { get; set; }
        public int SelectedIndex { get; set; }
        public T SelectedItem
        {
           get
            {
                return Items[SelectedIndex];
            }
            set
            {
                SelectedIndex = Array.IndexOf(Items, value);
            }
        }
        public delegate string GetItemName(T item);
        public GetItemName CalcItemName { get; set; }

        public delegate void OnSelectedItemChanged(T newItem, T oldItem);

        public void Show(OnSelectedItemChanged onSelectedItemChanged, float indentSubItems = 10.0f, float indent = 0.0f)
        {
            using (Layout.Vertical())
            {
                using(Layout.Horizontal())
                {
                    if (Mathf.Abs(indent) > 0.01f)
                    {
                        GUILayout.Space(indent);
                    }
                    GUILayout.Label(Title + ":");
                }

                var selected = SelectedIndex;
                var i = 0;
                foreach (var item in Items)
                {
                    using(Layout.Horizontal())
                    {
                        var ind = indent + indentSubItems;
                        if (Mathf.Abs(ind) > 0.01f)
                        {
                            GUILayout.Space(ind);
                        }

                        var isSelected = SelectedIndex == i;
                        var txt = CalcItemName != null ? CalcItemName(item) : item.ToString();

                        var newSelected = GUILayout.Toggle(isSelected, txt);
                        if (newSelected != isSelected)
                        {
                            SelectedIndex = i;
                        }
                    }
                    i++;
                }

                if (onSelectedItemChanged != null && SelectedIndex != selected)
                {
                    onSelectedItemChanged(SelectedItem, Items[selected]);
                }
            }
        }
    }
}