namespace Validot.Tests.Functional
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bogus;
    using Bogus.Extensions;

    using FluentAssertions;

    using Validot.Results;

    using Xunit;

    public class ConcurrencyTests
    {
        private void ShouldHaveSameContent(IValidationResult result1, IValidationResult result2)
        {
            result1.Codes.Should().HaveCount(result2.Codes.Count);

            if (result1.Codes.Any())
            {
                result1.Codes.Should().Contain(result2.Codes);
                result2.Codes.Should().Contain(result1.Codes);
            }

            if (result1.Paths.Any())
            {
                result1.Paths.Should().HaveCount(result2.Paths.Count);
                result1.Paths.Should().Contain(result2.Paths);
                result2.Paths.Should().Contain(result1.Paths);
            }

            foreach (var path in result1.MessageMap.Keys)
            {
                result1.MessageMap[path].Should().HaveCount(result2.MessageMap[path].Count);
                result1.MessageMap[path].Should().Contain(result2.MessageMap[path]);
                result2.MessageMap[path].Should().Contain(result1.MessageMap[path]);
            }

            foreach (var path in result1.CodeMap.Keys)
            {
                result1.CodeMap[path].Should().HaveCount(result2.CodeMap[path].Count);
                result1.CodeMap[path].Should().Contain(result2.CodeMap[path]);
                result2.CodeMap[path].Should().Contain(result1.CodeMap[path]);
            }

            result1.TranslationNames.Should().HaveCount(result2.TranslationNames.Count);

            if (result1.TranslationNames.Any())
            {
                result1.TranslationNames.Should().Contain(result2.TranslationNames);
                result2.TranslationNames.Should().Contain(result1.TranslationNames);
            }
        }

        public class FullModel
        {
            public string Text1 { get; set; }

            public string Text2 { get; set; }

            public string Text3 { get; set; }

            public string Text4 { get; set; }

            public string Text5 { get; set; }

            public int Number1 { get; set; }

            public int Number2 { get; set; }

            public int Number3 { get; set; }

            public int Number4 { get; set; }

            public int Number5 { get; set; }

            public decimal? SuperNumber1 { get; set; }

            public decimal? SuperNumber2 { get; set; }

            public decimal? SuperNumber3 { get; set; }

            public NestedModel NestedModel1 { get; set; }

            public NestedModel NestedModel2 { get; set; }

            public IReadOnlyList<NestedModel> ModelCollection { get; set; }

            public IReadOnlyList<int> StructCollection { get; set; }
        }

        public class NestedModel
        {
            public string Text1 { get; set; }

            public string Text2 { get; set; }

            public int Number1 { get; set; }

            public int Number2 { get; set; }

            public decimal? SuperNumber1 { get; set; }

            public decimal? SuperNumber2 { get; set; }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public async Task Should_HaveSameResults_When_RunInConcurrentEnvironment(int concurrencyLevel)
        {
            Randomizer.Seed = new Random(666);

            Specification<NestedModel> nestedModelSpecification = _ => _
                    .Member(m => m.Number1, m2 => m2.Rule(v => v < 10).WithMessage("Nested Message N1"))
                    .Member(m => m.Number2, m2 => m2.Rule(v => v < 10).WithMessage("Nested Message N2"))
                    .Member(m => m.SuperNumber1, m2 => m2.Rule(v => v < 10).WithMessage("Nested Message S1"))
                    .Member(m => m.SuperNumber2, m2 => m2.Rule(v => v < 10).WithMessage("Nested Message S2"))
                    .Member(m => m.Text1, m => m.Rule(v => v.Contains('a')).WithMessage("Nested Message T1"))
                    .Member(m => m.Text2, m => m.Rule(v => v.Contains('b')).WithMessage("Nested Message T2"));

            Specification<FullModel> fullModelSpecification = _ => _
                    .Member(m => m.Text1, m => m.Rule(v => v.Contains('a')).WithMessage("Message T1"))
                    .Member(m => m.Text2, m => m.Rule(v => v.Contains('b')).WithMessage("Message T2"))
                    .Member(m => m.Text3, m => m.Rule(v => v.Contains('c')).WithMessage("Message T3"))
                    .Member(m => m.Text4, m => m.Rule(v => v.Contains('d')).WithMessage("Message T4"))
                    .Member(m => m.Text5, m => m.Rule(v => v.Contains('e')).WithMessage("Message T5"))
                    .Member(m => m.Number1, m2 => m2.Rule(v => v < 10).WithMessage("Message N1"))
                    .Member(m => m.Number2, m2 => m2.Rule(v => v < 10).WithMessage("Message N2"))
                    .Member(m => m.Number3, m2 => m2.Rule(v => v < 10).WithMessage("Message N3"))
                    .Member(m => m.Number4, m2 => m2.Rule(v => v < 10).WithMessage("Message N4"))
                    .Member(m => m.Number5, m2 => m2.Rule(v => v < 10).WithMessage("Message N5"))
                    .Member(m => m.SuperNumber1, m2 => m2.Rule(v => v < 10).WithMessage("Message S1"))
                    .Member(m => m.SuperNumber2, m2 => m2.Rule(v => v < 10).WithMessage("Message S2"))
                    .Member(m => m.SuperNumber3, m2 => m2.Rule(v => v < 10).WithMessage("Message S3"))
                    .Member(m => m.NestedModel1, nestedModelSpecification)
                    .Member(m => m.NestedModel2, nestedModelSpecification)
                    .Member(m => m.ModelCollection, m => m
                        .MaxCollectionSize(10).WithMessage("No more than 10 items are allowed")
                        .AsCollection(nestedModelSpecification))
                    .Member(m => m.StructCollection, m => m
                        .MaxCollectionSize(10).WithMessage("No more than 10 items are allowed")
                        .AsCollection(m1 => m1.Rule(m2 => m2 < 10).WithMessage("Message C")));

            var validator = Validator.Factory.Create(fullModelSpecification);

            var nestedModelsHalfErrorsFaker = new Faker<NestedModel>()
                    .RuleFor(m => m.Number1, m => m.Random.Int(0, 9))
                    .RuleFor(m => m.Number2, m => m.Random.Int(0, 9))
                    .RuleFor(m => m.Text1, m => string.Join(" ", m.Lorem.Words(20)).ToLowerInvariant())
                    .RuleFor(m => m.Text2, m => string.Join("b", m.Lorem.Words(15)).ToLowerInvariant())
                    .RuleFor(m => m.SuperNumber1, m => m.Random.Number(0, 10))
                    .RuleFor(m => m.SuperNumber2, m => m.Random.Number(0, 9).OrNull(m, 0.01f));

            var fullModelHalfErrorsFaker = new Faker<FullModel>()
                    .RuleFor(m => m.Text1, m => string.Join("a", m.Lorem.Words(10)).ToLowerInvariant())
                    .RuleFor(m => m.Text2, m => string.Join("b", m.Lorem.Words(10)).ToLowerInvariant())
                    .RuleFor(m => m.Text3, m => string.Join("c", m.Lorem.Words(10)).ToLowerInvariant())
                    .RuleFor(m => m.Text4, m => string.Join("d", m.Lorem.Words(10)).ToLowerInvariant())
                    .RuleFor(m => m.Text5, m => string.Join(" ", m.Lorem.Words(20)).ToLowerInvariant())
                    .RuleFor(m => m.Number1, m => m.Random.Int(0, 9))
                    .RuleFor(m => m.Number2, m => m.Random.Int(0, 9))
                    .RuleFor(m => m.Number3, m => m.Random.Int(0, 9))
                    .RuleFor(m => m.Number4, m => m.Random.Int(0, 9))
                    .RuleFor(m => m.Number5, m => m.Random.Int(0, 10))
                    .RuleFor(m => m.SuperNumber1, m => m.Random.Decimal(0, 9))
                    .RuleFor(m => m.SuperNumber2, m => m.Random.Decimal(0, 9))
                    .RuleFor(m => m.SuperNumber3, m => m.Random.Decimal(0, 10).OrNull(m, 0.01f))
                    .RuleFor(m => m.NestedModel1, m => nestedModelsHalfErrorsFaker.Generate())
                    .RuleFor(m => m.NestedModel2, m => nestedModelsHalfErrorsFaker.Generate())
                    .RuleFor(m => m.ModelCollection, m => nestedModelsHalfErrorsFaker.GenerateBetween(0, 9).ToList())
                    .RuleFor(m => m.StructCollection, m => Enumerable.Range(1, m.Random.Int(1, 11)).Select(_ => m.Random.Number(0, 9)).ToList());

            var size = 5000;

            var dataSets = new IReadOnlyList<FullModel>[concurrencyLevel];
            var resultSets = new IReadOnlyList<IValidationResult>[concurrencyLevel];

            for (var i = 0; i < concurrencyLevel; ++i)
            {
                dataSets[i] = fullModelHalfErrorsFaker.Generate(size).ToList();
                var r = new IValidationResult[size];

                for (var j = 0; j < size; ++j)
                {
                    r[j] = validator.Validate(dataSets[i][j]);
                }

                resultSets[i] = r;
            }

            var tasks = new Task[concurrencyLevel];

            for (var i = 0; i < concurrencyLevel; ++i)
            {
                var dataSet = dataSets[i];
                var results = resultSets[i];

                tasks[i] = Task.Run(() => RunTaskForDataSet(dataSet, results));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            void RunTaskForDataSet(IReadOnlyList<FullModel> dataSet, IReadOnlyList<IValidationResult> results)
            {
                for (var j = 0; j < size; ++j)
                {
                    var validationResult = validator.Validate(dataSet[j]);

                    ShouldHaveSameContent(validationResult, results[j]);
                }
            }
        }
    }
}
