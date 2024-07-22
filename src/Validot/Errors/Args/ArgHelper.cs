namespace Validot.Errors.Args;

using System.Text;
using System.Text.RegularExpressions;

internal static class ArgHelper
{
    private static readonly IReadOnlyDictionary<string, string> EmptyParametersDictionary = new Dictionary<string, string>();

    public static char Divider { get; } = '|';

    public static char Assignment { get; } = '=';

    public static string FormatMessage(string? message, IReadOnlyList<ArgPlaceholder>? placeholders, IReadOnlyList<IArg>? args)
    {
        if (message is null)
        {
            return string.Empty;
        }

        if (args?.Any() != true)
        {
            return message;
        }

        if (placeholders?.Any() != true)
        {
            return message;
        }

        var messageBuilder = new StringBuilder(message);

        foreach (var placeholder in placeholders)
        {
            var arg = args.SingleOrDefault(a => a.Name == placeholder.Name);

            if (arg is null)
            {
                continue;
            }

            var withInvalidParameters =
                placeholder.Parameters?.Count > 0 &&
                placeholder.Parameters.Keys.Any(param => !arg.AllowedParameters.Contains(param));

            if (withInvalidParameters)
            {
                continue;
            }

            var value = arg.ToString(placeholder.Parameters);

            _ = messageBuilder.Replace(placeholder.Placeholder, value);
        }

        return messageBuilder.ToString();
    }

    public static IReadOnlyList<ArgPlaceholder> ExtractPlaceholders(string message)
    {
        ThrowHelper.NullArgument(message, nameof(message));

        var matches = CurlyBracketsRegex.Instance().Matches(message)
            .Cast<Match>()
            .Select(m => m.Value)
            .Distinct()
            .ToArray();

        var placeholders = new List<ArgPlaceholder>(matches.Length);

        foreach (var match in matches)
        {
            var parts = match.Split(Divider);

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
                    Parameters = EmptyParametersDictionary,
                });
            }
            else
            {
                Dictionary<string, string>? parameters = null;

                var invalidPart = false;

                for (var i = 1; i < parts.Length; ++i)
                {
                    var item = parts[i];

                    if (!item.Contains(Assignment, StringComparison.Ordinal))
                    {
                        invalidPart = true;

                        break;
                    }

                    var groups = item.Split(Assignment).Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();

                    if (groups.Length != 2)
                    {
                        invalidPart = true;

                        break;
                    }

                    parameters ??= [];

                    if (parameters.ContainsKey(groups[0]))
                    {
                        invalidPart = true;

                        break;
                    }

                    parameters.Add(groups[0], groups[1]);
                }

                if (invalidPart)
                {
                    continue;
                }

                placeholders.Add(new ArgPlaceholder
                {
                    Placeholder = placeholder,
                    Name = name,
                    Parameters = parameters ?? EmptyParametersDictionary,
                });
            }
        }

        return placeholders;
    }
}
