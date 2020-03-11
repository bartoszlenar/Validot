namespace Validot.Errors.Args
{
    using System;
    using System.Collections.Generic;

    public sealed class TypeArg : IArg<Type>
    {
        private const string TranslationParameter = "translation";

        private const string TranslationParameterValue = "true";

        private const string FormatParameter = "format";

        private const string NameFormat = "name";

        private const string ToStringFormat = "toString";

        private const string FullNameFormat = "fullName";

        private const string DefaultFormat = NameFormat;

        public TypeArg(string name, Type value)
        {
            ThrowHelper.NullArgument(name, nameof(name));

            Name = name;
            Value = value;
        }

        public string Name { get; }

        public Type Value { get; }

        public IReadOnlyCollection<string> AllowedParameters { get; } = new[]
        {
            TranslationParameter,
            FormatParameter
        };

        public string ToString(IReadOnlyDictionary<string, string> parameters)
        {
            if (parameters?.ContainsKey(TranslationParameter) == true &&
                parameters[TranslationParameter] == TranslationParameterValue)
            {
                return $"{{_translation|key=Type.{Value.GetFriendlyName(true)}}}";
            }

            var format = parameters?.ContainsKey(FormatParameter) == true
                ? parameters[FormatParameter]
                : DefaultFormat;

            if (format == ToStringFormat)
            {
                return Value.ToString();
            }

            if (format == FullNameFormat)
            {
                return Value.GetFriendlyName(true);
            }

            return Value.GetFriendlyName(format == FullNameFormat);
        }
    }
}
