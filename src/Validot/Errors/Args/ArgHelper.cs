namespace Validot.Errors.Args
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    internal static class ArgHelper
    {
        private static readonly Regex CurlyBracketsRegex = new Regex(@"(?<=\{)[^}]*(?=\})", RegexOptions.Compiled);

        private static readonly IReadOnlyDictionary<string, string> EmptyParametersDictionary = new Dictionary<string, string>();

        public static char Divider { get; } = '|';

        public static char Assignment { get; } = '=';

        public static string FormatMessage(string message, IReadOnlyList<ArgPlaceholder> placeholders, IReadOnlyList<IArg> args)
        {
            if (message == null)
            {
                return string.Empty;
            }

            if (args == null || !args.Any())
            {
                return message;
            }

            if (placeholders == null || !placeholders.Any())
            {
                return message;
            }

            var messageBuilder = new StringBuilder(message);

            foreach (ArgPlaceholder placeholder in placeholders)
            {
                IArg arg = args.SingleOrDefault(a => a.Name == placeholder.Name);

                if (arg == null)
                {
                    continue;
                }

                var withInvalidParameters =
                    placeholder.Parameters != null &&
                    placeholder.Parameters.Count > 0 &&
                    placeholder.Parameters.Keys.Any(param => !arg.AllowedParameters.Contains(param));

                if (withInvalidParameters)
                {
                    continue;
                }

                var value = arg.ToString(placeholder.Parameters);

                messageBuilder.Replace(placeholder.Placeholder, value);
            }

            return messageBuilder.ToString();
        }

        public static IReadOnlyList<ArgPlaceholder> ExtractPlaceholders(string message)
        {
            ThrowHelper.NullArgument(message, nameof(message));

            string[] matches = CurlyBracketsRegex.Matches(message)
                .Cast<Match>()
                .Select(m => m.Value)
                .Distinct()
                .ToArray();

            var placeholders = new List<ArgPlaceholder>(matches.Length);

            foreach (var match in matches)
            {
                string[] parts = match.Split(Divider);

                var name = parts.FirstOrDefault();

                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                var placeholder = $"{{{match}}}";

                if (parts.Length == 1)
                {
                    placeholders.Add(new ArgPlaceholder
                    {
                        Placeholder = placeholder,
                        Name = name,
                        Parameters = EmptyParametersDictionary
                    });
                }
                else
                {
                    Dictionary<string, string> parameters = null;

                    var invalidPart = false;

                    for (var i = 1; i < parts.Length; ++i)
                    {
                        var item = parts.ElementAt(i);

                        if (!item.Contains(Assignment))
                        {
                            invalidPart = true;

                            break;
                        }

                        string[] groups = item.Split(Assignment).Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();

                        if (groups.Length != 2)
                        {
                            invalidPart = true;

                            break;
                        }

                        if (parameters == null)
                        {
                            parameters = new Dictionary<string, string>();
                        }

                        if (parameters.ContainsKey(groups.ElementAt(0)))
                        {
                            invalidPart = true;

                            break;
                        }

                        parameters.Add(groups.ElementAt(0), groups.ElementAt(1));
                    }

                    if (invalidPart)
                    {
                        continue;
                    }

                    placeholders.Add(new ArgPlaceholder
                    {
                        Placeholder = placeholder,
                        Name = name,
                        Parameters = parameters ?? EmptyParametersDictionary
                    });
                }
            }

            return placeholders;
        }
    }
}
