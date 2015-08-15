using JetBrains.Annotations;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils
{
    public static class ChangingValue
    {
        [NotNull]
        public static ChangingValue<T> Create<T>(T initialValue)
        {
            return new ChangingValue<T>(initialValue);
        } 
    }
    public class ChangingValue<T>
    {
        public ChangingValue(T initialValue)
        {
            _value = initialValue;
        }

        private T _value;
        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (!Equals(_value, value))
                {
                    var old = _value;

                    _value = value;

                    OnValueChanged(old, value);
                }
            }
        }

        public delegate void ValueChangedArgs(T oldValue, T newValue);
        public event ValueChangedArgs ValueChanged;
        protected virtual void OnValueChanged(T oldvalue, T newvalue)
        {
            var handler = ValueChanged;
	        handler?.Invoke(oldvalue, newvalue);
        }

        public static implicit operator T([NotNull] ChangingValue<T> v)
        {
            return v.Value;
        }
    }
}