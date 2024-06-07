namespace Validot.Tests.Unit.Rules.Text
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using Validot.Testing;
    using Validot.Translations;

    using Xunit;

    public class StringRulesTests
    {
        [Theory]
        [InlineData("abc", "abc", true)]
        [InlineData("!@#$%^&*()_[]{};':\",/.<>?~789456123", "!@#$%^&*()_[]{};':\",/.<>?~789456123", true)]
        [InlineData("ęóąśłżźć", "ęóąśłżźć", true)]
        [InlineData("ABC", "ABC", true)]
        [InlineData("", "", true)]
        [InlineData("", "#", false)]
        [InlineData("abc", "cba", false)]
        [InlineData("abc", "abcd", false)]
        [InlineData("abc", "ABC", false)]
        [InlineData("abc", " abc ", false)]
        [InlineData("ĘÓĄŚŁŻŹĆ", "EOASLZZC", false)]
        public void EqualTo_Should_CollectError(string model, string value, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.EqualTo(value),
                shouldBeValid,
                MessageKey.Texts.EqualTo,
                Arg.Text("value", value),
                Arg.Enum("stringComparison", StringComparison.Ordinal));
        }

        [Theory]
        [InlineData("abc", "abc", false)]
        [InlineData("!@#$%^&*()_[]{};':\",/.<>?~789456123", "!@#$%^&*()_[]{};':\",/.<>?~789456123", false)]
        [InlineData("ęóąśłżźć", "ęóąśłżźć", false)]
        [InlineData("ABC", "ABC", false)]
        [InlineData("", "", false)]
        [InlineData("", "#", true)]
        [InlineData("abc", "cba", true)]
        [InlineData("abc", "abcd", true)]
        [InlineData("abc", "ABC", true)]
        [InlineData("abc", " abc ", true)]
        [InlineData("ĘÓĄŚŁŻŹĆ", "EOASLZZC", true)]
        public void NotEqualTo_Should_CollectError(string model, string value, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.NotEqualTo(value),
                shouldBeValid,
                MessageKey.Texts.NotEqualTo,
                Arg.Text("value", value),
                Arg.Enum("stringComparison", StringComparison.Ordinal));
        }

        [Theory]
        [InlineData("abc", "abc", true)]
        [InlineData("!@#$%^&*()_[]{};':\",/.<>?~789456123", "!@#$%^&*()_[]{};':\",/.<>?~789456123", true)]
        [InlineData("ęóąśłżźć", "ęóąśłżźć", true)]
        [InlineData("ĘÓĄŚŁŻŹĆ", "ęóąśłżźć", true)]
        [InlineData("ABC", "ABC", true)]
        [InlineData("abc", "ABC", true)]
        [InlineData("abc 123 !@# ĘÓĄŚŁŻŹĆ DEF", "ABC 123 !@# ęóąśłżźć def", true)]
        [InlineData("", "", true)]
        [InlineData("", "#", false)]
        [InlineData("abc", "cba", false)]
        [InlineData("abc", "abcd", false)]
        [InlineData("abc", " abc ", false)]
        [InlineData("ĘÓĄŚŁŻŹĆ", "EOASLZZC", false)]
        public void EqualTo_Should_CollectError_When_ComparisonIgnoreCase(string model, string value, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.EqualTo(value, StringComparison.OrdinalIgnoreCase),
                shouldBeValid,
                MessageKey.Texts.EqualTo,
                Arg.Text("value", value),
                Arg.Enum("stringComparison", StringComparison.OrdinalIgnoreCase));
        }

        [Theory]
        [InlineData("abc", "abc", false)]
        [InlineData("!@#$%^&*()_[]{};':\",/.<>?~789456123", "!@#$%^&*()_[]{};':\",/.<>?~789456123", false)]
        [InlineData("ęóąśłżźć", "ęóąśłżźć", false)]
        [InlineData("ĘÓĄŚŁŻŹĆ", "ęóąśłżźć", false)]
        [InlineData("ABC", "ABC", false)]
        [InlineData("abc", "ABC", false)]
        [InlineData("abc 123 !@# ĘÓĄŚŁŻŹĆ DEF", "ABC 123 !@# ęóąśłżźć def", false)]
        [InlineData("", "", false)]
        [InlineData("", "#", true)]
        [InlineData("abc", "cba", true)]
        [InlineData("abc", "abcd", true)]
        [InlineData("abc", " abc ", true)]
        [InlineData("ĘÓĄŚŁŻŹĆ", "EOASLZZC", true)]
        public void NotEqualTo_Should_CollectError_When_ComparisonIgnoreCase(string model, string value, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.NotEqualTo(value, StringComparison.OrdinalIgnoreCase),
                shouldBeValid,
                MessageKey.Texts.NotEqualTo,
                Arg.Text("value", value),
                Arg.Enum("stringComparison", StringComparison.OrdinalIgnoreCase));
        }

        public static IEnumerable<object[]> Contains_Should_CollectError_Data()
        {
            yield return new object[] { $"test{Environment.NewLine}abc", "ABC", StringComparison.Ordinal, false };
            yield return new object[] { $"test{Environment.NewLine}abc", "ABC", StringComparison.OrdinalIgnoreCase, true };
            yield return new object[] { $"test{Environment.NewLine}abc", $"{Environment.NewLine}abc", StringComparison.OrdinalIgnoreCase, true };
            yield return new object[] { $"test{Environment.NewLine}abc", $"{Environment.NewLine}ABC", StringComparison.OrdinalIgnoreCase, true };
            yield return new object[] { $"test{Environment.NewLine}abc", $"{Environment.NewLine}ABC", StringComparison.Ordinal, false };
        }

        [Theory]
        [InlineData("ruletest", "TEST", StringComparison.Ordinal, false)]
        [InlineData("ruletest", "TEST", StringComparison.OrdinalIgnoreCase, true)]
        [InlineData("ruletest", "test123", StringComparison.Ordinal, false)]
        [InlineData("ruletest", "test123", StringComparison.OrdinalIgnoreCase, false)]
        [InlineData("ruletest", "rule123", StringComparison.Ordinal, false)]
        [InlineData("ruletest", "rule123", StringComparison.OrdinalIgnoreCase, false)]
        [InlineData("abc !@# DEF", "abc !", StringComparison.Ordinal, true)]
        [InlineData("abc !@# DEF", "abc !", StringComparison.OrdinalIgnoreCase, true)]
        [InlineData("abc !@# DEF", "!@#", StringComparison.Ordinal, true)]
        [InlineData("abc !@# DEF", "!@#", StringComparison.OrdinalIgnoreCase, true)]
        [InlineData("abc !@# DEF", "# def", StringComparison.Ordinal, false)]
        [InlineData("abc !@# DEF", "# DEF", StringComparison.OrdinalIgnoreCase, true)]
        [MemberData(nameof(Contains_Should_CollectError_Data))]
        public void Contains_Should_CollectError(string model, string argValue, StringComparison stringComparison, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.Contains(argValue, stringComparison),
                shouldBeValid,
                MessageKey.Texts.Contains,
                Arg.Text("value", argValue),
                Arg.Enum("stringComparison", stringComparison));
        }

        public static IEnumerable<object[]> NotContains_Should_CollectError_Data()
        {
            yield return new object[] { $"test{Environment.NewLine}abc", "ABC", StringComparison.Ordinal, true };
            yield return new object[] { $"test{Environment.NewLine}abc", "ABC", StringComparison.OrdinalIgnoreCase, false };
            yield return new object[] { $"test{Environment.NewLine}abc", $"{Environment.NewLine}abc", StringComparison.OrdinalIgnoreCase, false };
            yield return new object[] { $"test{Environment.NewLine}abc", $"{Environment.NewLine}ABC", StringComparison.OrdinalIgnoreCase, false };
            yield return new object[] { $"test{Environment.NewLine}abc", $"{Environment.NewLine}ABC", StringComparison.Ordinal, true };
        }

        [Theory]
        [InlineData("ruletest", "TEST", StringComparison.Ordinal, true)]
        [InlineData("ruletest", "TEST", StringComparison.OrdinalIgnoreCase, false)]
        [InlineData("ruletest", "test123", StringComparison.Ordinal, true)]
        [InlineData("ruletest", "test123", StringComparison.OrdinalIgnoreCase, true)]
        [InlineData("ruletest", "rule123", StringComparison.Ordinal, true)]
        [InlineData("ruletest", "rule123", StringComparison.OrdinalIgnoreCase, true)]
        [InlineData("abc !@# DEF", "abc !", StringComparison.Ordinal, false)]
        [InlineData("abc !@# DEF", "abc !", StringComparison.OrdinalIgnoreCase, false)]
        [InlineData("abc !@# DEF", "!@#", StringComparison.Ordinal, false)]
        [InlineData("abc !@# DEF", "!@#", StringComparison.OrdinalIgnoreCase, false)]
        [InlineData("abc !@# DEF", "# def", StringComparison.Ordinal, true)]
        [InlineData("abc !@# DEF", "# DEF", StringComparison.OrdinalIgnoreCase, false)]
        [MemberData(nameof(NotContains_Should_CollectError_Data))]
        public void NotContains_Should_CollectError(string model, string argValue, StringComparison stringComparison, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.NotContains(argValue, stringComparison),
                shouldBeValid,
                MessageKey.Texts.NotContains,
                Arg.Text("value", argValue),
                Arg.Enum("stringComparison", stringComparison));
        }

        public static IEnumerable<object[]> NotEmpty_Should_CollectError_NewLines_Data()
        {
            yield return new object[] { $"{Environment.NewLine}", true };
            yield return new object[] { $"\t{Environment.NewLine}{Environment.NewLine}", true };
        }

        [Theory]
        [InlineData("abc", true)]
        [InlineData(" ", true)]
        [InlineData("", false)]
        [MemberData(nameof(NotEmpty_Should_CollectError_NewLines_Data))]
        public void NotEmpty_Should_CollectError(string model, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.NotEmpty(),
                shouldBeValid,
                MessageKey.Texts.NotEmpty);
        }

        public static IEnumerable<object[]> NotWhiteSpace_Should_CollectError_NewLines_Data()
        {
            yield return new object[] { $"{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}_", true };
            yield return new object[] { $"\t{Environment.NewLine}\t\t_", true };

            yield return new object[] { $"{Environment.NewLine}", false };
            yield return new object[] { $"\t{Environment.NewLine}", false };
            yield return new object[] { $"{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}", false };
            yield return new object[] { $"\t{Environment.NewLine}\t{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}", false };
        }

        [Theory]
        [InlineData("abc", true)]
        [InlineData("\t\t\t\t_\t\t\t", true)]
        [InlineData(" ", false)]
        [InlineData("\t", false)]
        [InlineData("", false)]
        [MemberData(nameof(NotWhiteSpace_Should_CollectError_NewLines_Data))]
        public void NotWhiteSpace_Should_CollectError(string model, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.NotWhiteSpace(),
                shouldBeValid,
                MessageKey.Texts.NotWhiteSpace);
        }

        public static IEnumerable<object[]> SingleLine_Should_CollectError_NewLines_Data()
        {
            yield return new object[] { $"abc{Environment.NewLine}", false };
            yield return new object[] { $"{Environment.NewLine}", false };
            yield return new object[] { $"{Environment.NewLine}{Environment.NewLine}", false };
            yield return new object[] { $"a{Environment.NewLine}b", false };
            yield return new object[] { $"\t{Environment.NewLine}{Environment.NewLine}", false };
        }

        [Theory]
        [InlineData("", true)]
        [InlineData("abc", true)]
        [MemberData(nameof(SingleLine_Should_CollectError_NewLines_Data))]
        public void SingleLine_Should_CollectError(string model, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.SingleLine(),
                shouldBeValid,
                MessageKey.Texts.SingleLine);
        }

        public static IEnumerable<object[]> ExactLength_Should_CollectError_NewLines_Data()
        {
            yield return new object[] { $"abc{Environment.NewLine}", 4, true };
            yield return new object[] { $"{Environment.NewLine}", 1, true };
            yield return new object[] { $"{Environment.NewLine}{Environment.NewLine}", 2, true };
            yield return new object[] { $"a{Environment.NewLine}b", 3, true };
            yield return new object[] { $"{Environment.NewLine}", 0, false };
        }

        [Theory]
        [InlineData("abc", 3, true)]
        [InlineData("ĘÓĄŚŁŻŹĆ", 8, true)]
        [InlineData("123545", 6, true)]
        [InlineData("ABC_CDE", 7, true)]
        [InlineData("", 0, true)]
        [InlineData("1234567890", 10, true)]
        [InlineData("abc ", 3, false)]
        [InlineData("Ę Ó Ą Ś Ł Ż Ź Ć ", 15, false)]
        [MemberData(nameof(ExactLength_Should_CollectError_NewLines_Data))]
        public void ExactLength_Should_CollectError(string model, int argValue, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.ExactLength(argValue),
                shouldBeValid,
                MessageKey.Texts.ExactLength,
                Arg.Number("length", argValue));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void ExactLength_Should_ThrowException_When_NegativeLength(int argValue)
        {
            Tester.TestExceptionOnInit<string>(m => m.ExactLength(argValue), typeof(ArgumentOutOfRangeException));
        }

        public static IEnumerable<object[]> MaxLength_Should_CollectError_NewLines_Data()
        {
            yield return new object[] { $"abc{Environment.NewLine}", 5, true };
            yield return new object[] { $"{Environment.NewLine}", 1, true };
            yield return new object[] { $"{Environment.NewLine}", 0, false };
            yield return new object[] { $"{Environment.NewLine}{Environment.NewLine}", 1, false };
            yield return new object[] { $"a{Environment.NewLine}b", 3, true };
            yield return new object[] { $"a{Environment.NewLine}b", 2, false };
        }

        [Theory]
        [InlineData("abc", 3, true)]
        [InlineData("abc", 2, false)]
        [InlineData("", 0, true)]
        [InlineData("", 1, true)]
        [InlineData("abc1234567890", int.MaxValue, true)]
        [InlineData("\t\t\t", 3, true)]
        [InlineData("\t\t\t_", 3, false)]
        [InlineData("X", 0, false)]
        [MemberData(nameof(MaxLength_Should_CollectError_NewLines_Data))]
        public void MaxLength_Should_CollectError(string model, int argValue, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.MaxLength(argValue),
                shouldBeValid,
                MessageKey.Texts.MaxLength,
                Arg.Number("max", argValue));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void MaxLength_Should_ThrowException_When_NegativeLength(int argValue)
        {
            Tester.TestExceptionOnInit<string>(m => m.MaxLength(argValue), typeof(ArgumentOutOfRangeException));
        }

        public static IEnumerable<object[]> MinLength_Should_CollectError_NewLines_Data()
        {
            yield return new object[] { $"abc{Environment.NewLine}", 5, false };
            yield return new object[] { $"{Environment.NewLine}", 0, true };
            yield return new object[] { $"{Environment.NewLine}", 1, true };
            yield return new object[] { $"{Environment.NewLine}{Environment.NewLine}", 3, false };
            yield return new object[] { $"a{Environment.NewLine}b", 3, true };
            yield return new object[] { $"a{Environment.NewLine}b", 4, false };
        }

        [Theory]
        [InlineData("abc", 3, true)]
        [InlineData("abc", 2, true)]
        [InlineData("abc", 4, false)]
        [InlineData("", 0, true)]
        [InlineData("", 1, false)]
        [InlineData("abc1234567890", int.MaxValue, false)]
        [InlineData("\t\t\t", 3, true)]
        [InlineData("\t\t\t_", 3, true)]
        [MemberData(nameof(MinLength_Should_CollectError_NewLines_Data))]
        public void MinLength_Should_CollectError(string model, int argValue, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.MinLength(argValue),
                shouldBeValid,
                MessageKey.Texts.MinLength,
                Arg.Number("min", argValue));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void MinLength_Should_ThrowException_When_NegativeLength(int argValue)
        {
            Tester.TestExceptionOnInit<string>(m => m.MinLength(argValue), typeof(ArgumentOutOfRangeException));
        }

        public static IEnumerable<object[]> LengthBetween_Should_CollectError_NewLines_Data()
        {
            yield return new object[] { $"abc{Environment.NewLine}", 0, 3, false };
            yield return new object[] { $"abc{Environment.NewLine}", 0, 4, true };

            yield return new object[] { $"{Environment.NewLine}", 0, 1, true };
            yield return new object[] { $"{Environment.NewLine}", 1, int.MaxValue, true };

            yield return new object[] { $"{Environment.NewLine}{Environment.NewLine}", 0, 2, true };
            yield return new object[] { $"{Environment.NewLine}{Environment.NewLine}", 0, 1, false };

            yield return new object[] { $"a{Environment.NewLine}b", 2, 3, true };
            yield return new object[] { $"a{Environment.NewLine}b", 0, 2, false };
        }

        [Theory]
        [InlineData("abc", 0, 3, true)]
        [InlineData("abc", 1, 3, true)]
        [InlineData("abc", 2, 3, true)]
        [InlineData("abc", 3, 3, true)]
        [InlineData("abc", 3, 4, true)]
        [InlineData("abc", 0, 2, false)]
        [InlineData("abc", 1, 1, false)]
        [InlineData("abc", 0, 1, false)]
        [InlineData("abc", 0, int.MaxValue, true)]
        [InlineData("abc", 4, 5, false)]
        [InlineData("abc", 4, int.MaxValue, false)]
        [MemberData(nameof(LengthBetween_Should_CollectError_NewLines_Data))]
        public void LengthBetween_Should_CollectError(string model, int min, int max, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.LengthBetween(min, max),
                shouldBeValid,
                MessageKey.Texts.LengthBetween,
                Arg.Number("min", min),
                Arg.Number("max", max));
        }

        [Theory]
        [InlineData(-1, 1)]
        [InlineData(1, -1)]
        [InlineData(int.MinValue, 1)]
        [InlineData(int.MinValue, int.MaxValue)]
        [InlineData(int.MinValue, int.MinValue)]
        public void LengthBetween_Should_ThrowException_When_NegativeLength(int min, int max)
        {
            Tester.TestExceptionOnInit<string>(m => m.LengthBetween(min, max), typeof(ArgumentOutOfRangeException));
        }

        [Theory]
        [InlineData(int.MaxValue, 0)]
        [InlineData(1, 0)]
        [InlineData(1000, 100)]
        public void LengthBetween_Should_ThrowException_When_MinGreaterThanMax(int min, int max)
        {
            Tester.TestExceptionOnInit<string>(m => m.LengthBetween(min, max), typeof(ArgumentException));
        }

        public static IEnumerable<object[]> Matches_String_Should_CollectError_Data()
        {
            var numericPattern = @"^(0|[1-9][0-9]*)$";

            yield return new object[] { numericPattern, "123", true };
            yield return new object[] { numericPattern, "123.123", false };
            yield return new object[] { numericPattern, "abc", false };
            yield return new object[] { numericPattern, "123abc", false };

            var alphabeticPattern = @"^[a-zA-Z]+$";

            yield return new object[] { alphabeticPattern, "ABCabc", true };
            yield return new object[] { alphabeticPattern, "ABC abc", false };
            yield return new object[] { alphabeticPattern, "ABC.abc", false };
            yield return new object[] { alphabeticPattern, "123", false };
            yield return new object[] { alphabeticPattern, "123abc", false };

            var containsUppercasePattern = @"[A-Z]";

            yield return new object[] { containsUppercasePattern, "ABC", true };
            yield return new object[] { containsUppercasePattern, "ABCabc", true };
            yield return new object[] { containsUppercasePattern, string.Empty, false };
            yield return new object[] { containsUppercasePattern, "abc", false };
            yield return new object[] { containsUppercasePattern, "abc   A", true };

            var guidPattern = @"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$";

            yield return new object[] { guidPattern, "60e2f13f-685d-4165-8615-66ae0a9fcf8d", true };
            yield return new object[] { guidPattern, "60e2f13f-685d-4165-8615-66ae0a9fcf8da", false };
            yield return new object[] { guidPattern, "60e2f13f-685d-4165-8615-66ae0-a9fcf8d", false };
            yield return new object[] { guidPattern, "60e2f13f685d4165861566ae0a9fcf8d", false };
            yield return new object[] { guidPattern, "x0e2f13f-685d-4165-8615-66ae0a9fcf8d", false };
        }

        [Theory]
        [MemberData(nameof(Matches_String_Should_CollectError_Data))]
        public void Matches_String_Should_CollectError(string pattern, string model, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.Matches(pattern),
                shouldBeValid,
                MessageKey.Texts.Matches,
                Arg.Text("pattern", pattern));
        }

        public static IEnumerable<object[]> Matches_Regex_Should_CollectError_Data()
        {
            var numericPattern = new Regex(@"^(0|[1-9][0-9]*)$", RegexOptions.CultureInvariant);

            yield return new object[] { numericPattern, "123", true };
            yield return new object[] { numericPattern, "123.123", false };
            yield return new object[] { numericPattern, "abc", false };
            yield return new object[] { numericPattern, "123abc", false };

            var alphabeticPattern = new Regex(@"^[a-zA-Z]+$", RegexOptions.CultureInvariant);

            yield return new object[] { alphabeticPattern, "ABCabc", true };
            yield return new object[] { alphabeticPattern, "ABC abc", false };
            yield return new object[] { alphabeticPattern, "ABC.abc", false };
            yield return new object[] { alphabeticPattern, "123", false };
            yield return new object[] { alphabeticPattern, "123abc", false };

            var containsUppercasePattern = new Regex(@"[A-Z]", RegexOptions.CultureInvariant);

            yield return new object[] { containsUppercasePattern, "ABC", true };
            yield return new object[] { containsUppercasePattern, "ABCabc", true };
            yield return new object[] { containsUppercasePattern, string.Empty, false };
            yield return new object[] { containsUppercasePattern, "abc", false };
            yield return new object[] { containsUppercasePattern, "abc   A", true };

            var guidPattern = new Regex(@"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$", RegexOptions.CultureInvariant);

            yield return new object[] { guidPattern, "60e2f13f-685d-4165-8615-66ae0a9fcf8d", true };
            yield return new object[] { guidPattern, "60e2f13f-685d-4165-8615-66ae0a9fcf8da", false };
            yield return new object[] { guidPattern, "60e2f13f-685d-4165-8615-66ae0-a9fcf8d", false };
            yield return new object[] { guidPattern, "60e2f13f685d4165861566ae0a9fcf8d", false };
            yield return new object[] { guidPattern, "x0e2f13f-685d-4165-8615-66ae0a9fcf8d", false };
        }

        [Theory]
        [MemberData(nameof(Matches_Regex_Should_CollectError_Data))]
        public void Matches_Regex_Should_CollectError(Regex pattern, string model, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.Matches(pattern),
                shouldBeValid,
                MessageKey.Texts.Matches,
                Arg.Text("pattern", pattern.ToString()));
        }

        public static IEnumerable<object[]> StartsWith_NewLine_Should_CollectError_Data()
        {
            yield return new object[] { $"abc{Environment.NewLine}d", $"abc{Environment.NewLine}", StringComparison.Ordinal, true };
            yield return new object[] { $"abc{Environment.NewLine}d", $"abc{Environment.NewLine}d", StringComparison.Ordinal, true };
            yield return new object[] { $"abc{Environment.NewLine}d", $"abc{Environment.NewLine}D", StringComparison.Ordinal, false };
            yield return new object[] { $"abc{Environment.NewLine}d", $"abc{Environment.NewLine}D", StringComparison.OrdinalIgnoreCase, true };

            yield return new object[] { $"{Environment.NewLine}", $"{Environment.NewLine}", StringComparison.Ordinal, true };
            yield return new object[] { $"{Environment.NewLine}", $"{Environment.NewLine}", StringComparison.OrdinalIgnoreCase, true };
        }

        [Theory]
        [InlineData("abc", "ab", StringComparison.Ordinal, true)]
        [InlineData("abc", "aB", StringComparison.Ordinal, false)]
        [InlineData("abc", "aB", StringComparison.OrdinalIgnoreCase, true)]
        [InlineData("abc", "abc", StringComparison.Ordinal, true)]
        [InlineData("abc", "abC", StringComparison.Ordinal, false)]
        [InlineData("abc", "abC", StringComparison.OrdinalIgnoreCase, true)]
        [MemberData(nameof(StartsWith_NewLine_Should_CollectError_Data))]
        public void StartsWith_Should_CollectError(string model, string value, StringComparison stringComparison, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.StartsWith(value, stringComparison),
                shouldBeValid,
                MessageKey.Texts.StartsWith,
                Arg.Text("value", value),
                Arg.Enum("stringComparison", stringComparison));
        }

        public static IEnumerable<object[]> EndsWith_NewLine_Should_CollectError()
        {
            yield return new object[] { $"abc{Environment.NewLine}d", $"bc{Environment.NewLine}d", StringComparison.Ordinal, true };
            yield return new object[] { $"abc{Environment.NewLine}d", $"abc{Environment.NewLine}d", StringComparison.Ordinal, true };
            yield return new object[] { $"abc{Environment.NewLine}d", $"abc{Environment.NewLine}D", StringComparison.Ordinal, false };
            yield return new object[] { $"abc{Environment.NewLine}d", $"abc{Environment.NewLine}D", StringComparison.OrdinalIgnoreCase, true };

            yield return new object[] { $"{Environment.NewLine}", $"{Environment.NewLine}", StringComparison.Ordinal, true };
            yield return new object[] { $"{Environment.NewLine}", $"{Environment.NewLine}", StringComparison.OrdinalIgnoreCase, true };
        }

        [Theory]
        [InlineData("abc", "bc", StringComparison.Ordinal, true)]
        [InlineData("abc", "Bc", StringComparison.Ordinal, false)]
        [InlineData("abc", "Bc", StringComparison.OrdinalIgnoreCase, true)]
        [InlineData("abc", "abc", StringComparison.Ordinal, true)]
        [InlineData("abc", "Abc", StringComparison.Ordinal, false)]
        [InlineData("abc", "Abc", StringComparison.OrdinalIgnoreCase, true)]
        [MemberData(nameof(EndsWith_NewLine_Should_CollectError))]
        public void EndsWith_Should_CollectError(string model, string value, StringComparison stringComparison, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.EndsWith(value, stringComparison),
                shouldBeValid,
                MessageKey.Texts.EndsWith,
                Arg.Text("value", value),
                Arg.Enum("stringComparison", stringComparison));
        }

        [Fact]
        public void Contains_Should_ThrowException_When_NullValue()
        {
            Tester.TestExceptionOnInit<string>(m => m.Contains(null), typeof(ArgumentNullException));
        }

        [Fact]
        public void EndsWith_Should_ThrowException_When_NullValue()
        {
            Tester.TestExceptionOnInit<string>(m => m.EndsWith(null), typeof(ArgumentNullException));
        }

        [Fact]
        public void EqualTo_Should_ThrowException_When_NullValue()
        {
            Tester.TestExceptionOnInit<string>(m => m.EqualTo(null), typeof(ArgumentNullException));
        }

        [Fact]
        public void Matches_Regex_Should_ThrowException_When_NullValue()
        {
            Tester.TestExceptionOnInit<string>(m => m.Matches((Regex)null), typeof(ArgumentNullException));
        }

        [Fact]
        public void Matches_String_Should_ThrowException_When_NullValue()
        {
            Tester.TestExceptionOnInit<string>(m => m.Matches((string)null), typeof(ArgumentNullException));
        }

        [Fact]
        public void NotContains_Should_ThrowException_When_NullValue()
        {
            Tester.TestExceptionOnInit<string>(m => m.NotContains(null), typeof(ArgumentNullException));
        }

        [Fact]
        public void NotEqualTo_Should_ThrowException_When_NullValue()
        {
            Tester.TestExceptionOnInit<string>(m => m.NotEqualTo(null), typeof(ArgumentNullException));
        }

        [Fact]
        public void StartsWith_Should_ThrowException_When_NullValue()
        {
            Tester.TestExceptionOnInit<string>(m => m.StartsWith(null), typeof(ArgumentNullException));
        }
    }
}
