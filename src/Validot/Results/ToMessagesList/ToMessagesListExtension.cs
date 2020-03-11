namespace Validot
{
    using System;
    using System.Collections.Generic;

    using Validot.Results;

    public static class ToMessagesListExtension
    {
        private const string PathSeparator = ": ";

        public static IEnumerable<string> ToMessagesList(this IValidationResult @this, bool includePaths = true, string translation = null)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            if (@this.IsValid)
            {
                return Array.Empty<string>();
            }

            var errorsMessages = @this.Details.GetErrorMessages(translation);

            var list = new List<string>(errorsMessages.Count);

            foreach (var pair in errorsMessages)
            {
                foreach (var error in pair.Value)
                {
                    if (includePaths && !string.IsNullOrEmpty(pair.Key))
                    {
                        list.Add($"{pair.Key}{PathSeparator}{error}");
                    }
                    else
                    {
                        list.Add($"{error}");
                    }
                }
            }

            return list;
        }
    }
}
