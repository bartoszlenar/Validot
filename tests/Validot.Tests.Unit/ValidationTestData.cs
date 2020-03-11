namespace Validot.Tests.Unit
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public static class ValidationTestData
    {
        public class TestCase
        {
            public string Name { get; set; }

            public Specification<TestClass> Specification { get; set; }

            public IReadOnlyDictionary<string, IReadOnlyList<string>> ExpectedErrorsMap { get; set; }

            public IReadOnlyList<ValidationTestCase> ValidationCases { get; set; }
        }

        public class ValidationTestCase
        {
            public TestClass Model { get; set; }

            public IReadOnlyDictionary<string, IReadOnlyList<string>> ErrorMessages { get; set; }

            public IReadOnlyDictionary<string, IReadOnlyList<string>> FailFastErrorMessages { get; set; }
        }

        public class TestCollection<T> : IEnumerable<T>
        {
            private readonly IEnumerable<T> _innerCollection;

            public TestCollection(IEnumerable<T> innerCollection)
            {
                _innerCollection = innerCollection;
            }

            public IEnumerator<T> GetEnumerator() => _innerCollection.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public class TestMember
        {
            public string MemberText { get; set; }

            public DateTimeOffset MemberDate { get; set; }

            public TestClass NestedSelf { get; set; }
        }

        public struct TestStruct
        {
            public int StructNumber { get; set; }
        }

        public class TestClass
        {
            public string HybridField;

            public int ValueField;

            public string Hybrid { get; set; }

            public int Value { get; set; }

            public object Reference { get; set; }

            public bool? Nullable { get; set; }

            public TestClass Self { get; set; }

            public TestCollection<TestClass> SelfCollection { get; set; }

            public TestCollection<int> Collection { get; set; }

            public TestMember Member { get; set; }

            public TestStruct StructMember { get; set; }

            public TestCollection<TestMember> MembersCollection { get; set; }
        }

        private static readonly Dictionary<string, IReadOnlyList<string>> NoMessages = new Dictionary<string, IReadOnlyList<string>>();

        private static readonly TestClass _dataSet1 = new TestClass()
        {
            Hybrid = "Some text value",
            HybridField = "Text in field",
            Collection = new TestCollection<int>(new[]
            {
                1,
                2,
                3
            }),
            StructMember = new TestStruct()
            {
                StructNumber = 321
            },
            Nullable = true,
            Member = new TestMember()
            {
                MemberText = "Some text value"
            }
        };

        public static IReadOnlyList<TestCase> Cases { get; } = new[]
        {
            new TestCase()
            {
                Name = "Just Optional, not other rules",
                Specification = s => s.Optional(),
                ExpectedErrorsMap = NoMessages,
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = null,
                        ErrorMessages = NoMessages,
                        FailFastErrorMessages = NoMessages
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        ErrorMessages = NoMessages,
                        FailFastErrorMessages = NoMessages
                    },
                    new ValidationTestCase()
                    {
                        Model = _dataSet1,
                        ErrorMessages = NoMessages,
                        FailFastErrorMessages = NoMessages
                    },
                },
            },
            new TestCase()
            {
                Name = "No rules at all",
                Specification = s => s,
                ExpectedErrorsMap = new Dictionary<string, IReadOnlyList<string>>()
                {
                    [""] = new[] { "Required" }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = null,
                        ErrorMessages = new Dictionary<string, IReadOnlyList<string>>()
                        {
                            [""] = new[] { "Required" }
                        },
                        FailFastErrorMessages = new Dictionary<string, IReadOnlyList<string>>()
                        {
                            [""] = new[] { "Required" }
                        },
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        ErrorMessages = NoMessages,
                        FailFastErrorMessages = NoMessages
                    },
                    new ValidationTestCase()
                    {
                        Model = _dataSet1,
                        ErrorMessages = NoMessages,
                        FailFastErrorMessages = NoMessages
                    },
                },
            },
        };

        public static IEnumerable<object[]> CasesForErrorsMap_Data()
        {
            return Cases.Select(c => new object[]
            {
                c.Name,
                c.Specification,
                c.ExpectedErrorsMap
            });
        }

        public static IEnumerable<object[]> CasesForValidation_Data()
        {
            foreach (var c in Cases)
            {
                var i = 0;

                foreach (var v in c.ValidationCases)
                {
                    yield return new object[]
                    {
                        $"{c.Name}_{i++}",
                        c.Specification,
                        v.Model,
                        v.ErrorMessages
                    };
                }
            }
        }

        public static IEnumerable<object[]> CasesForValidationWithFailFast_Data()
        {
            foreach (var c in Cases)
            {
                var i = 0;

                foreach (var v in c.ValidationCases)
                {
                    yield return new object[]
                    {
                        $"{c.Name}_{++i}",
                        c.Specification,
                        v.Model,
                        v.FailFastErrorMessages
                    };
                }
            }
        }
    }
}
