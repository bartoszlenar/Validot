namespace Validot.Translations
{
    using System.Collections.Generic;

    public static partial class Translation
    {
        public static IReadOnlyDictionary<string, string> Portuguese { get; } = new Dictionary<string, string>
        {
            [MessageKey.Global.Error] = "Erro",
            [MessageKey.Global.Required] = "Obrigatório",
            [MessageKey.Global.Forbidden] = "Proibido",
            [MessageKey.Global.ReferenceLoop] = "(ciclo de referência)",

            [MessageKey.BoolType.True] = "Deve ser verdadeiro",
            [MessageKey.BoolType.False] = "Deve ser falso",

            [MessageKey.CharType.EqualToIgnoreCase] = "Deve ser igual a {value} (ignorando maiúsculas e minúsculas)",
            [MessageKey.CharType.NotEqualToIgnoreCase] = "Não deve ser igual a {value} (ignorando maiúsculas e minúsculas)",

            [MessageKey.GuidType.EqualTo] = "Deve ser igual a {value}",
            [MessageKey.GuidType.NotEqualTo] = "Não deve ser igual a {value}",
            [MessageKey.GuidType.NotEmpty] = "Não deve ser só zeros",

            [MessageKey.Collections.EmptyCollection] = "Deve estar vazio",
            [MessageKey.Collections.NotEmptyCollection] = "Não deve estar vazio",
            [MessageKey.Collections.ExactCollectionSize] = "Deve conter exatamente {size} elementos",
            [MessageKey.Collections.MaxCollectionSize] = "Deve conter no máximo {size} elementos",
            [MessageKey.Collections.MinCollectionSize] = "Deve conter no mínimo {size} elementos",
            [MessageKey.Collections.CollectionSizeBetween] = "Deve conter entre {min} e {max} elementos",

            [MessageKey.Numbers.EqualTo] = "Deve ser igual a  {value}",
            [MessageKey.Numbers.NotEqualTo] = "Não deve ser igual a {value}",
            [MessageKey.Numbers.GreaterThan] = "Deve ser maior que {min}",
            [MessageKey.Numbers.GreaterThanOrEqualTo] = "Deve ser maior ou igual a {min}",
            [MessageKey.Numbers.LessThan] = "Deve ser menor que {max}",
            [MessageKey.Numbers.LessThanOrEqualTo] = "Deve ser menor ou igual a {max}",
            [MessageKey.Numbers.Between] = "Deve estar entre {min} e {max} (exclusivo)",
            [MessageKey.Numbers.BetweenOrEqualTo] = "Deve estar entre {min} e {max} (inclusive)",
            [MessageKey.Numbers.NonZero] = "Não deve ser zero",
            [MessageKey.Numbers.Positive] = "Deve ser positivo",
            [MessageKey.Numbers.NonPositive] = "Não deve ser positivo",
            [MessageKey.Numbers.Negative] = "Deve ser negativo",
            [MessageKey.Numbers.NonNegative] = "Não deve ser negativo",
            [MessageKey.Numbers.NonNaN] = "Não deve ser NaN",

            [MessageKey.Texts.Email] = "Deve ser um endereço de email válido",
            [MessageKey.Texts.EqualTo] = "Deve ser igual a {value}",
            [MessageKey.Texts.NotEqualTo] = "Não deve ser igual a {value}",
            [MessageKey.Texts.Contains] = "Deve conter {value}",
            [MessageKey.Texts.NotContains] = "Não deve conter {value}",
            [MessageKey.Texts.NotEmpty] = "Não deve estar vazio",
            [MessageKey.Texts.NotWhiteSpace] = "Não deve consistir apenas em caracteres de espaço em branco",
            [MessageKey.Texts.SingleLine] = "Deve consistir em uma única linha",
            [MessageKey.Texts.ExactLength] = "Deve ter {length} caracteres",
            [MessageKey.Texts.MaxLength] = "Deve ter no máximo {max} caracteres",
            [MessageKey.Texts.MinLength] = "Deve ter no mínimo {min} caracteres",
            [MessageKey.Texts.LengthBetween] = "Deve ter entre {min} e {max} caracteres",
            [MessageKey.Texts.Matches] = "Deve corresponder ao padrão de expressão regular {pattern}",
            [MessageKey.Texts.StartsWith] = "Deve começar com {value}",
            [MessageKey.Texts.EndsWith] = "Deve terminar com {value}",

            [MessageKey.Times.EqualTo] = "Deve ser igual a {value}",
            [MessageKey.Times.NotEqualTo] = "Não deve ser igual a {value}",
            [MessageKey.Times.After] = "Deve ser posterior a {value}",
            [MessageKey.Times.AfterOrEqualTo] = "Deve ser igual ou posterior a {value}",
            [MessageKey.Times.Before] = "Deve ser anterior a {value}",
            [MessageKey.Times.BeforeOrEqualTo] = "Deve ser igual ou anterior a {value}",
            [MessageKey.Times.Between] = "Deve estar entre {min} e {max} (exclusivo)",
            [MessageKey.Times.BetweenOrEqualTo] = "Deve estar entre {min} e {max} (inclusive)",

            [MessageKey.TimeSpanType.EqualTo] = "Deve ser igual a {value}",
            [MessageKey.TimeSpanType.NotEqualTo] = "Não deve ser igual a {value}",
            [MessageKey.TimeSpanType.GreaterThan] = "Deve ser maior que {value}",
            [MessageKey.TimeSpanType.GreaterThanOrEqualTo] = "Deve ser maior ou igual que {value}",
            [MessageKey.TimeSpanType.LessThan] = "Deve ser menor que {value}",
            [MessageKey.TimeSpanType.LessThanOrEqualTo] = "Deve ser menor ou igual que {value}",
            [MessageKey.TimeSpanType.Between] = "Deve estar entre {min} e {max} (exclusivo)",
            [MessageKey.TimeSpanType.BetweenOrEqualTo] = "Deve estar entre {min} e {max} (inclusivo)",
            [MessageKey.TimeSpanType.NonZero] = "Não deve ser zero",
            [MessageKey.TimeSpanType.Positive] = "Deve ser positivo",
            [MessageKey.TimeSpanType.NonPositive] = "Não deve ser positivo",
            [MessageKey.TimeSpanType.Negative] = "Deve ser negativo",
            [MessageKey.TimeSpanType.NonNegative] = "Não deve ser negativo"
        };
    }
}