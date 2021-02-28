namespace Validot.Translations
{
    using System.Collections.Generic;

    public static partial class Translation
    {
        public static IReadOnlyDictionary<string, string> Spanish { get; } = new Dictionary<string, string>
        {
            [MessageKey.Global.Error] = "Error",
            [MessageKey.Global.Required] = "Requerido",
            [MessageKey.Global.Forbidden] = "Prohibido",
            [MessageKey.Global.ReferenceLoop] = "(bucle de referencia)",

            [MessageKey.BoolType.True] = "Debe ser verdadero",
            [MessageKey.BoolType.False] = "Debe ser falso",

            [MessageKey.CharType.EqualToIgnoreCase] = "Debe ser igual a {value} (ignorando el caso)",
            [MessageKey.CharType.NotEqualToIgnoreCase] = "No debe ser igual a {value} (ignorando el caso)",

            [MessageKey.GuidType.EqualTo] = "Debe ser igual {value}",
            [MessageKey.GuidType.NotEqualTo] = "No debe ser igual a {value}",
            [MessageKey.GuidType.NotEmpty] = "No deben ser cero",

            [MessageKey.Collections.EmptyCollection] = "Debe estar vacío",
            [MessageKey.Collections.NotEmptyCollection] = "No debe estar vacío",
            [MessageKey.Collections.ExactCollectionSize] = "Debe contener {size} elementos",
            [MessageKey.Collections.MaxCollectionSize] = "Debe contener como máximo {size} elementos",
            [MessageKey.Collections.MinCollectionSize] = "Debe contener como mínimo {size} elementos",
            [MessageKey.Collections.CollectionSizeBetween] = "Debe contener entre {min} y {max} elementos",

            [MessageKey.Numbers.EqualTo] = "Debe ser igual a {value}",
            [MessageKey.Numbers.NotEqualTo] = "No debe ser igual {value}",
            [MessageKey.Numbers.GreaterThan] = "Debe ser mayor que {min}",
            [MessageKey.Numbers.GreaterThanOrEqualTo] = "Debe ser mayor o igual que {min}",
            [MessageKey.Numbers.LessThan] = "Debe ser menor que {max}",
            [MessageKey.Numbers.LessThanOrEqualTo] = "Debe ser menor o igual que {max}",
            [MessageKey.Numbers.Between] = "Debe estar entre {min} y {max} (exclusivo)",
            [MessageKey.Numbers.BetweenOrEqualTo] = "Debe estar entre {min} y {max} (inclusive)",
            [MessageKey.Numbers.NonZero] = "No debe ser cero",
            [MessageKey.Numbers.Positive] = "Debe ser positivo",
            [MessageKey.Numbers.NonPositive] = "No debe ser positivo",
            [MessageKey.Numbers.Negative] = "Debe ser negativo",
            [MessageKey.Numbers.NonNegative] = "No debe ser negativo",
            [MessageKey.Numbers.NonNaN] = "No debe ser NaN",

            [MessageKey.Texts.Email] = "Debe ser un correo electrónico válido",
            [MessageKey.Texts.EqualTo] = "Debe ser igual a {value}",
            [MessageKey.Texts.NotEqualTo] = "No debe ser igual a {value}",
            [MessageKey.Texts.Contains] = "Debe contener {value}",
            [MessageKey.Texts.NotContains] = "No debe contener {value}",
            [MessageKey.Texts.NotEmpty] = "No debe estar vacío",
            [MessageKey.Texts.NotWhiteSpace] = "No debe contener espacios",
            [MessageKey.Texts.SingleLine] = "Debe constar de una sola línea",
            [MessageKey.Texts.ExactLength] = "Debe tener {length} caracteres",
            [MessageKey.Texts.MaxLength] = "Debe tener como máximo {max} caracteres",
            [MessageKey.Texts.MinLength] = "Debe tener al menos {min} caracteres",
            [MessageKey.Texts.LengthBetween] = "Debe tener entre {min} y {max} caracteres",
            [MessageKey.Texts.Matches] = "Debe coincidir con el patrón de expresión regular {pattern}",
            [MessageKey.Texts.StartsWith] = "Debe comenzar con {value}",
            [MessageKey.Texts.EndsWith] = "Debe terminar con {value}",

            [MessageKey.Times.EqualTo] = "Debe ser igual a {value}",
            [MessageKey.Times.NotEqualTo] = "No debe ser igual a {value}",
            [MessageKey.Times.After] = "Debe ser posterior a {value}",
            [MessageKey.Times.AfterOrEqualTo] = "Debe ser igual o posterior a {value}",
            [MessageKey.Times.Before] = "Debe ser anterior a {value}",
            [MessageKey.Times.BeforeOrEqualTo] = "Debe ser anterior a {value}",
            [MessageKey.Times.Between] = "Debe estar entre {min} y {max} (exclusivo)",
            [MessageKey.Times.BetweenOrEqualTo] = "Debe estar entre {min} y {max} (inclusive)",

            [MessageKey.TimeSpanType.EqualTo] = "Debe ser igual a {value}",
            [MessageKey.TimeSpanType.NotEqualTo] = "No debe ser igual a {value}",
            [MessageKey.TimeSpanType.GreaterThan] = "Debe ser mayor que {value}",
            [MessageKey.TimeSpanType.GreaterThanOrEqualTo] = "Debe ser mayor o igual que {value}",
            [MessageKey.TimeSpanType.LessThan] = "Debe ser menor que {value}",
            [MessageKey.TimeSpanType.LessThanOrEqualTo] = "Debe ser menor o igual que {value}",
            [MessageKey.TimeSpanType.Between] = "Debe estar entre {min} y {max} (exclusivo)",
            [MessageKey.TimeSpanType.BetweenOrEqualTo] = "Debe estar entre {min} y {max} (inclusivo)",
            [MessageKey.TimeSpanType.NonZero] = "No debe ser cero",
            [MessageKey.TimeSpanType.Positive] = "Debe ser positivo",
            [MessageKey.TimeSpanType.NonPositive] = "No debe ser positivo",
            [MessageKey.TimeSpanType.Negative] = "Debe ser negativo",
            [MessageKey.TimeSpanType.NonNegative] = "No debe ser negativo"
        };
    }
}
