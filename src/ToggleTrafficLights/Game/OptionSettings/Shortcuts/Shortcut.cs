using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;
using JetBrains.Annotations;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.OptionSettings.Shortcuts
{
    // not SavedInputKey: don't want to save in C:S settings file, but instead in custom XML file
    // SavedInputKey doesn't work without a settings file
    // additional SavedInputKey can't handle multiple keys (excluding modifiers)
    public sealed class Shortcut : IEquatable<Shortcut>, IImmutable
    {
        //the unity key handling is just horrible: Event for GUI, Input for Update -- and of course both are totally different...
        // ergo: ignoring Event.... (I don't need it (yet...))

        public static bool IsControlPressed() => Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        public static bool IsControlPressed(Event e) => e.control;
        public static bool IsAltPressed() => Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        public static bool IsAltPressed(Event e) => e.alt;
        public static bool IsShiftPressed() => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        public static bool IsShiftPressed(Event e) => e.shift;

        public static string ControlString = "Ctrl";
        public static string AltString = "Alt";
        public static string ShiftString = "Shift";


        public static KeyCode[] NoKeys { get; } = new KeyCode[0];

        [NotNull]
        public KeyCode[] KeyCodes { get; }
        public bool Alt { get; }
        public bool Shift { get; }
        public bool Control { get; }

        #region create
        protected Shortcut([NotNull] KeyCode[] keyCodes, bool control, bool alt, bool shift)
        {
            KeyCodes = keyCodes;
            Alt = alt;
            Shift = shift;
            Control = control;
        }

        public static Shortcut None()
        {
            return new Shortcut(new[] { KeyCode.None, }, false, false, false);
        }
        public static Shortcut Create()
        {
            return new Shortcut(new KeyCode[0], false, false, false);
        }
        public static Shortcut Create(KeyCode key)
        {
            return new Shortcut(new[] { key }, false, false, false);
        }

        public Shortcut Add(KeyCode key)
        {
            return new Shortcut(KeyCodes.ImmutableAppend(key), alt: Alt, shift: Shift, control: Control);
        }
        public Shortcut With(KeyCode key) => Add(key);
        public Shortcut WithAlt()
        {
            return new Shortcut(KeyCodes, alt: true, shift: Shift, control: Control);
        }
        public Shortcut WithShift()
        {
            return new Shortcut(KeyCodes, alt: Alt, shift: true, control: Control);
        }
        public Shortcut WithControl()
        {
            return new Shortcut(KeyCodes, alt: Alt, shift: Shift, control: true);
        }

        public static Shortcut Create(KeyCode key, bool control, bool alt, bool shift)
        {
            return new Shortcut(new[] { key }, alt: alt, shift: shift, control: control);
        }
        public static Shortcut Create([NotNull] KeyCode[] keys, bool control, bool alt, bool shift)
        {
            return new Shortcut(keys, control: control, alt: alt, shift: shift);
        }
        #endregion

        #region status

        public bool ModifiersPressed()
        {
            return (!Alt || IsAltPressed())
                   && (!Shift || IsShiftPressed())
                   && (!Control || IsControlPressed());
        }
        public bool IsPressed()
        {
            if (!ModifiersPressed())
            {
                return false;
            }
            if (KeyCodes.Length == 0)
            {
                return true;
            }
            if (KeyCodes.All(key => key == KeyCode.None))
            {
                return false;
            }

            return KeyCodes.Where(key => key != KeyCode.None).All(Input.GetKey);
        }

        public bool IsDown()
        {
            if (!ModifiersPressed())
            {
                return false;
            }
            if (KeyCodes.Length == 0)
            {
                return true;
            }
            if (KeyCodes.All(key => key == KeyCode.None))
            {
                return false;
            }

            //special case: 1 key
            //most likely to be used always
            if (KeyCodes.Length == 1)
            {
                return Input.GetKeyDown(KeyCodes.Single());
            }

            //special case: 2 keys
            // most likely to be used when more than one key
            if (KeyCodes.Length == 2)
            {
                return (Input.GetKeyDown(KeyCodes[0]) && Input.GetKeyDown(KeyCodes[1]))
                       || (Input.GetKeyDown(KeyCodes[0]) && Input.GetKey(KeyCodes[1]))
                       || (Input.GetKey(KeyCodes[0]) && Input.GetKeyDown(KeyCodes[1]));
            }

            //more than 2 keys
            //all codes must be pressed exept one, which must be down
            for (var i = 0; i < KeyCodes.Length; i++)
            {
                var current = KeyCodes[i];

                if (current == KeyCode.None)
                {
                    continue;
                }
                if (!Input.GetKeyDown(current))
                {
                    continue;
                }

                //all other keys must be pressed
                bool match = true;
                for (int j = 0; j < KeyCodes.Length; j++)
                {
                    if (j == i)
                    {
                        continue;
                    }
                    var c = KeyCodes[j];

                    if (c == KeyCode.None)
                    {
                        continue;
                    }


                    match &= Input.GetKey(c) || Input.GetKeyDown(c);

                    if (!match)
                    {
                        break;
                    }
                }

                if (match)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region ToString
        public IEnumerable<string> GetKeyStrings()
        {

            //modifiers
            if (Control)
            {
                yield return ControlString;
            }
            if (Alt)
            {
                yield return AltString;
            }
            if (Shift)
            {
                yield return ShiftString;
            }

            //actual keys
            foreach (var key in KeyCodes)
            {
                yield return key.ToString();
            }
        }
        public override string ToString()
        {
            return string.Join("+", GetKeyStrings().ToArray());
        }
        #endregion

        #region parse
        public static bool TryParse(string keyString, out Shortcut shortcut)
        {
            if (keyString.IsNullOrWhiteSpace())
            {
                shortcut = Create().With(KeyCode.None);
                return true;
            }

            var parts = keyString.Split('+');

            if (parts.Length == 0)
            {
                shortcut = null;
                return false;
            }


            int currentIndex = 0;

            //modifiers must appear first
            bool modifiersParsed = false;
            bool control = false, alt = false, shift = false;
            KeyCode[] keys = new KeyCode[0];

            while (currentIndex < parts.Length)
            {
                var head = parts[currentIndex++].Trim();

                if (!modifiersParsed)
                {
                    if (head == ControlString && !control)
                    {
                        control = true;
                    }
                    else if (head == AltString && !alt)
                    {
                        alt = true;
                    }
                    else if (head == ShiftString && !shift)
                    {
                        shift = true;
                    }
                    else
                    {
                        modifiersParsed = true;
                    }
                }

                if (modifiersParsed)
                {
                    //yay...old .net/mono -> no Enum.TryParse...
                    try
                    {
                        var key = (KeyCode)Enum.Parse(typeof(KeyCode), head);
                        keys = keys.ImmutableAppend(key);
                    }
                    catch
                    {
                        DebugLog.Info($"Failed to parse shortcut \"{keyString}\" at \"{head}\"");
                        shortcut = null;
                        return false;
                    }
                }
            }

            shortcut = Create(keys, alt, control, shift);
            return true;
        }
        #endregion

        #region Equality members

        public bool Equals(Shortcut other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return KeyCodes.SequenceEqual(other.KeyCodes)
                   && Alt == other.Alt
                   && Shift == other.Shift
                   && Control == other.Control;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            return obj is Shortcut && Equals((Shortcut) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = KeyCodes.GetHashCode();
                hashCode = (hashCode*397) ^ Alt.GetHashCode();
                hashCode = (hashCode*397) ^ Shift.GetHashCode();
                hashCode = (hashCode*397) ^ Control.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Shortcut left, Shortcut right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Shortcut left, Shortcut right)
        {
            return !Equals(left, right);
        }

        #endregion
    }

}