namespace Validot.Errors.Args
{
    using System;
    using System.Collections.Generic;

    public sealed class EnumArg<T> : IArg<T>
        where T : struct
    {
        private const string TranslationParameter = "translation";

        private const string TranslationParameterValue = "true";

        private const string FormatParameter = "format";

        private const string DefaultFormat = "G";

        public EnumArg(string name, T value)
        {
            ThrowHelper.NullArgument(name, nameof(name));

            Name = name;
            Value = value;
        }

        public string Name { get; }

        public T Value { get; }

        public IReadOnlyCollection<string> AllowedParameters { get; } = new[]
        {
            FormatParameter,
            TranslationParameter
        };

        public string ToString(IReadOnlyDictionary<string, string> parameters)
        {
            if (parameters?.ContainsKey(TranslationParameter) == true &&
                parameters[TranslationParameter] == TranslationParameterValue)
            {
                var key = Enum.Format(typeof(T), Value, "f");

                return TranslationArg.CreatePlaceholder($"Enum.{typeof(T).FullName}.{key}");
            }

            var format = parameters?.ContainsKey(FormatParameter) == true
                ? parameters[FormatParameter]
                : DefaultFormat;

            return Enum.Format(typeof(T), Value, format);
        }
    }
}
