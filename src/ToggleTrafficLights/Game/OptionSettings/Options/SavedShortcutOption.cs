using System.Diagnostics;
using System.Xml.Linq;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using JetBrains.Annotations;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.OptionSettings.Options
{
    public class SavedShortcutOption : SavedOption<Shortcut>
    {
        public SavedShortcutOption(string name, 
            Shortcut defaultValue, 
            Serializer serializeMethod, 
            Deserializer deserializeMethod, 
            bool enabled = true, 
            bool save = true) 
            : base(name, defaultValue, serializeMethod, deserializeMethod, enabled, save)
        {
        }


        public static XElement Serialize([NotNull] string name, [NotNull] Shortcut value)
        {
            return new XElement(name, value.ToString());
        }
        public static Option<Shortcut> Deserialize([NotNull] string name, [NotNull] XElement xml)
        {
            Debug.Assert(xml.Name == name);

            var value = xml.Value;

            Shortcut sc;
            if (Shortcut.TryParse(value, out sc))
            {
                return Option.Some(sc);
            }
            return Option.None<Shortcut>();
        }
    }
}