using System.Xml.Linq;
using Craxy.CitiesSkylines.ToggleTrafficLights.Game.OptionSettings;
using NUnit.Framework;

namespace ToggleTrafficLightsTests.Game.SomeOptions
{
    [TestFixture]
    public class SavedOptionTests
    {
        //naming: http://osherove.com/blog/2005/4/3/naming-standards-for-unit-tests.html
        private static SavedOption<int> CreateSavedIntOption(int defaultValue)
        {
            var option = new SavedOption<int>(name: "Integer",
                defaultValue: defaultValue,
                serializeMethod: Serializer.Serialize.Int,
                deserializeMethod: Serializer.Deserialize.Int,
                enabled: true, save: true
                );
            return option;
        }

        [Test]
        public void A_newly_created_option_Should_be_default()
        {
            var option = CreateSavedIntOption(0);

            Assert.That(option.IsDefault, Is.True);
        }
        [Test]
        public void A_newly_created_option_Should_not_be_changed()
        {
            var option = CreateSavedIntOption(0);

            Assert.That(option.IsChanged, Is.False);
        }
        [Test]
        public void A_newly_created_option_Should_serialize_to_its_default_value()
        {
            var option = CreateSavedIntOption(0);

            var xml = option.Serialize(false);

            var expected = new XElement(option.Name, new XAttribute(nameof(option.Enabled), option.Enabled), option.Value);

            Assert.True(XNode.DeepEquals(xml, expected));
        }
        [Test]
        public void An_option_with_save_property_set_to_false_Should_serialize_to_Null()
        {
            var option = CreateSavedIntOption(0);
            option.Save = false;

            var xml = option.Serialize(false);

            Assert.That(xml, Is.Null);
        }
        [Test]
        public void A_serialized_option_Should_deserialize_to_itself()
        {
            var option = CreateSavedIntOption(15);
            var xml = option.Serialize(false);

            var newOption = CreateSavedIntOption(0);
            newOption.Deserialize(xml);

            Assert.That(newOption.Value, Is.EqualTo(option.Value));
            Assert.That(newOption.Enabled, Is.EqualTo(option.Enabled));
        }

        [Test]
        public void An_option_with_a_changed_valued_Should_be_changed()
        {
            var option = CreateSavedIntOption(0);
            option.Value = 42;

            Assert.That(option.IsChanged, Is.True);
        }

        [Test]
        public void An_option_with_a_changed_value_Should_not_be_changed_after_it_was_saved()
        {
            var option = CreateSavedIntOption(0);
            option.Value = 42;

            Assert.That(option.IsChanged, Is.True);

            option.Serialize(true);

            Assert.That(option.IsChanged, Is.False);
        }
        [Test]
        public void An_option_with_a_changed_value_Should_still_be_changed_after_it_was_serialized_but_not_saved()
        {
            var option = CreateSavedIntOption(0);
            option.Value = 42;

            Assert.That(option.IsChanged, Is.True);

            option.Serialize(false);

            Assert.That(option.IsChanged, Is.True);
        }

    }
}