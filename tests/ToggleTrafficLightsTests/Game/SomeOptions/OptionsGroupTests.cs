using System;
using System.Linq;
using System.Xml.Linq;
using Craxy.CitiesSkylines.ToggleTrafficLights.Game.OptionSettings;
using NUnit.Framework;

namespace ToggleTrafficLightsTests.Game.SomeOptions
{
    public class OptionsGroupTests
    {
        public class OptionsGroupWithoutANestedGroup : OptionsGroup
        {
            private static SavedOption<int> CreateSavedIntOption(string name, int defaultValue)
            {
                var option = new SavedOption<int>(name: name,
                    defaultValue: defaultValue,
                    serializeMethod: Serializer.Serialize.Int,
                    deserializeMethod: Serializer.Deserialize.Int,
                    enabled: true, save: true
                    );
                return option;
            }

            public SavedOption<int> Value1 { get; } = CreateSavedIntOption(nameof(Value1), 0);
            public SavedOption<int> Value2 { get; } = CreateSavedIntOption(nameof(Value2), 42);

            #region Overrides of OptionsGroup
            public override string Name { get; } = "TestGroup";
            #endregion
        }

        public class OptionsGroupWithOneNestedGroup : OptionsGroupWithoutANestedGroup
        {
            public OptionsGroupWithoutANestedGroup NestedGroup { get; } = new OptionsGroupWithoutANestedGroup();
        }


        [Test]
        public void An_options_group_with_two_options_Should_find_two_options()
        {
            var group = new OptionsGroupWithoutANestedGroup();

            Assert.That(group.GetOptions().Count(), Is.EqualTo(2));
        }
        [Test]
        public void An_options_group_with_two_options_Should_have_two_SerializableOptions()
        {
            var group = new OptionsGroupWithoutANestedGroup();

            Assert.That(group.GetSerializableOptions().Count(), Is.EqualTo(2));
        }

        [Test]
        public void An_options_group_with_two_unchanged_options_Should_be_unchanged()
        {
            var group = new OptionsGroupWithoutANestedGroup();

            Assert.That(group.IsAnyValueChanged(), Is.False);
        }
        [Test]
        public void An_options_group_with_two_options_with_their_default_values_Should_be_default()
        {
            var group = new OptionsGroupWithoutANestedGroup();

            Assert.That(group.AreAllValuesDefault(), Is.True);
        }

        [Test]
        public void An_options_group_with_two_options_with_one_changed_option_Should_be_changed()
        {
            var group = new OptionsGroupWithoutANestedGroup();

            group.Value1.Value = 1234;

            Assert.That(group.IsAnyValueChanged(), Is.True);
        }

        [Test]
        public void An_options_group_with_two_options_Should_serialize_both_options_under_the_groups_name()
        {
            var group = new OptionsGroupWithoutANestedGroup();

            var xml = group.Serialize(false);

            var expected = new XElement(group.Name,
                group.Value1.Serialize(false),
                group.Value2.Serialize(false)
                );

            Assert.That(XNode.DeepEquals(xml, expected), Is.True);
        }

        [Test]
        public void An_options_group_with_two_options_Should_serialize_and_deserialize_afterwards_to_itself()
        {
            var group = new OptionsGroupWithoutANestedGroup();

            group.Value1.Value = 1234;
            group.Value2.Value = 9876;


            var xml = group.Serialize(false);

            var newGroup = new OptionsGroupWithoutANestedGroup();
            newGroup.Deserialize(xml);

            Assert.That(group.Value1.Value, Is.EqualTo(newGroup.Value1.Value));
            Assert.That(group.Value2.Value, Is.EqualTo(newGroup.Value2.Value));
        }

        [Test]
        public void An_options_group_with_two_options_and_a_group_Should_have_three_ISerializableOptions()
        {
            var group = new OptionsGroupWithOneNestedGroup();

            Assert.That(group.GetSerializableOptions().Count(), Is.EqualTo(3));
        }
        [Test]
        public void An_options_group_with_two_options_and_a_group_Should_have_one_group()
        {
            var group = new OptionsGroupWithOneNestedGroup();

            Assert.That(group.GetGroups().Count(), Is.EqualTo(1));
        }

        [Test]
        public void An_options_group_with_two_options_and_a_group_Should_serialize_two_options_and_the_nested_group()
        {
            var group = new OptionsGroupWithOneNestedGroup();

            var xml = group.Serialize(false);
            
            var expected = new XElement(group.Name,
                group.NestedGroup.Serialize(false),
                group.Value1.Serialize(false),
                group.Value2.Serialize(false)
                );

            Console.WriteLine(expected);
            Console.WriteLine(xml);

            Assert.That(XNode.DeepEquals(xml, expected), Is.True);
        }
        [Test]
        public void An_options_group_with_two_options_and_a_group_Should_serialize_and_deserializeToItself()
        {
            var group = new OptionsGroupWithOneNestedGroup();
            group.Value1.Value = 789;
            group.Value2.Value = 456;
            group.NestedGroup.Value1.Value = 123;
            group.NestedGroup.Value2.Value = 258;

            var xml = group.Serialize(false);

            var newGroup = new OptionsGroupWithOneNestedGroup();
            newGroup.Deserialize(xml);

            Assert.IsTrue(newGroup.Value1.Value == group.Value1.Value 
                && newGroup.Value2.Value == group.Value2.Value
                && newGroup.NestedGroup.Value1.Value == group.NestedGroup.Value1.Value
                && newGroup.NestedGroup.Value2.Value == group.NestedGroup.Value2.Value
                );
        }

        [Test]
        public void An_options_group_with_two_options_and_a_group_Should_be_changed_when_a_value_in_the_nested_group_was_changed()
        {
            var group = new OptionsGroupWithOneNestedGroup();

            Assert.That(group.IsChanged, Is.False);

            group.NestedGroup.Value1.Value = 12346;

            Assert.That(group.IsChanged, Is.True);
        }
    }
}