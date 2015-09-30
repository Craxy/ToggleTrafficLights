using System;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table.Extensions
{
    public static class RowTag
    {
        public static readonly string PropertyValue = "PropertyValueRow";
        public static readonly string Header = "HeaderRow";
        public static readonly string VerticalSpace = "VerticalSpaceRow";
        public static readonly string ColorField = "ColorFieldRow";
        public static readonly string DropDown = "DropDownRow";


        public static Func<Row, bool> SelectRowWithTag(string tag)
        {
            return row => row.Tag == tag;
        }
    }
}