namespace Validot.Tests.Unit.Rules.Text
{
    using System;
    using System.Collections.Generic;

    using Validot.Testing;
    using Validot.Translations;

    using Xunit;

    public class EmailRulesTests
    {
        [Theory]
        [InlineData(-1)]
        [InlineData(20)]
        [InlineData(100)]
        public void Email_Should_ThrowException_When_EnumIsNotDefined(int mode)
        {
            Tester.TestExceptionOnInit<string>(m => m.Email(mode: (EmailValidationMode)mode), typeof(ArgumentException));
        }

        public static IEnumerable<object[]> Email_UsingMode_ComplexRegex_Should_CollectError_Data()
        {
            var testCases = GetTestCases();

            foreach (var testCase in testCases)
            {
                yield return new object[] { testCase.Key, testCase.Value.complexRegex };
            }
        }

        [Theory]
        [MemberData(nameof(Email_UsingMode_ComplexRegex_Should_CollectError_Data))]
        public void Email_UsingMode_ComplexRegex_Should_CollectError(string model, bool expectedIsValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.Email(mode: EmailValidationMode.ComplexRegex),
                expectedIsValid,
                MessageKey.Texts.Email);
        }

        [Theory]
        [MemberData(nameof(Email_UsingMode_ComplexRegex_Should_CollectError_Data))]
        public void Email_Should_CollectError_WithComplexRegexMode_ByDefault(string model, bool expectedIsValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.Email(),
                expectedIsValid,
                MessageKey.Texts.Email);
        }

        public static IEnumerable<object[]> Email_UsingMode_DataAnnotationsCompatible_Should_CollectError_Data()
        {
            var testCases = GetTestCases();

            foreach (var testCase in testCases)
            {
                yield return new object[] { testCase.Key, testCase.Value.dataAnnotations };
            }
        }

        [Theory]
        [MemberData(nameof(Email_UsingMode_DataAnnotationsCompatible_Should_CollectError_Data))]
        public void Email_UsingMode_DataAnnotationsCompatible_Should_CollectError(string model, bool expectedIsValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.Email(mode: EmailValidationMode.DataAnnotationsCompatible),
                expectedIsValid,
                MessageKey.Texts.Email);
        }

        private static Dictionary<string, (bool complexRegex, bool dataAnnotations)> GetTestCases()
        {
            var dictionary = new Dictionary<string, (bool complexRegex, bool dataAnnotations)>()
            {
                ["prettyandsimple@example.com"] = (complexRegex: true, dataAnnotations: true),
                ["very.common@example.com"] = (complexRegex: true, dataAnnotations: true),
                ["disposable.style.email.with+symbol@example.com"] = (complexRegex: true, dataAnnotations: true),
                ["other.email-with-dash@example.com"] = (complexRegex: true, dataAnnotations: true),
                ["fully-qualified-domain@example.com."] = (complexRegex: false, dataAnnotations: true),
                ["user.name+tag+sorting@example.com"] = (complexRegex: true, dataAnnotations: true),
                ["x@example.com"] = (complexRegex: true, dataAnnotations: true),
                ["example-indeed@strange-example.com"] = (complexRegex: true, dataAnnotations: true),
                ["admin@mailserver1"] = (complexRegex: false, dataAnnotations: true),
                ["email@123.123.123.123"] = (complexRegex: true, dataAnnotations: true),
                ["email@[123.123.123.123]"] = (complexRegex: true, dataAnnotations: true),
                ["1234567890@example.com"] = (complexRegex: true, dataAnnotations: true),
                ["#!$%&'*+-/=?^_`{}|~@example.org"] = (complexRegex: false, dataAnnotations: true),
                [@"""()<>[]:,;@\\\""!#$%&'-/=?^_`{}| ~.a""@example.org"] = (complexRegex: true, dataAnnotations: false),
                ["Abc.example.com"] = (complexRegex: false, dataAnnotations: false),
                ["A@b@c@example.com"] = (complexRegex: false, dataAnnotations: false),
                [@"a""b(c)d,e:f;g<h>i[j\k]l@example.com"] = (complexRegex: false, dataAnnotations: true),
                [@"just""not""right@example.com"] = (complexRegex: false, dataAnnotations: true),
                [@"this is""not\allowed@example.com"] = (complexRegex: false, dataAnnotations: true),
                [@"this\ still\""not\\allowed@example.com"] = (complexRegex: false, dataAnnotations: true),
                ["Duy"] = (complexRegex: false, dataAnnotations: false),
                [" email@example.com"] = (complexRegex: false, dataAnnotations: true),
                ["email@example.com "] = (complexRegex: false, dataAnnotations: true),
                [""] = (complexRegex: false, dataAnnotations: false),
                ["david.jones@proseware.com"] = (complexRegex: true, dataAnnotations: true),
                ["d.j@server1.proseware.com"] = (complexRegex: true, dataAnnotations: true),
                ["jones@ms1.proseware.com"] = (complexRegex: true, dataAnnotations: true),
                ["j.@server1.proseware.com"] = (complexRegex: false, dataAnnotations: true),
                ["j@proseware.com9"] = (complexRegex: true, dataAnnotations: true),
                ["js#internal@proseware.com"] = (complexRegex: true, dataAnnotations: true),
                ["j_9@[129.126.118.1]"] = (complexRegex: true, dataAnnotations: true),
                ["j..s@proseware.com"] = (complexRegex: false, dataAnnotations: true),
                ["js*@proseware.com"] = (complexRegex: false, dataAnnotations: true),
                ["js@proseware..com"] = (complexRegex: false, dataAnnotations: true),
                ["js@proseware.com9"] = (complexRegex: true, dataAnnotations: true),
                ["j.s@server1.proseware.com"] = (complexRegex: true, dataAnnotations: true),
                ["js@contoso.中国"] = (complexRegex: true, dataAnnotations: true),
                [@"""j""s""@proseware.com"] = (complexRegex: true, dataAnnotations: true),
                ["\u00A0@someDomain.com"] = (complexRegex: false, dataAnnotations: true),
                ["!#$%&'*+-/=?^_`|~@someDomain.com"] = (complexRegex: false, dataAnnotations: true),
                ["someName@some~domain.com"] = (complexRegex: false, dataAnnotations: true),
                ["someName@some_domain.com"] = (complexRegex: false, dataAnnotations: true),
                ["someName@1234.com"] = (complexRegex: true, dataAnnotations: true),
                ["someName@someDomain\uFFEF.com"] = (complexRegex: false, dataAnnotations: true),
                [" \r \t \n"] = (complexRegex: false, dataAnnotations: false),
                ["@someDomain.com"] = (complexRegex: false, dataAnnotations: false),
                ["@someDomain@abc.com"] = (complexRegex: false, dataAnnotations: false),
                ["someName"] = (complexRegex: false, dataAnnotations: false),
                ["someName@"] = (complexRegex: false, dataAnnotations: false),
                ["someName@a@b.com"] = (complexRegex: false, dataAnnotations: false),
            };

            return dictionary;
        }
    }
}
