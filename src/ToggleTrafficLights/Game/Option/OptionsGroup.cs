using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using ColossalFramework.UI;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.Option
{
    public interface IOptionsGroup : ISerializableOption
    {
        IEnumerable<ISerializableOption> GetSerializableOptions();
        IEnumerable<ISavedOption> GetOptions();
        IEnumerable<IOptionsGroup> GetGroups();
    }
    public abstract class OptionsGroup : IOptionsGroup
    {
        protected OptionsGroup(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public virtual bool IsChanged => IsAnyValueChanged();

        public virtual bool IsDefault => AreAllValuesDefault();

        #region status
        public bool IsAnyValueChanged()
        {
            return GetSerializableOptions().Any(o => o.IsChanged);
        }
        public bool AreAllValuesChanged()
        {
            return GetSerializableOptions().All(o => o.IsChanged);
        }
        public bool AreAllValuesDefault()
        {
            return GetSerializableOptions().All(o => o.IsDefault);
        }
        public bool IsAnyValuesDefault()
        {
            return GetSerializableOptions().Any(o => o.IsDefault);
        }
        #endregion

        #region options
        public virtual IEnumerable<ISerializableOption> GetSerializableOptions()
        {
                return GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .Where(p => p.PropertyType.GetInterfaces().Contains(typeof(ISerializableOption)))
                        .Select(p => p.GetValue(this, null))
                        .Cast<ISerializableOption>();
        } 

        public virtual IEnumerable<ISavedOption> GetOptions()
        {
            return GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
//                    .Where(p => p.PropertyType.IsSubclassOf(typeof(ISavedOption)))
                    .Where(p => p.PropertyType.GetInterfaces().Contains(typeof(ISavedOption)))
                    .Select(p => p.GetValue(this, null))
                    .Cast<ISavedOption>();
        }

        public virtual IEnumerable<IOptionsGroup> GetGroups()
        {
            return GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => p.PropertyType.GetInterfaces().Contains(typeof(IOptionsGroup)))
                    .Select(p => p.GetValue(this, null))
                    .Cast<IOptionsGroup>();
        } 
        #endregion

        #region serialize
        public virtual XElement Serialize(bool save)
        {
            return new XElement(Name,
                GetSerializableOptions().Select(o => o.Serialize(save))
                );
        }

        public virtual void Deserialize(XElement xml)
        {
            Debug.Assert(xml.Name == Name);

            GetSerializableOptions().ForEach(o =>
            {
                var e = xml.Element(o.Name);
                if (e != null)
                {
                    o.Deserialize(e);
                }
            });
        }
        #endregion
    }
}