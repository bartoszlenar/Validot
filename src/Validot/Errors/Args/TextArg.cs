namespace Validot.Errors.Args
{
    using System.Collections.Generic;
    using System.Globalization;

    public sealed class TextArg : IArg<string>
    {
        private const string CaseParameter = "case";

        private const string UpperCaseParameterValue = "upper";

        private const string LowerCaseParameterValue = "lower";

        private static readonly string[] StaticAllowedParameters =
        {
            CaseParameter
        };

        public TextArg(string name, string value)
        {
            ThrowHelper.NullArgument(name, nameof(name));
            ThrowHelper.NullArgument(value, nameof(value));

            Value = value;
            Name = name;
        }

        public TextArg(string name, char value)
            : this(name, value.ToString(CultureInfo.InvariantCulture))
        {
        }

        public string Name { get; }

        public string Value { get; }

        public IReadOnlyCollection<string> AllowedParameters => StaticAllowedParameters;

        public string ToString(IReadOnlyDictionary<string, string> parameters)
        {
            var caseParameter = parameters?.ContainsKey(CaseParameter) == true
                ? parameters[CaseParameter]
                : null;

            if (caseParameter != null &&
                caseParameter != UpperCaseParameterValue &&
                caseParameter != LowerCaseParameterValue)
            {
                caseParameter = null;
            }

            return Stringify(Value, caseParameter);
        }

        private static string Stringify(string value, string caseParameter)
        {
            if (caseParameter == UpperCaseParameterValue)
            {
                return value.ToUpper(CultureInfo.InvariantCulture);
            }

            if (caseParameter == LowerCaseParameterValue)
            {
                return value.ToLower(CultureInfo.InvariantCulture);
            }

            return value;
        }
    }
}
