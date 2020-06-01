namespace Validot.Translations
{
    using System.Collections.Generic;

    public static partial class Translation
    {
        public static IReadOnlyDictionary<string, string> Polish { get; } = new Dictionary<string, string>
        {
            [MessageKey.Global.Error] = "Błąd",
            [MessageKey.Global.Required] = "Wymagane",
            [MessageKey.Global.Forbidden] = "Zakazane",
            [MessageKey.Global.ReferenceLoop] = "(pętla referencji)",

            [MessageKey.BoolType.True] = "Musi być prawdą",
            [MessageKey.BoolType.False] = "Musi byc fałszem",

            [MessageKey.CharType.EqualToIgnoreCase] = "Musi być równe {value} (ignorując wielkość liter)",
            [MessageKey.CharType.NotEqualToIgnoreCase] = "Musi nie być równe {value} (ignorując wielkość liter)",

            [MessageKey.GuidType.EqualTo] = "Musi być równe {value}",
            [MessageKey.GuidType.NotEqualTo] = "Musi nie być równe {value}",
            [MessageKey.GuidType.NotEmpty] = "Musi nie być samymi zerami",

            [MessageKey.Collections.EmptyCollection] = "Musi być puste",
            [MessageKey.Collections.NotEmptyCollection] = "Musi nie być puste",
            [MessageKey.Collections.ExactCollectionSize] = "Musi zawierać dokładnie {size} elementów",
            [MessageKey.Collections.MaxCollectionSize] = "Musi zawierać maksymalnie {size} elementów",
            [MessageKey.Collections.MinCollectionSize] = "Musi zawierać minimalnie {size} elementów",
            [MessageKey.Collections.CollectionSizeBetween] = "Musi zawierać pomiędzy {min} a {max} elementów",

            [MessageKey.Numbers.EqualTo] = "Musi być równe {value}",
            [MessageKey.Numbers.NotEqualTo] = "Musi nie być równe {value}",
            [MessageKey.Numbers.GreaterThan] = "Musi być większe od {value}",
            [MessageKey.Numbers.GreaterThanOrEqualTo] = "Musi być większe od lub równe {value}",
            [MessageKey.Numbers.LessThan] = "Musi być mniejsze od {value}",
            [MessageKey.Numbers.LessThanOrEqualTo] = "Musi być mniejsze od lub równe {value}",
            [MessageKey.Numbers.Between] = "Musi być pomiędzy {min} a {max} (rozłącznie)",
            [MessageKey.Numbers.BetweenOrEqualTo] = "Musi być pomiędzy {min} a {max} (włącznie)",
            [MessageKey.Numbers.NonZero] = "Musi nie być zerem",
            [MessageKey.Numbers.Positive] = "Musi być pozytywne",
            [MessageKey.Numbers.NonPositive] = "Musi nie być pozytywne",
            [MessageKey.Numbers.Negative] = "Musi być negatywne",
            [MessageKey.Numbers.NonNegative] = "Musi nie być negatywne",
            [MessageKey.Numbers.NonNaN] = "Musi nie być NaN",

            [MessageKey.Texts.Email] = "Musi być poprawnym adresem email",
            [MessageKey.Texts.EqualTo] = "Musi być równe {value}",
            [MessageKey.Texts.NotEqualTo] = "Musi nie być równe {value}",
            [MessageKey.Texts.Contains] = "Musi zawierać {value}",
            [MessageKey.Texts.NotContains] = "Musi nie zawierać {value}",
            [MessageKey.Texts.NotEmpty] = "Musi nie być puste",
            [MessageKey.Texts.NotWhiteSpace] = "Musi nie zawierać wyłącznie białych znaków",
            [MessageKey.Texts.SingleLine] = "Musi zawierać jedynie pojedynczą linię",
            [MessageKey.Texts.ExactLength] = "Musi być długości dokładnie {length} znaków",
            [MessageKey.Texts.MaxLength] = "Musi być długości maksymalnie {max} znaków",
            [MessageKey.Texts.MinLength] = "Musi być długości minimalnie {min} znaków",
            [MessageKey.Texts.LengthBetween] = "Musi być długości pomiędzy {min} a {max} znaków",
            [MessageKey.Texts.Matches] = "Musi spełniać wzorzec RegEx {pattern}",
            [MessageKey.Texts.StartsWith] = "Musi zaczynać się od {value}",
            [MessageKey.Texts.EndsWith] = "Musi kończyć się od {value}",

            [MessageKey.Times.EqualTo] = "Musi być równe {value}",
            [MessageKey.Times.NotEqualTo] = "Musi nie być równe {value}",
            [MessageKey.Times.After] = "Musi być później niż {value}",
            [MessageKey.Times.AfterOrEqualTo] = "Musi być później niż lub równe {value}",
            [MessageKey.Times.Before] = "Musi być wcześniej niż {value}",
            [MessageKey.Times.BeforeOrEqualTo] = "Musi być wcześniej niż lub równe {value}",
            [MessageKey.Times.Between] = "Musi być pomiędzy {min} a {max} (rozłącznie)",
            [MessageKey.Times.BetweenOrEqualTo] = "Musi być pomiędzy {min} a {max} (włącznie)",

            [MessageKey.TimeSpanType.EqualTo] = "Musi być równe {value}",
            [MessageKey.TimeSpanType.NotEqualTo] = "Musi nie być równe {value}",
            [MessageKey.TimeSpanType.GreaterThan] = "Musi być większe od {value}",
            [MessageKey.TimeSpanType.GreaterThanOrEqualTo] = "Musi być większe od lub równe {value}",
            [MessageKey.TimeSpanType.LessThan] = "Musi być mniejsze od {value}",
            [MessageKey.TimeSpanType.LessThanOrEqualTo] = "Musi być mniejsze od lub równe {value}",
            [MessageKey.TimeSpanType.Between] = "Musi być pomiędzy {min} a {max} (rozłącznie)",
            [MessageKey.TimeSpanType.BetweenOrEqualTo] = "Musi być pomiędzy {min} a {max} (włącznie)",
            [MessageKey.TimeSpanType.NonZero] = "Musi nie być zerem",
            [MessageKey.TimeSpanType.Positive] = "Musi być pozytywne",
            [MessageKey.TimeSpanType.NonPositive] = "Musi nie być pozytywne",
            [MessageKey.TimeSpanType.Negative] = "Musi być negatywne",
            [MessageKey.TimeSpanType.NonNegative] = "Musi nie być negatywne"
        };
    }
}
