namespace Validot.Tests.Unit.Rules
{
    using System;
    using System.Collections.Generic;

    using Validot.Rules;
    using Validot.Testing;
    using Validot.Translations;

    using Xunit;

    public class GuidRulesTests
    {
        public static IEnumerable<object[]> EqualTo_Should_CollectError_Data()
        {
            yield return new object[] { new Guid("c2ce1f3b-17e5-412e-923b-6b4e268f31aa"), new Guid("c2ce1f3b-17e5-412e-923b-6b4e268f31aa"), true };
            yield return new object[] { new Guid("e2ce1f3b-17e5-412e-923b-6b4e268f31aa"), new Guid("c2ce1f3b-17e5-412e-923b-6b4e268f31aa"), false };
        }

        [Theory]
        [MemberData(nameof(EqualTo_Should_CollectError_Data))]
        public void EqualTo_Should_CollectError(Guid memberValue, Guid argValue, bool expectedIsValid)
        {
            Tester.TestSingleRule(
                memberValue,
                m => m.EqualTo(argValue),
                expectedIsValid,
                MessageKey.GuidType.EqualTo,
                Arg.GuidValue("value", argValue));
        }

        [Theory]
        [MemberData(nameof(EqualTo_Should_CollectError_Data))]
        public void EqualTo_Should_CollectError_FromNullable(Guid memberValue, Guid argValue, bool expectedIsValid)
        {
            Tester.TestSingleRule<Guid?>(
                memberValue,
                m => m.EqualTo(argValue),
                expectedIsValid,
                MessageKey.GuidType.EqualTo,
                Arg.GuidValue("value", argValue));
        }

        public static IEnumerable<object[]> NotEqualTo_Should_CollectError_Data()
        {
            yield return new object[] { new Guid("c2ce1f3b-17e5-412e-923b-6b4e268f31aa"), new Guid("c2ce1f3b-17e5-412e-923b-6b4e268f31aa"), false };
            yield return new object[] { new Guid("e2ce1f3b-17e5-412e-923b-6b4e268f31aa"), new Guid("c2ce1f3b-17e5-412e-923b-6b4e268f31aa"), true };
        }

        [Theory]
        [MemberData(nameof(NotEqualTo_Should_CollectError_Data))]
        public void NotEqualTo_Should_CollectError(Guid memberValue, Guid argValue, bool expectedIsValid)
        {
            Tester.TestSingleRule(
                memberValue,
                m => m.NotEqualTo(argValue),
                expectedIsValid,
                MessageKey.GuidType.NotEqualTo,
                Arg.GuidValue("value", argValue));
        }

        [Theory]
        [MemberData(nameof(NotEqualTo_Should_CollectError_Data))]
        public void NotEqualTo_Should_CollectError_FromNullable(Guid memberValue, Guid argValue, bool expectedIsValid)
        {
            Tester.TestSingleRule<Guid?>(
                memberValue,
                m => m.NotEqualTo(argValue),
                expectedIsValid,
                MessageKey.GuidType.NotEqualTo,
                Arg.GuidValue("value", argValue));
        }

        public static IEnumerable<object[]> NotEmpty_Should_CollectError_Data()
        {
            yield return new object[] { new Guid("00000000-0000-0000-0000-000000000000"), false };
            yield return new object[] { new Guid("00000000-0000-0000-0000-000000000001"), true };
            yield return new object[] { new Guid("e2ce1f3b-17e5-412e-923b-6b4e268f31aa"), true };
        }

        [Theory]
        [MemberData(nameof(NotEmpty_Should_CollectError_Data))]
        public void NotEmpty_Should_CollectError(Guid memberValue, bool expectedIsValid)
        {
            Tester.TestSingleRule(
                memberValue,
                m => m.NotEmpty(),
                expectedIsValid,
                MessageKey.GuidType.NotEmpty);
        }

        [Theory]
        [MemberData(nameof(NotEmpty_Should_CollectError_Data))]
        public void NotEmpty_Should_CollectError_FromNullable(Guid memberValue, bool expectedIsValid)
        {
            Tester.TestSingleRule<Guid?>(
                memberValue,
                m => m.NotEmpty(),
                expectedIsValid,
                MessageKey.GuidType.NotEmpty);
        }
    }
}
