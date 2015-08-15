using System;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils
{
    public class EventArgs<T> : EventArgs
    {
	    public T Value { get; }

	    public EventArgs(T value)
        {
            Value = value;
        }
    }
}