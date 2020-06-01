namespace Validot.Translations
{
    using System.Collections.Generic;

    public static partial class Translation
    {
        public static IReadOnlyDictionary<string, string> English { get; } = new Dictionary<string, string>
        {
            [MessageKey.Global.Error] = "Error",
            [MessageKey.Global.Required] = "Required",
            [MessageKey.Global.Forbidden] = "Forbidden",
            [MessageKey.Global.ReferenceLoop] = "(reference loop)",

            [MessageKey.BoolType.True] = "Must be true",
            [MessageKey.BoolType.False] = "Must be false",

            [MessageKey.CharType.EqualToIgnoreCase] = "Must be equal to {value} (ignoring case)",
            [MessageKey.CharType.NotEqualToIgnoreCase] = "Must not be equal to {value} (ignoring case)",

            [MessageKey.GuidType.EqualTo] = "Must be equal to {value}",
            [MessageKey.GuidType.NotEqualTo] = "Must not be equal to {value}",
            [MessageKey.GuidType.NotEmpty] = "Must not be all zeros",

            [MessageKey.Collections.EmptyCollection] = "Must be empty",
            [MessageKey.Collections.NotEmptyCollection] = "Must not be empty",
            [MessageKey.Collections.ExactCollectionSize] = "Must contain exactly {size} items",
            [MessageKey.Collections.MaxCollectionSize] = "Must contain at most {size} items",
            [MessageKey.Collections.MinCollectionSize] = "Must contain at least {size} items",
            [MessageKey.Collections.CollectionSizeBetween] = "Must contain between {min} and {max} items",

            [MessageKey.Numbers.EqualTo] = "Must be equal to {value}",
            [MessageKey.Numbers.NotEqualTo] = "Must not be equal to {value}",
            [MessageKey.Numbers.GreaterThan] = "Must be greater than {value}",
            [MessageKey.Numbers.GreaterThanOrEqualTo] = "Must be greater than or equal to {value}",
            [MessageKey.Numbers.LessThan] = "Must be less than {value}",
            [MessageKey.Numbers.LessThanOrEqualTo] = "Must be less than or equal to {value}",
            [MessageKey.Numbers.Between] = "Must be between {min} and {max} (exclusive)",
            [MessageKey.Numbers.BetweenOrEqualTo] = "Must be between {min} and {max} (inclusive)",
            [MessageKey.Numbers.NonZero] = "Must not be zero",
            [MessageKey.Numbers.Positive] = "Must be positive",
            [MessageKey.Numbers.NonPositive] = "Must not be positive",
            [MessageKey.Numbers.Negative] = "Must be negative",
            [MessageKey.Numbers.NonNegative] = "Must not be negative",
            [MessageKey.Numbers.NonNaN] = "Must not be NaN",

            [MessageKey.Texts.Email] = "Must be a valid email address",
            [MessageKey.Texts.EqualTo] = "Must be equal to {value}",
            [MessageKey.Texts.NotEqualTo] = "Must not be equal to {value}",
            [MessageKey.Texts.Contains] = "Must contain {value}",
            [MessageKey.Texts.NotContains] = "Must not contain {value}",
            [MessageKey.Texts.NotEmpty] = "Must not be empty",
            [MessageKey.Texts.NotWhiteSpace] = "Must not consist only of whitespace characters",
            [MessageKey.Texts.SingleLine] = "Must consist of single line",
            [MessageKey.Texts.ExactLength] = "Must be exact {length} characters in length",
            [MessageKey.Texts.MaxLength] = "Must be at most {max} characters in length",
            [MessageKey.Texts.MinLength] = "Must be at least {min} characters in length",
            [MessageKey.Texts.LengthBetween] = "Must be between {min} and {max} characters in length",
            [MessageKey.Texts.Matches] = "Must match RegEx pattern {pattern}",
            [MessageKey.Texts.StartsWith] = "Must start with {value}",
            [MessageKey.Texts.EndsWith] = "Must end with {value}",

            [MessageKey.Times.EqualTo] = "Must be equal to {value}",
            [MessageKey.Times.NotEqualTo] = "Must not be equal to {value}",
            [MessageKey.Times.After] = "Must be after than {value}",
            [MessageKey.Times.AfterOrEqualTo] = "Must be after than or equal to {value}",
            [MessageKey.Times.Before] = "Must be before than {value}",
            [MessageKey.Times.BeforeOrEqualTo] = "Must be before than or equal to {value}",
            [MessageKey.Times.Between] = "Must be between {min} and {max} (exclusive)",
            [MessageKey.Times.BetweenOrEqualTo] = "Must be between {min} and {max} (inclusive)",

            [MessageKey.TimeSpanType.EqualTo] = "Must be equal to {value}",
            [MessageKey.TimeSpanType.NotEqualTo] = "Must not be equal to {value}",
            [MessageKey.TimeSpanType.GreaterThan] = "Must be greater than {value}",
            [MessageKey.TimeSpanType.GreaterThanOrEqualTo] = "Must be greater than or equal to {value}",
            [MessageKey.TimeSpanType.LessThan] = "Must be less than {value}",
            [MessageKey.TimeSpanType.LessThanOrEqualTo] = "Must be less than or equal to {value}",
            [MessageKey.TimeSpanType.Between] = "Must be between {min} and {max} (exclusive)",
            [MessageKey.TimeSpanType.BetweenOrEqualTo] = "Must be between {min} and {max} (inclusive)",
            [MessageKey.TimeSpanType.NonZero] = "Must not be zero",
            [MessageKey.TimeSpanType.Positive] = "Must be positive",
            [MessageKey.TimeSpanType.NonPositive] = "Must not be positive",
            [MessageKey.TimeSpanType.Negative] = "Must be negative",
            [MessageKey.TimeSpanType.NonNegative] = "Must not be negative"
        };
    }
}
