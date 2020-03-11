namespace Validot
{
    using Validot.Specification;
    using Validot.Translations;

    public static class BoolRules
    {
        public static IRuleOut<bool> True(this IRuleIn<bool> @this)
        {
            return @this.RuleTemplate(v => v, MessageKey.BoolType.True);
        }

        public static IRuleOut<bool?> True(this IRuleIn<bool?> @this)
        {
            return @this.RuleTemplate(v => v.Value, MessageKey.BoolType.True);
        }

        public static IRuleOut<bool> False(this IRuleIn<bool> @this)
        {
            return @this.RuleTemplate(v => !v, MessageKey.BoolType.False);
        }

        public static IRuleOut<bool?> False(this IRuleIn<bool?> @this)
        {
            return @this.RuleTemplate(v => !v.Value, MessageKey.BoolType.False);
        }
    }
}
