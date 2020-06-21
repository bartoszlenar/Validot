namespace Validot.MemoryLeak
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Bogus;
    using Bogus.Extensions;

    public static class StreamDataSet
    {
        static StreamDataSet()
        {
             Specification<NestedModel> nestedModelSpecification = _ => _
                    .Member(m => m.Number1, m2 => m2.Rule(v => v < 10).WithMessage("Nested Message N1"))
                    .Member(m => m.Number2, m2 => m2.Rule(v => v < 10).WithMessage("Nested Message N2"))
                    .Member(m => m.SuperNumber1, m2 => m2.Rule(v => v < 10).WithMessage("Nested Message S1"))
                    .Member(m => m.SuperNumber2, m2 => m2.Rule(v => v < 10).WithMessage("Nested Message S2"))
                    .Member(m => m.Text1, m => m.Rule(v => v.Contains('a', StringComparison.InvariantCultureIgnoreCase)).WithMessage("Nested Message T1"))
                    .Member(m => m.Text2, m => m.Rule(v => v.Contains('b', StringComparison.InvariantCultureIgnoreCase)).WithMessage("Nested Message T2"));
                
                Specification = _ => _
                    .Member(m => m.Text1, m => m.Rule(v => v.Contains('a', StringComparison.InvariantCultureIgnoreCase)).WithMessage("Message T1"))
                    .Member(m => m.Text2, m => m.Rule(v => v.Contains('b', StringComparison.InvariantCultureIgnoreCase)).WithMessage("Message T2"))
                    .Member(m => m.Text3, m => m.Rule(v => v.Contains('c', StringComparison.InvariantCultureIgnoreCase)).WithMessage("Message T3"))
                    .Member(m => m.Text4, m => m.Rule(v => v.Contains('d', StringComparison.InvariantCultureIgnoreCase)).WithMessage("Message T4"))
                    .Member(m => m.Text5, m => m.Rule(v => v.Contains('e', StringComparison.InvariantCultureIgnoreCase)).WithMessage("Message T5"))
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
            
             var nestedModelsHalfErrorsFaker = new Faker<NestedModel>()
                    .RuleFor(m => m.Number1, m => m.Random.Int(0, 9))
                    .RuleFor(m => m.Number2, m => m.Random.Int(0, 9))
                    .RuleFor(m => m.Text1, m => string.Join(" ", m.Lorem.Words(20)))
                    .RuleFor(m => m.Text2, m => string.Join("b", m.Lorem.Words(15)))
                    .RuleFor(m => m.SuperNumber1, m => m.Random.Number(0, 10))
                    .RuleFor(m => m.SuperNumber2, m => m.Random.Number(0, 9).OrNull(m, 0.01f));

                Faker = new Faker<FullModel>()
                    .RuleFor(m => m.Text1, m => string.Join("a", m.Lorem.Words(10)))
                    .RuleFor(m => m.Text2, m => string.Join("b", m.Lorem.Words(10)))
                    .RuleFor(m => m.Text3, m => string.Join("c", m.Lorem.Words(10)))
                    .RuleFor(m => m.Text4, m => string.Join("d", m.Lorem.Words(10)))
                    .RuleFor(m => m.Text5, m => string.Join(" ", m.Lorem.Words(20)))
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

        public static Specification<FullModel> Specification { get; private set; }
        
        public static Faker<FullModel> Faker { get; private set;}
    }
}
