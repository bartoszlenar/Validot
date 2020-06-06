namespace Validot.Tests.Unit
{
    using System.Collections.Generic;

    using FluentAssertions;

    using Xunit;

    public class CodeHelperTests
    {
        public static IEnumerable<object[]> Codes_Invalid()
        {
            yield return new object[] { "code code" };
            yield return new object[] { "code " };
            yield return new object[] { " code" };
        }

        public static IEnumerable<object[]> Codes_Valid()
        {
            yield return new object[] { "#ABC" };
            yield return new object[] { "_ABC_" };
            yield return new object[] { "code" };
            yield return new object[] { "code1_code2" };
            yield return new object[] { "CODE_!@#_123" };
            yield return new object[] { "CODE," };
            yield return new object[] { ",CODE" };
        }

        [Theory]
        [MemberData(nameof(Codes_Valid))]
        public void IsCodeValid_Should_ReturnTrue_When_CodeIsValid(string code)
        {
            CodeHelper.IsCodeValid(code).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(Codes_Invalid))]
        public void IsCodeValid_Should_ReturnFalse_When_CodeIsInvalid(string code)
        {
            CodeHelper.IsCodeValid(code).Should().BeFalse();
        }
    }
}
