using System;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table.Entries;
using JetBrains.Annotations;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table.Extensions
{
    public static class EntryExtensions
    {
        //leading C because: ambiguity with ComponentsExtensions, because generic type constraints are not part of the signature.
        // C for Component

        public static T CName<T>([NotNull] this T entry, [NotNull] string name)
            where T : Entry
        {
            entry.Name = name;

            return entry;
        }
        public static T CTag<T>([NotNull] this T entry, [NotNull] string tag)
            where T : Entry
        {
            entry.Tag = tag;

            return entry;
        }

        public static T CSize<T>([NotNull] this T entry, Vector2 size)
            where T : Entry
        {
            entry.Size = size;

            return entry;
        }
        public static T CHeight<T>([NotNull] this T entry, float height)
            where T : Entry
        {
            entry.Height = height;

            return entry;
        }
        public static T CWidth<T>([NotNull] this T entry, float width)
            where T : Entry
        {
            entry.Width = width;

            return entry;
        }

        public static T CRelativePosition<T>([NotNull] this T entry, Vector3 relativePosition)
            where T : Entry
        {
            entry.RelativePosition = relativePosition;

            return entry;
        }
        public static T CX<T>([NotNull] this T entry, float x)
            where T : Entry
        {
            entry.X = x;

            return entry;
        }
        public static T CY<T>([NotNull] this T entry, float y)
            where T : Entry
        {
            entry.Y = y;
            return entry;
        }

        public static T CText<T, TComponent>([NotNull] this T entry, string text)
            where T : TextEntry<TComponent>
            where TComponent : UITextComponent
        {
            entry.Text = text;
            return entry;
        }

        //YAY C# type interference is great: this method can't be used without specifying the types...
        //        public static T Text<T, TComponent>([NotNull] this T entry, [NotNull] string text)
        //            where T : Entry<TComponent> 
        //            where TComponent : UITextComponent
        //        {
        //            entry.UIComponent.text = text;
        //            return entry;
        //        }
        //        public static T DoOnComponent<T>([NotNull] this T entry, [CanBeNull] Action<UIComponent> action)
        //            where T : Entry
        //        {
        //            action?.Invoke(entry.Component);
        //            return entry;
        //        }

        //again: YAY C# type interference: types must be specified to use with specific types (like using a predefined action...)
        public static T DoOnComponent<T, TComponent>([NotNull] this T entry, [CanBeNull] Action<TComponent> action)
            where T : Entry<TComponent>
            where TComponent : UIComponent
        {
            action?.Invoke(entry.UIComponent);
            return entry;
        }
    }
}