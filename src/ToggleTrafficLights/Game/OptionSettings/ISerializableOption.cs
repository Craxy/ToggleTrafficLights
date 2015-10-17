using System.Xml.Linq;
using JetBrains.Annotations;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.OptionSettings
{
    public interface ISerializableOption
    {
        [NotNull]
        string Name { get; }

        bool IsChanged { get; }
        bool IsDefault { get; }

        [CanBeNull]
        XElement Serialize(bool save);
        void Deserialize([NotNull] XElement xml);
    }
}