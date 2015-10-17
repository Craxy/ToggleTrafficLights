using System;
using System.Diagnostics;
using System.Xml.Linq;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using JetBrains.Annotations;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.OptionSettings
{
    public interface ISavedOption : ISerializableOption
    {
        bool Enabled { get; set; }
        bool Save { get; }
    }

    public interface ISavedOption<T> : ISavedOption
    {
        T DefaultValue { get; }
        T SavedValue { get; }
        T Value { get; set; }
    }

    public class SavedOption<T> : ISavedOption<T>
    {
        #region ctor
        public SavedOption(string name, T defaultValue, Serializer serializeMethod, Deserializer deserializeMethod, bool enabled = true, bool save = true)
        {
            Name = name;
            _value = _savedValue = DefaultValue = defaultValue;
            SerializeMethod = serializeMethod;
            DeserializeMethod = deserializeMethod;

            Enabled = enabled;
            Save = save;
        }

        #endregion

        #region properties
        public string Name { get; }

        //TODO: Enabled is not part of changed
        /// <summary>
        /// user defined value to enable or disable the option (like shortcut)
        /// 
        /// doesn't have any effect for serializing, deserializing, etc.
        /// but when use it the code might check this value
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// enable or disable saving, ergo serializing
        /// disables deserializing too, ergo uses DefaultValue when loaded
        /// 
        /// Value can still be changed during runtime, but want be saved over game sessions
        /// </summary>
        public bool Save { get; set; }
        #endregion

        #region value
        public T DefaultValue { get; }

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
        public event EventHandler<ValueChangedEventArgs<T>> ValueChanged;
        protected virtual void OnValueChanged(T oldValue, T newValue)
        {
            ValueChanged?.Invoke(this, ValueChangedEventArgs.Create(oldValue, newValue));
        }

        private T _savedValue;
        public T SavedValue
        {
            get
            {
                return _savedValue;
            }
            set
            {
                if (!Equals(_savedValue, value))
                {
                    var old = _savedValue;

                    _savedValue = value;

                    OnValueChanged(old, value);
                }
            }
        }
        public event EventHandler<ValueChangedEventArgs<T>> ValueSaved;
        protected virtual void OnValueSaved(T oldValue, T newValue)
        {
            ValueSaved?.Invoke(this, ValueChangedEventArgs.Create(oldValue, newValue));
        }

        public bool IsChanged => !Equals(Value, SavedValue);
        public bool IsDefault => Equals(Value, DefaultValue);
        #endregion

        #region serialize
        public delegate XElement Serializer(string name, T value);
        public Serializer SerializeMethod { get; }
        public delegate Option<T> Deserializer(string name, XElement xml);
        public Deserializer DeserializeMethod { get; }

        public XElement Serialize(bool save)
        {
            if (!Save)
            {
                return null;
            }

            if (SerializeMethod == null)
            {
                throw new InvalidOperationException("SerializeMethod is null");
            }

            try
            {
                try
                {
                    var xml = SerializeMethod(Name, Value);
                    xml.Add(new XAttribute(nameof(Enabled), Enabled));
                    return xml;
                }
                finally
                {
                    if (save)
                    {
                        SavedValue = Value;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Info($"Error while serializing option \"Name\": {e.Message}");
                return null;
            }
        }
        public void Deserialize([CanBeNull] XElement xml)
        {
            Debug.Assert(xml == null || xml.Name == Name);

            if (!Save)
            {
                return;
            }

            if (xml == null)
            {
                return;
            }

            if (DeserializeMethod == null)
            {
                throw new InvalidOperationException("DeserializeMethod is null");
            }

            try
            {
                var res = DeserializeMethod(Name, xml);
                if (res.IsSome())
                {
                    Value = SavedValue = res.GetValue();
                }

                var attr = xml.Attribute(nameof(Enabled));
                bool enabled;
                if (attr != null && bool.TryParse(attr.Value, out enabled))
                {
                    Enabled = enabled;
                }
            }
            catch (Exception e)
            {
                Log.Info($"Error while deserializing option \"Name\": {e.Message}");
            }
        }
        #endregion

        #region operators
        public static implicit operator T([NotNull] SavedOption<T> v)
        {
            return v.Value;
        }
        #endregion
    }
}