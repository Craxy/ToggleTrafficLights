using System.ComponentModel.Design.Serialization;
using Craxy.CitiesSkylines.ToggleTrafficLights.Game.OptionSettings.Shortcuts;
using NUnit.Framework;
using UnityEngine;

namespace ToggleTrafficLightsTests.Game.SomeOptions.Shortcuts
{
    [TestFixture]
    public class ShortcutTests
    {
        [Test]
        public void The_shortcut_Ctrl_Alt_M_Should_serialize_To_Ctrl_Alt_M_with_plus_as_separator()
        {
            var sc = Shortcut.Create(KeyCode.M).WithControl().WithAlt();

            var str = sc.ToString();

            var expected = "Ctrl+Alt+M";

            Assert.That(str, Is.EqualTo(expected));
        }
        [Test]
        public void Two_shortcut_that_are_the_same_Should_be_equal()
        {
            var sc1 = Shortcut.Create(KeyCode.M).WithControl().WithAlt();
            var sc2 = Shortcut.Create(KeyCode.M).WithControl().WithAlt();

            Assert.That(sc1.Equals(sc2), Is.True);
            Assert.That(sc1 == sc2, Is.True);
            Assert.That(System.Object.Equals(sc1, sc2), Is.True);
            Assert.That(!(sc1 != sc2), Is.True);
        }

        [Test]
        public void A_shortcut_Should_parse_to_itself()
        {
            var sc = Shortcut.Create(KeyCode.M).WithControl().WithAlt();

            var str = sc.ToString();

            Shortcut res;
            var parsed = Shortcut.TryParse(str, out res);

            Assert.That(parsed, Is.True);
            Assert.That(res, Is.EqualTo(sc));
        }
    }
}