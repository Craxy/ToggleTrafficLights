using System.Xml.Linq;
using JetBrains.Annotations;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.Option
{
    public interface ISerializableOption
    {
        [NotNull]
        string Name { get; }

        bool IsChanged { get; }
        bool IsDefault { get; }

        void MarkAsSaved();
        [CanBeNull]
        XElement Serialize();
        void Deserialize([NotNull] XElement xml);
    }
}