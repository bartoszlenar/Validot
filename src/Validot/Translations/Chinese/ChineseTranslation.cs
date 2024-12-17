namespace Validot.Translations
{
    using System.Collections.Generic;

    public static partial class Translation
    {
        public static IReadOnlyDictionary<string, string> Chinese { get; } = new Dictionary<string, string>
        {
            [MessageKey.Global.Error] = "错误",
            [MessageKey.Global.Required] = "需要",
            [MessageKey.Global.Forbidden] = "禁止",
            [MessageKey.Global.ReferenceLoop] = "(引用循环)",

            [MessageKey.BoolType.True] = "必须为真",
            [MessageKey.BoolType.False] = "必须为假",

            [MessageKey.CharType.EqualToIgnoreCase] = "必须等于 {value} (无视条件)",
            [MessageKey.CharType.NotEqualToIgnoreCase] = "不可等于 {value} (无视条件)",

            [MessageKey.GuidType.EqualTo] = "必须等于 {value}",
            [MessageKey.GuidType.NotEqualTo] = "不可等于 {value}",
            [MessageKey.GuidType.NotEmpty] = "不可全部为零",

            [MessageKey.Collections.EmptyCollection] = "必须为空",
            [MessageKey.Collections.NotEmptyCollection] = "不可为空",
            [MessageKey.Collections.ExactCollectionSize] = "必须包含 {size} 个对象",
            [MessageKey.Collections.MinCollectionSize] = "必须包含至少 {min} 个对象",
            [MessageKey.Collections.MaxCollectionSize] = "只能包含最多 {max} 个对象",
            [MessageKey.Collections.CollectionSizeBetween] = "只能包含 {min} 到 {max} 个对象",

            [MessageKey.Numbers.EqualTo] = "必须等于 {value}",
            [MessageKey.Numbers.NotEqualTo] = "不可等于 {value}",
            [MessageKey.Numbers.GreaterThan] = "必须大于 {min}",
            [MessageKey.Numbers.GreaterThanOrEqualTo] = "必须大于或等于 {min}",
            [MessageKey.Numbers.LessThan] = "只能小于 {max}",
            [MessageKey.Numbers.LessThanOrEqualTo] = "只能小于或等于 {max}",
            [MessageKey.Numbers.Between] = "只能在 {min} 到 {max} 之间 (不包括)",
            [MessageKey.Numbers.BetweenOrEqualTo] = "只能在 {min} 到 {max} 之间 (包括)",
            [MessageKey.Numbers.NonZero] = "不可为零",
            [MessageKey.Numbers.Positive] = "必须为正值",
            [MessageKey.Numbers.NonPositive] = "不可为正值",
            [MessageKey.Numbers.Negative] = "必须为负值",
            [MessageKey.Numbers.NonNegative] = "不可为负值",
            [MessageKey.Numbers.NonNaN] = "必须非NaN",

            [MessageKey.Texts.Email] = "必须为有效邮箱地址",
            [MessageKey.Texts.EqualTo] = "必须等于 {value}",
            [MessageKey.Texts.NotEqualTo] = "不可等于 {value}",
            [MessageKey.Texts.Contains] = "必须包含 {value}",
            [MessageKey.Texts.NotContains] = "不可包含 {value}",
            [MessageKey.Texts.NotEmpty] = "不可为空",
            [MessageKey.Texts.NotWhiteSpace] = "不可仅使用空格字符",
            [MessageKey.Texts.SingleLine] = "只能包含单行",
            [MessageKey.Texts.ExactLength] = "长度必须确切为 {length} 个字符",
            [MessageKey.Texts.MaxLength] = "长度最多为 {max} 个字符",
            [MessageKey.Texts.MinLength] = "长度最少为 {min} 个字符",
            [MessageKey.Texts.LengthBetween] = "长度只能在 {min} 到 {max} 个字符之间",
            [MessageKey.Texts.Matches] = "必须匹配正则表达式模板 {pattern}",
            [MessageKey.Texts.StartsWith] = "必须以 {value} 起始",
            [MessageKey.Texts.EndsWith] = "必须以 {value} 结束",

            [MessageKey.Times.EqualTo] = "必须等于 {value}",
            [MessageKey.Times.NotEqualTo] = "不可等于 {value}",
            [MessageKey.Times.After] = "必须晚于 {min}",
            [MessageKey.Times.AfterOrEqualTo] = "必须晚于或等于 {min}",
            [MessageKey.Times.Before] = "必须早于 {max}",
            [MessageKey.Times.BeforeOrEqualTo] = "必须早于或等于 {max}",
            [MessageKey.Times.Between] = "只能处于 {min} 到 {max} 之间 (不包括)",
            [MessageKey.Times.BetweenOrEqualTo] = "只能处于 {min} 到 {max} 之间 (包括)",

            [MessageKey.TimeSpanType.EqualTo] = "必须等于 {value}",
            [MessageKey.TimeSpanType.NotEqualTo] = "不可等于 {value}",
            [MessageKey.TimeSpanType.GreaterThan] = "必须大于 {min}",
            [MessageKey.TimeSpanType.GreaterThanOrEqualTo] = "必须大于或等于 {min}",
            [MessageKey.TimeSpanType.LessThan] = "只能小于 {max}",
            [MessageKey.TimeSpanType.LessThanOrEqualTo] = "只能小于或等于 {max}",
            [MessageKey.TimeSpanType.Between] = "只能处于 {min} 到 {max} 之间 (不包括)",
            [MessageKey.TimeSpanType.BetweenOrEqualTo] = "只能处于 {min} 到 {max} 之间 (包括)",
            [MessageKey.TimeSpanType.NonZero] = "不可为零",
            [MessageKey.TimeSpanType.Positive] = "必须为正值",
            [MessageKey.TimeSpanType.NonPositive] = "不可为正值",
            [MessageKey.TimeSpanType.Negative] = "必须为负值",
            [MessageKey.TimeSpanType.NonNegative] = "不可为负值"
        };
    }
}
