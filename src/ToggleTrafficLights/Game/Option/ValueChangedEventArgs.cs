using System;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.Option
{
    public class ValueChangedEventArgs
    {
        public static ValueChangedEventArgs<T> Create<T>(T oldValue, T newValue)
        {
            return new ValueChangedEventArgs<T>(oldValue, newValue);
        } 
    }
    public class ValueChangedEventArgs<T> : EventArgs
    {
        public T OldValue { get; }
        public T NewValue { get; }

        public ValueChangedEventArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}