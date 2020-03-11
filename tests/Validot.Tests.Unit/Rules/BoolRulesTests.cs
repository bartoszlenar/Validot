namespace Validot.Tests.Unit.Rules
{
    using Validot.Testing;
    using Validot.Translations;

    using Xunit;

    public class BoolRulesTests
    {
        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void True_Should_CollectError(bool value, bool expectedIsValid)
        {
            Tester
                .TestSingleRule(
                    value,
                    m => m.True(),
                    expectedIsValid,
                    MessageKey.BoolType.True);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void NullableTrue_Should_CollectError(bool value, bool expectedIsValid)
        {
            Tester
                .TestSingleRule<bool?>(
                    value,
                    m => m.True(),
                    expectedIsValid,
                    MessageKey.BoolType.True);
        }

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public void False_Should_CollectError(bool value, bool expectedIsValid)
        {
            Tester.TestSingleRule<bool?>(
                value,
                m => m.False(),
                expectedIsValid,
                MessageKey.BoolType.False);
        }

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public void NullableFalse_Should_CollectError(bool value, bool expectedIsValid)
        {
            Tester.TestSingleRule(
                value,
                m => m.False(),
                expectedIsValid,
                MessageKey.BoolType.False);
        }
    }
}
