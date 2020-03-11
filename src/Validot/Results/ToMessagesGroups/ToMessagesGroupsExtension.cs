namespace Validot
{
    using System.Collections.Generic;

    using Validot.Results;

    public static class ToMessagesGroupsExtension
    {
        public static IReadOnlyDictionary<string, IReadOnlyList<string>> ToMessagesGroups(this IValidationResult @this, string translationName = null)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return @this.Details.GetErrorMessages(translationName);
        }
    }
}
