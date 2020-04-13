namespace Validot.Translations
{
    using System.Collections.Generic;
    using System.Reflection;

    public static class MessageKey
    {
        static MessageKey()
        {
            var keys = new List<string>();

            var globalType = typeof(MessageKey);

            var innerTypes = globalType.GetNestedTypes(BindingFlags.Public | BindingFlags.Static);

            foreach (var innerType in innerTypes)
            {
                var properties = innerType.GetProperties(BindingFlags.Public | BindingFlags.Static);

                foreach (var property in properties)
                {
                    var key = $"{innerType.Name}.{property.Name}";
                    keys.Add(key);

                    property.SetValue(null, key);
                }
            }

            All = keys.ToArray();
        }

        public static IReadOnlyCollection<string> All { get; }

        private static void Touch()
        {
        }

        public static class GuidType
        {
            static GuidType()
            {
                Touch();
            }

            public static string EqualTo { get; private set; }

            public static string NotEqualTo { get; private set; }

            public static string NotEmpty { get; private set; }
        }

        public static class BoolType
        {
            static BoolType()
            {
                Touch();
            }

            public static string True { get; private set; }

            public static string False { get; private set; }
        }

        public static class Collections
        {
            static Collections()
            {
                Touch();
            }

            public static string EmptyCollection { get; private set; }

            public static string NotEmptyCollection { get; private set; }

            public static string ExactCollectionSize { get; private set; }

            public static string MaxCollectionSize { get; private set; }

            public static string MinCollectionSize { get; private set; }

            public static string CollectionSizeBetween { get; private set; }
        }

        public static class Numbers
        {
            static Numbers()
            {
                Touch();
            }

            public static string EqualTo { get; private set; }

            public static string NotEqualTo { get; private set; }

            public static string GreaterThan { get; private set; }

            public static string GreaterThanOrEqualTo { get; private set; }

            public static string LessThan { get; private set; }

            public static string LessThanOrEqualTo { get; private set; }

            public static string Between { get; private set; }

            public static string BetweenOrEqualTo { get; private set; }

            public static string NonZero { get; private set; }

            public static string Positive { get; private set; }

            public static string NonPositive { get; private set; }

            public static string Negative { get; private set; }

            public static string NonNegative { get; private set; }

            public static string NonNaN { get; private set; }
        }

        public static class Texts
        {
            static Texts()
            {
                Touch();
            }

            public static string Email { get; private set; }

            public static string EqualTo { get; private set; }

            public static string NotEqualTo { get; private set; }

            public static string Contains { get; private set; }

            public static string NotContains { get; private set; }

            public static string NotEmpty { get; private set; }

            public static string NotWhiteSpace { get; private set; }

            public static string SingleLine { get; private set; }

            public static string ExactLength { get; private set; }

            public static string MaxLength { get; private set; }

            public static string MinLength { get; private set; }

            public static string LengthBetween { get; private set; }

            public static string Matches { get; private set; }

            public static string StartsWith { get; private set; }

            public static string EndsWith { get; private set; }
        }

        public static class CharType
        {
            static CharType()
            {
                Touch();
            }

            public static string EqualToIgnoreCase { get; private set; }

            public static string NotEqualToIgnoreCase { get; private set; }
        }

        public static class Times
        {
            static Times()
            {
                Touch();
            }

            public static string EqualTo { get; private set; }

            public static string NotEqualTo { get; private set; }

            public static string After { get; private set; }

            public static string AfterOrEqualTo { get; private set; }

            public static string Before { get; private set; }

            public static string BeforeOrEqualTo { get; private set; }

            public static string Between { get; private set; }

            public static string BetweenOrEqualTo { get; private set; }
        }

        public static class TimeSpanType
        {
            static TimeSpanType()
            {
                Touch();
            }

            public static string EqualTo { get; private set; }

            public static string NotEqualTo { get; private set; }

            public static string GreaterThan { get; private set; }

            public static string GreaterThanOrEqualTo { get; private set; }

            public static string LessThan { get; private set; }

            public static string LessThanOrEqualTo { get; private set; }

            public static string Between { get; private set; }

            public static string BetweenOrEqualTo { get; private set; }

            public static string NonZero { get; private set; }

            public static string Positive { get; private set; }

            public static string NonPositive { get; private set; }

            public static string Negative { get; private set; }

            public static string NonNegative { get; private set; }
        }

        public static class Global
        {
            static Global()
            {
                Touch();
            }

            public static string Forbidden { get; private set; }

            public static string Required { get; private set; }

            public static string Default { get; private set; }

            public static string ReferenceLoop { get; private set; }
        }
    }
}
