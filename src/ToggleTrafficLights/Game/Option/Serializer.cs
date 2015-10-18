using System.Globalization;
using System.Xml.Linq;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;
using JetBrains.Annotations;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.Option
{
    public static class Serializer
    {
        public static class Serialize
        {
            public static XElement Int([NotNull] string name, int value)
            {
                return new XElement(name, value.ToString(CultureInfo.InvariantCulture));
            }
            public static XElement Float([NotNull] string name, float value)
            {
                return new XElement(name, value.ToString(CultureInfo.InvariantCulture));
            }
            public static XElement Bool([NotNull] string name, bool value)
            {
                return new XElement(name, value.ToString(CultureInfo.InvariantCulture));
            }
            public static XElement Color([NotNull] string name, Color value)
            {
                return new XElement(name, value.ToHexStringRGBA());
            }
        }

        public static class Deserialize
        {
            public static Option<int> Int([NotNull] string name, [NotNull] XElement xml)
            {
                int value;
                if (int.TryParse(xml.Value, out value))
                {
                    return Utils.Option.Some(value);
                }
                return Utils.Option.None<int>();
            }
            public static Option<float> Float([NotNull] string name, [NotNull] XElement xml)
            {
                float value;
                if (float.TryParse(xml.Value, out value))
                {
                    return Utils.Option.Some(value);
                }
                return Utils.Option.None<float>();
            }
            public static Option<bool> Bool([NotNull] string name, [NotNull] XElement xml)
            {
                bool value;
                if (bool.TryParse(xml.Value, out value))
                {
                    return Utils.Option.Some(value);
                }
                return Utils.Option.None<bool>();
            }
            public static Option<Color> Color([NotNull] string name, [NotNull] XElement xml)
            {
                string _;
                Color value;
                if (xml.Value.TryParseHexColor(out value, out _))
                {
                    return Utils.Option.Some(value);
                }
                return Utils.Option.None<Color>();
            }
        }
    }

}