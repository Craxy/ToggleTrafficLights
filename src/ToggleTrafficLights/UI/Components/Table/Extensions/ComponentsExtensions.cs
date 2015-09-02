using ColossalFramework.UI;
using JetBrains.Annotations;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table.Extensions
{
    public static class ComponentsExtensions
    {
        #region UIComponent
        public static T ToolTip<T>([NotNull] this T component, [NotNull] string text)
            where T : UIComponent
        {
            component.tooltip = text;
            return component;
        }
        public static T Width<T>([NotNull] this T component, float width)
            where T : UIComponent
        {
            component.width = width;
            return component;
        }
        public static T Height<T>([NotNull] this T component, float height)
            where T : UIComponent
        {
            component.height = height;
            return component;
        }
        #endregion

        #region TextComponent
        public static T TextScale<T>([NotNull] this T label, float textScale)
            where T : UITextComponent
        {
            label.textScale = textScale;
            return label;
        }
        public static T TextColor<T>([NotNull] this T label, Color32 color)
            where T : UITextComponent
        {
            label.textColor = color;
            return label;
        }
        #endregion 
    }
}