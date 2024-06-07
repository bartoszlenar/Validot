namespace Validot.Tests.Unit.Rules.Text
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
        public void EqualToIgnoreCase_Should_CollectError(char modek, char value, bool expectedIsValid)
        {
            Tester.TestSingleRule(
                modek,
                m => m.EqualToIgnoreCase(value),
                expectedIsValid,
                MessageKey.CharType.EqualToIgnoreCase,
                Arg.Text("value", value));
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
        public void NotEqualToIgnoreCase_Should_CollectError(char model, char argValue, bool expectedIsValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.NotEqualToIgnoreCase(argValue),
                expectedIsValid,
                MessageKey.CharType.NotEqualToIgnoreCase,
                Arg.Text("value", argValue));
        }
    }
}
