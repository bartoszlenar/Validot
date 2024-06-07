namespace Validot.Tests.Unit.Rules
{
    using Validot.Testing;
    using Validot.Translations;

    using Xunit;

    public class CharRulesTests
    {
        [Theory]
        [InlineData('a', 'a', true)]
        [InlineData('A', 'a', true)]
        [InlineData('a', 'A', true)]
        [InlineData('A', 'A', true)]
        [InlineData('A', 'b', false)]
        [InlineData('a', 'B', false)]
        [InlineData('a', 'b', false)]
        [InlineData('A', 'B', false)]
        [InlineData('Ż', 'Ż', true)]
        [InlineData('ć', 'Ć', true)]
        [InlineData('Ą', 'ó', false)]
        public void EqualIgnoreCase_Should_CollectError(char value, char test, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                value,
                m => m.EqualToIgnoreCase(test),
                shouldBeValid,
                MessageKey.CharType.EqualToIgnoreCase,
                Arg.Text("value", test));
        }

        [Theory]
        [InlineData('a', 'a', true)]
        [InlineData('A', 'a', true)]
        [InlineData('a', 'A', true)]
        [InlineData('A', 'A', true)]
        [InlineData('A', 'b', false)]
        [InlineData('a', 'B', false)]
        [InlineData('a', 'b', false)]
        [InlineData('A', 'B', false)]
        [InlineData('Ż', 'Ż', true)]
        [InlineData('ć', 'Ć', true)]
        [InlineData('Ą', 'ó', false)]
        public void EqualIgnoreCase_Nullable_Should_CollectError(char value, char test, bool shouldBeValid)
        {
            Tester.TestSingleRule<char?>(
                value,
                m => m.EqualToIgnoreCase(test),
                shouldBeValid,
                MessageKey.CharType.EqualToIgnoreCase,
                Arg.Text("value", test));
        }

        [Theory]
        [InlineData('a', 'a', false)]
        [InlineData('A', 'a', false)]
        [InlineData('a', 'A', false)]
        [InlineData('A', 'A', false)]
        [InlineData('A', 'b', true)]
        [InlineData('a', 'B', true)]
        [InlineData('a', 'b', true)]
        [InlineData('A', 'B', true)]
        [InlineData('Ż', 'Ż', false)]
        [InlineData('ć', 'Ć', false)]
        [InlineData('Ą', 'ó', true)]
        public void NotEqualIgnoreCase_Should_CollectError(char value, char test, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                value,
                m => m.NotEqualToIgnoreCase(test),
                shouldBeValid,
                MessageKey.CharType.NotEqualToIgnoreCase,
                Arg.Text("value", test));
        }

        [Theory]
        [InlineData('a', 'a', false)]
        [InlineData('A', 'a', false)]
        [InlineData('a', 'A', false)]
        [InlineData('A', 'A', false)]
        [InlineData('A', 'b', true)]
        [InlineData('a', 'B', true)]
        [InlineData('a', 'b', true)]
        [InlineData('A', 'B', true)]
        [InlineData('Ż', 'Ż', false)]
        [InlineData('ć', 'Ć', false)]
        [InlineData('Ą', 'ó', true)]
        public void NotEqualIgnoreCase_Nullable_Should_CollectError(char value, char test, bool shouldBeValid)
        {
            Tester.TestSingleRule<char?>(
                value,
                m => m.NotEqualToIgnoreCase(test),
                shouldBeValid,
                MessageKey.CharType.NotEqualToIgnoreCase,
                Arg.Text("value", test));
        }
    }
}
