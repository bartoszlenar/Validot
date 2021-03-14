namespace Validot.Translations
{
    using System.Collections.Generic;

    public static partial class Translation
    {
        public static IReadOnlyDictionary<string, string> Russian { get; } = new Dictionary<string, string>
        {
            [MessageKey.Global.Error] = "Ошибка",
            [MessageKey.Global.Required] = "Требуется",
            [MessageKey.Global.Forbidden] = "Запрещается",
            [MessageKey.Global.ReferenceLoop] = "(ссылочный цикл)",

            [MessageKey.BoolType.True] = "Должен быть верным",
            [MessageKey.BoolType.False] = "Должен быть неверным",

            [MessageKey.CharType.EqualToIgnoreCase] = "Должен быть равен {value} (без учета регистра)",
            [MessageKey.CharType.NotEqualToIgnoreCase] = "Не должен быть равен {value} (без учета регистра)",

            [MessageKey.GuidType.EqualTo] = "Должно быть равно {value}",
            [MessageKey.GuidType.NotEqualTo] = "Не должно быть равно {value}",
            [MessageKey.GuidType.NotEmpty] = "Не должно полностью состоять из нулей",

            [MessageKey.Collections.EmptyCollection] = "Должен быть пуст",
            [MessageKey.Collections.NotEmptyCollection] = "Не должен быть пуст",
            [MessageKey.Collections.ExactCollectionSize] = "Должен содержать точно {size} элементов",
            [MessageKey.Collections.MaxCollectionSize] = "Должен содержать не более {size} элементов",
            [MessageKey.Collections.MinCollectionSize] = "Должен содержать не менее {size} элементов",
            [MessageKey.Collections.CollectionSizeBetween] = "Должен содержать от {min} до {max} элементов",

            [MessageKey.Numbers.EqualTo] = "Должно быть равно {value}",
            [MessageKey.Numbers.NotEqualTo] = "Не должно быть равно {value}",
            [MessageKey.Numbers.GreaterThan] = "Должно быть больше {min}",
            [MessageKey.Numbers.GreaterThanOrEqualTo] = "Должно быть больше или равно {min}",
            [MessageKey.Numbers.LessThan] = "Должно быть меньше {max}",
            [MessageKey.Numbers.LessThanOrEqualTo] = "Должно быть меньше или равно {max}",
            [MessageKey.Numbers.Between] = "Должно быть от {min} до {max} (исключая)",
            [MessageKey.Numbers.BetweenOrEqualTo] = "Должно быть от {min} до {max} (включительно)",
            [MessageKey.Numbers.NonZero] = "Не должно быть нулевым",
            [MessageKey.Numbers.Positive] = "Должно быть положительным",
            [MessageKey.Numbers.NonPositive] = "Не должно быть положительным",
            [MessageKey.Numbers.Negative] = "Должно быть отрицательным",
            [MessageKey.Numbers.NonNegative] = "Не должно быть отрицательным",
            [MessageKey.Numbers.NonNaN] = "Не должно быть NaN",

            [MessageKey.Texts.Email] = "Должен быть корректный адрес электронной почты",
            [MessageKey.Texts.EqualTo] = "Должен быть равен {value}",
            [MessageKey.Texts.NotEqualTo] = "Не должен быть равен {value}",
            [MessageKey.Texts.Contains] = "Должен содержать {value}",
            [MessageKey.Texts.NotContains] = "Не должен содержать {value}",
            [MessageKey.Texts.NotEmpty] = "Не должен быть пуст",
            [MessageKey.Texts.NotWhiteSpace] = "Не должен состоять только из пробелов",
            [MessageKey.Texts.SingleLine] = "Должен состоять из одной строки",
            [MessageKey.Texts.ExactLength] = "Должен быть точно {length} символов в длину",
            [MessageKey.Texts.MaxLength] = "Должен быть не больше {max} символов в длину",
            [MessageKey.Texts.MinLength] = "Должен быть не менее {min} символов в длину",
            [MessageKey.Texts.LengthBetween] = "Должен быть от {min} до {max} символов в длину",
            [MessageKey.Texts.Matches] = "Должен соответствовать шаблону регулярного выражения {pattern}",
            [MessageKey.Texts.StartsWith] = "Должен начинаться с {value}",
            [MessageKey.Texts.EndsWith] = "Должен заканчиваться на {value}",

            [MessageKey.Times.EqualTo] = "Должно быть равно {value}",
            [MessageKey.Times.NotEqualTo] = "Не должно быть равно {value}",
            [MessageKey.Times.After] = "Должно быть позже, чем {value}",
            [MessageKey.Times.AfterOrEqualTo] = "Должно быть позже или равно {value}",
            [MessageKey.Times.Before] = "Должно быть раньше, чем {value}",
            [MessageKey.Times.BeforeOrEqualTo] = "Должно быть раньше или равно {value}",
            [MessageKey.Times.Between] = "Должно быть от {min} до {max} (исключая)",
            [MessageKey.Times.BetweenOrEqualTo] = "Должно быть от {min} до {max} (включительно)",

            [MessageKey.TimeSpanType.EqualTo] = "Должно быть равно {value}",
            [MessageKey.TimeSpanType.NotEqualTo] = "Должно быь не равно {value}",
            [MessageKey.TimeSpanType.GreaterThan] = "Должно быть больше, чем {value}",
            [MessageKey.TimeSpanType.GreaterThanOrEqualTo] = "Должно быть больше или равно {value}",
            [MessageKey.TimeSpanType.LessThan] = "Должно быть меньше, чем {value}",
            [MessageKey.TimeSpanType.LessThanOrEqualTo] = "Должно быть меньше или равно {value}",
            [MessageKey.TimeSpanType.Between] = "Должно быть от {min} до {max} (исключая)",
            [MessageKey.TimeSpanType.BetweenOrEqualTo] = "Должно быть от {min} до {max} (включительно)",
            [MessageKey.TimeSpanType.NonZero] = "Не должно быть нулевым",
            [MessageKey.TimeSpanType.Positive] = "Должно быть положительным",
            [MessageKey.TimeSpanType.NonPositive] = "Не должно быть положительным",
            [MessageKey.TimeSpanType.Negative] = "Должно быть отрицательным",
            [MessageKey.TimeSpanType.NonNegative] = "Не должно быть отрицательным"
        };
    }
}
