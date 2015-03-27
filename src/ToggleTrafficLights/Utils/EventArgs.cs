using System;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils
{
    public class EventArgs<T> : EventArgs
    {
        private readonly T _value;
        public T Value
        {
            get { return _value; }
        }

        public EventArgs(T value)
        {
            _value = value;
        }
    }
}