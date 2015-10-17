using System.Globalization;
using System.Xml.Linq;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using JetBrains.Annotations;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.OptionSettings
{
    public static class Serializer
    {
        public static class Serialize
        {
            public static XElement Int([NotNull] string name, int value)
            {
                return new XElement(name, value.ToString(CultureInfo.InvariantCulture));
            }
        }

        public static class Deserialize
        {
            public static Option<int> Int([NotNull] string name, [NotNull] XElement xml)
            {
                int value;
                if (int.TryParse(xml.Value, out value))
                {
                    return Option.Some(value);
                }
                return Option.None<int>();
            }
        }
    }

}