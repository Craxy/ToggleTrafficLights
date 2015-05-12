using System;
using System.Linq;
using System.Xml.Schema;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.Menu.Components
{
    public class ComboBox<T>
    {
        public bool ShowDropDown { get; private set; }
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
        private Rect _rect = new Rect(0,0,0,0);

        public delegate string GetItemName(T item);
        public GetItemName CalcItemName { get; set; }

        public string GetItemNameByIndex(int index)
        {
            return CalcItemName(Items[index]);
        }

        public ComboBox()
        {
            ShowDropDown = false;
         
            CalcItemName = item => item.ToString();
        }


        public int Show()
        {
            if (Items == null || Items.Length == 0)
            {
                throw new InvalidOperationException("No items specified!");
            }
            if (Items.Length == 1)
            {
                GUILayout.Label(GetItemNameByIndex(0));
                return 0;
            }

            var items = Items.Select(v => CalcItemName(v)).ToArray();

            //width of button
            var width = items.Max(v => GUI.skin.button.CalcSize(new GUIContent(v)).x);


            //id for button
            var controlId = GUIUtility.GetControlID(FocusType.Passive);


            if (GUILayout.Button(items[SelectedIndex] + " ↓", GUILayout.Width(width + 12)))
            {
                ShowDropDown = true;
            }

            //get position of Button
            if (Event.current.type == EventType.Repaint)
            {
                var lastRect = GUILayoutUtility.GetLastRect();
                if (lastRect != _rect)
                {
                    _rect = lastRect;
                }
            }

            if(ShowDropDown && _rect.height != 0)
            {
                var rect = new Rect(_rect.x,
                                    _rect.y + GUI.skin.button.CalcSize);

                var rect = new Rect(_rect.x, _rect.y + )
            }
        }
    }
}