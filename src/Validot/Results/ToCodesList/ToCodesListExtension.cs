namespace Validot
{
    using System.Collections.Generic;

    using Validot.Results;

    public static class ToCodesListExtension
    {
        public static IReadOnlyList<string> ToCodesList(this IValidationResult @this)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return @this.Details.GetErrorCodes();
        }
    }
}
