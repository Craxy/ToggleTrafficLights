using System;
using ColossalFramework.UI;
using JetBrains.Annotations;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components.Table
{
    public class Entry
    {
        public UIComponent Component { get; }
        internal Entry([NotNull] UIComponent component)
        {
            Component = component;
        }

        #region properties of Component
	    public virtual string Name
	    {
            get { return Component.name; }
            set { Component.name = value; }
	    }

        public virtual string Tag
        {
            get { return Component.tag; }
            set { Component.tag = value; }
        }
        public virtual Vector3 RelativePosition
        {
            get { return Component.relativePosition; }
            set { Component.relativePosition = value; }
        }
	    public float X
	    {
		    get { return RelativePosition.x; }
		    set { RelativePosition = new Vector3(value, RelativePosition.y, RelativePosition.z); }
	    }
        public float Y
	    {
		    get { return RelativePosition.y; }
		    set { RelativePosition = new Vector3(RelativePosition.x, value, RelativePosition.z); }
	    }

        public virtual Vector2 Size
        {
            get { return Component.size; }
            set { Component.size = value; }
        }
        public float Width
        {
            get { return Component.width; }
            set { Component.width = Mathf.Max(0.0f, value); }
        }
        public float Height
        {
            get { return Component.height; }
            set { Component.height = Mathf.Max(0.0f, value); }
        }
        #endregion

        #region Overrides of Object

        public override string ToString()
        {
            return GetType().Name;
        }

        #endregion
    }

    public class Entry<T> : Entry
        where T : UIComponent
    {
        public T UIComponent => (T)Component;
        public Entry([NotNull] T component) : base(component)
        {
        }
    }
}