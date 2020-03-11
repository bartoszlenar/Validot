namespace Validot.Rules
{
    using System;

    using Validot.Specification;
    using Validot.Translations;

    public static class GuidRules
    {
        public static IRuleOut<Guid> EqualTo(this IRuleIn<Guid> @this, Guid value)
        {
            return @this.RuleTemplate(v => v == value, MessageKey.GuidType.EqualTo, Arg.GuidValue(nameof(value), value));
        }

        public static IRuleOut<Guid?> EqualTo(this IRuleIn<Guid?> @this, Guid value)
        {
            return @this.RuleTemplate(v => v.Value == value, MessageKey.GuidType.EqualTo, Arg.GuidValue(nameof(value), value));
        }

        public static IRuleOut<Guid> NotEqualTo(this IRuleIn<Guid> @this, Guid value)
        {
            return @this.RuleTemplate(v => v != value, MessageKey.GuidType.NotEqualTo, Arg.GuidValue(nameof(value), value));
        }

        public static IRuleOut<Guid?> NotEqualTo(this IRuleIn<Guid?> @this, Guid value)
        {
            return @this.RuleTemplate(v => v.Value != value, MessageKey.GuidType.NotEqualTo, Arg.GuidValue(nameof(value), value));
        }

        public static IRuleOut<Guid> NotEmpty(this IRuleIn<Guid> @this)
        {
            return @this.RuleTemplate(v => v != Guid.Empty, MessageKey.GuidType.NotEmpty);
        }

        public static IRuleOut<Guid?> NotEmpty(this IRuleIn<Guid?> @this)
        {
            return @this.RuleTemplate(v => v.Value != Guid.Empty, MessageKey.GuidType.NotEmpty);
        }
    }
}
