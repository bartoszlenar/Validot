namespace Validot.Translations
{
    using System.Collections.Generic;

    public static partial class Translation
    {
        public static IReadOnlyDictionary<string, string> German { get; } = new Dictionary<string, string>
        {
            [MessageKey.Global.Error] = "Fehler",
            [MessageKey.Global.Required] = "Erforderlich",
            [MessageKey.Global.Forbidden] = "Verboten",
            [MessageKey.Global.ReferenceLoop] = "(Referenzschleife)",

            [MessageKey.BoolType.True] = "Muss wahr sein",
            [MessageKey.BoolType.False] = "Darf nicht wahr sein",

            [MessageKey.CharType.EqualToIgnoreCase] = "Muss gleich sein wie {value} (Groß-/Kleinschreibung wird ignoriert)",
            [MessageKey.CharType.NotEqualToIgnoreCase] = "Darf nicht gleich sein wie {value} (Groß-/Kleinschreibung wird ignoriert)",

            [MessageKey.GuidType.EqualTo] = "Muss gleich sein wie {value}",
            [MessageKey.GuidType.NotEqualTo] = "Darf nicht gleich sein wie {value}",
            [MessageKey.GuidType.NotEmpty] = "Darf nicht nur nullen sein",

            [MessageKey.Collections.EmptyCollection] = "Muss leer sein",
            [MessageKey.Collections.NotEmptyCollection] = "Darf nicht leer sein",
            [MessageKey.Collections.ExactCollectionSize] = "Muss genau {size} Elemente haben",
            [MessageKey.Collections.MaxCollectionSize] = "Muss maximal {size} Elemente haben",
            [MessageKey.Collections.MinCollectionSize] = "Muss mindestens {size} Elemente haben",
            [MessageKey.Collections.CollectionSizeBetween] = "Muss zwischen {min} und {max} Elemente haben",

            [MessageKey.Numbers.EqualTo] = "Muss gleich sein wie {value}",
            [MessageKey.Numbers.NotEqualTo] = "Darf nicht gleich sein wie {value}",
            [MessageKey.Numbers.GreaterThan] = "Muss mehr sein als {min}",
            [MessageKey.Numbers.GreaterThanOrEqualTo] = "Muss größer oder gleich sein als {min}",
            [MessageKey.Numbers.LessThan] = "Muss kleiner sein als {max}",
            [MessageKey.Numbers.LessThanOrEqualTo] = "Muss kleiner oder gleich sein als {max}",
            [MessageKey.Numbers.Between] = "Muss zwischen {min} und {max} liegen (exklusiv)",
            [MessageKey.Numbers.BetweenOrEqualTo] = "Muss zwischen {min} und {max} liegen (exklusiv)",
            [MessageKey.Numbers.NonZero] = "Darf nicht Null sein",
            [MessageKey.Numbers.Positive] = "Muss positiv sein",
            [MessageKey.Numbers.NonPositive] = "Darf nicht positiv sein",
            [MessageKey.Numbers.Negative] = "Muss negativ sein",
            [MessageKey.Numbers.NonNegative] = "Darf nicht negativ sein",
            [MessageKey.Numbers.NonNaN] = "Darf nicht NaN sein",

            [MessageKey.Texts.Email] = "Muss eine gültige E-Mail-Adresse sein",
            [MessageKey.Texts.EqualTo] = "Muss gleich sein wie {value}",
            [MessageKey.Texts.NotEqualTo] = "Darf nicht gleich sein als {value}",
            [MessageKey.Texts.Contains] = "Muss {value} enthalten",
            [MessageKey.Texts.NotContains] = "Darf {value} nicht enthalten",
            [MessageKey.Texts.NotEmpty] = "Darf nicht leer sein",
            [MessageKey.Texts.NotWhiteSpace] = "Darf nicht nur aus Leerzeichen bestehen",
            [MessageKey.Texts.SingleLine] = "Muss aus einer einzigen Zeile bestehen",
            [MessageKey.Texts.ExactLength] = "Muss genau {length} Zeichen lang sein",
            [MessageKey.Texts.MaxLength] = "Muss maximal {max} Zeichen lang sein",
            [MessageKey.Texts.MinLength] = "Muss mindestens {min} Zeichen lang sein",
            [MessageKey.Texts.LengthBetween] = "Muss zwischen {min} und {max} Zeichen lang sein",
            [MessageKey.Texts.Matches] = "Muss dem RegEx-Muster {pattern} entsprechen",
            [MessageKey.Texts.StartsWith] = "Muss mit {value} beginnen",
            [MessageKey.Texts.EndsWith] = "Muss mit {value} enden",

            [MessageKey.Times.EqualTo] = "Muss zum gleichen Zeitpunkt sein sein wie {value}",
            [MessageKey.Times.NotEqualTo] = "Darf um gleichen Zeitpunkt sein sein wie {value}",
            [MessageKey.Times.After] = "Muss nach dem {value} liegen",
            [MessageKey.Times.AfterOrEqualTo] = "Muss danach oder zum gleichen Zeitpunkt sein wie {value}",
            [MessageKey.Times.Before] = "Muss vor dem {value} liegen",
            [MessageKey.Times.BeforeOrEqualTo] = "Muss davor oder zum gleichen Zeitpunkt sein wie {value}",
            [MessageKey.Times.Between] = "Muss zwischen dem {min} und dem {max} liegen (exklusiv)",
            [MessageKey.Times.BetweenOrEqualTo] = "Muss zwischen dem {min} und dem {max} liegen (einschließlich)",

            [MessageKey.TimeSpanType.EqualTo] = "Muss gleich sein wie {value}",
            [MessageKey.TimeSpanType.NotEqualTo] = "Darf nicht gleich sein wie {value}",
            [MessageKey.TimeSpanType.GreaterThan] = "Muss größer sein als {value}",
            [MessageKey.TimeSpanType.GreaterThanOrEqualTo] = "Muss größer als oder gleich sein wie {value}",
            [MessageKey.TimeSpanType.LessThan] = "Muss kleiner sein als {value}",
            [MessageKey.TimeSpanType.LessThanOrEqualTo] = "Muss kleiner als oder gleich sein wie {value}",
            [MessageKey.TimeSpanType.Between] = "Muss zwischen {min} und {max} liegen (exklusiv)",
            [MessageKey.TimeSpanType.BetweenOrEqualTo] = "Muss zwischen {min} und {max} liegen (einschließlich)",
            [MessageKey.TimeSpanType.NonZero] = "Darf nicht Null sein",
            [MessageKey.TimeSpanType.Positive] = "Muss positiv sein",
            [MessageKey.TimeSpanType.NonPositive] = "Darf nicht positiv sein",
            [MessageKey.TimeSpanType.Negative] = "Muss negativ sein",
            [MessageKey.TimeSpanType.NonNegative] = "Darf nicht negativ sein"
        };
    }
}
