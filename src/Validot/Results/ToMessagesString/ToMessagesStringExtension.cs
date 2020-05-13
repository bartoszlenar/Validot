namespace Validot
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Validot.Results;

    public static class ToMessagesStringExtension
    {
        private const string PathSeparator = ": ";

        public static string ToMessagesString(this IValidationResult @this, bool includePaths = true, string translation = null)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            if (!@this.AnyErrors)
            {
                return string.Empty;
            }

            var errorsMessages = @this.Details.GetErrorMessages(translation);

            var capacity = GetCapacity(errorsMessages, includePaths);

            var builder = new StringBuilder(capacity);

            foreach (var pair in errorsMessages)
            {
                foreach (var error in pair.Value)
                {
                    if (includePaths && !string.IsNullOrEmpty(pair.Key))
                    {
                        builder.Append($"{pair.Key}{PathSeparator}{error}{Environment.NewLine}");
                    }
                    else
                    {
                        builder.Append($"{error}{Environment.NewLine}");
                    }
                }
            }

            return builder.ToString();
        }

        private static int GetCapacity(IReadOnlyDictionary<string, IReadOnlyList<string>> errorsMessages, bool includePaths)
        {
            var capacity = 0;

            foreach (var pair in errorsMessages)
            {
                foreach (var error in pair.Value)
                {
                    if (includePaths && !string.IsNullOrEmpty(pair.Key))
                    {
                        capacity += pair.Key.Length + PathSeparator.Length;
                    }

                    capacity += error.Length;
                }

                capacity += pair.Value.Count * Environment.NewLine.Length;
            }

            return capacity;
        }
    }
}
